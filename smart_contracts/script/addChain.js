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
  const polygonZKEVMTestnetChainId = 1442;
  const seopliaChainId = 11155111;
  const goerliChainId = 5;

  // Utils contract addresses
  const mumbaiAddress = "0x4DeD7815977d7F3B0b58F578d6fa18f48DF8dE18";
  const polygonZKEVMTestnetAddress =
    "0x4f033bF08e610DDeBe5fA9707d5334Ad5c5A893e";
  const seopliaAddress = "0xCb5B8FB3a71C44dEBcF0C0E745CCBdF968682bbB";
  const goerliAddress = "0x52Cb4B27503848ABd8dd3629474835299E1E99af";

  const mumbai = {
    chainId: mumbaiChainId,
    chainAddress: mumbaiAddress,
  };
  const polygonZKEVMTestnet = {
    chainId: polygonZKEVMTestnetChainId,
    chainAddress: polygonZKEVMTestnetAddress,
  };
  const seoplia = {
    chainId: seopliaChainId,
    chainAddress: seopliaAddress,
  };
  const goerli = {
    chainId: goerliChainId,
    chainAddress: goerliAddress,
  };

  // Bridge is available from mumbai to seoplia and vice versa AND from polygonZKEVMTestnet to goerli and vice versa
  const addresses = {
    source: {
      ...(chainId == mumbai.chainId
        ? mumbai
        : chainId == polygonZKEVMTestnet.chainId
        ? polygonZKEVMTestnet
        : chainId == seoplia.chainId
        ? seoplia
        : goerli),
    },
    destination: {
      ...(chainId == mumbai.chainId
        ? seoplia
        : chainId == seoplia.chainId
        ? mumbai
        : chainId == polygonZKEVMTestnet.chainId
        ? goerli
        : polygonZKEVMTestnet),
    },
  };

  console.log("addresses : ", addresses);

  const isChainLxLy =
    chainId == polygonZKEVMTestnet.chainId || chainId == goerli.chainId;

  const utilsContractFactory = await hre.ethers.getContractFactory(
    isChainLxLy ? "src/UtilsLxLy.sol:Utils" : "src/UtilsCCIP.sol:Utils"
  );
  const utilsContract = await utilsContractFactory.attach(
    addresses.source.chainAddress
  );

  console.log("Utils contract created");
  console.log("Connecting user to Utils contract");
  const utils = await utilsContract.connect(deployer);
  console.log("User connected to Utils contract");
  const tx = await utils.setChain(
    addresses.destination.chainId,
    addresses.destination.chainAddress
  );

  console.log("----------------------------------");
  console.log(tx);
  const response = await tx.wait();
  console.log("----------------------------------");
  console.log(response);
  // console.log("address of entry : " + response.events[0].data)
}

addChain()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
