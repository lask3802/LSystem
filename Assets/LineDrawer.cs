using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class LineDrawer : MonoBehaviour, ILineDrawer
{
    public Material mat;

    public Color Color = Color.red;

    // [SerializeField]
    private NativeArray<Line> Lines;
    private int lineCounts;

    // Start is called before the first frame update
    void Awake()
    {
        //PushLine(new Vector3(0.5f,0,0), new Vector3(0.5f,0.5f,0));
        //PushLine(new Vector3(0f,0.5f,0), new Vector3(1f,0.5f,0));
        Lines = new NativeArray<Line>(1024*1024, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
    }

    public void PushLine(Vector3 start, Vector3 end)
    {
        var item = new Line {Start = start, End = end};
        // if (Lines.Contains(item)) return;
        Lines[lineCounts++] = item;
    }

    void OnPostRender()
    {
        if (!mat)
        {
            Debug.LogError("Please Assign a material on the inspector");
            return;
        }
        /*GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();*/

        DrawLines();

        /*GL.PopMatrix();*/
    }

    private void OnDrawGizmos()
    {
        DrawLines();
    }

    private void DrawLines()
    {
        GL.Begin(GL.LINES);
        mat.SetPass(0);
        for(var idx = 0 ; idx < lineCounts ; idx++)
        {
            var line = Lines[idx];
            GL.Color(Color);
            GL.Vertex(line.Start);
            GL.Vertex(line.End);
        }

        GL.End();
    }

    private void OnDestroy()
    {
        Lines.Dispose();
    }

    private Vector3 NormalizedToScreen(Vector3 normalizedVertex)
    {
        return new Vector3(normalizedVertex.x * Screen.width, normalizedVertex.y * Screen.height, normalizedVertex.z);
    }

    [Serializable]
    public struct Line
    {
        public Vector3 Start;
        public Vector3 End;
    }
}