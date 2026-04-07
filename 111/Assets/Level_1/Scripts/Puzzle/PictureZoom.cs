using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureZoom : MonoBehaviour
{
    // 拖入你的特写画框
    public GameObject closeUp;

    // 点击小画框
    void OnMouseDown()
    {
        // 打开特写
        closeUp.SetActive(true);
    }
}
