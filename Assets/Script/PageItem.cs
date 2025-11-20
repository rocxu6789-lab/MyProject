using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageItem : MonoBehaviour
{
    public RawImage img;
    public Text indexTxt;
    void Start()
    {

    }

    public void ShowTexture(int index)
    {
        indexTxt.text = index.ToString();
        Debug.Log($"加载页面纹理，索引: {index}");
        img.texture = Resources.Load<Texture>($"Texture/{index}");
    }
}
