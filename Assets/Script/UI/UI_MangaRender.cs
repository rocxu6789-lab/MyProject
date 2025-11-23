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
    MangaNodeData CurrNodeData;
    void Start()
    {
        sld.wholeNumbers = true;
        CurrNodeData = MangaContainer.Instance.CurrNodeData;
        mangaPages.SetPageRange(CurrNodeData.StartIndex, CurrNodeData.EndIndex);
        sld.minValue = CurrNodeData.StartIndex;
        sld.maxValue = CurrNodeData.EndIndex;
        // mangaPages.ShowPage(CurrNodeData.StartIndex, (index) =>
        // {
        //     SetPageInfo(index);
        // });
    }
    void SetPageInfo(int index)
    {
        sld.value = index;
        proTxt.text = $"{index}/{CurrNodeData.EndIndex}";
        infoTxt.text = $"第{index}页";
    }
    void OnSliderValueChanged(float value)
    {
        Debug.Log("OnSliderValueChanged: " + value);
        mangaPages.ShowPage((int)value, (index) =>
        {
            SetPageInfo(index);
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
    void OnCloseBtnClick()
    {
        Destroy(gameObject);
    }
}
