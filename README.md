BuildIt is a metaverse project developed for the hackathon. It provides users with the ability to own virtual land within a map, place items on the land they own, and even sell the land to other users. The land is represented as ERC721 tokens, while the items are represented as ERC1155 tokens. All interactions within the metaverse are secured by smart contracts.

## Installation

Go to `smartcontract` folder and install foundry and hardhat.

```bash
cd smartcontracts/
forge init
npm install
```

To compile smart contracts:

```bash
forge compile --skip test --skip script

# Or If you want to compile test files

forge compile --via-ir
```

To run test on smart contracts:

```bash
forge test --via-ir
```

To deploy smart contracts:

```bash
npx hardhat deploy --network $ChainName
```

Note: Don't forget to make .env file, refer .env.example file.

## Smart Contracts ( Mantle Testnet )

| Contract                                                                                                             | Explorer Link                                                                                                                        |
| -------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| [Map.sol](https://github.com/Ahmed-Aghadi/BuildIt-cross-chain/blob/main/smart_contracts/src/Map.sol)                 | [0x9f85f13F5FFb3b1dcb7B318F1712b6f42A4CFFd4](https://explorer.testnet.mantle.xyz/address/0x9f85f13F5FFb3b1dcb7B318F1712b6f42A4CFFd4) |
| [Utils.sol](https://github.com/Ahmed-Aghadi/BuildIt-cross-chain/blob/main/smart_contracts/src/Utils.sol)             | [0x8e539DfdA07e5Bb63F6768eaDb800F01FC25C336](https://explorer.testnet.mantle.xyz/address/0x8e539DfdA07e5Bb63F6768eaDb800F01FC25C336) |
| [Faucet.sol](https://github.com/Ahmed-Aghadi/BuildIt-cross-chain/blob/main/smart_contracts/src/Faucet.sol)           | [0x69A46a7b195eb6C89454C41dE3e5Bd96C694D8FB](https://explorer.testnet.mantle.xyz/address/0x69A46a7b195eb6C89454C41dE3e5Bd96C694D8FB) |
| [Marketplace.sol](https://github.com/Ahmed-Aghadi/BuildIt-cross-chain/blob/main/smart_contracts/src/Marketplace.sol) | [0xcB82Ac8Ad0cd14DD4d6f6397B66Bf11dA538F12A](https://explorer.testnet.mantle.xyz/address/0xcB82Ac8Ad0cd14DD4d6f6397B66Bf11dA538F12A) |

## Axelar

| Chain                            | AxelarScan Link                                                                                                                                                                |
| -------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| Polygon Mumbai to Fantom Testnet | [0x6a75d12c64aa8eee1e747d62ccae645d981564ce32c22166e308bcaf058decb0](https://testnet.axelarscan.io/gmp/0x6a75d12c64aa8eee1e747d62ccae645d981564ce32c22166e308bcaf058decb0:34)  |
| Fantom Testnet to Polygon Mumbai | [0x245ffb0c9f17ef3c677f9ffb20604ec68839f7c3fbb3aa5b3b2cf5f54c8ebcad](https://testnet.axelarscan.io/gmp/0x245ffb0c9f17ef3c677f9ffb20604ec68839f7c3fbb3aa5b3b2cf5f54c8ebcad:2)   |
| Polygon Mumbai to Fantom Testnet | [0x5e92df374722bda02cde583c6f112421f38450e1a57fd93f4fbfb0e1c166af3e](https://testnet.axelarscan.io/gmp/0x5e92df374722bda02cde583c6f112421f38450e1a57fd93f4fbfb0e1c166af3e:119) |
| Fantom Testnet to Polygon Mumbai | [0x8591148343c3283188b7c83a9c7949091925868b2cce25c5e9db9932a74f7420](https://testnet.axelarscan.io/gmp/0x8591148343c3283188b7c83a9c7949091925868b2cce25c5e9db9932a74f7420:2)   |

### Experience

- **Simple to implement**: Docs are to the point, thus by just reading the simple example given in docs for General Message Passing (GMP), we're able to implement the logic according to our Game.
- **AxelarScan API and Explorer**: The API helps to query the gas fees required and status can also be queried which helps a lot to understand the flow. Explorer also helps a lot to understand the flow and makes it simple to test things around.

## Table of Contents

- [Inspiration](#inspiration)
- [What It Does](#what-it-does)
- [How We Built It](#how-we-built-it)
- [Challenges We Ran Into](#challenges-we-ran-into)
- [Accomplishments That We're Proud Of](#accomplishments-that-were-proud-of)
- [What We Learned](#what-we-learned)
- [What's Next for BuildIt](#whats-next-for-buildit)

## Inspiration

The inspiration behind BuildIt comes from the desire to create an immersive metaverse experience where users can explore, own, and customize virtual land. We wanted to empower users to express their creativity and engage with a virtual world where they have control over their own unique space.

## What It Does

BuildIt allows users to:

- Own virtual land within a map represented as ERC721 tokens.
- Place items on their owned land, such as buildings, roads, and other structures, represented as ERC1155 tokens.
- Sell their land to other users, transferring ownership and associated items.
- Connect their wallets (e.g., Metamask, Coinbase, WalletConnect) to interact with the metaverse.

When a user connects their wallet, the game fetches data from the smart contracts and highlights the portion of the map that the user owns. Users can then click the "Edit" button to place or remove items on their land. They have the option to cancel or confirm the changes, which updates the items in the appropriate locations. Smart contract checks ensure that users can only interact with the land they own.

In addition, BuildIt includes a marketplace where users can sell their land through direct listings or auctions. Chainlink automation can be utilized for auction listings, and if the chain supports Chainlink price feeds, the land can be sold in USD. The marketplace provides an easy and secure way for users to trade their land.

## How We Built It

BuildIt was built using the following technologies and tools:

- Unity: The game was developed using Unity and built for Webgl.
- Smart Contracts: Four smart contracts were developed using Foundry and Hardhat:
  - Map Contract: Responsible for the Lands in the Map, implemented as an ERC721 contract.
  - Utils Contract: Represents the items that can be placed on the land, implemented as an ERC1155 contract.
  - Faucet Contract: Allows users to obtain items for free initially. It is funded to provide items for judges and other participants.
  - Marketplace Contract: Facilitates land sales through direct listings and auctions.
- Map Size: The map size is determined in the smart contract, allowing the deployment of multiple maps with different sizes. The current deployment consists of a map with a size of 15 by 15 tiles, where each land is a 5 by 5 tile.
- Item Minting: Three items are minted in the Utils contract: road, house, and special item.
- Wallet Integration: Users can connect their wallets, such as Metamask, Coinbase, and WalletConnect, to interact with the metaverse.
- Gasless Transactions: All smart contracts implement ERC2771Context, enabling users to perform gasless transactions when the relayer is funded.

## Challenges We Ran Into

During the development of BuildIt, we encountered several challenges, including:

- Integrating Unity with the Ethereum blockchain and ensuring secure and efficient interactions between the game and smart contracts.
- Implementing ERC721 and ERC1155 token standards and handling the transfer of ownership between users and their land/items.
- Optimizing gas usage and transaction costs in smart contract deployments.
- Developing a user-friendly interface and seamless wallet integration for a smooth user experience.

## Accomplishments That We're Proud Of

Throughout the development process, we achieved several accomplishments that we're proud of, including:

- Successfully integrating the Unity game engine with the Blockchain and smart contracts.
- Creating a metaverse where users can own virtual land and customize it with various items.
- Implementing a marketplace where users can buy and sell land securely through direct listings and auctions.
- Enabling gasless transactions for users by implementing ERC2771Context in all smart contracts.
- Conducting comprehensive testing, including fuzz testing, to ensure the stability and reliability of the application.

## What We Learned

The development of BuildIt provided us with valuable learning experiences, including:

- Gaining in-depth knowledge of integrating smart contracts with Unity.
- Understanding the intricacies of token standards like ERC721 and ERC1155.
- Optimizing gas usage and transaction costs in smart contract deployments.
- Enhancing user experience through seamless wallet integration and fetching data from smart contracts.

## What's Next for BuildIt

BuildIt is an ongoing project, and we have exciting plans for its future:

- Adding multiple maps with different sizes to expand the metaverse and accommodate more users.
- Conducting further research on gasless transactions to reduce transaction costs and improve user experience.
- Exploring cross-chain integrations to enable interoperability with other blockchain platforms.
- Enhancing the variety of items and customizations available to users.
- Engaging with the community to gather feedback and implement new features based on user suggestions.

We are dedicated to continuously improving and expanding BuildIt to create a vibrant and immersive metaverse experience for all users.
