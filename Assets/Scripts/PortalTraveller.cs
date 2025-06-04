using System.Collections.Generic;
using UnityEngine;

public class PortalTraveller : MonoBehaviour
{
    public GameObject GraphicsObject;
    public GameObject GraphicsClone { get; set; }
    public Vector3 PreviousOffsetFromPortal { get; set; }

    public Material[] OriginalMaterials { get; set; }
    public Material[] CloneMaterials { get; set; }

    public virtual void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }


    // 포탈에 접촉했을 때 한 번 호출
    public virtual void EnterPortalThreshold()
    {
        if (GraphicsClone == null)
        {
            GraphicsClone = Instantiate(GraphicsObject);
            GraphicsClone.transform.parent = GraphicsObject.transform.parent;
            GraphicsClone.transform.localScale = GraphicsObject.transform.localScale;
            OriginalMaterials = GetMaterials(GraphicsObject);
            CloneMaterials = GetMaterials(GraphicsClone);
        }
        else
        {
            GraphicsClone.SetActive(true);
        }
    }

    public virtual void ExitPortalThreshold()
    {
        GraphicsClone.SetActive(false);
        // Disable slicing
        for (int i = 0; i < OriginalMaterials.Length; i++)
        {
            OriginalMaterials[i].SetVector("sliceNormal", Vector3.zero);
        }
    }

    public void SetSliceOffsetDst(float dst, bool clone)
    {
        for (int i = 0; i < OriginalMaterials.Length; i++)
        {
            if (clone)
            {
                CloneMaterials[i].SetFloat("sliceOffsetDst", dst);
            }
            else
            {
                OriginalMaterials[i].SetFloat("sliceOffsetDst", dst);
            }

        }
    }

    Material[] GetMaterials(GameObject g)
    {
        var renderers = g.GetComponentsInChildren<MeshRenderer>();
        var matList = new List<Material>();
        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.materials)
            {
                matList.Add(mat);
            }
        }
        return matList.ToArray();
    }
}