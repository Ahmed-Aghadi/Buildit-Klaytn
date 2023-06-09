// SPDX-License-Identifier: UNLICENSED
pragma solidity ^0.8.13;

// import "solmate/tokens/ERC1155.sol";
import {ERC1155} from "openzeppelin/token/ERC1155/ERC1155.sol";
import "solmate/utils/LibString.sol";
// import "solmate/auth/Owned.sol";
import "openzeppelin/access/Ownable.sol";
import "openzeppelin/metatx/ERC2771Context.sol";

contract Utils is ERC2771Context, ERC1155, Ownable {
    string public baseUri;
    uint256 public utilCount;

    constructor(
        string memory _baseUri,
        address trustedForwarder
    ) ERC2771Context(trustedForwarder) ERC1155(_baseUri) {
        baseUri = _baseUri;
    }

    function mint(uint256 amount) public onlyOwner {
        utilCount += 1;
        _mint(_msgSender(), utilCount, amount, "");
    }

    function uri(
        uint256 id
    ) public view virtual override returns (string memory) {
        return string(abi.encodePacked(baseUri, LibString.toString(id)));
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
