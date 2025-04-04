using System.Linq.Expressions;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private OVRCameraRig CameraRig;

    private Transform TransformComponent;
    private Rigidbody RigidBodyComponent;

    [SerializeField]
    private float MoveSpeed = 10.0f;
    [SerializeField]
    private float MouseSensitivity = 10.0f;

    protected void Awake()
    {
        CameraRig = GameObject.Find("Camera Rig").GetComponent<OVRCameraRig>();
        Assert.IsNotNull(CameraRig, "CameraRig가 할당되어 있지 않습니다.");

        TransformComponent = GetComponent<Transform>();
        Assert.IsNotNull(TransformComponent, "TransformComponent 할당되어 있지 않습니다.");

        RigidBodyComponent = GetComponent<Rigidbody>();
        Assert.IsNotNull(TransformComponent, "RigidBodyComponent 할당되어 있지 않습니다.");

    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        #region 이동
        if (OVRManager.isHmdPresent)
        {
            ControllerMove();
        }
        else
        {
            KeyboardMove();
        }
        #endregion
    }

    private void ControllerMove()
    {
        if (OVRInput.Get(OVRInput.Touch.PrimaryThumbstick))
        {
            Vector2 vInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            Vector3 vCoord = new Vector3(vInput.x, 0.0f, vInput.y);

            Vector3 CameraForward = CameraRig.centerEyeAnchor.transform.forward;
            CameraForward.y = 0;
            CameraForward.Normalize();

            Vector3 CameraRight = CameraRig.centerEyeAnchor.transform.right;
            CameraRight.y = 0;
            CameraRight.Normalize();

            Vector3 MoveDir = CameraForward * vCoord.z + CameraRight * vCoord.x;
            MoveDir.Normalize();

            TransformComponent.position += MoveDir * MoveSpeed * Time.deltaTime;
        }
    }

    private void KeyboardMove()
    {
        Vector3 vKeyInput = Vector3.zero;

        #region 키 입력
        if (Input.GetKey(KeyCode.W))
        {
            vKeyInput.z += 1.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            vKeyInput.z -= 1.0f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            vKeyInput.x -= 1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            vKeyInput.x += 1.0f;
        }
        vKeyInput.Normalize();
        #endregion

        if (vKeyInput.sqrMagnitude > 0.01f)
        {
            Vector3 CameraForward = CameraRig.centerEyeAnchor.transform.forward;
            CameraForward.y = 0;
            CameraForward.Normalize();

            Vector3 CameraRight = CameraRig.centerEyeAnchor.transform.right;
            CameraRight.y = 0;
            CameraRight.Normalize();

            Vector3 MoveDir = CameraForward * vKeyInput.z + CameraRight * vKeyInput.x;
            MoveDir.Normalize();

            TransformComponent.position += MoveDir * MoveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            float MouseX = Input.GetAxis("Mouse X") * MouseSensitivity;
            float MouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;

            Quaternion Pitch = Quaternion.AngleAxis(-MouseY, Vector3.right);
            Quaternion Yaw = Quaternion.AngleAxis(MouseX, Vector3.up);

            // 기존 회전에 덧붙이기 (회전 순서: yaw → pitch)
            TransformComponent.rotation = TransformComponent.rotation * Yaw;
            TransformComponent.rotation = TransformComponent.rotation * Pitch;
        }
    }
}