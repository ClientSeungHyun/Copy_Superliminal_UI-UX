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
            SceneTransitionManager.Instance.ChangeLevel("Map");
        }
    }
}
