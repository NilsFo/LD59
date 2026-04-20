using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

// Put this script on a Camera
public class DrawLines : MonoBehaviour
{
    // Fill/drag these in from the editor

    // Choose the Unlit/Color shader in the Material Settings
    // You can change that color, to change the color of the connecting lines
    public Material lineMat;

    public Queue<(Vector3, Vector3)> lines = new Queue<(Vector3, Vector3)>();

    // Connect all of the `points` to the `mainPoint`
    void DrawConnectingLines()
    {
        if (lines.Count > 0)
        {
            // Loop through each point to connect to the mainPoint
            while (lines.Count > 0)
            {
                (Vector3 p1, Vector3 p2) = lines.Dequeue();

                GL.Begin(GL.LINES);
                lineMat.SetPass(0);
                GL.Color(new Color(lineMat.color.r, lineMat.color.g, lineMat.color.b, lineMat.color.a));
                GL.Vertex3(p1.x, p1.y, p1.z);
                GL.Vertex3(p2.x, p2.y, p2.z);
                GL.End();
            }
        }
    }

    void OnEnable()
    {
        RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
    }

    void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
    }

    private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        DrawConnectingLines();
    }

    // To show the lines in the game window whne it is running
    void OnPostRender()
    {
        DrawConnectingLines();
    }

    // To show the lines in the editor
    /*void OnDrawGizmos()
    {
        DrawConnectingLines();
    }*/
}