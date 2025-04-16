using Unity.VisualScripting;
using UnityEngine;

public class NoObjectDoor : MonoBehaviour
{
    private GameObject Player;
    private Material MyMat;
    private Collider MyCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.Find("Player");
        MyMat = this.GetComponent<Renderer>().material;
        MyCollider = this.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Player.GetComponent<Player>().SelectObject != null)
        {
            MyMat.SetFloat("_Scale", 10);
            MyCollider.isTrigger = false;
        }
        else
        {
            MyMat.SetFloat("_Scale", 4);
            MyCollider.isTrigger = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider != null)
            Debug.Log("붙음");
    }
}
