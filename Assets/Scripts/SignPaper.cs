using UnityEngine;

public class SignPaper : MonoBehaviour
{
    public Material signedMat;
    public GameObject howToControlWall;
    private bool isSigned;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isSigned = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Sign()
    {
        if(!isSigned && signedMat)
        {
            this.GetComponent<MeshRenderer>().material = signedMat;

            GameObject.Find("AudioManager").GetComponent<AudioManager>().PlaySound(this.gameObject, "sign");
            howToControlWall.SetActive(false);
        }
    }
}
