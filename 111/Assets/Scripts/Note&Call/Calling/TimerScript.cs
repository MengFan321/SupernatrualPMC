using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    [Header("=== 配置 ===")]
    public float waitTime = 60f;
    public Animator phoneAnimator;
    public string boolName = "IsCalling";
    public Button phoneButton;

    [Header("=== 内部状态 ===")]
    private float currentTimer = 0f;
    private bool isCalling = false;
    private bool hasStarted = false;

    void Start()
    {
        if (phoneAnimator != null)
        {
            phoneAnimator.enabled = false;
        }
    }

    void Update()
    {
        if (!hasStarted && currentTimer < waitTime)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= waitTime)
            {
                StartCalling();
            }
        }
    }

    void StartCalling()
    {
        if (hasStarted) return;
        hasStarted = true;
        isCalling = true;

        // 1. 启用动画
        if (phoneAnimator != null)
        {
            phoneAnimator.enabled = true;
            phoneAnimator.SetBool(boolName, true);
        }

        // 2. 【音效播放】开始播放铃声
        AudioManager.Instance?.PlayLoopSFX("RingTone");
    }

    public void OnPhoneClicked()
    {
        if (isCalling)
        {
            StopCalling();
        }
    }

    public void StopCalling()
    {
        isCalling = false;

        // 停止动画
        if (phoneAnimator != null)
        {
            phoneAnimator.SetBool(boolName, false);
        }
        else
        {
            Debug.LogError("phoneAnimator 是空的！");
        }

        // 停止音效
        AudioManager.Instance?.StopSFX("RingTone");

        PanelManager panelManager = FindObjectOfType<PanelManager>();
        if (panelManager != null)
        {
            panelManager.OpenPanel("PhonePanel");
        }
    }
}