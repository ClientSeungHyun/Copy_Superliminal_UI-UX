using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
   
    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);

        Application.targetFrameRate = 60;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeLevel("Map");
        }
    }

    void ChangeLevel(string SceneName)
    {
        SceneManager.LoadScene("LoadingScene");
        LevelManager.Instance.NextSceneName = SceneName;
    }
}
