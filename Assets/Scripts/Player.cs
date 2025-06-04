using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : PortalTraveller
{
    private OVRCameraRig        CameraRig;

    private Transform           TransformCom;
    private Rigidbody           RigidBodyCom;
    private CapsuleCollider     CapsuleColliderCom;

    public GameObject SelectObject;
    [SerializeField]
    private Transform   SelectObjectTransformCom;
    private Vector3     SelectObjectOriginalScale;
    private float       SelectObjectInitialDistance;

    [SerializeField]
    private float   MoveSpeed = 7.0f;
    [SerializeField]
    private float   MouseSensitivity = 10.0f;
    [SerializeField]
    private float   JumpPower = 1000.0f;

    [SerializeField]
    private bool    isGround = true;

    [SerializeField]
    private GameObject RightController;

    public float yaw;
    public float pitch;
    float smoothYaw;
    float smoothPitch;
    Vector3 velocity;

    protected void Awake()
    {
       
    }

    void Start()
    {
        CameraRig = GameObject.Find("Camera Rig").GetComponent<OVRCameraRig>();
        Assert.IsNotNull(CameraRig, "CameraRig가 할당되어 있지 않습니다.");

        TransformCom = GetComponent<Transform>();
        Assert.IsNotNull(TransformCom, "TransformComponent 할당되어 있지 않습니다.");

        RigidBodyCom = GetComponent<Rigidbody>();
        Assert.IsNotNull(RigidBodyCom, "RigidBodyComponent 할당되어 있지 않습니다.");
        RigidBodyCom.mass = 3.0f;

        CapsuleColliderCom = GetComponent<CapsuleCollider>();
        Assert.IsNotNull(CapsuleColliderCom, "CapsuleColliderCom 할당되어 있지 않습니다.");

        JumpPower = 1000.0f;
        isGround = true;

        yaw = transform.eulerAngles.y;
        pitch = CameraRig.centerEyeAnchor.transform.localEulerAngles.x;
        smoothYaw = yaw;
        smoothPitch = pitch;
    }

    void Update()
    {
        //FindSelectObject();
        //DragSelectObject();

        CheckGround();

    }

    private void FixedUpdate()
    {
        #region 이동
        if (OVRManager.isHmdPresent)
        {
            ControllerLocomotion();
        }
        else
        {
            KeyboardLocomotion();
        }
        #endregion
    }

    private void LateUpdate()
    {

    }

    private void ControllerLocomotion()
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

            Vector3 TargetPosition = RigidBodyCom.position + MoveDir * MoveSpeed * Time.fixedDeltaTime;
            RigidBodyCom.MovePosition(TargetPosition);
        }
        else
        {
            Vector3 CurrentVelocity = RigidBodyCom.linearVelocity;
            RigidBodyCom.linearVelocity = new Vector3(0, CurrentVelocity.y, 0);
        }

        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            isGround = false;
            RigidBodyCom.AddForce(new Vector3(0, JumpPower, 0));

        }
    }

    private void KeyboardLocomotion()
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

            Vector3 MoveDir = (CameraForward * vKeyInput.z + CameraRight * vKeyInput.x).normalized;

            Vector3 TargetPosition = RigidBodyCom.position + MoveDir * MoveSpeed * Time.fixedDeltaTime;
            RigidBodyCom.MovePosition(TargetPosition);
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            float MouseX = Input.GetAxis("Mouse X") * MouseSensitivity;

            Quaternion Yaw = Quaternion.AngleAxis(MouseX, Vector3.up);

            CameraRig.centerEyeAnchor.transform.rotation = CameraRig.centerEyeAnchor.transform.rotation * Yaw;
        }

        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            isGround = false;
            RigidBodyCom.AddForce(new Vector3(0, JumpPower, 0));
        }
    }

    private void FindSelectObject()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray Mouseray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit Hit;
            if (Physics.Raycast(Mouseray, out Hit, 100.0f, LayerMask.GetMask("Grabable")))
            {
                SelectObject = Hit.transform.gameObject;
                SelectObject.GetComponent<Rigidbody>().useGravity = false;

                SelectObjectOriginalScale = SelectObject.transform.localScale;
                SelectObjectInitialDistance = Vector3.Distance(CameraRig.centerEyeAnchor.transform.position, SelectObject.transform.position);

                SelectObjectTransformCom = Hit.transform;
            }
        }
    }

    private void DragSelectObject()
    {
        if (SelectObject)
        {
            Ray MouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 TargetPosition = MouseRay.GetPoint(10.0f);

            int GrabableLayerMask = 1 << LayerMask.NameToLayer("Grabable");

            // Raycast 해서 targetPosition 업데이트
            RaycastHit hit;
            if (Physics.Raycast(MouseRay, out hit, 100f, ~GrabableLayerMask))
            {
                TargetPosition = hit.point;
            }

            Collider ObjectCollider = SelectObject.GetComponent<Collider>();
            float ObjectRadius = ObjectCollider.bounds.extents.magnitude * 0.5f;

            Vector3 OffsetFromSurface = hit.normal * ObjectRadius;
            TargetPosition += OffsetFromSurface;

            SelectObject.transform.position = Vector3.Lerp(SelectObject.transform.position, TargetPosition, 10f * Time.deltaTime);

            // 거리 비례 스케일 조정
            float CurrentDistance = Vector3.Distance(CameraRig.centerEyeAnchor.transform.position, TargetPosition);
            float ScaleMultiplier = CurrentDistance / SelectObjectInitialDistance;
            SelectObject.transform.localScale = SelectObjectOriginalScale * ScaleMultiplier;

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                SelectObject.GetComponent<Rigidbody>().useGravity = true;
                SelectObject.GetComponent<Rigidbody>().isKinematic = false;

                SelectObject = null;
            }
        }
    }

    private void CheckGround()
    {
        Vector3 CheckPosition = transform.position;
        CheckPosition.y -= CapsuleColliderCom.height * 0.5f;

        bool bRayHit = Physics.Raycast(CheckPosition, Vector3.down, 0.5f, LayerMask.GetMask("Ground"));
        bool bSphereHit = Physics.CheckSphere(CheckPosition, 0.3f, LayerMask.GetMask("Ground"));

        isGround = bRayHit || bSphereHit;

        // 디버그용
        Debug.DrawRay(CheckPosition, Vector3.down * 0.5f, isGround ? Color.green : Color.red);
    }

    public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        Vector3 eulerRot = rot.eulerAngles;
        float delta = Mathf.DeltaAngle(smoothYaw, eulerRot.y);
        yaw += delta;
        smoothYaw += delta;
        transform.eulerAngles = Vector3.up * smoothYaw;
        velocity = toPortal.TransformVector(fromPortal.InverseTransformVector(velocity));
        Physics.SyncTransforms();
    }
}