using UnityEngine;

public class TriggerCollideSound : MonoBehaviour
{
    public string actionName = "grab";  // 또는 "release"
    public bool useTrigger = true;

    void OnTriggerEnter(Collider other)
    {
        if (useTrigger)
            TryPlaySound(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (useTrigger && actionName == "release") // 예시
            TryPlaySound(other);
    }

    void TryPlaySound(Collider other)
    {
        if (!other.CompareTag("Untagged"))
        {
            AudioManager.Instance?.PlaySoundForTag(other.tag, actionName);
        }
    }
}
