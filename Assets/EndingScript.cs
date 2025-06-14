using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingScript : MonoBehaviour
{
    public GameObject endingPoint;

    private GameObject playerObject;

    private bool isLastFadeIn;
    private float sceneChangeTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isLastFadeIn = false;
        sceneChangeTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerObject)
        {
            float currentDist = Vector3.Distance(endingPoint.transform.position, playerObject.transform.position);

            if (!isLastFadeIn && currentDist < 15f)
            {
                SceneTransitionManager.Instance.StartFadeOut(1, Color.white);
                isLastFadeIn = true;
            }
            else if(isLastFadeIn)
            {
                sceneChangeTime += Time.deltaTime;

                if(sceneChangeTime >= 2.5f)
                {
                    SceneTransitionManager.Instance.StartFadeIn(1, Color.white);
                    SceneManager.LoadScene("StartScene");
                }
            }
        }
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.transform.GetComponent<Player>().StartPlayerEning(endingPoint.transform.position);
            playerObject = other.gameObject;
        }
    }
}
