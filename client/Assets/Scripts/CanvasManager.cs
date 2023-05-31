using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public Canvas canvas;
    public RectTransform buildingMenu;
    public Button editButton;
    public Button confirmButton;
    public Button cancelButton;
    public Prefab_ConnectWallet prefab_connectWallet;
    public UIController uiController;
    // Start is called before the first frame update
    void Start()
    {
        
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
    }

    public void AttachClickListeners()
    {
        editButton.onClick.AddListener(OnEditClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);
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
        buildingMenu.sizeDelta = new Vector2(180f, 240f);
        editButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
    }

    void OnCancelClicked()
    {
        uiController.RemoveEventListeners();
        buildingMenu.sizeDelta = new Vector2(180f, 210f);
        editButton.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
    }
}
