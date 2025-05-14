using System.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class RealTimeTexObj : MonoBehaviour
{
    private GameObject RTDCamera;
    private GameObject TargetDecalObject;
    private Camera renderCam;

    public RenderTexture renderTexture;
    public Material RealTimeMat;
   
    void Start()
    {
        RTDCamera = GameObject.Find("RTDCamera");
        renderCam = RTDCamera.GetComponent<Camera>();
        TargetDecalObject = GameObject.Find("RealTimeDecal");
        if(TargetDecalObject != null )
            TargetDecalObject.SetActive(false);

      
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.B))
        {
            CreateDecal();
            CopyTransform(this.transform, TargetDecalObject.transform);
            TargetDecalObject.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //버튼에 닿을 때만
        if (collision.gameObject.layer == 8)    
        {
            CreateDecal();
            CopyTransform(this.transform, TargetDecalObject.transform);
            TargetDecalObject.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    void CopyTransform(Transform source, Transform target)
    {
        target.position = source.position;
      //  target.rotation = source.rotation;
      //  target.localScale = source.localScale;
    }


    void CreateDecal()
    {
        // 카메라가 렌더링하는 이미지를 기반으로 데칼을 생성
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        // 카메라 렌더링 후 데이터를 읽어오기 위한 과정
        renderCam.targetTexture = renderTexture;
        renderCam.Render();  // 카메라 렌더링
        RenderTexture.active = renderTexture;  // 활성화된 렌더 텍스처 설정

        // 렌더 타겟의 이미지를 읽어들이기 위한 임시 텍스처 생성
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();


        // 데칼에 텍스처 적용 (예: 데칼의 머티리얼에 텍스처를 할당)
        RealTimeMat.SetTexture("Base_Map", texture);
        TargetDecalObject.GetComponent<DecalProjector>().material = RealTimeMat;

        // 렌더 텍스처 활성화 복구
        RenderTexture.active = currentRT;  // 원래 렌더 타겟으로 복구
    }
}
