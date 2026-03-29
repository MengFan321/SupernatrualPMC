using UnityEngine;
using System.Collections.Generic;
using System.Linq; // 需要引入 Linq 用于查找

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class SoundEffect
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [Header("音效列表")]
    public List<SoundEffect> soundEffects = new List<SoundEffect>();

    // 用于存储正在播放的音效源
    private Dictionary<string, AudioSource> activeSources = new Dictionary<string, AudioSource>();
    private Dictionary<string, SoundEffect> soundDictionary = new Dictionary<string, SoundEffect>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初始化音效字典
        UpdateSoundDictionary();
    }

    public void PlaySFX(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            SoundEffect sound = soundDictionary[soundName];
            if (sound.clip != null)
            {
                // 创建新的 AudioSource 来播放（而不是用 PlayOneShot）
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.clip = sound.clip;
                source.volume = sound.volume;
                source.loop = false; // 默认不循环
                source.Play();

                // 记录这个音效源
                if (activeSources.ContainsKey(soundName))
                {
                    // 如果已经存在同名音效，先销毁旧的
                    Destroy(activeSources[soundName]);
                    activeSources[soundName] = source;
                }
                else
                {
                    activeSources.Add(soundName, source);
                }

                // 播放完后自动销毁组件（非循环音效）
                if (!source.loop)
                {
                    Destroy(source, source.clip.length);
                    activeSources.Remove(soundName); // 播放完从字典移除
                }
            }
        }
        else
        {
            Debug.LogWarning($"音效 '{soundName}' 未找到！");
        }
    }

    // === 新增：停止指定音效 ===
    public void StopSFX(string soundName)
    {
        if (activeSources.ContainsKey(soundName))
        {
            AudioSource source = activeSources[soundName];
            if (source != null)
            {
                source.Stop();
                Destroy(source);
            }
            activeSources.Remove(soundName);
        }
    }

    // === 新增：播放循环音效（用于电话铃声）===
    public void PlayLoopSFX(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            SoundEffect sound = soundDictionary[soundName];
            if (sound.clip != null)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.clip = sound.clip;
                source.volume = sound.volume;
                source.loop = true; // 设置为循环
                source.Play();

                if (activeSources.ContainsKey(soundName))
                {
                    Destroy(activeSources[soundName]);
                }
                activeSources[soundName] = source;
            }
        }
    }

    public void AddSoundEffect(string name, AudioClip clip, float volume = 1f)
    {
        SoundEffect newSound = new SoundEffect
        {
            name = name,
            clip = clip,
            volume = volume
        };

        soundEffects.Add(newSound);
        UpdateSoundDictionary();
    }

    public void RemoveSoundEffect(string name)
    {
        SoundEffect soundToRemove = soundEffects.Find(sound => sound.name == name);
        if (soundToRemove != null)
        {
            soundEffects.Remove(soundToRemove);
            UpdateSoundDictionary();
        }
    }

    public void UpdateSoundEffect(string oldName, string newName, AudioClip newClip = null, float newVolume = -1f)
    {
        SoundEffect soundToUpdate = soundEffects.Find(sound => sound.name == oldName);
        if (soundToUpdate != null)
        {
            soundToUpdate.name = newName;
            if (newClip != null) soundToUpdate.clip = newClip;
            if (newVolume >= 0) soundToUpdate.volume = newVolume;

            UpdateSoundDictionary();
        }
    }

    private void UpdateSoundDictionary()
    {
        soundDictionary.Clear();
        foreach (var sound in soundEffects)
        {
            if (!soundDictionary.ContainsKey(sound.name))
            {
                soundDictionary.Add(sound.name, sound);
            }
            else
            {
                Debug.LogWarning($"重复的音效名称: {sound.name}");
            }
        }
    }

    public List<string> GetAllSoundNames()
    {
        List<string> names = new List<string>();
        foreach (var sound in soundEffects)
        {
            names.Add(sound.name);
        }
        return names;
    }
}