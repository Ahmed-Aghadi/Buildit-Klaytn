// SPDX-License-Identifier: MIT

pragma solidity 0.8.4;

import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/utils/structs/EnumerableSet.sol";
import "./interfaces/IERC2362.sol";

/**
* @title ADOMedianizer
* @notice This contract allows the owner to specify multiple oracles
* to use for collecting data and ultimately calculating the median of these providers
*/
contract ADOMedianizer is IERC2362, Ownable
{
	using EnumerableSet for EnumerableSet.AddressSet;

	EnumerableSet.AddressSet oracles;
	uint256 constant validity = 1 days;

    /**
    * @dev Allows owner to add an oracle provider
    * @param _oracle is the oracle address for the oracle being added
    */
	function addOracle(address _oracle)
	external  onlyOwner()
	{
		oracles.add(_oracle);
	}

    /**
    * @dev Allows the owner to remove an oracle provider
    * @param _oracle is the oracle address for the oracle being excluded
    */
	function removeOracle(address _oracle)
	external  onlyOwner()
	{
		oracles.remove(_oracle);
	}

    /**
    * @dev Getter returning an array of all oracles in this contract
    */
	function getOracles()
	external view returns(address[] memory _oracles) // stored as bytes32, eventually need casting :/
	{	

		_oracles = new address[](oracles.length());
		for(uint i=0; i< oracles.length();++i){
			_oracles[i] =  address(uint160(uint256(oracles._inner._values[i])));
		}
	}

    /**
    * @dev This function loops through the oracle addresses and medianizes the values
    * @param _id the standardized ADO data type/value pair id
    * @return median value, timestamp and status
    */
	function valueFor(bytes32 _id)
	external view override returns (int256, uint256, uint256)
	{
		if (oracles.length() == 0) { return (0, 0, 404); }

		int256[] memory values = new int256[](oracles.length());
		uint256         length = 0;

		for (uint256 i = 0; i < oracles.length(); ++i)
		{
			try IERC2362(oracles.at(i)).valueFor(_id) returns (int256 val, uint256 time, uint256 status)
			{
				if (status >= 200 && status < 300 && time + validity > block.timestamp) // valid HTTP status are all 2xx
				{
					values[length] = val;
					++length;
				}
			}
			catch (bytes memory /*lowLevelData*/) {}
		}

		if (length == 0) { return (0, 0, 400); }

		return (median(values, length), block.timestamp, 200);
	}


    /**
    * @dev Internal function that sorts values submitted by oracles and returns the median
    * @param array is the array containing the values submitted for the Id by all the oracles
    * @param length is the length of the array
    * @return median value
    */
	function median(int256[] memory array, uint256 length)
	internal pure returns(int256)
	{
		// insertion sort
		for (uint256 i = 1; i < length; ++i)
		{
			int256 temp = array[i];
			uint256 j;
			for (j = i; j > 0 && temp < array[j-1]; --j)
			{
				array[j] = array[j-1];
			}
			array[j] = temp;
		}
		// odd/even length management
		return length % 2 == 0 ? (array[length/2-1] + array[length/2]) / 2 : array[length/2];
	}

}
