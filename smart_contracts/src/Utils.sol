// SPDX-License-Identifier: UNLICENSED
pragma solidity ^0.8.13;

import "solmate/tokens/ERC1155.sol";
import "solmate/utils/LibString.sol";
import "solmate/auth/Owned.sol";

contract Utils is ERC1155, Owned {
    string public baseUri;
    uint256 public utilCount;

    constructor(string memory _baseUri) ERC1155() Owned(msg.sender) {
        baseUri = _baseUri;
    }

    function mint(uint256 amount) public onlyOwner {
        utilCount += 1;
        _mint(msg.sender, utilCount, amount, "");
    }

    function uri(
        uint256 id
    ) public view virtual override returns (string memory) {
        return string(abi.encodePacked(baseUri, LibString.toString(id)));
    }
}
