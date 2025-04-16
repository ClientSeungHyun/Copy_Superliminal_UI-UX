using UnityEngine;
using UnityEngine.Rendering;

public class MainCamera : MonoBehaviour {

    Portal[] portals;

    void Awake () 
    {
        portals = FindObjectsByType<Portal>(FindObjectsSortMode.None);
    }

    private void LateUpdate()
    {
        for (int i = 0; i < portals.Length; i++)
        {
            portals[i].Render();
        }
    }

}