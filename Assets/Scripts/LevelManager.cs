using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelManager : Singleton<GameManager>
{
    [SerializeField] private GameObject LoaderCanvas;
    [SerializeField] private Image ProgressBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Awake();

        DontDestroyOnLoad(this);
        Debug.Log("GameManager 생성됨!");
    }

    public async void LoadScene(string SceneName)
    {
        var Scene = SceneManager.LoadSceneAsync(SceneName);
        Scene.allowSceneActivation = false;

        LoaderCanvas.SetActive(true);

        do {
            await Task.Delay(100);
            ProgressBar.fillAmount = Scene.progress;
        } while (Scene.progress < 0.9f);

        Scene.allowSceneActivation = true;
        LoaderCanvas.SetActive(false);
    }
}
