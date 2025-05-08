using System.Diagnostics;
using UnityEngine;

public class raser : MonoBehaviour
{
    public LineRenderer laserLine; // ������ �ð�ȭ��
    public float laserDistance = 50f;
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
        // LeftController�� ��ġ�� ȸ�� ���� ��������
        Vector3 localPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion localRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        // TrackingSpace �������� ���� ��ǥ�� ��ȯ
        Vector3 worldPos = trackingSpace.TransformPoint(localPos);
        Vector3 forward = trackingSpace.TransformDirection(localRot * Vector3.forward);
        forward.Normalize();

        // ������ ���� ��ġ�� LeftController�� ��ġ�� ����
        laserLine.SetPosition(0, RController.transform.position);

        // ������ �߻�� Ray ����
        Ray ray = new Ray(LeftController.transform.position, forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, laserDistance))
        {
            // �������� �浹�� ���������� ��ġ�� ����
            laserLine.SetPosition(1, hit.point);

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                UnityEngine.Debug.Log("������ ���� ��ư ����!");
                // �浹�� ���� ������Ʈ ��������
                GameObject hitObject = hit.collider.gameObject;
                UnityEngine.Debug.Log("Hit object: " + hitObject.name);

            }

        }
        else
        {
            // �浹���� ���� ��� �������� �ִ� �Ÿ����� ��� ������ ����
            laserLine.SetPosition(1, LeftController.transform.position + forward * laserDistance);
        }
    }
}
