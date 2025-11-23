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
    int curIndex = 0;
    void Start()
    {
        sld.wholeNumbers = true;
        InitNodeInfo();
    }
    void InitNodeInfo()
    {
        CurrNodeData = MangaContainer.Instance.CurrNodeData;
        MangaContainer.Instance.SelectOptionIndex = -1;
        mangaPages.SetPageRange(CurrNodeData.StartIndex, CurrNodeData.EndIndex);
        sld.minValue = CurrNodeData.StartIndex;
        sld.maxValue = CurrNodeData.EndIndex;
        sld.value = CurrNodeData.StartIndex;
        curIndex = -1;
        var consumePower = MangaContainer.Instance.GetConsumePower();
        PlayerManager.Instance.UsePower(consumePower);
        Debug.Log($"切换节点: {CurrNodeData.Config.Name} 消耗体力: {consumePower} 当前体力: {PlayerManager.Instance.GetPower()}");
    }
    void OnSliderValueChanged(float value)
    {
        Debug.Log("OnSliderValueChanged: " + value);
        var intValue = (int)value;
        if (intValue == curIndex) { return; }

        mangaPages.ShowPage(intValue, (index) =>
        {
            curIndex = index;
            //界面展示完成回调
            Debug.Log("界面展示完成回调: " + index);
            sld.value = index;
            proTxt.text = $"{index}/{CurrNodeData.EndIndex}";
            infoTxt.text = $"{CurrNodeData.Config.Name} \n 第{index}页";
        }, () =>
        {
            Debug.Log("去上一节点");
            OnPreBtnClick();
        }, () =>
        {
            Debug.Log("去下一节点");
            OnNextBtnClick();
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
        if (MangaContainer.Instance.IsHavePreNode(out MangaNodeData nodeData))
        {
            MangaContainer.Instance.CurrNodeData = nodeData;
            InitNodeInfo();
            OnSliderValueChanged(CurrNodeData.StartIndex);
        }
        else
        {
            Debug.LogError("上一节点数据为空");
        }
    }
    void OnNextBtnClick()
    {
        Debug.Log("OnNextBtnClick");
        if (MangaContainer.Instance.IsHaveNextNode(out MangaNodeData nodeData))
        {
            MangaContainer.Instance.CurrNodeData = nodeData;
            InitNodeInfo();
            OnSliderValueChanged(CurrNodeData.StartIndex);
        }
        else
        {
            Debug.LogError("下一节点数据为空");
        }
    }
    void OnCloseBtnClick()
    {
        MangaContainer.Instance.SelectOptionIndex = -1;
        Destroy(gameObject);
    }
}
