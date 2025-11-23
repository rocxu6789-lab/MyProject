using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manga : MonoBehaviour
{
    public Button ClearScreenBtn;
    public GameObject contentPanel;
    public Button closeBtn;
    public Button preBtn;
    public Button nextBtn;
    public Slider sld;
    public TextMeshProUGUI proTxt;
    public TextMeshProUGUI infoTxt;
    public UC_MangaPage mangaPages;

    void Awake()
    {
        contentPanel.SetActive(true);
        closeBtn.onClick.AddListener(OnCloseBtnClick);
        ClearScreenBtn.onClick.AddListener(OnClearScreenBtnClick);
        preBtn.onClick.AddListener(OnPreBtnClick);
        nextBtn.onClick.AddListener(OnNextBtnClick);
        sld.onValueChanged.AddListener(OnSliderValueChanged);
    }
    void Start()
    {
        sld.wholeNumbers = true;
        var nodeData = MangaContainer.Instance.CurrNodeData;
        var startIndex = nodeData.StartIndex;
        var endIndex = nodeData.EndIndex;
        sld.minValue = startIndex;
        sld.maxValue = endIndex;
        mangaPages.SetPageRange(startIndex, endIndex);
        mangaPages.ShowPage(startIndex, (index) =>
        {
            sld.value = index;
            proTxt.text = $"{index}/{endIndex}";
            infoTxt.text = $"第{index}页";
        });
    }

    void OnClearScreenBtnClick()
    {
        Debug.Log("OnClearScreenBtnClick");
        contentPanel.SetActive(!contentPanel.activeSelf);
    }
    void OnPreBtnClick()
    {
        Debug.Log("OnPreBtnClick");
    }
    void OnNextBtnClick()
    {
        Debug.Log("OnNextBtnClick");
    }
    void OnSliderValueChanged(float value)
    {
        Debug.Log("OnSliderValueChanged: " + value);
    }
    void OnCloseBtnClick()
    {
        Destroy(gameObject);
    }
}
