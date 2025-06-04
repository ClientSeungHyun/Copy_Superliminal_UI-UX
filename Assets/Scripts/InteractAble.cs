using UnityEngine;

public class InteractAble : MonoBehaviour
{
    public LineRenderer laserLine;         // 레이저 시각화를 위한 LineRenderer
    public float laserDistance = 10f;      // 레이저 최대 거리
    public Transform trackingSpace; 
    public GameObject RController;         // 오른쪽 컨트롤러 오브젝트

    public GameObject takenObject;        // 집은 오브젝트
    private Transform targetForTakenObjects;
    private Vector3 scaleMultiplier;       // 오브젝트의 원래 크기
    private float distanceMultiplier;      // 오브젝트와 컨트롤러 사이의 거리
    private Vector3 lastHitPoint = Vector3.zero;
    private Vector3 lastRotation = Vector3.zero;
    private bool isRayTouchingSomething = true;
    private float lastRotationY;

    private float takenObjSize = 0;
    private int takenObjSizeIndex = 0;

    private int layerMask = 0;

    private float lastPositionCalculation;

    void Start()
    {
        laserLine.positionCount = 2;
        laserLine.useWorldSpace = true;

        targetForTakenObjects = GameObject.Find("targetForTakenObjects").transform;

        layerMask = 1 << LayerMask.NameToLayer("Grabable") | 1 << LayerMask.NameToLayer("RealTimeTexture");
    }

    void Update()
    {
        Vector3 localPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion localRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        Vector3 forward = trackingSpace.TransformDirection(localRot * Vector3.forward);
        forward.Normalize();

        // 레이저 시작 위치를 오른쪽 컨트롤러로 설정
        laserLine.SetPosition(0, RController.transform.position);

        Ray ray = new Ray(RController.transform.position, forward);

        ScalingObjects(ray, forward);  // 물체 조작
        InteractOjbects(ray);


    }

    // 물체를 집고 이동시키고, 크기 조정
    void ScalingObjects(Ray ray, Vector3 forward)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, laserDistance, layerMask))
        {
            // 레이저가 충돌한 곳까지의 위치 설정
            laserLine.SetPosition(1, hit.point);
            isRayTouchingSomething = true;

            // 오른쪽 컨트롤러에서 트리거 버튼을 눌렀을 때 물체를 집음
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                if (hit.transform.tag == "GetAble")
                {
                    takenObject = hit.collider.gameObject;
                    targetForTakenObjects.position = hit.point;

                    layerMask = ~((1 << LayerMask.NameToLayer("Grabable")) | (1 << LayerMask.NameToLayer("RealTimeTexture")));

                    // 물체와의 거리 및 크기 저장
                    distanceMultiplier = Vector3.Distance(RController.transform.position, takenObject.transform.position);
                    scaleMultiplier = takenObject.transform.localScale;
                    lastRotation = takenObject.transform.rotation.eulerAngles;  // 스케일 할 오브젝트의 현재 오일러 회전 값
                    lastRotationY = lastRotation.y - Camera.main.transform.eulerAngles.y;    //Y축 상대적 회전 값

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

                    // 그림자 비활성화 및 레이어 변경
                    if (takenObject.GetComponent<MeshRenderer>() != null)
                    {
                        takenObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        takenObject.GetComponent<MeshRenderer>().receiveShadows = false;
                    }
                    takenObject.gameObject.layer = LayerMask.NameToLayer("Grabable");

                    foreach (Transform child in takenObject.GetComponentsInChildren<Transform>())
                    {
                        takenObject.GetComponent<Rigidbody>().isKinematic = true;
                        takenObject.GetComponent<Collider>().isTrigger = true;
                        child.gameObject.layer = LayerMask.NameToLayer("Grabable");
                    }

                    // 오브젝트 크기 계산 - 가장 큰 사이즈로 설정
                    takenObjSize = takenObject.GetComponent<Collider>().bounds.size.y;
                    takenObjSizeIndex = 1;
                    if (takenObject.GetComponent<Collider>().bounds.size.x > takenObjSize)
                    {
                        takenObjSize = takenObject.GetComponent<Collider>().bounds.size.x;
                        takenObjSizeIndex = 0;
                    }
                    if (takenObject.GetComponent<Collider>().bounds.size.z > takenObjSize)
                    {
                        takenObjSize = takenObject.GetComponent<Collider>().bounds.size.z;
                        takenObjSizeIndex = 2;
                    }

                    GameObject.Find("AudioManager").GetComponent<AudioManager>().PlaySound(this.gameObject, "grab");
                }
            }
        }
        else
        {
            // 레이저가 아무 것도 충돌하지 않았을 때
            isRayTouchingSomething = false;
            laserLine.SetPosition(1, RController.transform.position + forward * laserDistance);
        }

        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            if (takenObject != null)
            {
                Vector3 centerCorrection = Vector3.zero;

                // 중앙에 맞게 계속 오브젝트를 이동
                if (takenObject.GetComponent<MeshRenderer>() != null)
                {
                    centerCorrection = takenObject.transform.position - takenObject.GetComponent<MeshRenderer>().bounds.center;
                }

                takenObject.transform.position = Vector3.Lerp(takenObject.transform.position, targetForTakenObjects.position + centerCorrection, Time.deltaTime * 10);
                takenObject.transform.rotation = Quaternion.Lerp(takenObject.transform.rotation, Quaternion.Euler(new Vector3(0, lastRotationY + Camera.main.transform.eulerAngles.y, 0)), Time.deltaTime * 20f);

                // hit.distance - 카메라에서 벽까지의 거리
                // 카메라가 바라보는 방향에서 본 표면의 정면 거리
                float cosine = Vector3.Dot(ray.direction, hit.normal);
                float cameraHeight = Mathf.Abs(hit.distance * cosine);

                takenObjSize = takenObject.GetComponent<Collider>().bounds.size[takenObjSizeIndex] * 1.3f; // 오브젝트의 가장 큰 길이에서 살짝 offset

                float positionCalculation = (hit.distance * takenObjSize / 2) / (cameraHeight); // 오브젝트가 벽에 붙는 위치 계산
                if (positionCalculation < 100f)
                {
                    lastPositionCalculation = positionCalculation;
                }

                // Ray가 어떤 오브젝트(예: 벽)에 닿지 않을 경우, 즉 최대 거리(rayMaxRange) 바깥일 경우에도 오브젝트 스케일을 증가시키되, 그 증가를 제한
                if (isRayTouchingSomething)
                {
                    lastHitPoint = hit.point;
                }
                else
                {
                    lastHitPoint = RController.transform.position + ray.direction * laserDistance;
                }

                targetForTakenObjects.position = Vector3.Lerp(targetForTakenObjects.position, lastHitPoint
                        - (ray.direction * lastPositionCalculation), Time.deltaTime * 10);
                takenObject.transform.localScale = scaleMultiplier * (Vector3.Distance(Camera.main.transform.position, takenObject.transform.position) / distanceMultiplier);
            }
        }

        // 오른쪽 트리거 버튼을 놓으면 물체가 놓임
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            if (takenObject != null)
            {
                takenObject.GetComponent<Rigidbody>().isKinematic = false;

                // Collider를 Trigger 해제
                foreach (Collider col in takenObject.GetComponents<Collider>())
                {
                    col.isTrigger = false;
                }

                if (takenObject.GetComponent<MeshRenderer>() != null)
                {
                    takenObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                    takenObject.GetComponent<MeshRenderer>().receiveShadows = true;
                }
                takenObject.transform.parent = null;
                layerMask = 1 << LayerMask.NameToLayer("Grabable") | 1 << LayerMask.NameToLayer("RealTimeTexture");


                foreach (Transform child in takenObject.GetComponentsInChildren<Transform>())
                {
                    takenObject.GetComponent<Rigidbody>().isKinematic = false;
                    takenObject.GetComponent<Collider>().isTrigger = false;
                    

                    if (takenObject.GetComponent<RealTimeTexObj>())
                    {
                        child.gameObject.layer = LayerMask.NameToLayer("RealTimeTexture");
                    }
                    else
                    {
                        child.gameObject.layer = LayerMask.NameToLayer("Grabable");
                    }
                }

                takenObject = null;
                GameObject.Find("AudioManager").GetComponent<AudioManager>().PlaySound(this.gameObject, "release");
            }
        }
    }

    void InteractOjbects(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5f, 1 << LayerMask.NameToLayer("Interact")))
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                if (hit.transform.name == "Testing_Agreement")
                {

                    SignPaper signPaper = hit.transform.GetComponent<SignPaper>();
                    signPaper.Sign();
                }
            }
        }
    }
}