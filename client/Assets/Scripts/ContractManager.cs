using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Thirdweb;
using UnityEngine;
using Newtonsoft.Json;
using System.Text.Json;
using System.Numerics;
using Nethereum.Contracts.QueryHandlers.MultiCall;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

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
    string mapContractAddressTheta = "0x9c0549334fbCDF0AFc85B83B2Bb4b422Ba38960f";
    string utilsContractAddressTheta = "0x3EC31e8B991FF0b3FfffD480e2A5F259B51DdF5c";
    string faucetContractAddressTheta = "0xc32Fb0cb3D6CcfD111146a2CA26A3FC774F3ECd2";
    string mapContractAddressMumbai = "0x3808000749fe82e14DEDd15510A2B1E1b076Ea5A";
    string utilsContractAddressMumbai = "0xC5f5C446fe1fEd411464C7d5E60c9B2d4c139F14";
    string faucetContractAddressMumbai = "0x4EE91D3dD15259dDfcf71d9F92e9007812B850f2";
    string mapContractAddress = "0x3808000749fe82e14DEDd15510A2B1E1b076Ea5A";
    string mapContractABI = "[{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_size\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"_perSize\",\"type\":\"uint256\"},{\"internalType\":\"string\",\"name\":\"_baseUri\",\"type\":\"string\"},{\"internalType\":\"address\",\"name\":\"_utilsAddress\",\"type\":\"address\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"inputs\":[],\"name\":\"InvalidLength\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"InvalidXIndex\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"InvalidYIndex\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"LandAlreadyOwned\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"NotOwner\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"SizeNotDivisibleByPerSize\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ZeroPerSize\",\"type\":\"error\"},{\"inputs\":[],\"name\":\"ZeroSize\",\"type\":\"error\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"baseUri\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"land\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"xIndex\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"yIndex\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"landCount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"landIds\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"map\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"xIndex\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"yIndex\",\"type\":\"uint256\"}],\"name\":\"mint\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"uint256[]\",\"name\":\"\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"\",\"type\":\"uint256[]\"},{\"internalType\":\"bytes\",\"name\":\"\",\"type\":\"bytes\"}],\"name\":\"onERC1155BatchReceived\",\"outputs\":[{\"internalType\":\"bytes4\",\"name\":\"\",\"type\":\"bytes4\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"\",\"type\":\"bytes\"}],\"name\":\"onERC1155Received\",\"outputs\":[{\"internalType\":\"bytes4\",\"name\":\"\",\"type\":\"bytes4\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"perSize\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"x\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"y\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"utilId\",\"type\":\"uint256\"}],\"name\":\"placeItem\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256[]\",\"name\":\"x\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"y\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"utilId\",\"type\":\"uint256[]\"}],\"name\":\"placeItems\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"x\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"y\",\"type\":\"uint256\"}],\"name\":\"removeItem\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"size\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"tokenURI\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"x\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"y\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"utilId\",\"type\":\"uint256\"}],\"name\":\"updateItem\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256[]\",\"name\":\"x\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"y\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"utilId\",\"type\":\"uint256[]\"}],\"name\":\"updateItems\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"utilCount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"utilsAddress\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";
    string utilsContractAddress = "0xC5f5C446fe1fEd411464C7d5E60c9B2d4c139F14";
    string utilsContractABI = "[{\"inputs\":[{\"internalType\":\"string\",\"name\":\"_baseUri\",\"type\":\"string\"}],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"user\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"indexed\":false,\"internalType\":\"uint256[]\",\"name\":\"amounts\",\"type\":\"uint256[]\"}],\"name\":\"TransferBatch\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"TransferSingle\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"string\",\"name\":\"value\",\"type\":\"string\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"URI\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address[]\",\"name\":\"owners\",\"type\":\"address[]\"},{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"}],\"name\":\"balanceOfBatch\",\"outputs\":[{\"internalType\":\"uint256[]\",\"name\":\"balances\",\"type\":\"uint256[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"baseUri\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"mint\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"amounts\",\"type\":\"uint256[]\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeBatchTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"uri\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"utilCount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";
    string faucetContractAddress = "0x4EE91D3dD15259dDfcf71d9F92e9007812B850f2";
    string faucetContractABI = "[{\"inputs\":[{\"internalType\":\"address\",\"name\":\"tokenAddress\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"tokenId\",\"type\":\"uint256\"}],\"name\":\"getToken\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"uint256[]\",\"name\":\"\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"\",\"type\":\"uint256[]\"},{\"internalType\":\"bytes\",\"name\":\"\",\"type\":\"bytes\"}],\"name\":\"onERC1155BatchReceived\",\"outputs\":[{\"internalType\":\"bytes4\",\"name\":\"\",\"type\":\"bytes4\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"\",\"type\":\"bytes\"}],\"name\":\"onERC1155Received\",\"outputs\":[{\"internalType\":\"bytes4\",\"name\":\"\",\"type\":\"bytes4\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";

    public static ContractManager Instance { get; private set; }
    public MapManager mapManager;
    public PlacementManager placementManager;
    public UIController uiController;
    public GameManager gameManager;
    public CanvasManager canvasManager;
    public StructureManager structureManager;
    public RoadManager roadManager;
    public TextMeshProUGUI loadingText;
    public Button utilsFaucetButton, landFaucetButton;
    Contract mapContract, utilsContract, faucetContract;
    Task initializeMapTask;
    string walletAddress;
    int roadBalance = 0;
    int houseBalance = 0;
    int specialBalance = 0;
    int roadEditedBalance = 0;
    int houseEditedBalance = 0;
    int specialEditedBalance = 0;
    int mapBalance = 0;
    int size = 0;
    int perSize = 0;
    int landCount = 0;
    int[] landOwnedIds = null;
    Land[] landOwnedIndexes = null;
    bool[,] landOwned = null;
    int[,] map = null;
    int[,] editedMap = null;
    bool isError = false;
    string errorText = "ERROR!!!";
    float timePassed = 0;
    float errorDuration = 5f;

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
        utilsFaucetButton.gameObject.SetActive(false);
        landFaucetButton.gameObject.SetActive(false);
        loadingText.enabled = false;
        OnWalletConnect();
        // Test();
    }
    private async Task Initialize()
    {
        var chainId = await ThirdwebManager.Instance.SDK.wallet.GetChainId();
        if(chainId == 80001)
        {
            mapContractAddress = mapContractAddressMumbai;
            utilsContractAddress = utilsContractAddressMumbai;
            faucetContractAddress = faucetContractAddressMumbai;
        }else if(chainId == 365)
        {
            mapContractAddress = mapContractAddressTheta;
            utilsContractAddress = utilsContractAddressTheta;
            faucetContractAddress = faucetContractAddressTheta;
        }
        mapContract = ThirdwebManager.Instance.SDK.GetContract(mapContractAddress, mapContractABI);
        utilsContract = ThirdwebManager.Instance.SDK.GetContract(utilsContractAddress, utilsContractABI);
        faucetContract = ThirdwebManager.Instance.SDK.GetContract(faucetContractAddress, faucetContractABI);
        initializeMapTask = InitializeMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (isError)
        {
            loadingText.enabled = true;
            loadingText.text = errorText;
            timePassed += Time.deltaTime;
            if (timePassed >= errorDuration)
            {
                isError = false;
                timePassed = 0;
                errorText = "ERROR!!!";
                loadingText.text = "LOADING: 0%";
                loadingText.enabled = false;
            }
        }
    }

    private async Task InitializeMap()
    {
        mapManager.destroyHighlights();
        size = await getSize();
        landOwned = new bool[size, size];
        map = new int[size, size];
        editedMap = new int[size, size];
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

    private async Task setItemBalances()
    {

        Debug.Log("ROAD BALANCE QUERYING...");
        string roadBalanceString = await utilsContract.ERC1155.BalanceOf(walletAddress, ROAD.ToString());
        Debug.Log("ROAD BALANCE STRING " + roadBalanceString);
        roadBalance = int.Parse(roadBalanceString);
        Debug.Log("roadBalance: " + roadBalance);
        houseBalance = await utilsContract.Read<int>("balanceOf", walletAddress, HOUSE);
        specialBalance = await utilsContract.Read<int>("balanceOf", walletAddress, SPECIAL);
        Debug.Log("item balances: " + roadBalance + " " + houseBalance + " " + specialBalance);
        if (roadBalance == 0 || houseBalance == 0 || specialBalance == 0)
        {
            Debug.Log("INSUFFICIENT UTILS BALANCE");
            utilsFaucetButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("SUFFICIENT UTILS BALANCE");
            utilsFaucetButton.gameObject.SetActive(false);
        }
        roadEditedBalance = roadBalance;
        houseEditedBalance = houseBalance;
        specialEditedBalance = specialBalance;
        Debug.Log("item edited balances: " + roadEditedBalance + " " + houseEditedBalance + " " + specialEditedBalance);
        uiController.updateRoadBalance(roadEditedBalance);
        uiController.updateHouseBalance(houseEditedBalance);
        uiController.updateSpecialBalance(specialEditedBalance);
    }
    private async Task<int> getMapBalance()
    {
        Debug.Log("TEST101010!!! " + walletAddress);
        int balance = await mapContract.Read<int>("balanceOf", walletAddress);
        Debug.Log("TEST111111!!!");
        Debug.Log("map balance of user: " + balance);
        if (balance == 0)
        {
            Debug.Log("INSUFFICIENT MAP BALANCE");
            landFaucetButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("SUFFICIENT MAP BALANCE");
            landFaucetButton.gameObject.SetActive(false);
        }
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

    private async Task updateMap(int size)
    {
        loadingText.enabled = true;
        loadingText.text = "Loading: 0%";
        map = new int[size, size];
        for (int x = 0; x < size; x++)
        {
            /*if(x % 2 == 0 || x % 3 == 0)
            {
                continue;
            }*/
            Task[] tasks = new Task[size];
            int index = 0;
            for (int y = 0; y < size; y++)
            {
                loadingText.text = "Loading: " + ((int)((x * size + y) * 100f / (size * size))) + "%";
                tasks[index++] = updateMapIndex(x, y);

            }
            // await Task.WhenAll(tasks);
            // now you are at the UI thread
            foreach (var t in tasks)
            {
                try
                {
                    await t;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
        loadingText.text = "Loading: 100%";
        loadingText.enabled = false;
        // await Task.WhenAll(tasks.Where(t => t != null).ToArray()); // if tasks returns null
        // await Task.WhenAll(tasks);
    }

    private async Task<int> updateMapIndex(int x, int y)
    {
        try
        {
            Debug.Log("map getting for: " + x + "," + y);
            map[x, y] = await mapContract.Read<int>("map", x, y);
            Debug.Log("Got: " + x + "," + y + "," + map[x, y]);
            return map[x, y];
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        return 0;
    }

    private void initializeMapItems()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Debug.Log("add: " + i + "," + j + "," + map[i, j]);
                int item = map[i, j];
                Vector3Int position = new Vector3Int(i, 0, j);
                if (item == EMPTY)
                {
                    structureManager.DeleteItem(position, false);
                    roadManager.FixRoadPrefabsAfterDelete(position, false);
                }
                else if (item == ROAD)
                {
                    roadManager.PlaceRoad(position, false);
                    roadManager.FinishPlacingRoad();
                }
                else if (item == HOUSE)
                {
                    structureManager.PlaceHouse(position, false);
                }
                else if (item == SPECIAL)
                {
                    structureManager.PlaceSpecial(position, false);
                }
                else
                {
                    // SHOULDN'T COME HERE AS I'VE NOT MINTED MORE ITEMS IN UTILS CONTRACT
                    Debug.Log("SHOULDN'T COME HERE AS I'VE NOT MINTED MORE ITEMS IN UTILS CONTRACT: " + i + "," + j + "," + map[i,j]);
                }
            }
        }
    }

    private async Task setUserData(bool queryMap = true)
    {
        mapManager.destroyHighlights();
        Debug.Log("TEST666!!!");
        // walletAddress = await ThirdwebManager.Instance.SDK.wallet.GetAddress();
        //
        walletAddress = "0x4CA5FE129837E965e49b507cfE36c0dc574e8864";
        //
        Debug.Log("TEST777!!!");
        await setItemBalances();
        Debug.Log("TEST888!!!");
        mapBalance = await getMapBalance();
        Debug.Log("TEST999!!!");
        Debug.Log("TEST121212!!!");
        uiController.updateMapBalance(mapBalance);
        Debug.Log("TEST131313!!!");
        landOwnedIds = await getLandOwnedIDs();
        Debug.Log("TEST141414!!!");
        Debug.Log("landOwnedIndexes");
        // landOwnedIndexes = await getLandOwnedIndexes();
        await initializeMapTask; // wait for map to be initialized if not already
        //
        landOwnedIndexes = new Land[1];
        landOwnedIndexes[0] = new Land { xIndex = 0, yIndex = 0 };
        landOwnedIndexes = new Land[2];
        landOwnedIndexes[0] = new Land { xIndex = 0, yIndex = 1 };
        landOwnedIndexes[1] = new Land { xIndex = 2, yIndex = 2 };
        //
        mapManager.highlightOwnedLands(landOwnedIndexes, perSize);
        updateLandOwned(size, perSize, landOwnedIndexes);
        if (queryMap)
        {
            // await updateMap(size);
            //
            map[2, 7] = 3;
            map[3, 8] = 2;
            map[4, 9] = 1;
            /*map[0, 0] = 1;
            map[1, 0] = 1;
            map[2, 0] = 1;
            map[3, 0] = 1;
            map[4, 0] = 2;
            map[5, 0] = 2;
            map[6, 0] = 2;
            map[3, 3] = 3;
            map[4, 4] = 3;
            map[5, 5] = 3;
            map[1, 5] = 3;
            map[2, 5] = 2;
            map[3, 7] = 1;*/
            //
            initializeMapItems();
            // DebugMap();
        }
    }

    private void DebugMap()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Debug.Log("map(" + i + "," + j + ") = " + map[i, j] + " | " + "editedMap(" + i + "," + j + ") = " + editedMap[i, j]);
            }
        }
    }

    public void updateEditedMap(Vector3Int position, CellType cell)
    {
        // Debug.Log("update: " + position.x + "," + position.z + "," + cell);
        if (cell == CellType.Empty)
        {
            editedMap[position.x, position.z] = EMPTY;
        }
        else if (cell == CellType.Road)
        {
            editedMap[position.x, position.z] = ROAD;
        }
        else if (cell == CellType.Structure)
        {
            editedMap[position.x, position.z] = HOUSE;
        }
        else if (cell == CellType.SpecialStructure)
        {
            editedMap[position.x, position.z] = SPECIAL;
        }
        else
        {
            // shouldn't come here
        }
    }

    public bool AddRoad()
    {
        if (roadEditedBalance == 0) return false;
        roadEditedBalance--;
        uiController.updateRoadBalance(roadEditedBalance);
        return true;
    }

    public bool AddHouse()
    {
        if (houseEditedBalance == 0) return false;
        houseEditedBalance--;
        uiController.updateHouseBalance(houseEditedBalance);
        return true;
    }

    public bool AddSpecial()
    {
        if (specialEditedBalance == 0) return false;
        specialEditedBalance--;
        uiController.updateSpecialBalance(specialEditedBalance);
        return true;
    }

    public void DeleteItem(Vector3Int position)
    {
        int x = position.x;
        int y = position.z;
        if (editedMap[x, y] == ROAD)
        {
            roadEditedBalance++;
            uiController.updateRoadBalance(roadEditedBalance);
        }
        else if (editedMap[x, y] == HOUSE)
        {
            houseEditedBalance++;
            uiController.updateHouseBalance(houseEditedBalance);
        }
        else if (editedMap[x, y] == SPECIAL)
        {
            specialEditedBalance++;
            uiController.updateSpecialBalance(specialEditedBalance);
        }
        else
        {
            Debug.Log("NO MATCH: " + editedMap[x, y] + " , " + x + " , " + y);
        }
    }

    public void CancelClicked()
    {
        gameManager.ClearInputActions();
        roadEditedBalance = roadBalance;
        houseEditedBalance = houseBalance;
        specialEditedBalance = specialBalance;
        uiController.updateRoadBalance(roadEditedBalance);
        uiController.updateHouseBalance(houseEditedBalance);
        uiController.updateSpecialBalance(specialEditedBalance);
        editedMap = new int[size, size];
        copyMapToEditedMap();
        initializeMapItems();
        // DebugMap();
    }

    private void copyMapToEditedMap()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                editedMap[i,j] = map[i,j];
            }
        }
    }

    public async Task confirmMapUpdates()
    {
        int[] x;
        int[] y;
        int[] utilId;
        int len = 0;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (map[i, j] != editedMap[i, j])
                {
                    len++;
                }
            }
        }
        if (len == 0)
        {
            return;
        }
        loadingText.enabled = true;
        loadingText.text = "Loading: 0%";
        canvasManager.RemoveClickListeners();
        x = new int[len];
        y = new int[len];
        utilId = new int[len];
        int index = 0;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (map[i, j] != editedMap[i, j])
                {
                    x[index] = i;
                    y[index] = j;
                    utilId[index] = editedMap[i, j];
                    Debug.Log("diff: " + i + "," + j + "," + map[i, j] + "," + editedMap[i, j]);
                    index++;
                }
            }
        }
        loadingText.text = "Loading: 10%";
        try
        {
            bool result = await utilsContract.Read<bool>("isApprovedForAll", walletAddress, mapContractAddress);
            if (!result)
            {
                loadingText.text = "Loading: 20%";
                await utilsContract.Write("setApprovalForAll", mapContractAddress, true);
            }
            loadingText.text = "Loading: 65%";
            await mapContract.Write("updateItems", x, y, utilId);
            loadingText.text = "Loading: 100%";
            loadingText.enabled = false;
            await setUserData();
        }
        catch(Exception e)
        {
            loadingText.text = "ERROR!!!";
            isError = true;
            Debug.LogError(e);
        }
        canvasManager.AttachClickListeners();
    }

    public bool userOwnsIndex(int x, int y)
    {
        return landOwned[x, y];
    }
    private async Task setData()
    {
        await Initialize();
        Debug.Log("TEST444!!!");
        await setUserData();
        Debug.Log("TEST555!!!");
        canvasManager.AttachClickListeners();
    }

    private void ResetData()
    {
        gameManager.ClearInputActions();
        canvasManager.RemoveClickListeners();
        walletAddress = "";
        mapBalance = 0;
        map = new int[size,size];
        editedMap = new int[size, size];
        initializeMapItems();
        uiController.updateMapBalance(mapBalance);
    }

    public async void GetUtilsFaucet()
    {
        loadingText.enabled = true;
        loadingText.text = "Loading: 0%";
        try
        {
            await faucetContract.Write("getToken", utilsContractAddress, ROAD);
            loadingText.text = "Loading: 30%";
            await faucetContract.Write("getToken", utilsContractAddress, HOUSE);
            loadingText.text = "Loading:60%";
            await faucetContract.Write("getToken", utilsContractAddress, SPECIAL);
            loadingText.text = "Loading: 90%";
            await setItemBalances();
            loadingText.text = "Loading: 100%";
        }
        catch (Exception ex)
        {
            loadingText.text = "ERROR!!!";
            isError = true;
            Debug.LogError(ex);
            await setItemBalances();
        }
        loadingText.enabled = false;
    }

    public async void GetLandFaucet()
    {
        loadingText.enabled = true;
        loadingText.text = "Loading: 0%";
        try
        {
            if (landCount >= ((size / perSize) * (size / perSize)))
            {
                errorText = "NO LAND LEFT";
                isError = true;
                return;
            }
            List<Land> landsUnavailable = new List<Land>();
            for (int i = 1; i <= landCount; i++)
            {
                List<object> result = await mapContract.Read<List<object>>("land", i);
                Debug.Log("xIndex: " + result[0].ToString());
                Debug.Log("yIndex: " + result[1].ToString());
                Land myLandResult = new Land();
                myLandResult.xIndex = BigInteger.Parse(result[0].ToString());
                myLandResult.yIndex = BigInteger.Parse(result[1].ToString());
                landsUnavailable.Add(myLandResult);
                loadingText.text = "Loading: " + (i * 50 / landCount) + "%";
            }
            loadingText.text = "Loading: 50%";
            int xAvailable = 0, yAvailable = 0;
            for (int x = 0; x < size / perSize; x++)
            {
                bool isAvailable = false;
                for (int y = 0; y < size / perSize; y++)
                {
                    Land currentLand = new Land();
                    currentLand.xIndex = x;
                    currentLand.yIndex = y;
                    if (!landsUnavailable.Contains(currentLand))
                    {
                        xAvailable = x;
                        yAvailable = y;
                        isAvailable = true;
                        break;
                    }
                }
                if (isAvailable)
                {
                    break;
                }
            }
            loadingText.text = "Loading: 60%";
            await mapContract.Write("mint", xAvailable, yAvailable);
            loadingText.text = "Loading: 90%";
            landCount = await getLandCount();
            loadingText.text = "Loading: 91%";
            await setUserData(false);
            loadingText.text = "Loading: 100%";
        }
        catch (Exception ex)
        {
            loadingText.text = "ERROR!!!";
            isError = true;
            Debug.LogError(ex);
        }
        loadingText.enabled = false;
    }

    public async void OnWalletConnect()
    {
        Debug.Log("TEST111!!!");
        uiController.connectWalletText.enabled = false;
        Debug.Log("TEST222!!!");
        await setData();
        Debug.Log("TEST333!!!");
    }

    public void OnWalletDisconnect()
    {
        uiController.connectWalletText.enabled = true;
        ResetData();
    }

    public async void OnSwitchNetwork()
    {
        ResetData();
        Start();
        await setData();
    }
}
