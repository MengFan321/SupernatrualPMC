using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("要跟随的玩家")]
    public Transform target;

    [Header("跟随速度")]
    public float smoothSpeed = 5f;

    [Header("相机固定高度（可不动）")]
    public float fixedZ = -10;

    void LateUpdate()
    {
        if (target == null) return;

        // 只跟随 X 轴，Y 轴不动（适合平台跳跃）
        Vector3 targetPos = new Vector3(target.position.x, transform.position.y, fixedZ);

        // 平滑跟随
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}