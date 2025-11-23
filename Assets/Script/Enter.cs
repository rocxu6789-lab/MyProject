using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Enter : MonoBehaviour
{
    public Button enterBtn;
    public Transform parent;
    void Awake()
    {
        TableManager.Init();
        MangaContainer.Instance.Init();
    }
    void Start()
    {
        enterBtn.onClick.AddListener(OnEnterBtnClick);
    }

    void OnEnterBtnClick()
    {
        var nodeId = MangaContainer.Instance.InitNodeId;
        var nodeData = MangaContainer.Instance.GetNodeDataByID(nodeId);
        MangaContainer.Instance.CurrNodeData = nodeData;
        Debug.Log("OnEnterBtnClick");
        ResourcesManager.Instance.LoadGameObject("Prefab/UI_Manga", (GameObject asset) =>
        {
            if (asset == null)
            {
                Debug.LogError("UI_Manga asset is null");
                return;
            }
            var go = GameObject.Instantiate(asset);
            ResourcesManager.Instance.SetParent(go, parent);
        });
    }
}
