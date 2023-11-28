// SPDX-License-Identifier: MIT

pragma solidity 0.8.4;
import "../interfaces/IERC2362.sol";

/**
* @title SampleOracle
* @notice This contract is only and example/place holder of an ADO
* compliant oracle. It is centralized and should not be used in a production
* environment. The purpose of this contract is to allow for testing the
* ADOMedianizer.sol contract.
*/
contract SampleOracle is IERC2362 {
    /*Variables*/
    mapping(bytes32=>mapping(uint=>int256)) valuesByID;
    mapping (bytes32 => uint[]) timestampsByID;

    /**
    * @dev Allows oracle provider to push the vlue for the corresponding _id
    * @param _id is the  standardized  ADO data type/value pair Id
    * @param _value is the value
    */
    function setValue(bytes32 _id, int256 _value) external{
      	valuesByID[_id][block.timestamp] = _value;
      	timestampsByID[_id].push(block.timestamp);
    }

    /**
    * @dev Allows the ADOMedianizer to retreive the value, timestamp, and status for the specifed Id
    * @param _id is the  standardized ADO data type/value pair id
    * @return _value the value corresponding to the id
    * @return _timestamp is the unix timestamp
    * @return _status is the  standardized data status
    */
    function valueFor(bytes32 _id) external view override returns(int _value,uint _timestamp,uint _status){
        uint len = timestampsByID[_id].length;
        if(len > 0){
            _timestamp = timestampsByID[_id][len - 1];
           	return(valuesByID[_id][_timestamp],_timestamp,200);
	      }
	   else return(0,0,404);
    }
}
