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
  let registryAddress = "0x0000000000000000000000000000000000000000";
  let registrarAddress = "0x0000000000000000000000000000000000000000";
  let linkAddress = "0x0000000000000000000000000000000000000000";
  let witnetPriceRouterAddress = "0x0000000000000000000000000000000000000000";
  let witnetPriceRouterUsdId4 = "0x00000000";
  let gasLimit = 999999;
  if (chainId == 1001) {
    // Klaytn Baobab testnet
    witnetPriceRouterAddress = "0xeD074DA2A76FD2Ca90C1508930b4FB4420e413B0";
    witnetPriceRouterUsdId4 = "0x6cc828d1";
  }

  log("----------------------------------------------------");
  const forwarderArg = [];
  const forwarder = await deploy("Forwarder", {
    from: deployer,
    args: forwarderArg,
    log: true,
    waitConfirmations: waitBlockConfirmations,
  });
  console.log("forwarder deployed to:", forwarder.address);
  log("----------------------------------------------------");
  const utilsArg = [utilsBaseUri, forwarder.address];
  const utils = await deploy("Utils", {
    from: deployer,
    args: utilsArg,
    log: true,
    waitConfirmations: waitBlockConfirmations,
  });
  console.log("utils deployed to:", utils.address);
  log("----------------------------------------------------");
  const mapArg = [size, perSize, mapBaseUri, utils.address, forwarder.address];
  const map = await deploy("Map", {
    from: deployer,
    args: mapArg,
    log: true,
    waitConfirmations: waitBlockConfirmations,
  });
  console.log("map deployed to:", map.address);
  log("----------------------------------------------------");
  const faucetArg = [forwarder.address];
  const faucet = await deploy("Faucet", {
    from: deployer,
    args: faucetArg,
    log: true,
    waitConfirmations: waitBlockConfirmations,
  });
  console.log("faucet deployed to:", faucet.address);
  log("----------------------------------------------------");
  const marketplaceArg = [
    witnetPriceRouterAddress,
    witnetPriceRouterUsdId4,
    map.address,
    utils.address,
    linkAddress,
    registrarAddress,
    registryAddress,
    gasLimit,
    forwarder.address,
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
    console.log("Verifying for Forwarder...");
    await verify(forwarder.address, forwarderArg);
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

    // wait for 10 second as in fantom testnet, it throws error (even at 3 seconds):
    // Error: nonce has already been used [ See: https://links.ethers.org/v5-errors-NONCE_EXPIRED ]
    await new Promise((r) => setTimeout(r, 10000));
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
      // { gasPrice: 1600000008 }
    );
    console.log("Transfered Utils " + i + " TX:", tx.hash);
    const receipt = await tx.wait();
    console.log("Transfered Utils " + i + " RECEIPT:", receipt.transactionHash);

    // wait for 10 second as in fantom testnet, it throws error:
    // Error: nonce has already been used [ See: https://links.ethers.org/v5-errors-NONCE_EXPIRED ]
    await new Promise((r) => setTimeout(r, 10000));
  }
};

module.exports.tags = ["all", "main"];
