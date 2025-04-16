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

public class raser : MonoBehaviour
{
    public LineRenderer laserLine; // ������ �ð�ȭ��
    public float laserDistance = 50f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform trackingSpace;
    public GameObject LeftController;


    void Start()
    {
        //UnityEngine.Debug.Log("�ߵ�");
        laserLine.positionCount = 2;
        laserLine.useWorldSpace = true;
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
        forward.Normalize();

        // 3. ������ �ð�ȭ
        laserLine.enabled = true;
        laserLine.SetPosition(0, LeftController.transform.position);

        Ray ray = new Ray(LeftController.transform.position, forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, laserDistance))
        {
            laserLine.SetPosition(1, hit.point);
        }
        else
        {
            laserLine.SetPosition(1, LeftController.transform.position + forward * laserDistance);
        }
    }

}
