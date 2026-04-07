using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 手机字母密码锁（TextMeshPro）
/// 修复：第6位字母不显示、立刻被清空的问题
/// 原理：先完整显示6位，再延迟一帧验证密码，保证玩家能看到输入
/// </summary>
public class PhonePassword_TMP : MonoBehaviour
{
    [Header("正确的6位字母密码")]
    public string correctPassword = "ABCDEF";

    [Header("6个密码显示框（TextMeshProUGUI）")]
    public TextMeshProUGUI[] passwordSlots;

    [Header("密码正确后显示的主界面")]
    public GameObject phoneMainUI;

    // 当前输入的密码
    private string inputPassword = "";

    // ===================================
    // 点击字母按键时调用，传入对应字母
    // ===================================
    public void InputLetter(string letter)
    {
        // 已满6位，不再接受输入
        if (inputPassword.Length >= 6)
            return;

        // 追加字母
        inputPassword += letter;

        // 先刷新显示，让玩家看到自己输入的内容（包括第6位）
        UpdatePasswordDisplay();

        // ==============================================
        // 关键修复：输满6位时，**等画面刷新后再验证**
        // ==============================================
        if (inputPassword.Length == 6)
        {
            Invoke(nameof(CheckPassword), 0.02f);
        }
    }

    // ===================================
    // 更新密码位显示
    // ===================================
    void UpdatePasswordDisplay()
    {
        for (int i = 0; i < passwordSlots.Length; i++)
        {
            if (i < inputPassword.Length)
            {
                passwordSlots[i].text = inputPassword[i].ToString();
            }
            else
            {
                passwordSlots[i].text = "";
            }
        }
    }

    // ===================================
    // 验证密码是否正确
    // ===================================
    void CheckPassword()
    {
        if (inputPassword == correctPassword)
        {
            // 正确：进入主界面
            gameObject.SetActive(false);
            phoneMainUI.SetActive(true);
        }
        else
        {
            // 错误：清空重输
            ClearPassword();
        }
    }

    // ===================================
    // 清空密码
    // ===================================
    public void ClearPassword()
    {
        inputPassword = "";
        UpdatePasswordDisplay();
    }
}