// SPDX-License-Identifier: UNLICENSED
pragma solidity ^0.8.13;

import {ERC1155TokenReceiver} from "solmate/tokens/ERC1155.sol";
import {ERC721TokenReceiver} from "solmate/tokens/ERC721.sol";
import "chainlink/v0.8/interfaces/AggregatorV3Interface.sol";
import "./Map.sol";
import "./Utils.sol";
import {AutomationRegistryInterface, State, Config} from "chainlink/v0.8/interfaces/AutomationRegistryInterface1_2.sol";
import {LinkTokenInterface} from "chainlink/v0.8/interfaces/LinkTokenInterface.sol";

contract Marketplace {
    error InvalidTokenId();
    error InvalidListingId();
    error InvalidListing();
    error ValidListing();
    error InvalidPrice();
    error NotTokenOwner();
    error NotListingOwner();
    error NotEnoughFunds();
    error NotAuction();
    error InvalidBid();
    error AuctionNotOver();
    error AuctionOver();
    error USDNotSupportedForAuction();
    error AuctionCantBeBought();
    error NotHighestBidder();
    struct Listing {
        address seller;
        bool inUSD;
        uint256 tokenId;
        uint256 price;
        uint256 timestamp;
        bool isValid;
        bool isAuction;
        uint256 aucionTime;
    }
    struct Bid {
        address bidder;
        uint256 amount;
    }
    AggregatorV3Interface internal eth_usd_priceFeed;
    Map internal map;
    Utils internal utils;
    uint public listingCount = 0;

    mapping(uint256 => Listing) public listings;
    mapping(address => uint256) public balances;
    mapping(uint256 => Bid) public highestBid;
    mapping(address => uint256) public auctionBalance;

    constructor(
        address eth_usd_priceFeedAddress,
        address mapAddress,
        address utilsAddress
    ) {
        eth_usd_priceFeed = AggregatorV3Interface(eth_usd_priceFeedAddress);
        map = Map(mapAddress);
        utils = Utils(utilsAddress);
    }

    /*
     * @dev If isAuction is true, the price is the minimum bid
     * @dev auctionTime is the time in seconds for which the auction will run, so timestamp + auctionTime is the end time
     * @dev If isAuction is false, the price is the fixed price and auctionTime is ignored
     * @dev For auction, isUSD should be false
     */
    function createListing(
        bool inUSD,
        uint256 tokenId,
        uint256 price,
        bool isAuction,
        uint256 auctionTime
    ) public {
        if (price <= 0) revert InvalidPrice();
        if (tokenId <= 0) revert InvalidTokenId();
        if (map.ownerOf(tokenId) != msg.sender) revert NotTokenOwner();
        if (isAuction && inUSD) revert USDNotSupportedForAuction();
        listingCount++;
        listings[listingCount] = Listing(
            msg.sender,
            inUSD,
            tokenId,
            price,
            block.timestamp,
            true,
            isAuction,
            auctionTime
        );
    }

    function deleteListing(uint listingId) public {
        if (isListingValid(listingId) == false) revert InvalidListing();
        if (map.ownerOf(listings[listingId].tokenId) != msg.sender)
            revert NotListingOwner();
        listings[listingId].isValid = false;
    }

    function buyListing(uint listingId) public payable {
        if (isListingValid(listingId) == false) revert InvalidListing();
        if (listings[listingId].isAuction) revert AuctionCantBeBought();
        uint price = getPrice(listingId);
        if (msg.value < price) revert NotEnoughFunds();
        uint excess = msg.value - price;
        if (excess > 0) {
            balances[msg.sender] += excess;
        }
        balances[listings[listingId].seller] += price;
        map.safeTransferFrom(
            listings[listingId].seller,
            msg.sender,
            listings[listingId].tokenId
        );
        listings[listingId].isValid = false;
    }

    function bid(uint listingId) public payable {
        if (listings[listingId].isValid == false) revert InvalidListing();
        if (listings[listingId].isAuction == false) revert NotAuction();
        if (msg.value <= highestBid[listingId].amount) revert InvalidBid();
        if (highestBid[listingId].amount > 0) {
            balances[highestBid[listingId].bidder] += highestBid[listingId]
                .amount;
            auctionBalance[highestBid[listingId].bidder] -= highestBid[
                listingId
            ].amount;
        }
        auctionBalance[msg.sender] += msg.value;
        highestBid[listingId] = Bid(msg.sender, msg.value);
    }

    /*
     * @dev If the auction is over and the seller is approved for all, the highest bidder will get the token
     * @dev If the auction is over and the seller is not approved for all, the highest bidder can withdraw the funds
     * @dev If the auction is over and the seller have deleted the listing, the highest bidder can withdraw the funds
     */
    function calculateWinner(uint listingId) public {
        if (listings[listingId].isAuction == false) revert NotAuction();
        if (
            block.timestamp <=
            listings[listingId].timestamp + listings[listingId].aucionTime
        ) revert AuctionNotOver();
        if (highestBid[listingId].amount <= 0) revert NotEnoughFunds();
        if (listings[listingId].isValid == false) {
            if (highestBid[listingId].amount > 0) {
                _invalidateAuctionBid(listingId);
            } else {
                revert InvalidListing();
            }
        }
        if (
            map.isApprovedForAll(listings[listingId].seller, address(this)) ==
            false
        ) {
            _invalidateAuctionBid(listingId);
        } else {
            if (listings[listingId].isValid == false) revert InvalidListing();
            balances[listings[listingId].seller] += highestBid[listingId]
                .amount;
            // not safe transfer from because calculate winner will be called by automation and it shouldn't revert
            map.transferFrom(
                listings[listingId].seller,
                highestBid[listingId].bidder,
                listings[listingId].tokenId
            );
            listings[listingId].isValid = false;
        }
    }

    /*
     * @dev If the auction is over but the seller is not approved for all or seller have deleted the listing, the highest bidder can withdraw the funds
     * @dev If the auction is not over but seller have deleted the listing, the highest bidder can withdraw the funds
     */
    function invalidateAuctionBid(uint listingId) public {
        if (listings[listingId].isAuction == false) revert NotAuction();
        if (
            block.timestamp <=
            listings[listingId].timestamp + listings[listingId].aucionTime
        ) {
            if (listings[listingId].isValid) {
                revert ValidListing();
            } else {
                revert AuctionNotOver();
            }
        } else {
            if (
                map.isApprovedForAll(listings[listingId].seller, address(this))
            ) {
                if (listings[listingId].isValid) {
                    revert ValidListing();
                }
            }
        }
        if (highestBid[listingId].amount <= 0) revert NotEnoughFunds();
        _invalidateAuctionBid(listingId);
    }

    function _invalidateAuctionBid(uint listingId) private {
        auctionBalance[highestBid[listingId].bidder] -= highestBid[listingId]
            .amount;
        balances[highestBid[listingId].bidder] += highestBid[listingId].amount;
        highestBid[listingId].amount = 0;
    }

    function withdraw() public {
        uint amount = balances[msg.sender];
        if (amount <= 0) revert NotEnoughFunds();
        balances[msg.sender] = 0;
        payable(msg.sender).transfer(amount);
    }

    /*
     * @dev Returns the price of a listing in ETH
     */
    function getPrice(uint listingId) public view returns (uint256) {
        if (listingId <= 0 || listingId > listingCount)
            revert InvalidListingId();
        if (listings[listingId].inUSD) {
            uint decimals = eth_usd_priceFeed.decimals();
            (
                ,
                /* uint80 roundID */ int answer /*uint startedAt*/ /*uint timeStamp*/ /*uint80 answeredInRound*/,
                ,
                ,

            ) = eth_usd_priceFeed.latestRoundData();
            uint256 priceInEth = (listings[listingId].price *
                10 ** (18 + decimals)) / uint(answer);
            return priceInEth;
        } else {
            return listings[listingId].price;
        }
    }

    function isListingValid(uint listingId) public view returns (bool) {
        if (listingId <= 0 || listingId > listingCount)
            revert InvalidListingId();
        if (
            map.ownerOf(listings[listingId].tokenId) !=
            listings[listingId].seller
        ) {
            return false;
        }
        return listings[listingId].isValid;
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

    function onERC721Received(
        address,
        address,
        uint256,
        bytes calldata
    ) external virtual returns (bytes4) {
        return ERC721TokenReceiver.onERC721Received.selector;
    }
}
