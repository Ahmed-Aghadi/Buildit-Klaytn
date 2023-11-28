// SPDX-License-Identifier: MIT
pragma solidity ^0.8.13;

abstract contract IWitnetPriceRouter {
    /**
     * @dev Exposed function pertaining to EIP standards
     * @param _id bytes32 ID of the query
     * @return int,uint,uint returns the value, timestamp, and status code of query
     */
    function valueFor(
        bytes32 _id
    ) external view virtual returns (int256, uint256, uint256);

    function lookupDecimals(
        bytes4 feedId
    ) external view virtual returns (uint8);
}
