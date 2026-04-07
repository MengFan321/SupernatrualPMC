using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 拼图碎片脚本
/// 功能：
/// 1. 游戏开始自动【真正随机打乱】
/// 2. 打乱强度可在编辑器自由调节
/// 3. 自由拖动，无限制
/// 4. 靠近正确位置自动吸附 + 锁定
/// </summary>
public class PuzzlePiece : MonoBehaviour
{
    [Header("拼图基础设置")]
    // 拼图的正确位置（自动记录，无需手动设置）
    public Vector2 correctPos;

    // 是否已经拼对（拼对后无法拖动）
    public bool isCorrect = false;

    [Header("随机打乱强度")]
    // 打乱范围：数值越大，碎片散得越开
    // 0 = 不打乱
    // 0.5 = 小范围打乱
    // 1 = 正常打乱
    // 2 = 大范围打乱
    public float shuffleStrength = 1f;

    // 拖动偏移（让手感更舒服，不用管）
    private Vector2 offset;
    // 是否正在拖动
    private bool isDragging = false;

    void Start()
    {
        // 记录编辑器里的位置为【正确位置】
        correctPos = transform.position;

        // ==========================================
        // 真正随机打乱！强度由 shuffleStrength 控制
        // ==========================================
        Vector2 randomOffset = Random.insideUnitCircle * shuffleStrength;
        transform.position = correctPos + randomOffset;
    }

    /// <summary>
    /// 鼠标按下 -> 开始拖动
    /// </summary>
    void OnMouseDown()
    {
        // 已经拼好的碎片不能再动
        if (isCorrect) return;

        isDragging = true;
        offset = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
    }

    /// <summary>
    /// 鼠标松开 -> 停止拖动，并检查是否拼对
    /// </summary>
    void OnMouseUp()
    {
        isDragging = false;

        // 如果距离正确位置很近，自动吸附并锁定
        if (Vector2.Distance(transform.position, correctPos) < 0.3f)
        {
            transform.position = correctPos;
            isCorrect = true;
        }
    }

    void Update()
    {
        // 拖动时碎片跟随鼠标
        if (isDragging)
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mouseWorldPos - offset;
        }
    }
}
