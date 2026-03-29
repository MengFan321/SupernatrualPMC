using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;

public class BookManager : MonoBehaviour
{
    public static BookManager Instance { get; private set; }

    [Header("=== 面板引用 ===")]
    [SerializeField] private GameObject bookPanel;
    [SerializeField] private GameObject residentContent;
    [SerializeField] private GameObject manualContent;

    [Header("=== 书签按钮 ===")]
    [SerializeField] private Button residentTabButton;
    [SerializeField] private Button manualTabButton;
    [SerializeField] private Color selectedTabColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color normalTabColor = new Color(0.6f, 0.6f, 0.6f, 1f);

    [Header("=== 员工手册内容 ===")]
    [SerializeField] private TextMeshProUGUI manualText;
    [SerializeField] private ParticleSystem updateEffect;
    [SerializeField] private AudioClip updateSound;
    private bool manualHasNewContent = false;

    [Header("=== 手册内容配置 ===")]
    [TextArea(3, 10)]
    [SerializeField] private string initialManualText = "耐心倾听，找到顾客真正想解决的问题并用钢笔标记，最好注意那些不寻常的句子";

    [TextArea(3, 10)]
    [SerializeField] private string afterFirstCallText = "如果你觉得太为难，桌子上的骰子帮助过不少走投无路的人。";

    [TextArea(3, 10)]
    [SerializeField] private string afterFirstResidentText = "最近留言系统出了问题，一次只能同时显示三句话";

    [TextArea(3, 10)]
    [SerializeField] private string afterSecondResidentText = "我司引进了一批先进的设备，有条件的员工可以自行购买";

    [TextArea(3, 10)]
    [SerializeField] private string afterLastResidentText = "房间的钥匙丢失很久了，最后一个见到它的人说它变成了一些金色的碎片，落进一些物品里了。";

    private Dictionary<GameProgress, string> manualTexts = new Dictionary<GameProgress, string>();

    // 游戏进度枚举
    public enum GameProgress
    {
        Initial,
        AfterFirstCall,
        AfterFirstResident,
        AfterSecondResident,
        AfterLastResident
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeManualTexts();
    }

    private void Start()
    {
        InitializeBook();
        SetupEventListeners();
        SwitchToResidentTab();
    }

    private void InitializeManualTexts()
    {
        manualTexts[GameProgress.Initial] = initialManualText;
        manualTexts[GameProgress.AfterFirstCall] = afterFirstCallText;
        manualTexts[GameProgress.AfterFirstResident] = afterFirstResidentText;
        manualTexts[GameProgress.AfterSecondResident] = afterSecondResidentText;
        manualTexts[GameProgress.AfterLastResident] = afterLastResidentText;
    }

    private void InitializeBook()
    {
        UpdateManualText(GameProgress.Initial);
        UpdateTabVisuals(true);
    }

    private void SetupEventListeners()
    {
        residentTabButton.onClick.AddListener(() => SwitchToResidentTab());
        manualTabButton.onClick.AddListener(() => SwitchToManualTab());
        RegisterGameEvents();
    }

    private void RegisterGameEvents()
    {
        // 这里可以注册游戏事件
    }

    #region 公开方法

    public void OpenBook()
    {
        if (bookPanel != null)
        {
            bookPanel.SetActive(true);
            if (manualHasNewContent)
            {
                ShowManualUpdateIndicator();
            }
        }
    }

    public void CloseBook()
    {
        if (bookPanel != null)
        {
            bookPanel.SetActive(false);
        }
    }

    public void SwitchToResidentTab() //标签切换至住户页面
    {
        residentContent.SetActive(true);
        manualContent.SetActive(false);
        UpdateTabVisuals(true);
        AudioManager.Instance?.PlaySFX("TabSwitch");
    }

    public void SwitchToManualTab() //切换至员工手册
    {
        residentContent.SetActive(false);
        manualContent.SetActive(true);
        UpdateTabVisuals(false);

        if (manualHasNewContent)
        {
            PlayManualUpdateEffect();
            manualHasNewContent = false;
        }
        AudioManager.Instance?.PlaySFX("TabSwitch");

        Blackboard blackboard = FindObjectOfType<Blackboard>(); //查找场景中的黑板
        blackboard.SetVariableValue("hasSwitchTab", true); // 设置 hasSwitchTab 为 true
    }

    public void UpdateManualText(GameProgress progress)
    {
        if (manualTexts.ContainsKey(progress))
        {
            string newText = manualTexts[progress];
            if (manualText.text != newText && !manualContent.activeSelf)
            {
                manualHasNewContent = true;
            }
            manualText.text = newText;
        }
    }

    #endregion

    #region UI视觉效果

    private void UpdateTabVisuals(bool residentSelected)
    {
        Image residentImage = residentTabButton.GetComponent<Image>();
        Image manualImage = manualTabButton.GetComponent<Image>();

        if (residentImage != null)
            residentImage.color = residentSelected ? selectedTabColor : normalTabColor;

        if (manualImage != null)
            manualImage.color = residentSelected ? normalTabColor : selectedTabColor;

        TextMeshProUGUI residentText = residentTabButton.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI manualText = manualTabButton.GetComponentInChildren<TextMeshProUGUI>();

        if (residentText != null && manualText != null)
        {
            residentText.fontStyle = residentSelected ? FontStyles.Bold : FontStyles.Normal;
            manualText.fontStyle = residentSelected ? FontStyles.Normal : FontStyles.Bold;
        }
    }

    private void ShowManualUpdateIndicator()
    {
        Transform indicator = manualTabButton.transform.Find("UpdateIndicator");
        if (indicator != null) indicator.gameObject.SetActive(true);
    }

    private void PlayManualUpdateEffect()
    {
        if (updateEffect != null) updateEffect.Play();
        if (updateSound != null) AudioSource.PlayClipAtPoint(updateSound, Camera.main.transform.position);
    }

    #endregion

    #region 工具方法

    public bool HasNewContent()
    {
        return manualHasNewContent;
    }

    #endregion
}