using Unity.VisualScripting;
using UnityEngine;

public class NoObjectDoor : MonoBehaviour
{
    private GameObject Player;
    private Material MyMat;
    private Collider MyCollider;
    private MeshRenderer MyRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.Find("Player");
       // MyMat = this.GetComponent<Renderer>().material;
        MyCollider = this.GetComponent<Collider>();
        MyRenderer = this.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Player.GetComponent<Grapable>().takenObject != null)
        {
            //MyMat.SetFloat("_Scale", 10);
            MyRenderer.enabled = true;
            MyCollider.isTrigger = false;
        }
        else
        {
            // MyMat.SetFloat("_Scale", 2);
            MyRenderer.enabled = false;
            MyCollider.isTrigger = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.collider != null)
           // Debug.Log("붙음");
    }
}
