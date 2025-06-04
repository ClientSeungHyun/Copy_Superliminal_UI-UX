using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class TagActionClip
    {
        public string action;
        public AudioClip clip;
    }

    [Header("Tag + Action 매핑 리스트")]
    public List<TagActionClip> tagActionClips;

    [Header("재생용 AudioSource")]
    public AudioSource audioSource;

    public Dictionary<string, AudioClip> soundMap;

    void Awake()
    {
        // 싱글톤
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        BuildSoundMap();
        DontDestroyOnLoad(this);
    }

    // 리스트를 Dictionary로 변환
    void BuildSoundMap()
    {
        soundMap = new Dictionary<string,  AudioClip>();

        foreach (var entry in tagActionClips)
        {
            if (!soundMap.ContainsKey(entry.action))
                soundMap.Add(entry.action, entry.clip);

            if (soundMap.ContainsKey(entry.action))
                soundMap[entry.action] = entry.clip;
        }
    }

    // 외부에서 호출하는 메서드
    public void PlaySound(GameObject gameObject, string action)
    {
        AudioSource targetAudioSource = gameObject.GetComponent<AudioSource>();
        if (!targetAudioSource)
        {
            targetAudioSource = gameObject.AddComponent<AudioSource>();
        }

        if (soundMap.TryGetValue(action, out var clip))
        {
            targetAudioSource.PlayOneShot(clip);
            return;

        }

        //Debug.LogWarning($"[AudioManager] 사운드를 찾을 수 없음 - 태그: {gameObject.tag}, 액션: {action}");
    }
}
