using UnityEngine;

public class GameManager: Singleton<GameManager>
{
   
    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this);
        Debug.Log("GameManager 생성됨!");
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
