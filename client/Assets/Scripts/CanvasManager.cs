using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public Canvas canvas;
    public Prefab_ConnectWallet prefab_connectWallet;
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
}
