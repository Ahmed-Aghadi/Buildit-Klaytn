using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public Canvas canvas;
    public RectTransform buildingMenuRect;
    public Button editButton;
    public Button confirmButton;
    public Button cancelButton;
    public Prefab_ConnectWallet prefab_connectWallet;
    public UIController uiController;
    public GameObject buildingMenu;
    public GameObject faucetButton;
    public Button marketplaceButton;
    public Text marketplaceButtonText;
    bool isMarketplaceOpen = false;
    public InputManager inputManager;
    public PlacementManager placementManager;
    public GameObject lightBlueHighlightPrefab;
    public GameObject redHighlightPrefab;
    List<GameObject> highlightPrefabs;
    // Start is called before the first frame update
    void Start()
    {
        highlightPrefabs = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        /*var canvasHeight = canvas.pixelRect.height;
        var canvasWidth = canvas.pixelRect.width;
        var canvasScale = canvas.scaleFactor;*/
        var canvasRect = canvas.GetComponent<RectTransform>().rect;
        /*var canvasRectWidth = canvasRect.width;*/
        var canvasRectHeight = canvasRect.height;

        var scaleCalc = canvasRectHeight / 540;

        prefab_connectWallet.transform.localScale = new Vector3(scaleCalc, scaleCalc, scaleCalc);

        if (isMarketplaceOpen)
        {
            Debug.Log("marketplace open");
            var position = inputManager.RaycastGround();
            if (position != null)
            {
                Debug.Log("position not null");
                if (placementManager.CheckIfPositionInBound(position.Value) == true)
                {
                    Debug.Log("This position is not out of bounds");
                    if (ContractManager.Instance.userOwnsIndex(position.Value.x, position.Value.z))
                    {
                        highlightLand(position.Value.x, position.Value.z,lightBlueHighlightPrefab);
                    }
                    else
                    {
                        highlightLand(position.Value.x, position.Value.z, redHighlightPrefab);
                    }
                }
            }

        }
    }
    void highlightLand(int x, int y, GameObject prefab)
    {
        if(highlightPrefabs.Count != 0)
        {
            ResetHighlightPrefabsList();
        }
        int perSize = ContractManager.Instance.perSize;
        if(perSize == 0)
        {
            return;
        }
        int x1 = (int)(x / perSize) * perSize;
        int y1 = (int)(y / perSize) * perSize;
        int x2 = x1 + perSize - 1;
        int y2 = y1 + perSize - 1;
        int tmpY1 = y1;
        while (x1 <= x2)
        {
            while (y1 <= y2)
            {

                var current = GameObject.Instantiate(prefab);
                current.transform.localPosition = new Vector3(x1, 0.002f, y1);
                highlightPrefabs.Add(current);
                y1++;
            }
            y1 = tmpY1;
            x1++;
        }
    }

    void ResetHighlightPrefabsList()
    {
        foreach (var item in highlightPrefabs)
        {
            Destroy(item);
        }
        highlightPrefabs.Clear();
        highlightPrefabs = new List<GameObject>();
    }

    public void AttachClickListeners()
    {
        editButton.onClick.AddListener(OnEditClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);
        confirmButton.onClick.AddListener(OnConfirmClicked);
    }

    public void RemoveClickListeners()
    {
        uiController.RemoveEventListeners();
        editButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        confirmButton.onClick.RemoveAllListeners();
    }

    void OnEditClicked()
    {
        uiController.AddEventListeners();
        buildingMenuRect.sizeDelta = new Vector2(180f, 240f);
        editButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
    }

    void OnCancelClicked()
    {
        uiController.RemoveEventListeners();
        buildingMenuRect.sizeDelta = new Vector2(180f, 210f);
        editButton.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        ContractManager.Instance.CancelClicked();
    }

    async void OnConfirmClicked()
    {
        Debug.Log("updating...");
        await ContractManager.Instance.confirmMapUpdates();
        Debug.Log("updated!!!");
        OnCancelClicked();
    }

    public void ToggleMarketplace()
    {
        isMarketplaceOpen = !isMarketplaceOpen;
        buildingMenu.SetActive(!isMarketplaceOpen);
        faucetButton.SetActive(!isMarketplaceOpen);
        marketplaceButtonText.text = !isMarketplaceOpen ? "Marketplace" : "Go Back";
        if (!isMarketplaceOpen)
        {
            if (highlightPrefabs.Count != 0)
            {
                ResetHighlightPrefabsList();
            }
        }
    }
}
