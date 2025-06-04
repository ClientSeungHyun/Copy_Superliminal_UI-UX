using UnityEngine;

public class EndingScript : MonoBehaviour
{
    public GameObject endingPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.transform.GetComponent<Player>().StartPlayerEning(endingPoint.transform.position);
        }
    }
}
