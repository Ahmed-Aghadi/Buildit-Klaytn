const { ethers, network } = require("hardhat");
const hre = require("hardhat");

const REVERSE_RECORDS_ADDRESS = "0x87f4483E4157a6592dd1d1546f145B5EE22c790a";
const REVERSE_RECORDS_ABI = ["function getName(address) view returns (string)"];

async function reverseResolve(address, provider) {
  let reverseRecords = new ethers.Contract(
    REVERSE_RECORDS_ADDRESS,
    REVERSE_RECORDS_ABI,
    provider
  );
  let name = await reverseRecords.getName(address);
  return name;
}

async function addChain() {
  accounts = await hre.ethers.getSigners();
  deployer = accounts[0];
  const chainId = network.config.chainId;
  console.log("Chain ID : " + chainId);

  const provider = new ethers.providers.JsonRpcProvider(
    "https://klaytn-mainnet-rpc.allthatnode.com:8551" // Klaytn mainnet RPC URL
  );

  console.log(
    await reverseResolve("0x0000ac03932ff48ee30209774e3f10fb0ac522e9", provider)
  );
}

addChain()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
