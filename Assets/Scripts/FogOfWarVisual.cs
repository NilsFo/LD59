using System;
using UnityEngine;
using UnityEngine.UI;

public class FogOfWarVisual : MonoBehaviour
{
    private static readonly int FogOfWarTexIndex = Shader.PropertyToID("_FogOfWar");

    private void Start()
    {
        var fog = FindFirstObjectByType<FogOfWar>();
        if (TryGetComponent<MeshRenderer>(out var meshRenderer))
        {
            meshRenderer.material.SetTexture(FogOfWarTexIndex, fog.texture);
            
        }

        if (TryGetComponent<Image>(out var img))
        {
            img.material.SetTexture(FogOfWarTexIndex, fog.texture);
        }
    }
}
