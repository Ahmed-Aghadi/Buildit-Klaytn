// SPDX-License-Identifier: UNLICENSED
pragma solidity ^0.8.13;

import {IKIP37Receiver} from "klaytn/KIP/token/KIP37/IKIP37Receiver.sol";
import "klaytn/KIP/token/KIP37/IKIP37.sol";
import "klaytn/metatx/ERC2771Context.sol";

contract Faucet is ERC2771Context {
    constructor(address trustedForwarder) ERC2771Context(trustedForwarder) {}

    function getToken(address tokenAddress, uint256 tokenId) public {
        IKIP37(tokenAddress).safeTransferFrom(
            address(this),
            _msgSender(),
            tokenId,
            10,
            ""
        );
    }

    function onKIP37Received(
        address,
        address,
        uint256,
        uint256,
        bytes memory
    ) public virtual returns (bytes4) {
        return IKIP37Receiver.onKIP37Received.selector;
    }

    function onKIP37BatchReceived(
        address,
        address,
        uint256[] memory,
        uint256[] memory,
        bytes memory
    ) public virtual returns (bytes4) {
        return IKIP37Receiver.onKIP37BatchReceived.selector;
    }
}
