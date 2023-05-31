using System.Collections;
using System.Collections.Generic;
using Thirdweb;
using UnityEngine;

public class ContractManager : MonoBehaviour
{
    string contractAddress = "";
    string contractABI = "";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnWalletConnect()
    {

    }

    public void getMapSize()
    {
        Contract mapContract = ThirdwebManager.Instance.SDK.GetContract(contractAddress, contractABI);
    }

    public void OnWalletDisconnect()
    {

    }

    public void OnSwitchNetwork()
    {

    }
}
