/** 
* This tests the oracle functions as they are called through the
* 
*/
const Web3 = require('web3');
const web3 = new Web3(new Web3.providers.WebsocketProvider('ws://localhost:8545'));  
const ADOMedianizer = artifacts.require("ADOMedianizer.sol");
const SampleOracle = artifacts.require("SampleOracle.sol");
var bytes = "0x2ecc80a3401165e1a04561d6ffe93662a31815d89cd63b00f248efd1cce47894";

contract('ADO Medianizer Tests', function(accounts) {
  let adoMedianizer;
  let sampleOracle1,sampleOracle2,sampleOracle3;

    beforeEach('Setup contract for each test', async function () {
        sampleOracle1 = await SampleOracle.new()
        sampleOracle2 = await SampleOracle.new()
        sampleOracle3 = await SampleOracle.new()
        //deploy medianizer contract
    	adoMedianizer = await ADOMedianizer.new()
        //add oracles to medianizer
    	await adoMedianizer.addOracle(sampleOracle1.address)
    	await adoMedianizer.addOracle(sampleOracle2.address)
    	await adoMedianizer.addOracle(sampleOracle3.address)
    })
    it("Test ADOMedianizer.valueFor", async function(){
        await sampleOracle1.setValue(bytes,1000)
        await sampleOracle2.setValue(bytes,2000)
        await sampleOracle3.setValue(bytes,3000)
        let vars = await adoMedianizer.valueFor(bytes)
        assert(vars[0] == 2000)
        assert(vars[1] > 0)
        assert(vars[2] == 200)
    })
    it("Test remove oracles", async function(){
        let vars = await adoMedianizer.getOracles();
        assert(vars[0] == sampleOracle1.address);
        assert(vars[1] == sampleOracle2.address);
        assert(vars[2] == sampleOracle3.address);
        assert(vars.length == 3);
        await adoMedianizer.removeOracle(sampleOracle1.address);
        vars = await adoMedianizer.getOracles();
        assert(vars[0] == sampleOracle3.address);
        assert(vars[1] == sampleOracle2.address);
        assert(vars.length == 2);
    })
    it("Test no oracles", async function(){  
        await adoMedianizer.removeOracle(sampleOracle1.address);
        await adoMedianizer.removeOracle(sampleOracle2.address);
        await adoMedianizer.removeOracle(sampleOracle3.address);  
        let vars = await adoMedianizer.getOracles();
        assert(vars.length == 0, "should be nor oracles"); 
        vars = await adoMedianizer.valueFor(bytes)
        assert(vars[0] == 0);
        assert(vars[1] == 0);
        assert(vars[2] == 404)
    })
    it("Test no value", async function(){
        let vars = await adoMedianizer.valueFor(bytes)
        assert(vars[0] == 0);
        assert(vars[1] == 0);
        assert(vars[2] == 400)
    })
    it("Test ownership change", async function(){
        assert(await adoMedianizer.owner.call() == accounts[0])
        await adoMedianizer.transferOwnership(accounts[1])
        assert(await adoMedianizer.owner.call() == accounts[1])
    })
    it("Test get oracles", async function(){
        let vars2 = await adoMedianizer.getOracles();
        assert(vars2[0] == sampleOracle1.address);
        assert(vars2[1] == sampleOracle2.address);
        assert(vars2[2] == sampleOracle3.address);
        assert(vars2.length == 3);
    })
 });