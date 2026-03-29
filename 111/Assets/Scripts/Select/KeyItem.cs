using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KeyItem : MonoBehaviour
{
    [Header("钥匙信息")]
    [Tooltip("钥匙标识符（格式：10X，必须与对应房门doorID相同）")]
    public string keyID = "101";

    [Header("钥匙外观")]
    public Image keyIcon;
    public Text levelText;

    private Vector3 originalScale;
    private string originalLevelText;

    private void Start()
    {
        originalScale = transform.localScale;

        if (levelText != null)
        {
            originalLevelText = levelText.text;

            // 显示keyID后两位作为关卡号
            if (keyID.StartsWith("10") && keyID.Length > 2)
            {
                levelText.text = keyID.Substring(2);
            }
        }
    }

    /// <summary>
    /// 验证是否匹配指定的房门
    /// </summary>
    public bool IsMatchWithDoor(Slot doorSlot)
    {
        if (doorSlot == null)
            return false;

        return keyID == doorSlot.doorID;
    }

    /// <summary>
    /// 脉冲效果
    /// </summary>
    private IEnumerator PulseEffect(float targetScale)
    {
        float duration = 0.2f;
        float elapsedTime = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = originalScale * targetScale;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale;

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(endScale, originalScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }
}