using System.Collections;
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
        }
        else
        {
            Debug.LogWarning("FadeImage가 연결되지 않았습니다!");
        }
    }

    public void ChangeLevel(string sceneName)
    {
        nextSceneName = sceneName;
        StartCoroutine(SceneLoadRoutine());
    }

    private IEnumerator SceneLoadRoutine()
    {
        // 1. 페이드 아웃
        yield return StartCoroutine(Fade(0, 1));

        // 2. 로딩 씬 로드
        SceneManager.LoadScene("LoadingScene");
    }

    public void StartFadeIn(float Duration = 1f)
    {
        fadeDuration = Duration;
        StartCoroutine(Fade(1, 0));
    }

    public void StartFadeOut(float Duration = 1f)
    {
        fadeDuration = Duration;
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
