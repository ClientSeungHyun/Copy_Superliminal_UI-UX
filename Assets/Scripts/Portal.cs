using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Portal : MonoBehaviour
{
    [Header("Main Settings")]
    public Portal LinkedPortal;
    public MeshRenderer PortalScreen;
    public int RecursionLimit = 5;

    [Header("Advanced Settings")]
    public float NearClipOffset = 0.05f;
    public float NearClipLimit = 0.2f;


    RenderTexture ViewTexture;
    Camera PortalCamera;
    Camera PlayerCamera;
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

        PortalCamera.projectionMatrix = PlayerCamera.projectionMatrix;

        //플레이어의 월드->연결된 포털의 로컬->현재 포털의 월드로
        Matrix4x4 LocalToWorldMatrix = transform.localToWorldMatrix * LinkedPortal.transform.worldToLocalMatrix * PlayerCamera.transform.localToWorldMatrix;

        Vector3 RenderPosition = LocalToWorldMatrix.GetColumn(3);
        Quaternion RenderRotation = LocalToWorldMatrix.rotation;

        PortalCamera.transform.SetPositionAndRotation(RenderPosition, RenderRotation);

        PortalScreen.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        LinkedPortal.PortalScreen.material.SetInt("DisplayMask", 1);

        //SetNearClipPlane();
        //HandleClipping();
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

    void LateUpdate()
    {
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

    void HandleClipping()
    {
        // There are two main graphical issues when slicing travellers
        // 1. Tiny sliver of mesh drawn on backside of portal
        //    Ideally the oblique clip plane would sort this out, but even with 0 offset, tiny sliver still visible
        // 2. Tiny seam between the sliced mesh, and the rest of the model drawn onto the portal screen
        // This function tries to address these issues by modifying the slice parameters when rendering the view from the portal
        // Would be great if this could be fixed more elegantly, but this is the best I can figure out for now
        const float hideDst = -1000;
        const float showDst = 1000;
        //float screenThickness = LinkedPortal.ProtectScreenFromClipping(PortalCamera.transform.position);

        //foreach (var traveller in TrackedTravellers)
        //{
        //    if (SameSideOfPortal(traveller.transform.position, portalCamPos))
        //    {
        //        // Addresses issue 1
        //        traveller.SetSliceOffsetDst(hideDst, false);
        //    }
        //    else
        //    {
        //        // Addresses issue 2
        //        traveller.SetSliceOffsetDst(showDst, false);
        //    }

        //    // Ensure clone is properly sliced, in case it's visible through this portal:
        //    int cloneSideOfLinkedPortal = -SideOfPortal(traveller.transform.position);
        //    bool camSameSideAsClone = LinkedPortal.SideOfPortal(portalCamPos) == cloneSideOfLinkedPortal;
        //    if (camSameSideAsClone)
        //    {
        //        traveller.SetSliceOffsetDst(screenThickness, true);
        //    }
        //    else
        //    {
        //        traveller.SetSliceOffsetDst(-screenThickness, true);
        //    }
        //}

        //var offsetFromPortalToCam = portalCamPos - transform.position;
        //foreach (var linkedTraveller in LinkedPortal.TrackedTravellers)
        //{
        //    var travellerPos = linkedTraveller.GraphicsObject.transform.position;
        //    var clonePos = linkedTraveller.GraphicsClone.transform.position;
        //    // Handle clone of linked portal coming through this portal:
        //    bool cloneOnSameSideAsCam = LinkedPortal.SideOfPortal(travellerPos) != SideOfPortal(portalCamPos);
        //    if (cloneOnSameSideAsCam)
        //    {
        //        // Addresses issue 1
        //        linkedTraveller.SetSliceOffsetDst(hideDst, true);
        //    }
        //    else
        //    {
        //        // Addresses issue 2
        //        linkedTraveller.SetSliceOffsetDst(showDst, true);
        //    }

        //    // Ensure traveller of linked portal is properly sliced, in case it's visible through this portal:
        //    bool camSameSideAsTraveller = LinkedPortal.SameSideOfPortal(linkedTraveller.transform.position, portalCamPos);
        //    if (camSameSideAsTraveller)
        //    {
        //        linkedTraveller.SetSliceOffsetDst(screenThickness, false);
        //    }
        //    else
        //    {
        //        linkedTraveller.SetSliceOffsetDst(-screenThickness, false);
        //    }
        //}
    }

    //// Called once all portals have been rendered, but before the player camera renders
    //public void PostPortalRender()
    //{
    //    foreach (var traveller in TrackedTravellers)
    //    {
    //        UpdateSliceParams(traveller);
    //    }
    //    ProtectScreenFromClipping(PlayerCamera.transform.position);
    //}

    // Sets the thickness of the portal screen so as not to clip with camera near plane when player goes through
    float ProtectScreenFromClipping(Vector3 viewPoint)
    {
        float halfHeight = PlayerCamera.nearClipPlane * Mathf.Tan(PlayerCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * PlayerCamera.aspect;
        float dstToNearClipPlaneCorner = new Vector3(halfWidth, halfHeight, PlayerCamera.nearClipPlane).magnitude;
        float screenThickness = dstToNearClipPlaneCorner;

        Transform screenT = PortalScreen.transform;
        bool camFacingSameDirAsPortal = Vector3.Dot(transform.forward, transform.position - viewPoint) > 0;
        screenT.localScale = new Vector3(screenT.localScale.x, screenT.localScale.y, screenThickness);
        screenT.localPosition = Vector3.forward * screenThickness * ((camFacingSameDirAsPortal) ? 0.5f : -0.5f);
        return screenThickness;
    }

    void UpdateSliceParams(PortalTraveller traveller)
    {
        // Calculate slice normal
        int side = SideOfPortal(traveller.transform.position);
        Vector3 sliceNormal = transform.forward * -side;
        Vector3 cloneSliceNormal = LinkedPortal.transform.forward * side;

        // Calculate slice centre
        Vector3 slicePos = transform.position;
        Vector3 cloneSlicePos = LinkedPortal.transform.position;

        // Adjust slice offset so that when player standing on other side of portal to the object, the slice doesn't clip through
        float sliceOffsetDst = 0;
        float cloneSliceOffsetDst = 0;
        float screenThickness = PortalScreen.transform.localScale.z;

        bool playerSameSideAsTraveller = SameSideOfPortal(PlayerCamera.transform.position, traveller.transform.position);
        if (!playerSameSideAsTraveller)
        {
            sliceOffsetDst = -screenThickness;
        }
        bool playerSameSideAsCloneAppearing = side != LinkedPortal.SideOfPortal(PlayerCamera.transform.position);
        if (!playerSameSideAsCloneAppearing)
        {
            cloneSliceOffsetDst = -screenThickness;
        }

        // Apply parameters
        for (int i = 0; i < traveller.OriginalMaterials.Length; i++)
        {
            traveller.OriginalMaterials[i].SetVector("sliceCentre", slicePos);
            traveller.OriginalMaterials[i].SetVector("sliceNormal", sliceNormal);
            traveller.OriginalMaterials[i].SetFloat("sliceOffsetDst", sliceOffsetDst);

            traveller.CloneMaterials[i].SetVector("sliceCentre", cloneSlicePos);
            traveller.CloneMaterials[i].SetVector("sliceNormal", cloneSliceNormal);
            traveller.CloneMaterials[i].SetFloat("sliceOffsetDst", cloneSliceOffsetDst);

        }

    }

    // Use custom projection matrix to align portal camera's near clip plane with the surface of the portal
    // Note that this affects precision of the depth buffer, which can cause issues with effects like screenspace AO
    void SetNearClipPlane()
    {
        // Learning resource:
        // http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
        Transform clipPlane = transform;
        int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, transform.position - PortalCamera.transform.position));

        Vector3 camSpacePos = PortalCamera.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        Vector3 camSpaceNormal = PortalCamera.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + NearClipOffset;

        // Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
        if (Mathf.Abs(camSpaceDst) > NearClipLimit)
        {
            Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

            // Update projection based on new clip plane
            // Calculate matrix with player cam so that player camera settings (fov, etc) are used
            PortalCamera.projectionMatrix = PlayerCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }
        else
        {
            PortalCamera.projectionMatrix = PlayerCamera.projectionMatrix;
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

    /*
     ** Some helper/convenience stuff:
     */

    int SideOfPortal(Vector3 pos)
    {
        return System.Math.Sign(Vector3.Dot(pos - transform.position, transform.forward));
    }

    bool SameSideOfPortal(Vector3 posA, Vector3 posB)
    {
        return SideOfPortal(posA) == SideOfPortal(posB);
    }

    Vector3 portalCamPos
    {
        get
        {
            return PortalCamera.transform.position;
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