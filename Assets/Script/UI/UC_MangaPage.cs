using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// 漫画阅读器
/// 支持拖拽翻页功能
/// </summary>
public class UC_MangaPage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public UC_PageItem preItem;
    public UC_PageItem currentItem;
    public UC_PageItem nextItem;

    [Header("翻页设置")]
    [Tooltip("翻页阈值（屏幕宽度的比例，默认0.33表示1/3）")]
    [Range(0.1f, 0.5f)]
    public float swipeThreshold = 0.33f;
    [Tooltip("翻页动画速度")]
    public float swipeSpeed = 10f;


    // 拖拽相关
    private Vector2 dragStartPos;
    private Vector2 dragCurPos;
    private bool isDragging = false;
    private float dragOffset = 0f;

    // 屏幕宽度
    private float screenWidth;

    // 是否正在动画
    private bool isAnimating = false;
    private RectTransform curRect;
    private RectTransform prevRect;
    private RectTransform nextRect;

    // 初始位置
    private Vector2 currentPageInitialPos;
    private Vector2 prevPageInitialPos;
    private Vector2 nextPageInitialPos;

    private bool _isBlockDrag = false;
    // 当前页面索引
    private int curIndex;
    private int startIndex;
    private int endIndex;
    private Action<int> onComplete;
    private Action onPreNode;
    private Action onNextNode;
    MangaPageData _CurPageData;
    void Awake()
    {
        screenWidth = Screen.width;
        curRect = currentItem.GetComponent<RectTransform>();
        currentPageInitialPos = curRect.anchoredPosition;

        prevRect = preItem.GetComponent<RectTransform>();
        prevRect.anchoredPosition = new Vector2(-screenWidth, 0);
        prevPageInitialPos = prevRect.anchoredPosition;
        preItem.gameObject.SetActive(false);

        nextRect = nextItem.GetComponent<RectTransform>();
        nextRect.anchoredPosition = new Vector2(screenWidth, 0);
        nextPageInitialPos = nextRect.anchoredPosition;
        nextItem.gameObject.SetActive(false);
    }

    public void SetPageRange(int startIndex, int endIndex)
    {
        this.startIndex = startIndex;
        this.endIndex = endIndex;
    }
    public void ShowPage(int index, Action<int> onComplete, Action onPreNode, Action onNextNode)
    {
        this.onComplete = onComplete;
        this.onPreNode = onPreNode;
        this.onNextNode = onNextNode;
        DoShowPage(index);
    }

    void DoShowPage(int index)
    {
        if (startIndex <= index && index <= endIndex)
        {
            curIndex = index;
            Debug.Log($"显示页面，索引: {curIndex}");
            string nodeId = MangaContainer.Instance.CurrNodeData.Config.ID;
            {
                // 显示当前页
                string pageId = nodeId.GetPageId(curIndex);
                _CurPageData = MangaContainer.Instance.GetPageDataByID(pageId);
                bool isOption = _CurPageData.IsOption(); // 如果当前页是选项页，则禁止拖拽
                bool isBattle = _CurPageData.IsBattle();
                _isBlockDrag = (isOption || isBattle) && MangaContainer.Instance.SelectOptionIndex == -1;
                currentItem.SetPageInfo(0, _CurPageData,
                (nextIndex) =>
                {
                    Debug.Log("选项1回调: " + nextIndex);
                    DoShowPage(nextIndex);
                },
                (nextIndex) =>
                {
                    Debug.Log("选项2回调: " + nextIndex);
                    DoShowPage(nextIndex);
                }, (awardCount) =>
                {
                    Debug.Log("获得奖励: " + awardCount);
                    MangaContainer.Instance.CurrNodeData.CurAwardCount += awardCount;
                },
                () =>
                {
                    Debug.Log("战斗完成");
                    DoShowPage(curIndex + 1);
                });
            }
            {
                // 显示上一页
                string pageId = nodeId.GetPreId(curIndex);
                MangaPageData pageData = MangaContainer.Instance.GetPageDataByID(pageId);
                preItem.SetPageInfo(-1, pageData, null, null, null, null);
            }
            {
                // 显示下一页
                string pageId = nodeId.GetNextId(curIndex);
                MangaPageData pageData = MangaContainer.Instance.GetPageDataByID(pageId);
                nextItem.SetPageInfo(1, pageData, null, null, null, null);
            }
            // 重置位置
            ResetPagePositions();
            // 回调
            onComplete?.Invoke(curIndex);
        }
        else
        {
            Debug.Log($"页面索引超出当前节点范围: {index} startIndex: {startIndex} endIndex: {endIndex}，尝试去上一节点或下一节点");
            if (MangaContainer.Instance.IsGuide)
            {
                Debug.Log("引导模式，回弹到当前页");
                StartCoroutine(SnapBackToCurrentPage());
                return;
            }
            //去上一节点
            if (index < startIndex)
            {
                if (MangaContainer.Instance.IsHavePreNode(out MangaNodeData nodeData))
                {
                    onPreNode?.Invoke();
                }
                else
                {
                    StartCoroutine(SnapBackToCurrentPage());
                }
            }
            //去下一节点
            else if (index > endIndex)
            {
                if (MangaContainer.Instance.IsHaveNextNode(out MangaNodeData nodeData))
                {
                    onNextNode?.Invoke();
                }
                else
                {
                    StartCoroutine(SnapBackToCurrentPage());
                }
            }
            else
            {
                Debug.LogError($"页面索引超出范围: {index}");
            }
        }
    }

    /// <summary>
    /// 重置所有页面位置
    /// </summary>
    private void ResetPagePositions()
    {
        curRect.anchoredPosition = currentPageInitialPos;
        prevRect.anchoredPosition = prevPageInitialPos;
        nextRect.anchoredPosition = nextPageInitialPos;
        dragOffset = 0f;
    }

    /// <summary>
    /// 开始拖拽
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isBlockDrag) return;
        if (isAnimating) return;

        isDragging = true;
        dragStartPos = eventData.position;
        dragCurPos = eventData.position;
    }

    /// <summary>
    /// 拖拽中
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (_isBlockDrag) return;
        if (!isDragging || isAnimating) return;

        dragCurPos = eventData.position;
        dragOffset = dragCurPos.x - dragStartPos.x;

        // 限制拖拽范围
        dragOffset = Mathf.Clamp(dragOffset, -screenWidth, screenWidth);

        // 更新页面位置
        UpdatePagePositions();
    }

    /// <summary>
    /// 结束拖拽
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isBlockDrag) return;
        if (!isDragging) return;

        isDragging = false;

        // 计算拖拽距离占屏幕宽度的比例
        float dragRatio = Mathf.Abs(dragOffset) / screenWidth;

        // 判断是否超过阈值
        if (dragRatio >= swipeThreshold)
        {
            // 翻页
            if (dragOffset > 0)
            {
                // 向右滑动，去上一页
                GoToPreviousPage();
            }
            else
            {
                // 向左滑动，去下一页
                GoToNextPage();
            }
        }
        else
        {
            // 回弹到当前页
            StartCoroutine(SnapBackToCurrentPage());
        }
    }

    /// <summary>
    /// 更新页面位置（拖拽过程中）
    /// </summary>
    private void UpdatePagePositions()
    {
        // 向右滑动（显示上一页）
        if (dragOffset > 0 && curIndex > startIndex)
        {
            curRect.anchoredPosition = currentPageInitialPos + new Vector2(dragOffset, 0);
            // 上一页从左侧滑入（初始位置在左侧一个屏幕宽度处）
            prevRect.anchoredPosition = prevPageInitialPos + new Vector2(dragOffset, 0);
        }
        // 向左滑动（显示下一页）
        else if (dragOffset < 0 && curIndex < endIndex)
        {
            curRect.anchoredPosition = currentPageInitialPos + new Vector2(dragOffset, 0);
            // 下一页从右侧滑入（初始位置在右侧一个屏幕宽度处）
            nextRect.anchoredPosition = nextPageInitialPos + new Vector2(dragOffset, 0);
        }
        else
        {
            // 拖拽方向无效或已到边界，只移动当前页
            curRect.anchoredPosition = currentPageInitialPos + new Vector2(dragOffset, 0);
        }
    }

    /// <summary>
    /// 去上一页
    /// </summary>
    private void GoToPreviousPage()
    {
        // if (curIndex <= startIndex)
        // {
        //     // 已经是第一页，回弹
        //     StartCoroutine(SnapBackToCurrentPage());
        //     return;
        // }
        StartCoroutine(SwipeToPage(-1));
    }

    /// <summary>
    /// 去下一页
    /// </summary>
    private void GoToNextPage()
    {
        // if (curIndex >= endIndex)
        // {
        //     // 已经是最后一页，回弹
        //     StartCoroutine(SnapBackToCurrentPage());
        //     return;
        // }
        StartCoroutine(SwipeToPage(1));
    }

    /// <summary>
    /// 滑动到指定页面
    /// </summary>
    private IEnumerator SwipeToPage(int dir)
    {
        //如果Item正在播放动画，则等待动画播放完成
        if (currentItem.isAnimating)
        {
            Debug.Log("Item正在播放动画，等待动画播放完成");
            StartCoroutine(SnapBackToCurrentPage());
            yield break;
        }
        isAnimating = true;

        float targetOffset = dragOffset > 0 ? screenWidth : -screenWidth;
        float startOffset = dragOffset;
        float elapsed = 0f;
        float duration = Mathf.Abs(targetOffset - startOffset) / (screenWidth * swipeSpeed);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            dragOffset = Mathf.Lerp(startOffset, targetOffset, t);
            UpdatePagePositions();
            yield return null;
        }
        if (dir > 0)
        {
            var isOption = _CurPageData.IsOption();
            var selectIndex = MangaContainer.Instance.SelectOptionIndex;
            if (isOption && selectIndex != -1)
            {
                //选项已被禁用 ，去选项页对应的下一页
                DoShowPage(_CurPageData.GetOptionSkip(selectIndex));
            }
            else
            {
                //去下一页
                var optionEnd = _CurPageData.GetOptionEnd();
                if (optionEnd > 0)
                {
                    DoShowPage(optionEnd);
                }
                else
                {
                    DoShowPage(curIndex + 1);
                }
            }
        }
        else
        {
            //去上一页
            var optionBackList = _CurPageData.GetOptionBackList();
            var selectIndex = MangaContainer.Instance.SelectOptionIndex;
            if (optionBackList.Count >= 2 && selectIndex != -1)
            {

                DoShowPage(optionBackList[selectIndex]);
            }
            else
            {

                DoShowPage(curIndex - 1);
            }
        }
        isAnimating = false;

    }

    /// <summary>
    /// 回弹到当前页
    /// </summary>
    private IEnumerator SnapBackToCurrentPage()
    {
        isAnimating = true;

        float startOffset = dragOffset;
        float elapsed = 0f;
        float duration = Mathf.Abs(startOffset) / (screenWidth * swipeSpeed);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            dragOffset = Mathf.Lerp(startOffset, 0f, t);
            UpdatePagePositions();
            yield return null;
        }

        ResetPagePositions();
        isAnimating = false;
    }

    /// <summary>
    /// 获取当前页面索引
    /// </summary>
    public int GetCurrentPageIndex()
    {
        return curIndex;
    }

}

