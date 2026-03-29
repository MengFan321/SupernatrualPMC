using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Slot : MonoBehaviour, IDropHandler
{
    [Header("房门设置")]
    [Tooltip("房门标识符（格式：10X，与钥匙keyID匹配）")]
    public string doorID = "101";

    [Header("UI组件")]
    [SerializeField] private Text levelText;

    [Header("场景跳转")]
    [Tooltip("拖放成功后跳转的场景名称")]
    public string targetSceneName = "Level1";

    [Header("反馈效果")]
    public AudioClip successSound;
    public AudioClip failSound;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// 初始化房门数据
    /// </summary>
    public void InitializeDoorData(int levelNumber)
    {
        // 设置doorID，格式：10X
        doorID = "10" + levelNumber;

        // 设置目标场景名称
        targetSceneName = "Level" + levelNumber;

        // 设置UI文本
        if (levelText != null)
        {
            levelText.text = doorID;
        }

        // 设置游戏对象名称
        gameObject.name = "Door_" + doorID;
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;

        if (draggedObject == null) return;

        DragByInterface dragScript = draggedObject.GetComponent<DragByInterface>();
        if (dragScript == null) return;

        KeyItem keyItem = draggedObject.GetComponent<KeyItem>();
        if (keyItem == null) return;

        // 验证钥匙和房门是否匹配
        if (keyItem.keyID == doorID)
        {
            OnDropSuccess(draggedObject, dragScript, keyItem);
        }
        else
        {
            OnDropFailed(draggedObject, dragScript, keyItem);
        }
    }

    /// <summary>
    /// 拖放成功处理
    /// </summary>
    private void OnDropSuccess(GameObject key, DragByInterface dragScript, KeyItem keyItem)
    {
        // 播放成功音效
        if (successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }

        // 将钥匙吸附到房门中心
        RectTransform keyRect = key.GetComponent<RectTransform>();
        RectTransform doorRect = GetComponent<RectTransform>();
        if (keyRect != null && doorRect != null)
        {
            keyRect.SetParent(doorRect);
            keyRect.anchoredPosition = Vector2.zero;
            keyRect.SetAsFirstSibling();
        }

        // 标记钥匙为已使用
        dragScript.MarkAsSuccessfullyPlaced();

        // 禁用房门交互
        CanvasGroup doorCanvasGroup = GetComponent<CanvasGroup>();
        if (doorCanvasGroup != null)
        {
            doorCanvasGroup.blocksRaycasts = false;
        }

        // 延迟执行场景跳转
        StartCoroutine(LoadSceneAfterDelay(0.2f));
    }

    /// <summary>
    /// 拖放失败处理
    /// </summary>
    private void OnDropFailed(GameObject key, DragByInterface dragScript, KeyItem keyItem)
    {
        // 播放失败音效
        if (failSound != null)
        {
            audioSource.PlayOneShot(failSound);
        }

        // 立即重置钥匙的父对象和层级
        if (key.transform.parent == transform)
        {
            // 如果钥匙当前是门的子对象，立即重置
            dragScript.ResetKey();
        }
        else
        {
            // 直接返回原始位置
            dragScript.ReturnToOriginalPosition();
        }
    }

    /// <summary>
    /// 延迟加载场景
    /// </summary>
    private IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError($"场景 '{targetSceneName}' 不存在！");
        }
    }
}