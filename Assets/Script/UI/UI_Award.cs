using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Award : MonoBehaviour
{
    public Button closeBtn;
    public TextMeshProUGUI awardTxt;
    void Start()
    {
        closeBtn.onClick.AddListener(OnCloseBtnClick);
        var curAwardCount = MangaContainer.Instance.CurrNodeData.CurAwardCount;
        var allAwardCount = MangaContainer.Instance.CurrNodeData.Config.AllAwardCount;
        awardTxt.text = "收集的物品数量:" + curAwardCount + "/" + allAwardCount;
    }

    void OnCloseBtnClick()
    {
        Destroy(gameObject);
    }
}
