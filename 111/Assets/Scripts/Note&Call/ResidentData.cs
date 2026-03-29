using UnityEngine;

[System.Serializable]
public class ResidentData
{
    [Header("住户基本信息")]
    public int residentId;           // 住户ID
    public string residentName;      // 住户姓名
    public int roomNumber;          // 房间号
    public bool isUnlocked;         // 是否已解锁
    public bool isGone;             // 是否已离开

    [Header("表情图片")]
    public Sprite neutralFace;      // 中性表情
    public Sprite happyFace;        // 高兴表情
    public Sprite sadFace;          // 悲伤表情
    public Sprite angryFace;        // 愤怒表情

    [Header("游戏进度")]
    public bool hasKey;             // 是否已留下钥匙
    public bool isCompleted;        // 是否已完成对话
    public int suspicionImpact;     // 怀疑度影响

    [Header("书本页面信息")]
    public string pageTitle;        // 书本页面标题
    [TextArea(3, 5)]
    public string pageDescription;  // 页面描述
    public Sprite pageImage;        // 页面图片

    // 构造函数
    public ResidentData(int id, string name, int room)
    {
        residentId = id;
        residentName = name;
        roomNumber = room;
        isUnlocked = false;
        isGone = false;
        hasKey = false;
        isCompleted = false;
    }
}