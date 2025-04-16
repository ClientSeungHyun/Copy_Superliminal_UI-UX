using System.Diagnostics;
using UnityEngine;

public class raser : MonoBehaviour
{
    public LineRenderer laserLine; // 레이저 시각화용
    public float laserDistance = 50f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform trackingSpace;
    void Start()
    {
        UnityEngine.Debug.Log("잘됨");
    }

    // Update is called once per frame
    void Update()
    {
        FireLaser();
        

    }

    void FireLaser()
    {
        Vector3 localPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        Quaternion localRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);

        // CameraRig을 기준으로 좌표 변환
        Vector3 worldPos = trackingSpace.TransformPoint(localPos);
        Vector3 forward = trackingSpace.TransformDirection(localRot * Vector3.forward);

        // 3. 레이저 시각화
        laserLine.enabled = true;
        laserLine.SetPosition(0, worldPos);

        Ray ray = new Ray(worldPos, forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, laserDistance))
        {
            laserLine.SetPosition(1, hit.point);
        }
        else
        {
            laserLine.SetPosition(1, worldPos + forward * laserDistance);
        }
    }

}
