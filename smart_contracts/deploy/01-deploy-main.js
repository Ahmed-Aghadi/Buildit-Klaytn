const { network } = require("hardhat");
const hre = require("hardhat");

module.exports = async ({ getNamedAccounts, deployments }) => {
  const { deploy, log } = deployments;
  const { deployer } = await getNamedAccounts();
  const chainId = network.config.chainId;
  const accounts = await hre.ethers.getSigners();
  const account = accounts[0];
  const waitBlockConfirmations = 6;
  const utilsBaseUri = "https://www.example.com/utils/";
  const mapBaseUri = "https://www.example.com/map/";
  const size = 15;
  const perSize = 5;

  const utilsMintCount = 3;
  const utilsMintAmount = 1000;
  const transferUtilsAmount = 500;

  // marketplace can only be deployed on goerli or sepolia testnet. As price feed is not available on other testnets
  let registryAddress = "0xE16Df59B887e3Caa439E0b29B42bA2e7976FD8b2"; // mumbai
  let registrarAddress = "0x57A4a13b35d25EE78e084168aBaC5ad360252467"; // mumbai
  let eth_usd_priceFeedAddress = "0x694AA1769357215DE4FAC081bf1f309aDC325306"; // sepolia
  let linkAddress = "0x326C977E6efc84E512bB9C30f76E30c160eD06FB"; // mumbai
  let gasLimit = 999999;
  if (chainId == 80001) {
    // mumbai
    registryAddress = "0xE16Df59B887e3Caa439E0b29B42bA2e7976FD8b2";
    registrarAddress = "0x57A4a13b35d25EE78e084168aBaC5ad360252467";
    linkAddress = "0x326C977E6efc84E512bB9C30f76E30c160eD06FB";
  } else if (chainId == 4002) {
    // fantom testnet
    registryAddress = "0xE16Df59B887e3Caa439E0b29B42bA2e7976FD8b2";
    registrarAddress = "0x57A4a13b35d25EE78e084168aBaC5ad360252467";
    linkAddress = "0xfaFedb041c0DD4fA2Dc0d87a6B0979Ee6FA7af5F";
  } else if (chainId == 11155111) {
    // sepolia
    registryAddress = "0xE16Df59B887e3Caa439E0b29B42bA2e7976FD8b2";
    registrarAddress = "0x9a811502d843E5a03913d5A2cfb646c11463467A";
    eth_usd_priceFeedAddress = "0x694AA1769357215DE4FAC081bf1f309aDC325306";
    linkAddress = "0x779877A7B0D9E8603169DdbD7836e478b4624789";
  }

  log("----------------------------------------------------");
  const utilsArg = [utilsBaseUri];
  const utils = await deploy("Utils", {
    from: deployer,
    args: utilsArg,
    log: true,
    waitConfirmations: waitBlockConfirmations,
  });
  console.log("utils deployed to:", utils.address);
  log("----------------------------------------------------");
  const mapArg = [size, perSize, mapBaseUri, utils.address];
  const map = await deploy("Map", {
    from: deployer,
    args: mapArg,
    log: true,
    waitConfirmations: waitBlockConfirmations,
  });
  console.log("map deployed to:", map.address);
  log("----------------------------------------------------");
  const faucetArg = [];
  const faucet = await deploy("Faucet", {
    from: deployer,
    args: faucetArg,
    log: true,
    waitConfirmations: waitBlockConfirmations,
  });
  console.log("faucet deployed to:", faucet.address);
  log("----------------------------------------------------");
  const marketplaceArg = [
    eth_usd_priceFeedAddress,
    map.address,
    utils.address,
    linkAddress,
    registrarAddress,
    registryAddress,
    gasLimit,
  ];
  const marketplace = await deploy("Marketplace", {
    from: deployer,
    args: marketplaceArg,
    log: true,
    waitConfirmations: waitBlockConfirmations,
  });
  console.log("marketplace deployed to:", marketplace.address);
  log("----------------------------------------------------");
  console.log("Minting Utils...");
  await mintUtils(account, utils.address, utilsMintCount, utilsMintAmount);
  log("----------------------------------------------------");
  console.log("Transfering Utils to Faucet...");
  await transferToFaucet(
    account,
    utils.address,
    faucet.address,
    utilsMintCount,
    transferUtilsAmount
  );
  log("----------------------------------------------------");
  try {
    console.log("Verifying for Utils...");
    await verify(utils.address, utilsArg);
    console.log("Verifying for Map...");
    await verify(map.address, mapArg);
    console.log("Verifying for Faucet...");
    await verify(faucet.address, faucetArg);
    console.log("Verifying for Marketplace...");
    await verify(marketplace.address, marketplaceArg);
  } catch (error) {
    console.log(error);
  }
  log("----------------------------------------------------");
};

const verify = async (contractAddress, args) => {
  console.log("Verifying contract...");
  try {
    await run("verify:verify", {
      address: contractAddress,
      constructorArguments: args,
    });
    console.log("verified");
  } catch (e) {
    if (e.message.toLowerCase().includes("already verified")) {
      console.log("Already Verified!");
    } else {
      console.log(e);
    }
  }
};

const mintUtils = async (account, utilsContractAddress, count, amount) => {
  const utilsContract = await hre.ethers.getContractAt(
    "Utils",
    utilsContractAddress,
    account
  );
  for (let i = 1; i <= count; i++) {
    const tx = await utilsContract.mint(amount);
    console.log("Minted Utils " + i + " TX:", tx.hash);
    const receipt = await tx.wait();
    console.log("Minted Utils " + i + " RECEIPT:", receipt.transactionHash);
  }
};

const transferToFaucet = async (
  account,
  utilsContractAddress,
  faucetContractAddress,
  count,
  amount
) => {
  const utilsContract = await hre.ethers.getContractAt(
    "Utils",
    utilsContractAddress,
    account
  );
  for (let i = 1; i <= count; i++) {
    const tx = await utilsContract.safeTransferFrom(
      account.address,
      faucetContractAddress,
      i,
      amount,
      "0x"
    );
    console.log("Transfered Utils " + i + " TX:", tx.hash);
    const receipt = await tx.wait();
    console.log("Transfered Utils " + i + " RECEIPT:", receipt.transactionHash);
  }
};

module.exports.tags = ["all", "main"];
