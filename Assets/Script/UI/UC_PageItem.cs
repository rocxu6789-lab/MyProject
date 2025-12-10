using System;
using System.Collections;
using System.Collections.Generic;
using Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UC_PageItem : MonoBehaviour
{
    [Header("图片")]
    public RawImage img;
    [Header("选项")]
    public GameObject optionGo;
    public Button optionBtn1;
    public TextMeshProUGUI optionTxt1;
    public Button optionBtn2;
    public TextMeshProUGUI optionTxt2;
    public GameObject optionTipsGo2;
    [Header("战斗")]
    public GameObject battleGo;
    public Button battleBtn;
    public TextMeshProUGUI battleIdTxt;
    public TextMeshProUGUI battlePowerTxt;
    [Header("结束尾页")]
    public GameObject endGo;
    public Animation endAni1;
    public Transform endParent;
    public GameObject butterflyGo;
    public Transform startPos;
    public Transform endPos;
    [Header("结束尾条提示")]
    public GameObject endTipsGo1;
    public GameObject endTipsGo2;

    MangaPageData _PageData;
    bool isShowEnd = false;
    bool isShowOption = false;
    bool isShowBattle = false;
    public bool isAnimating = false;
    Action<int> onOption1Click;
    Action<int> onOption2Click;
    Action<int> onDropAward;
    Action battleComplete;
    void Start()
    {
    }

    public void SetPageInfo(int typeIndex, MangaPageData pageData,
     Action<int> onOption1Click,
      Action<int> onOption2Click,
      Action<int> onDropAward,
      Action battleComplete)
    {
        isAnimating = false;
        _PageData = pageData;
        this.onOption1Click = onOption1Click;
        this.onOption2Click = onOption2Click;
        this.onDropAward = onDropAward;
        this.battleComplete = battleComplete;
        var isShow = _PageData != null;
        gameObject.SetActive(isShow);
        if (isShow)
        {
            ShowTexture();
            if (typeIndex == 0)
            {
                DropAward();
                ShowOption();
                ShowBattle();
                ShowEnd();
            }
            else
                this.StopAllCoroutines();
        }
        else
            this.StopAllCoroutines();
    }

    public void ShowEndTips(bool isShow)
    {
        var isHaveNextNode = MangaContainer.Instance.IsHaveNextNode(out MangaNodeData nodeData);
        if (isHaveNextNode)
        {
            endTipsGo1.SetActive(true && isShow);
            endTipsGo2.SetActive(false);
        }
        else
        {
            endTipsGo1.SetActive(false);
            endTipsGo2.SetActive(true && isShow);
        }
    }

    void DropAward()
    {
        var awardCount = _PageData.Config.AwardCount;
        if (awardCount > 0)
        {
            onDropAward?.Invoke(awardCount);
        }
    }

    void ShowEnd()
    {
        isShowEnd = _PageData.IsEnd();
        endGo.SetActive(isShowEnd);
        if (isShowEnd)
        {
            endAni1.Stop();
            endAni1.Play();
            butterflyGo.SetActive(true);
            endParent.gameObject.SetActive(false);
            this.StopAllCoroutines();
            isAnimating = true;
            StartCoroutine(LocalPositionLerp(butterflyGo.transform, startPos.localPosition, endPos.localPosition, 2f, () =>
            {
                isAnimating = false;
                butterflyGo.SetActive(false);
                endParent.gameObject.SetActive(true);
                Debug.Log("蝴蝶飞完成");
                var endSkip = _PageData.GetEndSkip();
                ResourcesManager.Instance.LoadGameObject(endSkip, (GameObject asset) =>
                {
                    if (asset == null)
                    {
                        Debug.LogError($"EndSkip prefab is null: {endSkip}");
                        return;
                    }
                    var go = GameObject.Instantiate(asset);
                    ResourcesManager.Instance.SetParent(go, endParent);
                });
                var curAwardCount = MangaContainer.Instance.CurrNodeData.CurAwardCount;
                if (curAwardCount > 0)
                {
                    //获得奖励
                    ResourcesManager.Instance.LoadGameObject("Prefab/UI_Award", (GameObject asset) =>
                    {
                        if (asset == null)
                        {
                            Debug.LogError($"UI_Award prefab is null: {endSkip}");
                            return;
                        }
                        var go = GameObject.Instantiate(asset);
                        ResourcesManager.Instance.SetParent(go, GameObject.Find("Canvas/Parent").transform);
                    });
                }
            }));


        }
        else
        {
            this.StopAllCoroutines();
            endAni1.Stop();
        }
    }

    void ShowBattle()
    {
        isShowBattle = _PageData.IsBattle();
        battleGo.SetActive(isShowBattle);
        if (isShowBattle)
        {
            int battleId = _PageData.GetBattleId();
            battleIdTxt.text = $"战斗ID: {battleId}";
            battlePowerTxt.text = $"{PlayerManager.Instance.GetPowerString(MangaContainer.Instance.GetConsumePower())}";
            if (PlayerManager.Instance.IsEnoughPower(battleId))
            {
                battleBtn.interactable = true;
                Debug.Log("体力足够,开始战斗");
                Debug.Log("点击后跳转外部功能界面，跳转战斗界面 battleId : " + battleId);
                battleBtn.onClick.RemoveAllListeners();
                battleBtn.onClick.AddListener(() => battleComplete?.Invoke());
            }
            else
            {
                battleBtn.interactable = false;
                Debug.Log("体力不足,无法战斗");
                battleBtn.onClick.RemoveAllListeners();
            }
        }
        else
            battleBtn.onClick.RemoveAllListeners();
    }

    void ShowOption()
    {
        isShowOption = _PageData.IsOption();
        optionGo.SetActive(isShowOption);
        if (isShowOption)
        {
            optionTxt1.text = _PageData.GetOptionName(0);
            optionTxt2.text = _PageData.GetOptionName(1);
            var index = MangaContainer.Instance.SelectOptionIndex;
            optionTipsGo2.SetActive(index != -1);
            optionBtn1.interactable = index == -1;
            optionBtn2.interactable = index == -1;
            optionBtn1.onClick.RemoveAllListeners();
            optionBtn2.onClick.RemoveAllListeners();
            optionBtn1.onClick.AddListener(() =>
            {
                MangaContainer.Instance.SelectOptionIndex = 0;
                onOption1Click?.Invoke(_PageData.GetOptionSkip(0));
            });
            optionBtn2.onClick.AddListener(() =>
            {
                MangaContainer.Instance.SelectOptionIndex = 1;
                onOption2Click?.Invoke(_PageData.GetOptionSkip(1));
            });
        }
        else
        {
            optionBtn1.onClick.RemoveAllListeners();
            optionBtn2.onClick.RemoveAllListeners();
        }
    }
    void ShowTexture()
    {
        img.enabled = false;
        ResourcesManager.Instance.LoadTexture($"{_PageData.Config.Texture}", (Texture texture) =>
        {
            if (texture == null)
            {
                Debug.LogError($"Texture is null: {_PageData.Config.Texture}");
                return;
            }
            if (img == null)
            {
                Debug.LogError($"img is null: {_PageData.Config.Texture}");
                return;
            }
            img.enabled = true;
            img.texture = texture;
            img.SetNativeSize();
            // 适配屏幕：保持长宽比不变，长边撑满屏幕
            var parentRect = GameObject.Find("Canvas/Parent").GetComponent<RectTransform>();
            var screenWidth = parentRect.rect.width;
            var screenHeight = parentRect.rect.height;
            var textureAspectRatio = (float)texture.width / texture.height;
            float finalWidth = screenHeight * textureAspectRatio;
            float finalHeight = screenHeight;
            img.rectTransform.sizeDelta = new Vector2(finalWidth, finalHeight);
        });

    }

    IEnumerator LocalPositionLerp(Transform trans, Vector3 start, Vector3 end, float duration, Action onComplete)
    {
        trans.localPosition = start;
        float startTime = Time.time;
        float t = (Time.time - startTime) / duration;
        while (t <= 1)
        {
            t = Mathf.Clamp((Time.time - startTime) / duration, 0, 2);
            trans.localPosition = Vector3.LerpUnclamped(start, end, t);
            yield return null;
        }
        trans.localPosition = end;
        onComplete?.Invoke();
    }
}
