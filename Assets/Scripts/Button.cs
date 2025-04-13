using UnityEngine;
using UnityEngine.UIElements;

public class Button : MonoBehaviour
{
    private Vector3 FirstPosition;
    private Transform MyTransform;

    public bool isPressed = false;
    private bool isCollisionExit = false;
    private float UnpressedTimer = 0.0f;

    //테스트용
    //public GameObject cube;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MyTransform = this.GetComponent<Transform>();
        FirstPosition = MyTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPressed)
        {
            MyTransform.position = new Vector3(FirstPosition.x, FirstPosition.y - 0.03f, FirstPosition.z);
        }

        if(isCollisionExit)
        {
            UnpressedTimer += Time.deltaTime;

            if (UnpressedTimer > 0.05f)
            {
                MyTransform.position = FirstPosition;
                isPressed = false;
                isCollisionExit = false;
            }

        }
        
        //테스트용
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    cube.GetComponent<Transform>().position = new Vector3(FirstPosition.x, FirstPosition.y + 10.0f, FirstPosition.z);
        //}

    }

    private void OnCollisionEnter(Collision collision)
    {
        isPressed = true;
        isCollisionExit = false;
    }
    private void OnCollisionStay(Collision collision)
    {
    }

    private void OnCollisionExit(Collision collision)
    {
        isCollisionExit = true;
        UnpressedTimer = 0.0f;
    }
}
