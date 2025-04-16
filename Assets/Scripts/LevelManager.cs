using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private Canvas LoaderCanvas;
    [SerializeField] private Image ProgressBar;

    public string NextSceneName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Awake();

        DontDestroyOnLoad(this);
        Debug.Log("GameManager 생성됨!");
    }

    public void FindProgress()
    {
        ProgressBar = GameObject.Find("ProgressBar").GetComponent<Image>();
        LoaderCanvas = ProgressBar.GetComponentInParent<Canvas>();
    }

    public async void LoadScene(string SceneName)
    {
        NextSceneName = SceneName;
        var Scene = SceneManager.LoadSceneAsync(SceneName);
        Scene.allowSceneActivation = false;

        LoaderCanvas.gameObject.SetActive(true);

        do {
            await Task.Delay(100);
            ProgressBar.fillAmount = Scene.progress;
        } while (Scene.progress < 0.9f);

        Scene.allowSceneActivation = true;
        LoaderCanvas.gameObject.SetActive(false);
    }
}
