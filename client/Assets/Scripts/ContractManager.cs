using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thirdweb;
using UnityEngine;
using Newtonsoft.Json;
using System.Text.Json;
using System.Numerics;
using UnityEngine.UIElements;

public struct Land
{
    public BigInteger xIndex;
    public BigInteger yIndex;
}

public class ContractManager : MonoBehaviour
{
    const int EMPTY = 0;
    const int ROAD = 1;
    const int HOUSE = 2;
    const int SPECIAL = 3;
    string mapContractAddress = "0x8b00E8128749B22E657620aB845186E6268515B4";
    string mapContractABI = "[\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"_size\",\r\n          \"type\": \"uint256\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"_perSize\",\r\n          \"type\": \"uint256\"\r\n        },\r\n        {\r\n          \"internalType\": \"string\",\r\n          \"name\": \"_baseUri\",\r\n          \"type\": \"string\"\r\n        },\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"_utilsAddress\",\r\n          \"type\": \"address\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"constructor\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"InvalidLength\",\r\n      \"type\": \"error\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"InvalidXIndex\",\r\n      \"type\": \"error\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"InvalidYIndex\",\r\n      \"type\": \"error\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"LandAlreadyOwned\",\r\n      \"type\": \"error\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"NotOwner\",\r\n      \"type\": \"error\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"SizeNotDivisibleByPerSize\",\r\n      \"type\": \"error\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"ZeroPerSize\",\r\n      \"type\": \"error\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"ZeroSize\",\r\n      \"type\": \"error\"\r\n    },\r\n    {\r\n      \"anonymous\": false,\r\n      \"inputs\": [\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"address\",\r\n          \"name\": \"owner\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"address\",\r\n          \"name\": \"spender\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"id\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"Approval\",\r\n      \"type\": \"event\"\r\n    },\r\n    {\r\n      \"anonymous\": false,\r\n      \"inputs\": [\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"address\",\r\n          \"name\": \"owner\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"address\",\r\n          \"name\": \"operator\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"indexed\": false,\r\n          \"internalType\": \"bool\",\r\n          \"name\": \"approved\",\r\n          \"type\": \"bool\"\r\n        }\r\n      ],\r\n      \"name\": \"ApprovalForAll\",\r\n      \"type\": \"event\"\r\n    },\r\n    {\r\n      \"anonymous\": false,\r\n      \"inputs\": [\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"address\",\r\n          \"name\": \"user\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"address\",\r\n          \"name\": \"newOwner\",\r\n          \"type\": \"address\"\r\n        }\r\n      ],\r\n      \"name\": \"OwnershipTransferred\",\r\n      \"type\": \"event\"\r\n    },\r\n    {\r\n      \"anonymous\": false,\r\n      \"inputs\": [\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"address\",\r\n          \"name\": \"from\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"address\",\r\n          \"name\": \"to\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"id\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"Transfer\",\r\n      \"type\": \"event\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"spender\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"id\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"approve\",\r\n      \"outputs\": [],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"owner\",\r\n          \"type\": \"address\"\r\n        }\r\n      ],\r\n      \"name\": \"balanceOf\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"baseUri\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"string\",\r\n          \"name\": \"\",\r\n          \"type\": \"string\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"getApproved\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"\",\r\n          \"type\": \"address\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"\",\r\n          \"type\": \"address\"\r\n        }\r\n      ],\r\n      \"name\": \"isApprovedForAll\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"bool\",\r\n          \"name\": \"\",\r\n          \"type\": \"bool\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"land\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"xIndex\",\r\n          \"type\": \"uint256\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"yIndex\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"landCount\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"landIds\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"map\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"xIndex\",\r\n          \"type\": \"uint256\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"yIndex\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"mint\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"name\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"string\",\r\n          \"name\": \"\",\r\n          \"type\": \"string\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256[]\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256[]\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256[]\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256[]\"\r\n        },\r\n        {\r\n          \"internalType\": \"bytes\",\r\n          \"name\": \"\",\r\n          \"type\": \"bytes\"\r\n        }\r\n      ],\r\n      \"name\": \"onERC1155BatchReceived\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"bytes4\",\r\n          \"name\": \"\",\r\n          \"type\": \"bytes4\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        },\r\n        {\r\n          \"internalType\": \"bytes\",\r\n          \"name\": \"\",\r\n          \"type\": \"bytes\"\r\n        }\r\n      ],\r\n      \"name\": \"onERC1155Received\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"bytes4\",\r\n          \"name\": \"\",\r\n          \"type\": \"bytes4\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"owner\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"\",\r\n          \"type\": \"address\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"id\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"ownerOf\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"owner\",\r\n          \"type\": \"address\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"perSize\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"x\",\r\n          \"type\": \"uint256\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"y\",\r\n          \"type\": \"uint256\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"utilId\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"placeItem\",\r\n      \"outputs\": [],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"uint256[]\",\r\n          \"name\": \"x\",\r\n          \"type\": \"uint256[]\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256[]\",\r\n          \"name\": \"y\",\r\n          \"type\": \"uint256[]\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256[]\",\r\n          \"name\": \"utilId\",\r\n          \"type\": \"uint256[]\"\r\n        }\r\n      ],\r\n      \"name\": \"placeItems\",\r\n      \"outputs\": [],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"x\",\r\n          \"type\": \"uint256\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"y\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"removeItem\",\r\n      \"outputs\": [],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"from\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"to\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"id\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"safeTransferFrom\",\r\n      \"outputs\": [],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"from\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"to\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"id\",\r\n          \"type\": \"uint256\"\r\n        },\r\n        {\r\n          \"internalType\": \"bytes\",\r\n          \"name\": \"data\",\r\n          \"type\": \"bytes\"\r\n        }\r\n      ],\r\n      \"name\": \"safeTransferFrom\",\r\n      \"outputs\": [],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"operator\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"bool\",\r\n          \"name\": \"approved\",\r\n          \"type\": \"bool\"\r\n        }\r\n      ],\r\n      \"name\": \"setApprovalForAll\",\r\n      \"outputs\": [],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"size\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"bytes4\",\r\n          \"name\": \"interfaceId\",\r\n          \"type\": \"bytes4\"\r\n        }\r\n      ],\r\n      \"name\": \"supportsInterface\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"bool\",\r\n          \"name\": \"\",\r\n          \"type\": \"bool\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"symbol\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"string\",\r\n          \"name\": \"\",\r\n          \"type\": \"string\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"id\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"tokenURI\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"string\",\r\n          \"name\": \"\",\r\n          \"type\": \"string\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"from\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"to\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"id\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"transferFrom\",\r\n      \"outputs\": [],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"newOwner\",\r\n          \"type\": \"address\"\r\n        }\r\n      ],\r\n      \"name\": \"transferOwnership\",\r\n      \"outputs\": [],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"utilCount\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"utilsAddress\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"\",\r\n          \"type\": \"address\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    }\r\n  ]";
    string utilsContractAddress = "0x4b22e4f5cfCb3e648a6F42Fa9D4E55985f9647D1";
    string utilsContractABI = "[\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"string\",\r\n          \"name\": \"_baseUri\",\r\n          \"type\": \"string\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"constructor\"\r\n    },\r\n    {\r\n      \"anonymous\": false,\r\n      \"inputs\": [\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"address\",\r\n          \"name\": \"owner\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"address\",\r\n          \"name\": \"operator\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"indexed\": false,\r\n          \"internalType\": \"bool\",\r\n          \"name\": \"approved\",\r\n          \"type\": \"bool\"\r\n        }\r\n      ],\r\n      \"name\": \"ApprovalForAll\",\r\n      \"type\": \"event\"\r\n    },\r\n    {\r\n      \"anonymous\": false,\r\n      \"inputs\": [\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"address\",\r\n          \"name\": \"user\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"address\",\r\n          \"name\": \"newOwner\",\r\n          \"type\": \"address\"\r\n        }\r\n      ],\r\n      \"name\": \"OwnershipTransferred\",\r\n      \"type\": \"event\"\r\n    },\r\n    {\r\n      \"anonymous\": false,\r\n      \"inputs\": [\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"address\",\r\n          \"name\": \"operator\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"address\",\r\n          \"name\": \"from\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"address\",\r\n          \"name\": \"to\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"indexed\": false,\r\n          \"internalType\": \"uint256[]\",\r\n          \"name\": \"ids\",\r\n          \"type\": \"uint256[]\"\r\n        },\r\n        {\r\n          \"indexed\": false,\r\n          \"internalType\": \"uint256[]\",\r\n          \"name\": \"amounts\",\r\n          \"type\": \"uint256[]\"\r\n        }\r\n      ],\r\n      \"name\": \"TransferBatch\",\r\n      \"type\": \"event\"\r\n    },\r\n    {\r\n      \"anonymous\": false,\r\n      \"inputs\": [\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"address\",\r\n          \"name\": \"operator\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"address\",\r\n          \"name\": \"from\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"address\",\r\n          \"name\": \"to\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"indexed\": false,\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"id\",\r\n          \"type\": \"uint256\"\r\n        },\r\n        {\r\n          \"indexed\": false,\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"amount\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"TransferSingle\",\r\n      \"type\": \"event\"\r\n    },\r\n    {\r\n      \"anonymous\": false,\r\n      \"inputs\": [\r\n        {\r\n          \"indexed\": false,\r\n          \"internalType\": \"string\",\r\n          \"name\": \"value\",\r\n          \"type\": \"string\"\r\n        },\r\n        {\r\n          \"indexed\": true,\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"id\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"URI\",\r\n      \"type\": \"event\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"balanceOf\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"address[]\",\r\n          \"name\": \"owners\",\r\n          \"type\": \"address[]\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256[]\",\r\n          \"name\": \"ids\",\r\n          \"type\": \"uint256[]\"\r\n        }\r\n      ],\r\n      \"name\": \"balanceOfBatch\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"uint256[]\",\r\n          \"name\": \"balances\",\r\n          \"type\": \"uint256[]\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"baseUri\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"string\",\r\n          \"name\": \"\",\r\n          \"type\": \"string\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"\",\r\n          \"type\": \"address\"\r\n        }\r\n      ],\r\n      \"name\": \"isApprovedForAll\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"bool\",\r\n          \"name\": \"\",\r\n          \"type\": \"bool\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"amount\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"mint\",\r\n      \"outputs\": [],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"owner\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"\",\r\n          \"type\": \"address\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"from\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"to\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256[]\",\r\n          \"name\": \"ids\",\r\n          \"type\": \"uint256[]\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256[]\",\r\n          \"name\": \"amounts\",\r\n          \"type\": \"uint256[]\"\r\n        },\r\n        {\r\n          \"internalType\": \"bytes\",\r\n          \"name\": \"data\",\r\n          \"type\": \"bytes\"\r\n        }\r\n      ],\r\n      \"name\": \"safeBatchTransferFrom\",\r\n      \"outputs\": [],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"from\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"to\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"id\",\r\n          \"type\": \"uint256\"\r\n        },\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"amount\",\r\n          \"type\": \"uint256\"\r\n        },\r\n        {\r\n          \"internalType\": \"bytes\",\r\n          \"name\": \"data\",\r\n          \"type\": \"bytes\"\r\n        }\r\n      ],\r\n      \"name\": \"safeTransferFrom\",\r\n      \"outputs\": [],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"operator\",\r\n          \"type\": \"address\"\r\n        },\r\n        {\r\n          \"internalType\": \"bool\",\r\n          \"name\": \"approved\",\r\n          \"type\": \"bool\"\r\n        }\r\n      ],\r\n      \"name\": \"setApprovalForAll\",\r\n      \"outputs\": [],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"bytes4\",\r\n          \"name\": \"interfaceId\",\r\n          \"type\": \"bytes4\"\r\n        }\r\n      ],\r\n      \"name\": \"supportsInterface\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"bool\",\r\n          \"name\": \"\",\r\n          \"type\": \"bool\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"address\",\r\n          \"name\": \"newOwner\",\r\n          \"type\": \"address\"\r\n        }\r\n      ],\r\n      \"name\": \"transferOwnership\",\r\n      \"outputs\": [],\r\n      \"stateMutability\": \"nonpayable\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"id\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"name\": \"uri\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"string\",\r\n          \"name\": \"\",\r\n          \"type\": \"string\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    },\r\n    {\r\n      \"inputs\": [],\r\n      \"name\": \"utilCount\",\r\n      \"outputs\": [\r\n        {\r\n          \"internalType\": \"uint256\",\r\n          \"name\": \"\",\r\n          \"type\": \"uint256\"\r\n        }\r\n      ],\r\n      \"stateMutability\": \"view\",\r\n      \"type\": \"function\"\r\n    }\r\n  ]";

    public static ContractManager Instance { get; private set; }
    public MapManager mapManager;
    public PlacementManager placementManager;
    public UIController uiController;
    public GameManager gameManager;
    public CanvasManager canvasManager;
    public StructureManager structureManager;
    public RoadManager roadManager;
    Contract mapContract, utilsContract;
    Task initializeMapTask;
    string walletAddress;
    int mapBalance;
    int size = 0;
    int perSize = 0;
    int landCount = 0;
    int[] landOwnedIds = null;
    Land[] landOwnedIndexes = null;
    bool[,] landOwned = null;
    int[,] map = null;

    public struct Index
    {
        public int xIndex;
        public int yIndex;
    }

    private void Awake()
    {
        // Single persistent instance at all times.

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Debug.LogWarning("Two ContractManager instances were found, removing this one.");
            Destroy(this.gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mapContract = ThirdwebManager.Instance.SDK.GetContract(mapContractAddress, mapContractABI);
        utilsContract = ThirdwebManager.Instance.SDK.GetContract(utilsContractAddress, utilsContractABI);
        initializeMapTask = InitializeMap();
        OnWalletConnect();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private async Task InitializeMap()
    {
        size = await getSize();
        landOwned = new bool[size, size];
        map = new int[size, size];
        placementManager.InitializeMap(size);
        mapManager.updateGridSize(size);
        perSize = await getPerSize();
        landCount = await getLandCount();
    }

    private async Task<int> getLandCount()
    {

        var landCount = await mapContract.Read<int>("landCount");
        Debug.Log("landCount: " + landCount);
        return landCount;
    }

    private async Task<int> getPerSize()
    {

        var perSize = await mapContract.Read<int>("perSize");
        Debug.Log("perSize: " + perSize);
        return perSize;
    }

    private async Task<int> getSize()
    {

        var size = await mapContract.Read<int>("size");
        Debug.Log("Size: " + size);
        return size;
    }
    private async Task<int> getMapBalance()
    {
        int balance = await mapContract.Read<int>("balanceOf", walletAddress);
        Debug.Log("map balance of user: " + balance);
        return balance;
    }

    private async Task<int[]> getLandOwnedIDs()
    {
        int[] ids = new int[mapBalance];
        int index = 0;
        for (int i = 1; i <= landCount; i++)
        {
            string owner = await mapContract.Read<string>("ownerOf", i);
            Debug.Log("owner of land " + i + ": " + owner);
            if (owner.ToLower() == walletAddress.ToLower())
            {
                ids[index++] = i;
            }
        }
        Debug.Log("ids: " + string.Join(",", ids));
        return ids;
    }

    private async Task<Land[]> getLandOwnedIndexes()
    {
        Land[] indexes = new Land[mapBalance];
        int i = 0;
        Debug.Log("landOwnedIds.Length: " + landOwnedIds.Length);
        foreach (int id in landOwnedIds)
        {
            Debug.Log("id: " + id);
            List<object> result = await mapContract.Read<List<object>>("land", id);
            Debug.Log("xIndex: " + result[0].ToString());
            Debug.Log("yIndex: " + result[1].ToString());
            Land myLandResult = new Land();
            myLandResult.xIndex = BigInteger.Parse(result[0].ToString());
            myLandResult.yIndex = BigInteger.Parse(result[1].ToString());
            indexes[i++] = myLandResult;
            i++;
        }
        Debug.Log("total i " + i);

        var s = "{";
        for (int x = 0; x < indexes.Length; x++)
        {
            s += "(" + indexes[x].xIndex + "," + indexes[x].yIndex + "),";
        }
        s += '}';
        Debug.Log("indexes: " + s);
        return indexes;
    }

    private void updateLandOwned(int size, int perSize, Land[] landOwnedIndexes)
    {
        landOwned = new bool[size, size];
        foreach (var land in landOwnedIndexes)
        {
            int x1 = (int)land.xIndex * perSize;
            int y1 = (int)land.yIndex * perSize;
            int x2 = x1 + perSize - 1;
            int y2 = y1 + perSize - 1;
            int tmpY1 = y1;
            while (x1 <= x2)
            {
                while (y1 <= y2)
                {
                    landOwned[x1, y1] = true;
                    y1++;
                }
                y1 = tmpY1;
                x1++;
            }
        }
    }

    private async void updateMap(int size)
    {
        map = new int[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                map[x,y] = await mapContract.Read<int>("map", x, y);
            }
        }
    }

    private void initializeMapItems()
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                int item = map[i,j];
                Vector3Int position = new Vector3Int(i,0,j);
                if (item == EMPTY)
                {
                    structureManager.DeleteItem(position,false);
                    roadManager.FixRoadPrefabsAfterDelete(position,false);
                } else if (item == ROAD)
                {
                    roadManager.PlaceRoad(position, false);
                    roadManager.FinishPlacingRoad();
                } else if (item == HOUSE)
                {
                    structureManager.PlaceHouse(position, false);
                } else if (item == SPECIAL)
                {
                    structureManager.PlaceSpecial(position, false);
                }
                else
                {
                    // SHOULDN't COME HERE AS I'VE NOT MINTED MORE ITEMS IN UTILS CONTRACT
                }
            }
        }
    }

    private async Task setUserData()
    {
        // walletAddress = await ThirdwebManager.Instance.SDK.wallet.GetAddress();
        //
        walletAddress = "0x0de82DCC40B8468639251b089f8b4A4400022e04";
        //
        mapBalance = await getMapBalance();
        uiController.updateMapBalance(mapBalance);
        landOwnedIds = await getLandOwnedIDs();
        Debug.Log("landOwnedIndexes");
        // landOwnedIndexes = await getLandOwnedIndexes();
        await initializeMapTask; // wait for map to be initialized if not already
        //
        landOwnedIndexes = new Land[2];
        landOwnedIndexes[0] = new Land { xIndex = 0, yIndex = 1 };
        landOwnedIndexes[1] = new Land { xIndex = 2, yIndex = 2 };
        //
        mapManager.highlightOwnedLands(landOwnedIndexes, perSize);
        updateLandOwned(size, perSize, landOwnedIndexes);
        updateMap(size);
        //
        map[0,0] = 1;
        map[1,0] = 1;
        map[2,0] = 1;
        map[3,0] = 1;
        map[4, 0] = 2;
        map[5, 0] = 2;
        map[6, 0] = 2;
        map[3, 3] = 3;
        map[4, 4] = 3;
        map[5, 5] = 3;
        map[1,5] = 3;
        map[2,5] = 2;
        map[3, 7] = 1;
        //
        initializeMapItems();
    }

    public bool userOwnsIndex(int x, int y)
    {
        return landOwned[x, y];
    }
    private async Task setData()
    {
        await setUserData();
        canvasManager.AttachClickListeners();
    }

    private void ResetData()
    {
        gameManager.ClearInputActions();
        canvasManager.RemoveClickListeners();
        walletAddress = "";
        mapBalance = 0;
        uiController.updateMapBalance(mapBalance);
    }

    public async void OnWalletConnect()
    {
        uiController.connectWalletText.enabled = false;
        await setData();
    }

    public void OnWalletDisconnect()
    {
        uiController.connectWalletText.enabled = true;
        ResetData();
    }

    public async void OnSwitchNetwork()
    {
        ResetData();
        await setData();
    }
}
