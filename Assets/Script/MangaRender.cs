using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 漫画阅读器
/// 支持拖拽翻页功能
/// </summary>
public class MangaRender : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int startIndex = 0;
    public int endIndex = 20;
    public PageItem currentItem;
    public PageItem preItem;
    public PageItem nextItem;

    [Header("翻页设置")]
    [Tooltip("翻页阈值（屏幕宽度的比例，默认0.33表示1/3）")]
    [Range(0.1f, 0.5f)]
    public float swipeThreshold = 0.33f;
    [Tooltip("翻页动画速度")]
    public float swipeSpeed = 10f;

    // 当前页面索引
    private int curIndex = 0;

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


    void Start()
    {
        screenWidth = Screen.width;
        curRect = currentItem.GetComponent<RectTransform>();
        currentPageInitialPos = curRect.anchoredPosition;

        prevRect = preItem.GetComponent<RectTransform>();
        prevPageInitialPos = prevRect.anchoredPosition;
        preItem.gameObject.SetActive(false);

        nextRect = nextItem.GetComponent<RectTransform>();
        nextPageInitialPos = nextRect.anchoredPosition;
        nextItem.gameObject.SetActive(false);

        // 设置初始页面
        curIndex = Mathf.Clamp(startIndex, 0, endIndex - 1);
        ShowPage(curIndex);

        Debug.Log($"漫画阅读器初始化完成，当前页索引: {curIndex}");
    }

    /// <summary>
    /// 显示指定索引的页面
    /// </summary>
    public void ShowPage(int index)
    {
        if (index >= startIndex && index < endIndex)
        {

            index = Mathf.Clamp(index, 0, endIndex - 1);
            curIndex = index;
            Debug.Log($"显示页面，索引: {curIndex}");

            // 显示当前页
            currentItem.ShowTexture(index);
            currentItem.gameObject.SetActive(true);

            // 显示上一页
            var preIndex = index - 1;
            var isShow = preIndex >= startIndex && preIndex < endIndex;
            preItem.gameObject.SetActive(isShow);
            if (isShow) preItem.ShowTexture(preIndex);

            // 显示下一页
            var nextIndex = index + 1;
            isShow = nextIndex >= startIndex && nextIndex < endIndex;
            nextItem.gameObject.SetActive(isShow);
            if (isShow) nextItem.ShowTexture(nextIndex);

            // 重置位置
            ResetPagePositions();

        }
        else
        {
            Debug.LogError($"页面索引超出范围: {index}");
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
        if (dragOffset > 0 && curIndex > 0)
        {
            curRect.anchoredPosition = currentPageInitialPos + new Vector2(dragOffset, 0);
            // 上一页从左侧滑入（初始位置在左侧一个屏幕宽度处）
            prevRect.anchoredPosition = prevPageInitialPos + new Vector2(dragOffset, 0);
        }
        // 向左滑动（显示下一页）
        else if (dragOffset < 0 && curIndex < endIndex - 1)
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
        if (curIndex <= 0)
        {
            // 已经是第一页，回弹
            StartCoroutine(SnapBackToCurrentPage());
            return;
        }
        StartCoroutine(SwipeToPage(curIndex - 1));
    }

    /// <summary>
    /// 去下一页
    /// </summary>
    private void GoToNextPage()
    {
        if (curIndex >= endIndex - 1)
        {
            // 已经是最后一页，回弹
            StartCoroutine(SnapBackToCurrentPage());
            return;
        }
        StartCoroutine(SwipeToPage(curIndex + 1));
    }

    /// <summary>
    /// 滑动到指定页面
    /// </summary>
    private IEnumerator SwipeToPage(int targetPageIndex)
    {
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

        // 切换到目标页面
        ShowPage(targetPageIndex);

        // 输出索引
        Debug.Log($"翻页完成，当前页索引: {curIndex}");

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

