using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 点击场景里的报纸 → 打开报纸UI
/// </summary>
public class OpenNewspaper : MonoBehaviour
{
    [Header("拖入报纸UI")]
    public GameObject newspaperUI;

    void OnMouseDown()
    {
        newspaperUI.SetActive(true);
    }
}
