using System;
using UnityEngine;
using UnityEngine.Events;

public class FogOfWarBehaviourScript : MonoBehaviour
{
    public Texture2D texture;

    public int width = 1600;
    public int height = 1600;
    
    public Color hiddenColor;
    public Color revealColor;

    public bool hasChanges = false;
    
    [Header("Listeners")] 
    public UnityEvent<Texture2D> onFogOfWarChanged;

    private void Awake()
    {
        if (onFogOfWarChanged == null) onFogOfWarChanged = new UnityEvent<Texture2D>();
        if(texture == null) texture = new Texture2D(width, height, TextureFormat.RGBA32 , false);
    }

    void Start()
    {
        DrawRectangle(hiddenColor, 0, 0, width-1, height-1);
        RevealCircleAt(200, 200, 45);
        RevealCircleAt(800, 200, 300);
        RevealCircleAt(952, 745, 50);
        RevealCircleAt(1400, 1255, 150);
        
        hasChanges = true;
    }
    
    void Update()
    {
        if (hasChanges)
        {
            texture.Apply();
            hasChanges = false;
            onFogOfWarChanged.Invoke(texture);
        }
    }
    
    public Texture2D DrawCircle(Color color, int x, int y, int radius = 1)
    {
        float rSquared = radius * radius;

        for (int u = x - radius; u < x + radius + 1; u++)
        for (int v = y - radius; v < y + radius + 1; v++)
            if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                texture.SetPixel(u, v, color);
        hasChanges = true;
        return texture;
    }
    
    public Texture2D DrawRectangle(Color color, int x1, int y1, int x2, int y2)
    {
        for (int u = x1; u < x2 + 1; u++)
        {
            for (int v = y1; v < y2 + 1; v++)
            {
                texture.SetPixel(u, v, color);
            }
        }
        
        hasChanges = true;
        return texture;
    }
    
    public void RevealCircleAt(int x, int y, int radius)
    {
        DrawCircle(revealColor, x, y, radius);
    }
    
    public void HiddedCircleAt(int x, int y, int radius)
    {
        DrawCircle(hiddenColor, x, y, radius);
    }
    
    public void RevealRectangleAt(int x, int y, int x2, int y2)
    {
        DrawRectangle(hiddenColor, x, y, x2, y2);
    }
    
    public void HiddedRectangleAt(int x, int y, int x2, int y2)
    {
        DrawRectangle(hiddenColor, x, y, x2, y2);
    }
}
