using System.Diagnostics;
using UnityEngine;

public class raser : MonoBehaviour
{
    public LineRenderer laserLine; // 레이저 시각화용
    public float laserDistance = 100f;
    public Transform trackingSpace;
    public GameObject RController;

    void Start()
    {
        laserLine.positionCount = 2;
        laserLine.useWorldSpace = true;
    }

    void Update()
    {
        FireLaser();
    }

    void FireLaser()
    {
        // LeftController의 위치와 회전 정보 가져오기
        Vector3 localPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion localRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        // TrackingSpace 기준으로 월드 좌표로 변환
        Vector3 worldPos = trackingSpace.TransformPoint(localPos);
        Vector3 forward = trackingSpace.TransformDirection(localRot * Vector3.forward);
        forward.Normalize();

        // 레이저 시작 위치를 LeftController의 위치로 설정
        laserLine.SetPosition(0, RController.transform.position);

        // 레이저 발사용 Ray 설정
        Ray ray = new Ray(RController.transform.position, forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, laserDistance))
        {
            // 레이저가 충돌한 지점까지의 위치를 설정
            laserLine.SetPosition(1, hit.point);

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                UnityEngine.Debug.Log("오른쪽 검지 버튼 눌림!");
                // 충돌한 게임 오브젝트 가져오기
                GameObject hitObject = hit.collider.gameObject;
                UnityEngine.Debug.Log("Hit object: " + hitObject.name);

            }

        }
        else
        {
            // 충돌하지 않은 경우 레이저가 최대 거리까지 계속 가도록 설정
            laserLine.SetPosition(1, RController.transform.position + forward * laserDistance);
        }
    }
}
