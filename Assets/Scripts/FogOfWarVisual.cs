using System;
using UnityEngine;

public class FogOfWarVisual : MonoBehaviour
{
    private void Start()
    {
        var fog = FindFirstObjectByType<FogOfWarBehaviourScript>();
        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.mainTexture = fog.texture;
    }
}
