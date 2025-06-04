using UnityEngine;
using TMPro;
using System.Collections;

public class SubtitleManager : MonoBehaviour
{
    public SubtitleSet subtitleSet;
    public TMP_Text subtitleTextUI;

    private Coroutine displayCoroutine;

    public void OnSignalReceived(string id)
    {
        SubtitleData data = subtitleSet.GetSubtitle(id);
        if (data != null)
        {
            if (displayCoroutine != null) StopCoroutine(displayCoroutine);
            displayCoroutine = StartCoroutine(ShowSubtitle(data.text, data.duration));
        }
    }

    private IEnumerator ShowSubtitle(string text, float duration)
    {
        subtitleTextUI.text = text;
        subtitleTextUI.enabled = true;
        yield return new WaitForSeconds(duration);
        subtitleTextUI.enabled = false;
    }
}
