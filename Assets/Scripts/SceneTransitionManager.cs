using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : Singleton<SceneTransitionManager>
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private string nextSceneName;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        if (fadeImage == null)
        {
            fadeImage = GetComponentInChildren<Image>();
        }

        if (fadeImage != null)
        {
            fadeImage.color = new Color(0f, 0f, 0f, 0f);
            fadeImage.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("FadeImage가 연결되지 않았습니다!");
        }
    }

    public void ChangeLevel(string sceneName)
    {
        nextSceneName = sceneName;
        fadeImage.gameObject.SetActive(true);

        StartCoroutine(SceneLoadRoutine());
    }

    private IEnumerator SceneLoadRoutine()
    {
        yield return StartCoroutine(Fade(0, 1));

        SceneManager.LoadScene("LoadingScene");
    }

    public void StartFadeIn(float Duration = 1f, Color? fadeColor = null)
    {
        Color color = fadeColor ?? Color.black;
        fadeImage.color = color;

        fadeDuration = Duration;
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(Fade(1, 0));
    }

    public void StartFadeOut(float Duration = 1f, Color? fadeColor = null)
    {
        Color color = fadeColor ?? Color.black;
        fadeImage.color = color;

        fadeDuration = Duration;
        fadeImage.gameObject.SetActive(true);
        StartCoroutine(Fade(0, 1));
    }

    private IEnumerator Fade(float start, float end)
    {
        float currentTime = 0f;
        float percent = 0f;

        if (fadeDuration == 0f)
        {
            percent = 1f;
        }

        while (percent < 1f)
        {
            currentTime += Time.unscaledDeltaTime;
            percent = Mathf.Clamp01(currentTime / fadeDuration);

            if (fadeImage != null)
            {
                Color newColor = fadeImage.color;
                newColor.a = Mathf.Lerp(start, end, percent);
                fadeImage.color = newColor;
            }

            yield return null;
        }

        if (fadeImage != null)
        {
            Color finalColor = fadeImage.color;
            finalColor.a = end;
            fadeImage.color = finalColor;

            if(end == 0)
                fadeImage.gameObject.SetActive(false);
        }
    }

    public string NextSceneName()
    {
        return nextSceneName;
    }

    public void NextSceneName(string sceneName = "")
    {
        nextSceneName = sceneName;
    }

    public void FadeDuration(float duration)
    {
        fadeDuration = duration;
    }    
}
