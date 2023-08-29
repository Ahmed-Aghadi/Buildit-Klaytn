const { ethers, network } = require("hardhat");
const hre = require("hardhat");

async function addChain() {
  accounts = await hre.ethers.getSigners();
  deployer = accounts[0];
  const chainId = network.config.chainId;
  console.log("Chain ID : " + chainId);
  console.log("Creating Utils contract");
  const utilsContractFactory = await hre.ethers.getContractFactory("Utils");
  const mumbaiName = "Polygon";
  const fantomTestnetName = "Fantom";
  const arbitrumGoerliName = "arbitrum";

  const mumbaiAddress = "0x1e32B261781Ed5aD7dA316f61074864De0a88eC7";
  const fantomTestnetAddress = "0x8E10a436eafE80B2388D56e8Bb4435C31C930dbf";
  const arbitrumGoerliAddress = "0x4A4e6Cc94507B6aD2c91aD765d3f5B566B15d895";

  // mumbai
  const utilsContract = await utilsContractFactory.attach(mumbaiAddress);

  // // fantom testnet
  // const utilsContract = await utilsContractFactory.attach(
  //   "0x8E10a436eafE80B2388D56e8Bb4435C31C930dbf"
  // );

  // // arbitrum goerli
  // const utilsContract = await utilsContractFactory.attach(
  //   "0x4A4e6Cc94507B6aD2c91aD765d3f5B566B15d895"
  // );

  console.log("Utils contract created");
  console.log("Connecting user to Utils contract");
  const utils = await utilsContract.connect(deployer);
  console.log("User connected to Utils contract");

  // mumbai
  const tx = await utils.setChain(arbitrumGoerliName, arbitrumGoerliAddress);

  // // fantom testnet
  // const tx = await utils.setChain(
  //   "Polygon",
  //   "0x1e32B261781Ed5aD7dA316f61074864De0a88eC7"
  // );

  // // arbitrum goerli
  // const tx = await utils.setChain(
  //   "Polygon",
  //   "0x1e32B261781Ed5aD7dA316f61074864De0a88eC7"
  // );

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
