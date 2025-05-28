using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    private TextMeshProUGUI loadingTmp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        progressBar = GameObject.Find("ProgressBar").GetComponent<Image>();
        progressBar.fillAmount = 0f;

        loadingTmp = GameObject.Find("LoadingText").GetComponent<TextMeshProUGUI>();

        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogError("SceneTransitionManager 인스턴스가 존재하지 않습니다. 씬 로드를 중단합니다.");
            return;
        }

        string nextScene = SceneTransitionManager.Instance.NextSceneName();
        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogError("다음 씬 이름이 비어 있습니다. 씬 로드를 중단합니다.");
            return;
        }

        SceneTransitionManager.Instance.StartFadeIn(0f);

        StartCoroutine(LoadSceneProcess());
    }

    private IEnumerator LoadSceneProcess()
    {
        AsyncOperation asyncOper = SceneManager.LoadSceneAsync(SceneTransitionManager.Instance.NextSceneName());
        asyncOper.allowSceneActivation = false;

        float loadingPercent = 0f;

        float unsacledTimer1 = 0.0f;
        float unsacledTimer2 = 0.0f;
        float textTimer = 1f;

        while (!asyncOper.isDone)
        {
            yield return null;

            textTimer += Time.unscaledDeltaTime * 2f;
            if (textTimer >= 4f)
                textTimer = 0f;

            if (asyncOper.progress < 0.9f || loadingPercent < 89f)
            {
                unsacledTimer1 += Time.unscaledDeltaTime;

                if (loadingPercent <= 89f)
                {
                    loadingPercent = Mathf.Lerp(loadingPercent, 89f, unsacledTimer1 * 0.5f);
                }
            }
            else
            {
                unsacledTimer2 += Time.unscaledDeltaTime;

                loadingPercent = 89f;
                loadingPercent = Mathf.Lerp(loadingPercent, 100f, unsacledTimer2 * 0.7f);

                if (loadingPercent == 100f)
                {
                    SceneTransitionManager.Instance.StartFadeIn(3f);
                    asyncOper.allowSceneActivation = true;
                }
            }

            progressBar.fillAmount = loadingPercent * 0.01f;

            string loadingText = "Loading";
            for (int i = 1; i <= (int)textTimer; ++i) 
            {
                loadingText += ".";
            }
            loadingTmp.text = loadingText;
            

        }
    }

}
