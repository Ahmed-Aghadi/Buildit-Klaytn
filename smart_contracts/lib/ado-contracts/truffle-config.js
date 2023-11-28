module.exports =
{
	plugins: ["truffle-security", "solidity-coverage"],
	networks:
	{
		coverage:
		{
			host: "localhost",
			port: 8555,          // <-- If you change this, also set the port option in .solcover.js.
			network_id: "*",
			gas: 0xFFFFFFFFFFF, // <-- Use this high gas value
			gasPrice: 0x01           // <-- Use this low gas price
		}
	},
	compilers: {
		solc: {
			version: "0.8.4",
			settings: {
				optimizer: {
					enabled: true,
					runs: 200
				}
			}
		}
	},
	mocha:
	{
		enableTimeouts: false
	}
};
