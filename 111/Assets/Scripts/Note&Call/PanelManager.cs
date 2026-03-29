using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 面板数据类，用于在 Inspector 中配置
[Serializable]
public class UIPanel
{
    public string panelName;          // 面板名称（用于调试）
    public GameObject panelObject;    // 面板 GameObject
    public Button openButton;         // 打开面板的按钮
    public Button closeButton;        // 关闭面板的按钮（在面板内部）
    public bool closeOthers = true;   // 打开时是否关闭其他面板
    public Action onOpenCallback;     // 打开时的自定义回调
    public Action onCloseCallback;    // 关闭时的自定义回调
}

public class PanelManager : MonoBehaviour
{
    [Header("面板配置")]
    [SerializeField] private List<UIPanel> panels = new List<UIPanel>();

    [Header("其他设置")]
    [SerializeField] private bool logDebugInfo = true;  // 是否打印调试信息

    private Dictionary<string, UIPanel> panelDictionary = new Dictionary<string, UIPanel>();
    private UIPanel currentlyOpenPanel = null;

    private void Awake()
    {
        InitializePanels();
    }

    private void InitializePanels()
    {
        panelDictionary.Clear();

        for (int i = 0; i < panels.Count; i++)
        {
            var panel = panels[i];

            // 如果面板名称为空，使用默认名称
            if (string.IsNullOrEmpty(panel.panelName))
            {
                panel.panelName = panel.panelObject != null ? panel.panelObject.name : $"Panel_{i}";
            }

            // 添加到字典
            if (!panelDictionary.ContainsKey(panel.panelName))
            {
                panelDictionary.Add(panel.panelName, panel);
            }

            // 设置打开按钮事件
            if (panel.openButton != null)
            {
                // 移除旧的监听器，避免重复添加
                panel.openButton.onClick.RemoveAllListeners();
                string panelName = panel.panelName; // 创建局部变量供闭包使用
                panel.openButton.onClick.AddListener(() => OpenPanel(panelName));
            }

            // 设置关闭按钮事件
            if (panel.closeButton != null)
            {
                panel.closeButton.onClick.RemoveAllListeners();
                string panelName = panel.panelName; // 创建局部变量供闭包使用
                panel.closeButton.onClick.AddListener(() => ClosePanel(panelName));
            }

            // 初始化时关闭所有面板
            if (panel.panelObject != null)
            {
                panel.panelObject.SetActive(false);
            }
        }

        TimerScript timer = FindObjectOfType<TimerScript>();
        if (timer != null && timer.phoneButton != null)
        {
            // 为电话按钮添加“打开面板”事件（假设电话面板叫 "PhonePanel"）
            timer.phoneButton.onClick.AddListener(() => OpenPanel("PhonePanel"));

            // 为电话按钮添加“停止电话”事件
            timer.phoneButton.onClick.AddListener(timer.StopCalling);
        }

        if (logDebugInfo)
        {
            Debug.Log($"UIManager 初始化完成，共配置了 {panels.Count} 个面板");
        }
    }

    // 打开指定面板
    public void OpenPanel(string panelName)
    {
        if (!panelDictionary.TryGetValue(panelName, out UIPanel panel))
        {
            Debug.LogWarning($"未找到名为 '{panelName}' 的面板配置");
            return;
        }

        if (panel.panelObject == null)
        {
            Debug.LogError($"面板 '{panelName}' 的 GameObject 未设置");
            return;
        }

        // 如果已经是打开的，则关闭它（切换功能）
        if (panel.panelObject.activeSelf)
        {
            ClosePanel(panelName);
            return;
        }

        // 如果需要关闭其他面板
        if (panel.closeOthers && currentlyOpenPanel != null && currentlyOpenPanel != panel)
        {
            ClosePanel(currentlyOpenPanel.panelName);
        }

        // 打开面板
        panel.panelObject.SetActive(true);
        currentlyOpenPanel = panel;

        // 执行自定义回调
        panel.onOpenCallback?.Invoke();

        if (logDebugInfo)
        {
            Debug.Log($"打开了面板: {panelName}");
        }
    }

    // 关闭指定面板
    public void ClosePanel(string panelName)
    {
        if (!panelDictionary.TryGetValue(panelName, out UIPanel panel))
        {
            Debug.LogWarning($"未找到名为 '{panelName}' 的面板配置");
            return;
        }

        if (panel.panelObject == null)
        {
            Debug.LogError($"面板 '{panelName}' 的 GameObject 未设置");
            return;
        }

        // 关闭面板
        panel.panelObject.SetActive(false);

        // 如果关闭的是当前打开的面板，清空引用
        if (currentlyOpenPanel == panel)
        {
            currentlyOpenPanel = null;
        }

        // 执行自定义回调
        panel.onCloseCallback?.Invoke();

        if (logDebugInfo)
        {
            Debug.Log($"关闭了面板: {panelName}");
        }
    }

    // 获取指定面板
    public UIPanel GetPanel(string panelName)
    {
        panelDictionary.TryGetValue(panelName, out UIPanel panel);
        return panel;
    }

    // 检查面板是否打开
    public bool IsPanelOpen(string panelName)
    {
        if (!panelDictionary.TryGetValue(panelName, out UIPanel panel))
        {
            return false;
        }
        return panel.panelObject != null && panel.panelObject.activeSelf;
    }

    // 关闭所有面板
    public void CloseAllPanels()
    {
        foreach (var panel in panels)
        {
            if (panel.panelObject != null && panel.panelObject.activeSelf)
            {
                panel.panelObject.SetActive(false);
                panel.onCloseCallback?.Invoke();
            }
        }
        currentlyOpenPanel = null;

        if (logDebugInfo)
        {
            Debug.Log("关闭了所有面板");
        }
    }

    // 添加新面板（运行时动态添加）
    public void AddPanel(UIPanel newPanel)
    {
        if (string.IsNullOrEmpty(newPanel.panelName))
        {
            newPanel.panelName = newPanel.panelObject != null ? newPanel.panelObject.name : $"Panel_{panels.Count}";
        }

        // 检查是否已存在同名面板
        if (panelDictionary.ContainsKey(newPanel.panelName))
        {
            Debug.LogWarning($"已存在名为 '{newPanel.panelName}' 的面板，将使用新名称 '{newPanel.panelName}_new'");
            newPanel.panelName = $"{newPanel.panelName}_new";
        }

        panels.Add(newPanel);
        InitializePanels(); // 重新初始化以设置按钮事件
    }

    // 移除面板
    public void RemovePanel(string panelName)
    {
        for (int i = panels.Count - 1; i >= 0; i--)
        {
            if (panels[i].panelName == panelName)
            {
                // 如果面板是打开的，先关闭它
                if (panels[i].panelObject != null && panels[i].panelObject.activeSelf)
                {
                    ClosePanel(panelName);
                }

                panels.RemoveAt(i);
                panelDictionary.Remove(panelName);

                if (logDebugInfo)
                {
                    Debug.Log($"移除了面板: {panelName}");
                }
                return;
            }
        }
    }

    // 更新面板配置（用于动态修改）
    public void UpdatePanel(string panelName, UIPanel updatedPanel)
    {
        if (panelDictionary.ContainsKey(panelName))
        {
            // 保存旧面板的状态
            bool wasActive = panelDictionary[panelName].panelObject != null &&
                           panelDictionary[panelName].panelObject.activeSelf;

            // 移除旧面板
            RemovePanel(panelName);

            // 添加更新后的面板
            AddPanel(updatedPanel);

            // 恢复之前的活动状态
            if (wasActive)
            {
                OpenPanel(updatedPanel.panelName);
            }
        }
    }

    // 为特定面板添加回调
    public void AddOpenCallback(string panelName, Action callback)
    {
        if (panelDictionary.TryGetValue(panelName, out UIPanel panel))
        {
            panel.onOpenCallback += callback;
        }
    }

    public void AddCloseCallback(string panelName, Action callback)
    {
        if (panelDictionary.TryGetValue(panelName, out UIPanel panel))
        {
            panel.onCloseCallback += callback;
        }
    }
}