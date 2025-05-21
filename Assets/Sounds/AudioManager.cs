using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class TaggedAudio
    {
        public string tagName;
        public string actionName; // 예: "grab", "release"
        public AudioClip clip;
    }

    public List<TaggedAudio> tagAudioList;
    public AudioSource audioSource;

    private Dictionary<string, Dictionary<string, AudioClip>> audioDict = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var item in tagAudioList)
        {
            if (!audioDict.ContainsKey(item.tagName))
                audioDict[item.tagName] = new Dictionary<string, AudioClip>();

            if (!audioDict[item.tagName].ContainsKey(item.actionName))
                audioDict[item.tagName][item.actionName] = item.clip;
        }
    }

    public void PlaySoundForTag(string tag, string action)
    {
        if (audioDict.ContainsKey(tag) && audioDict[tag].ContainsKey(action))
        {
            audioSource.PlayOneShot(audioDict[tag][action]);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] 사운드 없음: 태그 {tag}, 액션 {action}");
        }
    }
}

