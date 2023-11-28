// SPDX-License-Identifier: UNLICENSED
pragma solidity ^0.8.13;

import "forge-std/Test.sol";
import "forge-std/console.sol";
import "../src/Map.sol";
import "../src/Utils.sol";
import "../src/Marketplace.sol";
import "../src/Forwarder.sol";

import {IKIP37Receiver} from "klaytn/KIP/token/KIP37/IKIP37Receiver.sol";
import {IKIP37Receiver} from "klaytn/KIP/token/KIP37/IKIP37Receiver.sol";

contract MarketplaceTest is Test {
    MockWitnetRouter mockWitnetRouter;
    Forwarder forwarder;
    Marketplace marketplace;
    Map map;
    Utils utils;
    uint80 roundId;
    int256 value;
    uint256 timestamp;
    uint256 status;
    uint8 decimals;
    string mapBaseUri;
    string utilsBaseUri;
    uint256 size;
    uint256 perSize;
    uint256 mapCount;
    uint256 utilCount;
    uint256 utilAmount;

    bytes4 constant ERC1155_RECEIVED = 0xf23a6e61;
    bytes4 constant ERC1155_BATCH_RECEIVED = 0xbc197c81;
    bytes4 constant witnetPriceRouterUsdId4 = 0x00000001;

    function setUp() public {
        roundId = 1;
        value = 2000_0000_0000;
        timestamp = 1623931200;
        status = 1;
        decimals = 8;
        mapBaseUri = "https://example1.com/";
        utilsBaseUri = "https://example2.com/";
        size = 15;
        perSize = 5;
        mapCount = 3;
        utilCount = 3;
        utilAmount = 1000;
        forwarder = new Forwarder();
        utils = new Utils(utilsBaseUri, address(forwarder));
        map = new Map(size, 5, mapBaseUri, address(utils), address(forwarder));
        mockWitnetRouter = new MockWitnetRouter(
            value,
            timestamp,
            status,
            decimals
        );
        marketplace = new Marketplace(
            address(mockWitnetRouter),
            witnetPriceRouterUsdId4,
            address(map),
            address(utils),
            address(0x0),
            address(0x01),
            address(0x02),
            999999,
            address(forwarder)
        );
        uint i = 0;
        uint j = 0;
        uint mapCountTemp = mapCount;
        // mint map like
        // 0,0 0,1 0,2 0,3 0,4
        // 1,0 1,1 1,2 1,3 1,4
        // 2,0 2,1 2,2 2,3 2,4
        // ...
        while (mapCountTemp > 0) {
            map.mint(i, j);
            if (j == (size / perSize) - 1) {
                i++;
                j = 0;
            } else {
                j++;
            }
            mapCountTemp--;
        }
        for (i = 0; i < utilCount; i++) {
            utils.mint(utilAmount);
        }
    }

    function onKIP17Received(
        address,
        address,
        uint256,
        bytes calldata
    ) external virtual returns (bytes4) {
        return IKIP17Receiver.onKIP17Received.selector;
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

    function testCreateListing1(uint256 _tokenId) public {
        bool _inUSD = true;
        uint _price = 2000 ether; // answer/decimals * 10 ** 18 = 2000 ether
        vm.assume(_tokenId != 0 && _tokenId <= mapCount);
        marketplace.createListing(_inUSD, _tokenId, _price, false, 60, 0);
        (
            address seller,
            bool inUSD,
            uint256 tokenId,
            uint256 price,
            uint256 timestamp,
            bool isValid,
            bool isAuction,
            uint256 auctionTime
        ) = marketplace.listings(1);
        assertEq(seller, address(this), "Seller is not correct");
        assertEq(inUSD, _inUSD, "inUSD is not correct");
        assertEq(tokenId, _tokenId, "tokenId is not correct");
        assertEq(price, _price, "price is not correct");
        assertEq(timestamp, block.timestamp, "timestamp is not correct");
        assertEq(isValid, true, "isValid is not correct");
        assertEq(isAuction, false, "isAuction is not correct");
        assertEq(auctionTime, 60, "auctionEndTime is not correct");
        uint256 priceInETH = marketplace.getPrice(1);
        assertEq(priceInETH, 1 ether, "priceInETH is not correct");
    }

    function testCreateListing(
        bool _inUSD,
        uint256 _tokenId,
        uint256 _price // bool _isAuction, // uint256 _auctionTime, // uint96 _amount
    ) public {
        // vm.assume(!(_isAuction && _inUSD));
        bool _isAuction = false;
        uint256 _auctionTime = 0;
        uint96 _amount = 0;
        uint256 MAX_UINT256 = 2 ** 256 - 1;
        _price = bound(_price, 0, MAX_UINT256 / 10 ** (18 + decimals));
        vm.assume(_tokenId != 0 && _tokenId <= mapCount && _price != 0);
        marketplace.createListing(
            _inUSD,
            _tokenId,
            _price,
            _isAuction,
            _auctionTime,
            _amount
        );
        (
            address seller,
            bool inUSD,
            uint256 tokenId,
            uint256 price,
            uint256 timestamp,
            bool isValid,
            bool isAuction,
            uint256 auctionTime
        ) = marketplace.listings(1);
        assertEq(seller, address(this), "Seller is not correct");
        assertEq(inUSD, _inUSD, "inUSD is not correct");
        assertEq(tokenId, _tokenId, "tokenId is not correct");
        assertEq(price, _price, "price is not correct");
        assertEq(timestamp, block.timestamp, "timestamp is not correct");
        assertEq(isValid, true, "isValid is not correct");
        assertEq(isAuction, _isAuction, "isAuction is not correct");
        assertEq(auctionTime, _auctionTime, "auctionEndTime is not correct");
        uint256 priceInETH = marketplace.getPrice(1);
        uint256 expectedPriceInETH = (_price * 10 ** (decimals)) / uint(value);
        if (_inUSD) {
            assertEq(
                priceInETH,
                expectedPriceInETH,
                "priceInETH is not correct"
            );
        } else {
            assertEq(priceInETH, _price, "priceInETH is not correct");
        }
    }

    function testDeleteListing(
        bool _inUSD,
        uint256 _tokenId,
        uint256 _price // bool _isAuction, // uint256 _auctionTime, // uint96 _amount
    ) public {
        // vm.assume(!(_isAuction && _inUSD));
        bool _isAuction = false;
        uint256 _auctionTime = 0;
        uint96 _amount = 0;
        vm.assume(_tokenId != 0 && _tokenId <= mapCount && _price != 0);
        marketplace.createListing(
            _inUSD,
            _tokenId,
            _price,
            _isAuction,
            _auctionTime,
            _amount
        );
        marketplace.deleteListing(1);
        (, , , , , bool isValid, , ) = marketplace.listings(1);
        assertEq(isValid, false, "isValid is not correct");
    }

    function testBuyDirectListing(
        bool _inUSD,
        uint256 _tokenId,
        uint256 _price
    ) public {
        uint256 MAX_UINT256 = 2 ** 256 - 1;
        _price = bound(_price, 0, MAX_UINT256 / 10 ** (18 + decimals));
        vm.assume(_tokenId != 0 && _tokenId <= mapCount && _price != 0);
        marketplace.createListing(_inUSD, _tokenId, _price, false, 0, 0);
        map.approve(address(marketplace), _tokenId);
        vm.deal(address(this), marketplace.getPrice(1));
        marketplace.buyListing{value: marketplace.getPrice(1)}(1);
        (, , , , , bool isValid, , ) = marketplace.listings(1);
        assertEq(isValid, false, "isValid is not correct");
        assertEq(
            map.ownerOf(_tokenId),
            address(this),
            "map owner is not correct"
        );
    }
}

contract MockAggregatorV3 {
    uint80 immutable roundId;
    int256 immutable answer;
    uint256 immutable startedAt;
    uint256 immutable updatedAt;
    uint80 immutable answeredInRound;
    uint8 immutable i_decimals;

    constructor(
        uint80 _roundId,
        int256 _answer,
        uint256 _startedAt,
        uint256 _updatedAt,
        uint80 _answeredInRound,
        uint8 _decimals
    ) {
        roundId = _roundId;
        answer = _answer;
        startedAt = _startedAt;
        updatedAt = _updatedAt;
        answeredInRound = _answeredInRound;
        i_decimals = _decimals;
    }

    function latestRoundData()
        public
        view
        returns (uint80, int256, uint256, uint256, uint80)
    {
        return (roundId, answer, startedAt, updatedAt, answeredInRound);
    }

    function decimals() public view returns (uint8) {
        return i_decimals;
    }
}

contract MockWitnetRouter {
    int256 immutable i_value;
    uint256 immutable i_timestamp;
    uint256 immutable i_status;
    uint8 immutable i_decimals;

    constructor(
        int256 _value,
        uint256 _timestamp,
        uint256 _status,
        uint8 _decimals
    ) {
        i_value = _value;
        i_timestamp = _timestamp;
        i_status = _status;
        i_decimals = _decimals;
    }

    function valueFor(
        bytes32 /* feedId */
    )
        external
        view
        returns (int256 _value, uint256 _timestamp, uint256 _status)
    {
        return (i_value, i_timestamp, i_status);
    }

    function lookupDecimals(bytes4 /* feedId */) external view returns (uint8) {
        return i_decimals;
    }
}
