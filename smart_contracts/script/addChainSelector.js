const { ethers, network } = require("hardhat");
const hre = require("hardhat");

// Run: npx hardhat run script/addChain.js --network sourceChain
// Example: npx hardhat run script/addChain.js --network arbitrumGoerli
// you can get sourceChain from hardhat.config.js
async function addChain() {
  accounts = await hre.ethers.getSigners();
  deployer = accounts[0];
  const chainId = network.config.chainId;
  console.log("Chain ID : " + chainId);
  console.log("Creating Utils contract");

  const mumbaiChainId = 80001;
  const seopliaChainId = 11155111;

  // Chainlink CCIP Bridge uses chainId 12532609583862916517 for Polygon Mumbai and 16015286601757825753 for Sepolia
  const replaceChainId = (_chainId) =>
    _chainId == mumbaiChainId ? "12532609583862916517" : "16015286601757825753";

  // Utils contract addresses
  const mumbaiAddress = "0x82BFe300311f0324423406382fC6bdccbC2BaB47";
  const seopliaAddress = "0xE9cEAe69B724F4340Ca3D6D2F0D147b0Bc3E1978";

  const mumbai = {
    chainId: mumbaiChainId,
    chainAddress: mumbaiAddress,
  };
  const seoplia = {
    chainId: seopliaChainId,
    chainAddress: seopliaAddress,
  };

  // Bridge is available from mumbai to seoplia and vice versa AND from polygonZKEVMTestnet to goerli and vice versa
  const addresses = {
    source: {
      ...(chainId == mumbai.chainId ? mumbai : seoplia),
    },
    destination: {
      ...(chainId == mumbai.chainId ? seoplia : mumbai),
    },
  };

  console.log("addresses: ", addresses);

  const isChainCCIP = chainId == mumbai.chainId || chainId == seoplia.chainId;

  if (!isChainCCIP) {
    console.log("Chain is not CCIP");
    return;
  }

  const utilsContractFactory = await hre.ethers.getContractFactory(
    "src/UtilsCCIP.sol:Utils"
  );
  const utilsContract = await utilsContractFactory.attach(
    addresses.source.chainAddress
  );

  console.log("Utils contract created");
  console.log("Connecting user to Utils contract");
  const utils = await utilsContract.connect(deployer);
  console.log("User connected to Utils contract");

  console.log("Setting chain id to chain selector");

  console.log(
    "transaction arguments: ",
    addresses.destination.chainId,
    replaceChainId(addresses.destination.chainId)
  );

  let tx = await utils.setChainSelector(
    addresses.destination.chainId,
    replaceChainId(addresses.destination.chainId)
  );

  console.log("----------------------------------");
  console.log(tx);
  let response = await tx.wait();
  console.log("----------------------------------");
  console.log(response);

  console.log("Setting selector chain to chain selector");

  console.log(
    "transaction arguments: ",
    replaceChainId(addresses.destination.chainId),
    replaceChainId(addresses.destination.chainId)
  );

  tx = await utils.setChainSelector(
    replaceChainId(addresses.destination.chainId),
    replaceChainId(addresses.destination.chainId)
  );

  console.log("----------------------------------");
  console.log(tx);
  response = await tx.wait();
  console.log("----------------------------------");
  console.log(response);
}

addChain()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
