using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic; // 用于List类型

public class DragByInterface : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rectTrans;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;
    private int originalSiblingIndex;
    private bool wasDroppedSuccessfully = false;

    [Header("拖拽设置")]
    public float returnSpeed = 5f;
    public float dragAlpha = 0.6f;

    private void Start()
    {
        rectTrans = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        SaveOriginalPosition();
    }

    /// <summary>
    /// 保存原始位置
    /// </summary>
    public void SaveOriginalPosition()
    {
        if (rectTrans != null)
        {
            originalPosition = rectTrans.anchoredPosition;
            originalParent = rectTrans.parent;
            originalSiblingIndex = rectTrans.GetSiblingIndex();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (wasDroppedSuccessfully)
        {
            eventData.pointerDrag = null;
            return;
        }

        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = dragAlpha;

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (wasDroppedSuccessfully) return;

        rectTrans.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (wasDroppedSuccessfully) return;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // 如果没有被放置，返回原始位置
        ReturnToOriginalPosition();
    }

    /// <summary>
    /// 标记为成功放置
    /// </summary>
    public void MarkAsSuccessfullyPlaced()
    {
        wasDroppedSuccessfully = true;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// 返回原始位置
    /// </summary>
    public void ReturnToOriginalPosition()
    {
        StartCoroutine(SmoothReturn(0.3f));
    }

    /// <summary>
    /// 返回原始位置 - 重载版本，带参数
    /// </summary>
    public void ReturnToOriginalPosition(float duration)
    {
        StartCoroutine(SmoothReturn(duration));
    }

    /// <summary>
    /// 平滑返回动画
    /// </summary>
    private IEnumerator SmoothReturn(float duration)
    {
        Vector2 startPos = rectTrans.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            rectTrans.anchoredPosition = Vector2.Lerp(startPos, originalPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTrans.anchoredPosition = originalPosition;
        if (originalParent != null)
        {
            rectTrans.SetParent(originalParent);
            rectTrans.SetSiblingIndex(originalSiblingIndex);
        }
    }

    /// <summary>
    /// 重置钥匙状态
    /// </summary>
    public void ResetKey()
    {
        wasDroppedSuccessfully = false;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        if (rectTrans != null)
        {
            rectTrans.anchoredPosition = originalPosition;
            if (originalParent != null)
            {
                rectTrans.SetParent(originalParent);
                rectTrans.SetSiblingIndex(originalSiblingIndex);
            }
        }
    }
}