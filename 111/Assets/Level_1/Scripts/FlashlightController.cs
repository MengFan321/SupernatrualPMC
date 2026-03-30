using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public float size = 3f;
    private Camera mainCam;

    // 开关
    private bool isMaskOn = true;
    // 固定位置
    private bool isFixed = false;
    private Vector3 fixedPos;

    private SpriteMask spriteMask; // 关键：用SpriteMask，不是SpriteRenderer

    void Start()
    {
        mainCam = Camera.main;
        spriteMask = GetComponent<SpriteMask>(); // 获取SpriteMask

        if (mainCam == null)
            Debug.LogError("找不到主相机！");

        transform.localScale = Vector3.one * size;
    }

    void Update()
    {
        if (mainCam == null) return;

        // ===================== F 键 开关遮罩 =====================
        if (Input.GetKeyDown(KeyCode.F))
        {
            isMaskOn = !isMaskOn;
            spriteMask.enabled = isMaskOn; // 控制SpriteMask开关
        }

        // ===================== 右键 固定/解锁 =====================
        if (Input.GetMouseButtonDown(1))
        {
            isFixed = !isFixed;

            if (isFixed)
                fixedPos = transform.position;
        }

        // 关闭状态不更新位置
        if (!isMaskOn) return;

        // 未固定 → 跟随鼠标
        if (!isFixed)
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = mainCam.nearClipPlane;
            Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(mouseScreenPos);
            transform.position = mouseWorldPos;
        }
        else
        {
            // 固定 → 停在原地
            transform.position = fixedPos;
        }
    }
}