using UnityEngine;

public class Grapable : MonoBehaviour
{
    public LineRenderer laserLine;         // 레이저 시각화를 위한 LineRenderer
    public float laserDistance = 50f;      // 레이저 최대 거리
    public Transform trackingSpace;        // 트래킹 공간 (VR의 플레이 영역)
    public GameObject RController;         // 오른쪽 컨트롤러 오브젝트

    private GameObject takenObject;        // 집은 오브젝트
    private Vector3 scaleMultiplier;       // 오브젝트의 원래 크기
    private float distanceMultiplier;      // 오브젝트와 컨트롤러 사이의 거리

    void Start()
    {
        laserLine.positionCount = 2;
        laserLine.useWorldSpace = true;
    }

    void Update()
    {
        FireLaser();           // 레이저 발사
        HandleObjectManipulation();  // 물체 조작
    }

    // 오른쪽 컨트롤러에서 레이저를 발사
    void FireLaser()
    {
        Vector3 localPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion localRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        Vector3 worldPos = trackingSpace.TransformPoint(localPos);
        Vector3 forward = trackingSpace.TransformDirection(localRot * Vector3.forward);
        forward.Normalize();

        // 레이저 시작 위치를 오른쪽 컨트롤러로 설정
        laserLine.SetPosition(0, RController.transform.position);

        Ray ray = new Ray(RController.transform.position, forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, laserDistance))
        {
            // 레이저가 충돌한 곳까지의 위치 설정
            laserLine.SetPosition(1, hit.point);

            // 오른쪽 컨트롤러에서 트리거 버튼을 눌렀을 때 물체를 집음
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                takenObject = hit.collider.gameObject;

                // 물체와의 거리 및 크기 저장
                distanceMultiplier = Vector3.Distance(RController.transform.position, takenObject.transform.position);
                scaleMultiplier = takenObject.transform.localScale;

                // Rigidbody가 없으면 추가
                if (takenObject.GetComponent<Rigidbody>() == null)
                {
                    takenObject.AddComponent<Rigidbody>();
                }
                takenObject.GetComponent<Rigidbody>().isKinematic = true;

                // Collider를 Trigger로 설정
                foreach (Collider col in takenObject.GetComponents<Collider>())
                {
                    col.isTrigger = true;
                }

                takenObject.gameObject.layer = 8;  // 특정 레이어로 설정
            }
        }
        else
        {
            // 레이저가 아무 것도 충돌하지 않았을 때
            laserLine.SetPosition(1, RController.transform.position + forward * laserDistance);
        }
    }

    // 물체를 집고 이동시키고, 크기 조정
    void HandleObjectManipulation()
    {
        if (takenObject != null)
        {
            // 현재 컨트롤러의 위치
            Vector3 currentControllerPosition = RController.transform.position;
            float currentDistance = Vector3.Distance(currentControllerPosition, takenObject.transform.position);

            // 물체의 크기를 컨트롤러와의 거리 비례하여 조정
            takenObject.transform.localScale = scaleMultiplier * (currentDistance / distanceMultiplier);

            // 물체를 오른쪽 컨트롤러 위치로 부드럽게 이동
            takenObject.transform.position = Vector3.Lerp(takenObject.transform.position, currentControllerPosition, Time.deltaTime * 5);

            // 오른쪽 트리거 버튼을 놓으면 물체가 놓임
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                takenObject.GetComponent<Rigidbody>().isKinematic = false;

                // Collider를 Trigger 해제
                foreach (Collider col in takenObject.GetComponents<Collider>())
                {
                    col.isTrigger = false;
                }

                takenObject.gameObject.layer = 0; // 기본 레이어로 복원
                takenObject = null;
            }
        }
    }
}