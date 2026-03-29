using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // 添加UI命名空间

public class TransScene : MonoBehaviour
{
    [Header("=== 场景切换配置 ===")]
    [SerializeField] private string targetSceneName = "Select";  // 在Inspector中可配置的目标场景名

    [Header("=== 按钮引用 (可选) ===")]
    [SerializeField] private Button switchButton;  // 可手动指定按钮，如为空则自动获取

    private void Start()
    {
        // 如果未手动指定按钮，尝试自动获取当前物体上的按钮组件
        if (switchButton == null)
        {
            switchButton = GetComponent<Button>();
        }

        // 注册按钮点击事件
        if (switchButton != null)
        {
            switchButton.onClick.AddListener(LoadTargetScene);
        }
    }

    /// <summary>
    /// 加载目标场景
    /// </summary>
    public void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            // 可选：播放切换音效
            AudioManager.Instance?.PlaySFX("SceneSwitch");

            // 可选：添加加载动画
            StartCoroutine(LoadSceneWithDelay(0.2f));
        }
        else
        {
            Debug.LogWarning("目标场景名为空，请检查Inspector配置");
        }
    }

    /// <summary>
    /// 带延迟的场景加载（用于播放动画效果）
    /// </summary>
    private IEnumerator LoadSceneWithDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        SceneManager.LoadScene(targetSceneName);
    }

    /// <summary>
    /// 外部调用切换场景的方法（保留兼容性）
    /// </summary>
    public void TS()
    {
        LoadTargetScene();
    }
}