using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class TaggedAudio
    {
        public string tagName;
        public AudioClip clip;
    }

    public List<TaggedAudio> tagAudioList;
    public AudioSource audioSource;

    private Dictionary<string, AudioClip> audioDict = new Dictionary<string, AudioClip>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        foreach (var item in tagAudioList)
        {
            if (!audioDict.ContainsKey(item.tagName))
                audioDict.Add(item.tagName, item.clip);
        }
    }

    public void PlaySoundForTag(string tag)
    {
        if (audioDict.ContainsKey(tag))
        {
            audioSource.PlayOneShot(audioDict[tag]);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] 사운드 없음: 태그 {tag}");
        }
    }
}
