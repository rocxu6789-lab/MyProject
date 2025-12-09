using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Enter : MonoBehaviour
{
    public Button enterBtn;
    public Button skipBtn;
    public Toggle toggle;
    public Transform parent;
    void Awake()
    {
        TableManager.Init();
        MangaContainer.Instance.Init();
    }
    void Start()
    {
        enterBtn.onClick.AddListener(OnEnterBtnClick);
        skipBtn.onClick.AddListener(OnSkipBtnClick);
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    void OnToggleValueChanged(bool isOn)
    {
        Debug.Log("OnToggleValueChanged: " + isOn);
        MangaContainer.Instance.IsGuide = isOn;
    }
    void OnSkipBtnClick()
    {
        Debug.Log("OnSkipBtnClick");
        //set data
        MangaContainer.Instance.InitNodeId = "1_1_2";//设置初始节点id
        MangaContainer.Instance.InitStartIndex = 2;//设置初始开始页
        var nodeId = MangaContainer.Instance.InitNodeId;
        var nodeData = MangaContainer.Instance.GetNodeDataByID(nodeId);
        MangaContainer.Instance.CurrNodeData = nodeData;
        DoOpenManga();
    }

    void OnEnterBtnClick()
    {
        Debug.Log("OnEnterBtnClick");
        var nodeId = MangaContainer.Instance.InitNodeId;
        var nodeData = MangaContainer.Instance.GetNodeDataByID(nodeId);
        MangaContainer.Instance.CurrNodeData = nodeData;
        DoOpenManga();
    }
    void DoOpenManga()
    {

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
