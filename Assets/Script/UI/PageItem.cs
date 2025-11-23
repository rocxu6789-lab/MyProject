using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageItem : MonoBehaviour
{
    public RawImage img;
    void Start()
    {

    }

    public void ShowTexture(int index)
    {
        var pageId = $"{MangaContainer.Instance.CurrNodeData.Config.ID}_{index}";
        var pageData = MangaContainer.Instance.GetPageDataByID(pageId);
        var isShow = pageData != null;
        gameObject.SetActive(isShow);
        if (isShow)
        {
            ResourcesManager.Instance.LoadTexture($"{pageData.Config.Texture}", (Texture texture) =>
            {
                img.texture = texture;
            });
        }
    }
}
