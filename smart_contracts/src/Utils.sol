// SPDX-License-Identifier: UNLICENSED
pragma solidity ^0.8.13;

import {KIP37} from "klaytn/KIP/token/KIP37/KIP37.sol";
import "klaytn/utils/Strings.sol";
import "klaytn/access/Ownable.sol";
import "klaytn/metatx/ERC2771Context.sol";

contract Utils is ERC2771Context, KIP37, Ownable {
    error CrossChainNotSupported();
    error InvalidChain();
    error InsufficientBalance();
    string public baseUri;
    uint256 public utilCount;

    constructor(
        string memory _baseUri,
        address trustedForwarder
    ) ERC2771Context(trustedForwarder) KIP37(_baseUri) {
        baseUri = _baseUri;
    }

    function mintMore(uint id, uint amount) public onlyOwner {
        _mint(_msgSender(), id, amount, "");
    }

    function mint(uint256 amount) public onlyOwner {
        utilCount += 1;
        _mint(_msgSender(), utilCount, amount, "");
    }

    function uri(
        uint256 id
    ) public view virtual override returns (string memory) {
        return string(abi.encodePacked(baseUri, Strings.toString(id)));
    }

    function _msgSender()
        internal
        view
        virtual
        override(Context, ERC2771Context)
        returns (address sender)
    {
        return ERC2771Context._msgSender();
    }

    function _msgData()
        internal
        view
        virtual
        override(Context, ERC2771Context)
        returns (bytes calldata)
    {
        return ERC2771Context._msgData();
    }
}
