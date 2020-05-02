using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace DefaultNamespace
{
    public unsafe class MeshLineDrawer : MonoBehaviour, ILineDrawer
    {
        public Material mat;

        public Color Color = Color.red;

        public MeshTopology Topology = MeshTopology.LineStrip;

        // [SerializeField]
        private NativeArray<Vector3> Lines;
        private NativeArray<int> Index;
        private NativeArray<Color> Colors;
        private int pointCounts;

        private List<Mesh> meshes = new List<Mesh>();

        private Transform mTransform;

        public Vector3 Position;
        public Vector3 Rotation;
        private static int ArraySize = Int32.MaxValue / sizeof(Color);

        private bool IsDirty;

        // Start is called before the first frame update
        void Awake()
        {
            //PushLine(new Vector3(0.5f,0,0), new Vector3(0.5f,0.5f,0));
            //PushLine(new Vector3(0f,0.5f,0), new Vector3(1f,0.5f,0));
            Lines = new NativeArray<Vector3>(ArraySize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            Index = new NativeArray<int>(ArraySize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            Colors = new NativeArray<Color>(ArraySize, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        }

        void Start()
        {
            mTransform = transform;
            var mesh = new Mesh {indexFormat = IndexFormat.UInt32};
            meshes.Add(mesh);
            //mMesh.subMeshCount = 10;
        }

        public void PushLine(Vector3 start, Vector3 end)
        {
            //var item = new Line {Start = start, End = end};
            // if (Lines.Contains(item)) return;
            if (Topology == MeshTopology.LineStrip)
            {
                if (pointCounts == 0)
                {
                    Lines[pointCounts] = start;
                    Index[pointCounts] = pointCounts;
                    Colors[pointCounts] = Color;
                    pointCounts++;
                }

                Lines[pointCounts] = end;
                Index[pointCounts] = pointCounts;
                Colors[pointCounts] = Color;
                pointCounts++;
            }
            else if (Topology == MeshTopology.Lines)
            {
                Lines[pointCounts] = start;
                Index[pointCounts] = pointCounts;
                Colors[pointCounts] = Color;
                pointCounts++;


                Lines[pointCounts] = end;
                Index[pointCounts] = pointCounts;
                Colors[pointCounts] = Color;
                pointCounts++;
            }

            IsDirty = true;


            /*if (meshes.Count * 65536 * 2 <= lineCounts)
            {
                meshes.Add(new Mesh());
            }*/
        }


        void Update()
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
            /* if(meshes.Count>0)
                 Gizmos.DrawWireMesh(meshes[0], 0, Vector3.zero, Quaternion.identity, Vector3.one);*/
        }

        private void DrawLines()
        {
            for (var i = 0; i < meshes.Count; i++)
            {
                var mesh = meshes[0];
                if (IsDirty)
                {
                    var baseIdx = 0;
                    var length = pointCounts - baseIdx;
                    mesh.SetVertices(Lines, baseIdx, length);
                    mesh.SetIndices(Index, 0, length, Topology, 0, true);
                    mesh.SetColors(Colors, 0, length);
                    IsDirty = false;
                }

                Graphics.DrawMesh(mesh, Position, Quaternion.Euler(Rotation), mat, 0);
            }
        }

        private void OnDestroy()
        {
            Lines.Dispose();
            Index.Dispose();
            Colors.Dispose();
            if (!Application.isPlaying)
                foreach (var mesh in meshes)
                {
                    DestroyImmediate(mesh);
                }
            else
                foreach (var mesh in meshes)
                {
                    Destroy(mesh);
                }
        }

        private Vector3 NormalizedToScreen(Vector3 normalizedVertex)
        {
            return new Vector3(normalizedVertex.x * Screen.width, normalizedVertex.y * Screen.height,
                normalizedVertex.z);
        }

        [Serializable]
        public struct Line
        {
            public Vector3 Start;
            public Vector3 End;
        }
    }
}