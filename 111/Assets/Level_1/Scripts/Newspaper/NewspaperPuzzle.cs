using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 报纸解谜交互
/// 功能：
/// 1. 打开报纸特写
/// 2. 玩家输入指定文字
/// 3. 正确 → 显示折角碎片
/// 4. 错误 → 清空输入框
/// </summary>
public class NewspaperPuzzle : MonoBehaviour
{
    [Header("✅ 正确答案（玩家必须输入这个）")]
    public string correctAnswer = "YOURTEXT";

    [Header("✅ 输入框（拖入 TMP Input Field）")]
    public TMP_InputField inputField;

    [Header("✅ 折角碎片（正确后显示）")]
    public GameObject fragment;

    [Header("✅ （可选）提交后禁用输入，防止重复提交")]
    public bool disableAfterCorrect = true;

    // 记录是否已经解开
    private bool isSolved = false;

    // ===================================
    // 初始化：默认隐藏碎片
    // ===================================
    void Start()
    {
        if (fragment != null)
            fragment.SetActive(false);

        isSolved = false;
    }

    // ===================================
    // 点击【提交按钮】时调用
    // ===================================
    public void OnSubmit()
    {
        // 已经解开就不再处理
        if (isSolved) return;

        // 去掉首尾空格，避免玩家多打空格
        string playerInput = inputField.text.Trim();

        // 检查答案是否正确
        if (playerInput == correctAnswer)
        {
            // 正确！
            SolvedPuzzle();
        }
        else
        {
            // 错误：清空输入框
            inputField.text = "";
        }
    }

    // ===================================
    // 解谜成功：显示碎片
    // ===================================
    void SolvedPuzzle()
    {
        isSolved = true;
        fragment.SetActive(true);

        // 可选：禁用输入框，不让再输入
        if (disableAfterCorrect)
            inputField.interactable = false;
    }

    // ===================================
    // 关闭报纸（给关闭按钮用）
    // ===================================
    public void CloseNewspaper()
    {
        gameObject.SetActive(false);
    }
}
