const { network } = require("hardhat");

module.exports = async ({ getNamedAccounts, deployments }) => {
  const { deploy, log } = deployments;
  const { deployer } = await getNamedAccounts();
  const waitBlockConfirmations = 6;
  const utilsBaseUri = "https://www.example.com/utils/";
  const mapBaseUri = "https://www.example.com/map/";
  const size = 15;
  const perSize = 5;

  // log("----------------------------------------------------");
  // const utilsArg = [utilsBaseUri];
  // const utils = await deploy("Utils", {
  //   from: deployer,
  //   args: utilsArg,
  //   log: true,
  //   waitConfirmations: waitBlockConfirmations,
  // });
  // console.log("utils deployed to:", utils.address);
  // log("----------------------------------------------------");
  // const mapArg = [
  //   size,
  //   perSize,
  //   mapBaseUri,
  //   "0x4b22e4f5cfCb3e648a6F42Fa9D4E55985f9647D1",
  // ];
  // const map = await deploy("Map", {
  //   from: deployer,
  //   args: mapArg,
  //   log: true,
  //   waitConfirmations: waitBlockConfirmations,
  // });
  // console.log("map deployed to:", map.address);
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
  // console.log("Verifying for Utils...");
  // await verify(utils.address, utilsArg);
  // console.log("Verifying for Map...");
  // await verify(map.address, mapArg);
  console.log("Verifying for Faucet...");
  await verify(faucet.address, faucetArg);
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

module.exports.tags = ["all", "main"];
