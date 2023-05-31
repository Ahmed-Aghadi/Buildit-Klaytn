using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    // public GameObject plane;
    public GameObject terrain;
    public Renderer rend;
    const float PLANE_DEFAULT_SIZE = 15f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateGridSize(int size)
    {
        var scale = size / 2f;
        rend.material.mainTextureScale = new Vector2(scale, scale);
        terrain.transform.localScale = new Vector3(size / PLANE_DEFAULT_SIZE, 1, size / PLANE_DEFAULT_SIZE);
        terrain.transform.localPosition = new Vector3(scale - 0.5f, 0, scale - 0.5f);
    }
}
