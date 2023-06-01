// SPDX-License-Identifier: UNLICENSED
pragma solidity ^0.8.13;

import "solmate/tokens/ERC721.sol";
import "solmate/tokens/ERC1155.sol";
import "solmate/utils/LibString.sol";
import "solmate/auth/Owned.sol";

import {ERC1155TokenReceiver} from "solmate/tokens/ERC1155.sol";

error ZeroSize();
error ZeroPerSize();
error SizeNotDivisibleByPerSize();
error InvalidXIndex();
error InvalidYIndex();
error LandAlreadyOwned();
error NotOwner();
error InvalidLength();

contract Map is ERC721, Owned {
    // // rectangular land with coordinates of corners as (x,y), (x,y+perSize), (x+perSize,y), (x+perSize,y+perSize)
    struct Land {
        uint256 xIndex;
        uint256 yIndex;
    }
    uint256 public size;
    uint256 public perSize;
    uint256 public landCount;
    string public baseUri;
    uint256 public utilCount;
    address public utilsAddress;

    // x/perSize,y/perSize to LandId
    // [ (x/perSize,y/perSize) to LandId ] of rectangular land with coordinates of corners as (x,y), (x,y+perSize), (x+perSize,y), (x+perSize,y+perSize)
    mapping(uint256 => mapping(uint256 => uint256)) public landIds;
    // mapping of LandId to Land (xIndex, yIndex)
    mapping(uint256 => Land) public land;

    // x,y to utilId
    mapping(uint256 => mapping(uint256 => uint256)) public map;

    constructor(
        uint256 _size,
        uint256 _perSize,
        string memory _baseUri,
        address _utilsAddress
    ) ERC721("Map", "MAP") Owned(msg.sender) {
        if (_size == 0) revert ZeroSize();
        if (_perSize == 0) revert ZeroPerSize();
        size = _size;
        perSize = _perSize;
        if (_size % perSize != 0) revert SizeNotDivisibleByPerSize();
        baseUri = _baseUri;
        utilsAddress = _utilsAddress;
    }

    function mint(uint256 xIndex, uint256 yIndex) public returns (uint256) {
        if (xIndex >= size / perSize) revert InvalidXIndex();
        if (yIndex >= size / perSize) revert InvalidYIndex();
        if (landIds[xIndex][yIndex] != 0) revert LandAlreadyOwned();
        landCount += 1;
        // land[id] = Land(x, y);
        landIds[xIndex][yIndex] = landCount;
        land[landCount] = Land(xIndex, yIndex);
        _mint(msg.sender, landCount);
        return landCount;
    }

    function placeItem(uint256 x, uint256 y, uint256 utilId) public {
        if (msg.sender != ownerOf(landIds[x / perSize][y / perSize]))
            revert NotOwner();
        ERC1155 utils = ERC1155(utilsAddress);
        if (map[x][y] != 0) {
            utils.safeTransferFrom(address(this), msg.sender, map[x][y], 1, "");
        }
        utils.safeTransferFrom(msg.sender, address(this), utilId, 1, "");
        map[x][y] = utilId;
    }

    function removeItem(uint256 x, uint256 y) public {
        if (msg.sender != ownerOf(landIds[x / perSize][y / perSize]))
            revert NotOwner();
        ERC1155 utils = ERC1155(utilsAddress);
        if (map[x][y] != 0) {
            utils.safeTransferFrom(address(this), msg.sender, map[x][y], 1, "");
        }
        map[x][y] = 0;
    }

    function updateItem(uint256 x, uint256 y, uint256 utilId) public {
        if (utilId == 0) {
            removeItem(x, y);
        } else {
            placeItem(x, y, utilId);
        }
    }

    function placeItems(
        uint256[] memory x,
        uint256[] memory y,
        uint256[] memory utilId
    ) public {
        if (x.length != y.length) revert InvalidLength();
        if (x.length != utilId.length) revert InvalidLength();
        for (uint256 i = 0; i < x.length; i++) {
            placeItem(x[i], y[i], utilId[i]);
        }
    }

    function updateItems(
        uint256[] memory x,
        uint256[] memory y,
        uint256[] memory utilId
    ) public {
        if (x.length != y.length) revert InvalidLength();
        if (x.length != utilId.length) revert InvalidLength();
        for (uint256 i = 0; i < x.length; i++) {
            updateItem(x[i], y[i], utilId[i]);
        }
    }

    function tokenURI(
        uint256 id
    ) public view virtual override returns (string memory) {
        return string.concat(baseUri, LibString.toString(id));
    }

    function onERC1155Received(
        address,
        address,
        uint256,
        uint256,
        bytes memory
    ) public virtual returns (bytes4) {
        return ERC1155TokenReceiver.onERC1155Received.selector;
    }

    function onERC1155BatchReceived(
        address,
        address,
        uint256[] memory,
        uint256[] memory,
        bytes memory
    ) public virtual returns (bytes4) {
        return ERC1155TokenReceiver.onERC1155BatchReceived.selector;
    }
}
