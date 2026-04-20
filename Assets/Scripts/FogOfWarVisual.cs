using System;
using UnityEngine;

public class FogOfWarVisual : MonoBehaviour
{
    private void Start()
    {
        var fog = FindFirstObjectByType<FogOfWar>();
        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.mainTexture = fog.texture;
    }
}
