using UnityEngine;

public class TriggerCollideSound : MonoBehaviour
{
    public string actionName = "grab";  // �Ǵ� "release"
    public bool useTrigger = true;

    void OnTriggerEnter(Collider other)
    {
        if (useTrigger)
            TryPlaySound(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (useTrigger && actionName == "release") // ����
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
