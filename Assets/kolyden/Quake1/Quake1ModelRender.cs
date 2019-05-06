using System;
using System.Collections.Generic;
using UnityEngine;

namespace kolyden.Quake1
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class Quake1ModelRender : MonoBehaviour
    {
        public Quake1Model model;

        public MeshFilter meshFilter
        {
            get
            {
                if (_cachedMeshFilter) return _cachedMeshFilter;
                _cachedMeshFilter = GetComponent<MeshFilter>();
                return _cachedMeshFilter;
            }
        }

        public MeshRenderer meshRenderer
        {
            get
            {
                if (_cachedMeshRenderer) return _cachedMeshRenderer;
                _cachedMeshRenderer = GetComponent<MeshRenderer>();
                return _cachedMeshRenderer;
            }
        }

        // =====================================================================
        private MeshFilter _cachedMeshFilter;
        private MeshRenderer _cachedMeshRenderer;
        private Mesh _mesh;

        private bool _updateMesh = true;
        private readonly Dictionary<string, int> _framesDict =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        void Start()
        {
            _framesDict.Clear();
            if (model == null) return;

            for (int i = 0; i < model.frames.Length; i++)
                _framesDict.Add(model.frames[i].name, i);
        }

        void Update()
        {
            if (!_updateMesh || model == null)
                return;

            if (_mesh == null)
            {
                _mesh = new Mesh {name = model.name};
                meshFilter.mesh = _mesh;
            }

            var index = 0;//FindFrameIndex("axstnd1");
            if (index < 0) return;

            var frame = model.frames[index];
            MakeMesh(model, frame.vertices, _mesh);
            _mesh.UploadMeshData(false);
        }

        private int FindFrameIndex(string frameName)
        {
            return _framesDict.TryGetValue(frameName, out var index)
                ? index
                : -1;
        }

        private static void MakeMesh(Quake1Model model, MDLVertex[] vertex, Mesh mesh)
        {
            var count = model.tris.Length * 3;
            var vertices = new Vector3[count];
            var normals = new Vector3[count];
            var uv = new Vector2[count];
            var colors = new Color32[count];
            var indexes = new int[count];

            for (int i = 0; i < count; i++)
            {
                indexes[i] = i;
                colors[i] = Color.white;
            }

            int index = 0;
            for (int i = 0; i < model.tris.Length; i++)
            {
                var tri = model.tris[i];

                for (int j = 2; j >= 0; j--)
                {
                    var pvert = vertex[tri[j]];

                    float s = model.texs[tri[j]].s;
                    float t = model.texs[tri[j]].t;

                    if (tri.facesfront == 0 &&
                        model.texs[tri[j]].onseam != 0)
                        s += 0.5f * model.skinSize.x;

                    float tx = (s + .5f) / model.skinSize.x;
                    float ty = (t + .5f) / model.skinSize.y;
                    uv[index] = new Vector2(tx, ty);

                    normals[index] = Quake1Const.Normals[pvert.normalIndex];

                    var x = model.scale.x * pvert.v0 + model.translate.x;
                    var y = model.scale.y * pvert.v1 + model.translate.y;
                    var z = model.scale.z * pvert.v2 + model.translate.z;
                    vertices[index] = new Vector3(x, y, z);

                    index++;
                }
            }

            mesh.Clear(true);
            mesh.subMeshCount = 1;
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uv;
            mesh.colors32 = colors;
            mesh.SetIndices(indexes, MeshTopology.Triangles, 0);
        }
    }
}