using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneInteract : MonoBehaviour
{
    [Header("拖入手机UI界面")]
    public GameObject phoneUI;

    void OnMouseDown()
    {
        // 点击手机 → 打开UI
        phoneUI.SetActive(true);
    }
}
