using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Portal : MonoBehaviour
{
    public Portal LinkedPortal;
    public MeshRenderer PortalScreen;
    public int RecursionLimit = 5;

    public float NearClipOffset = 0.05f;
    public float NearClipLimit = 0.2f;


    RenderTexture ViewTexture;
    Camera PortalCamera;
    Camera PlayerCamera;
    public Camera PlayerCameraLeft;
    Material FirstRecursionMat;
    List<PortalTraveller> TrackedTravellers;
    MeshFilter ScreenMeshFilter;

    void Awake()
    {
        PlayerCamera = Camera.main;
        PortalCamera = GetComponentInChildren<Camera>();
        PortalCamera.enabled = false;
        TrackedTravellers = new List<PortalTraveller>();
        ScreenMeshFilter = PortalScreen.GetComponent<MeshFilter>();
        PortalScreen.material.SetInt("DisplayMask", 1);
    }

    void Start()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }

    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        HandleTravellers();
    }

    public void Render()
    {
        //시야 내에 없으면 넘김
        if (!CameraUtility.VisibleFromCamera(LinkedPortal.PortalScreen, PlayerCamera))
        {
            return;
        }

        CreateViewTexture();

        PortalCamera.projectionMatrix = PlayerCameraLeft.projectionMatrix;

        //플레이어의 월드->연결된 포털의 로컬->현재 포털의 월드로
        Matrix4x4 newWorldMatrix = transform.localToWorldMatrix * LinkedPortal.transform.worldToLocalMatrix * PlayerCameraLeft.transform.localToWorldMatrix;

        Vector3 RenderPosition = newWorldMatrix.GetColumn(3);
        Quaternion RenderRotation = newWorldMatrix.rotation;

        PortalCamera.transform.SetPositionAndRotation(RenderPosition, RenderRotation);

        PortalScreen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        LinkedPortal.PortalScreen.material.SetInt("DisplayMask", 1);

        PortalCamera.Render();

        PortalScreen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        //PortalScreen.receiveShadows = false;
    }

    void CreateViewTexture()
    {
        if (ViewTexture == null || ViewTexture.width != Screen.width || ViewTexture.height != Screen.height)
        {
            if (ViewTexture != null)
            {
                ViewTexture.Release();
            }

            ViewTexture = new RenderTexture(Screen.width, Screen.height, 0);
            PortalCamera.targetTexture = ViewTexture;
            LinkedPortal.PortalScreen.material.SetTexture("_MainTex", ViewTexture);
        }
    }

    void HandleTravellers()
    {
        for (int i = 0; i < TrackedTravellers.Count; i++)
        {
            PortalTraveller Traveller = TrackedTravellers[i];
            Transform TravellerTransform = Traveller.transform;
            Matrix4x4 m = LinkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * TravellerTransform.localToWorldMatrix;

            // 앞 뒤 판단
            Vector3 offsetFromPortal = TravellerTransform.position - transform.position;    // 포탈 -> 플레이어 방향 벡터
            int portalSide = System.Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward)); // 1앞쪽, -1뒤쪽
            int portalSideOld = System.Math.Sign(Vector3.Dot(Traveller.PreviousOffsetFromPortal, transform.forward));

           // 텔레포트 조건
            if (portalSide != portalSideOld)
            {
                Vector3 positionOld = TravellerTransform.position;
                Quaternion rotOld = TravellerTransform.rotation;
                Traveller.Teleport(transform, LinkedPortal.transform, m.GetColumn(3), m.rotation);
                Traveller.GraphicsClone.transform.SetPositionAndRotation(positionOld, rotOld);
                //Can't rely on OnTriggerEnter/Exit to be called next frame since it depends on when FixedUpdate runs
                LinkedPortal.OnTravellerEnterPortal(Traveller);
                TrackedTravellers.RemoveAt(i);
                i--;

            }
            else
            {
                Traveller.GraphicsClone.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
                //UpdateSliceParams(Traveller);
                Traveller.PreviousOffsetFromPortal = offsetFromPortal;
            }
        }
    }

    void OnTravellerEnterPortal(PortalTraveller Traveller)
    {
        if (!TrackedTravellers.Contains(Traveller))
        {
            Traveller.EnterPortalThreshold();
            Traveller.PreviousOffsetFromPortal = Traveller.transform.position - transform.position;
            TrackedTravellers.Add(Traveller);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        PortalTraveller Traveller = other.GetComponent<PortalTraveller>();
        if (Traveller)
        {
            OnTravellerEnterPortal(Traveller);
        }
    }

    void OnTriggerExit(Collider other)
    {
        PortalTraveller Traveller = other.GetComponent<PortalTraveller>();
        if (Traveller && TrackedTravellers.Contains(Traveller))
        {
            Traveller.ExitPortalThreshold();
            TrackedTravellers.Remove(Traveller);
        }
    }

    void OnValidate()
    {
        if (LinkedPortal != null)
        {
            LinkedPortal.LinkedPortal = this;
        }
    }
}