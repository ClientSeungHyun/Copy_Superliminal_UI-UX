using TMPro;
using UnityEngine;

public class ButtonDoor : MonoBehaviour
{
    public GameObject Button;
    public GameObject Button2 = null;
    private Vector3 FirstPosition;
    private Vector3 OpenPosition;
    private Transform MyTransform;
    private float Speed = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MyTransform = this.GetComponent<Transform>();
        FirstPosition = MyTransform.position;
        OpenPosition = MyTransform.GetChild(0).position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Button2 == null && Button.GetComponent<Button>().isPressed)
        {
            MovetoOpenPosition();
        }
        else if(Button.GetComponent<Button>().isPressed && Button2.GetComponent<Button>().isPressed)
        {
            MovetoOpenPosition();
        }
        else
        {
            MovetoFirstPosition();
        }
    }

    private void MovetoOpenPosition()
    {
        MyTransform.position = Vector3.MoveTowards(transform.position, OpenPosition, Speed * Time.deltaTime);
    }

    private void MovetoFirstPosition()
    {
        MyTransform.position = Vector3.MoveTowards(transform.position, FirstPosition, Speed * Time.deltaTime);
    }
}
