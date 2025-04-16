using System.Diagnostics;
using UnityEngine;

public class raser : MonoBehaviour
{
    public LineRenderer laserLine; // ������ �ð�ȭ��
    public float laserDistance = 50f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform trackingSpace;
    void Start()
    {
        UnityEngine.Debug.Log("�ߵ�");
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

        // CameraRig�� �������� ��ǥ ��ȯ
        Vector3 worldPos = trackingSpace.TransformPoint(localPos);
        Vector3 forward = trackingSpace.TransformDirection(localRot * Vector3.forward);

        // 3. ������ �ð�ȭ
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
