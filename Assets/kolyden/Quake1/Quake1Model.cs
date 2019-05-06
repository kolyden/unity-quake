using System;
using UnityEngine;

namespace kolyden.Quake1
{
    public class Quake1Model : ScriptableObject
    {
        public Vector3 scale;       // Model scale factors.
        public Vector3 translate;   // Model origin.
        public float boundingradius;// Model bounding radius.
        public Vector3 eyeposition; // Eye position (useless?)

        public Vector2Int skinSize;
        public Texture2D[] skins;

        public MDLTexcoord[] texs;
        public MDLTriangle[] tris;

        public MDLSimpleFrame[] frames;
    }

    public static class MDLHeader
    {
        public const int Id = 1330660425; // 0x4F504449 = "IDPO" for IDPOLYGON
        public const int Version = 6;     // Version = 6
    }

    [Serializable]
    public struct MDLTexcoord
    {
        public int onseam;
        public int s;
        public int t;
    }

    [Serializable]
    public struct MDLTriangle
    {
        public int facesfront;  /* 0 = backface, 1 = frontface */
        public int vertex0;   /* vertex indices */
        public int vertex1;   /* vertex indices */
        public int vertex2;   /* vertex indices */

        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return vertex0;
                    case 1: return vertex1;
                    case 2: return vertex2;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }
    }

    [Serializable]
    public struct MDLVertex
    {
        public byte v0;
        public byte v1;
        public byte v2;
        public byte normalIndex;

        public byte this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return v0;
                    case 1: return v1;
                    case 2: return v2;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }
    }

    [Serializable]
    public struct MDLSimpleFrame
    {
        public string name;
        public MDLVertex bboxmin;
        public MDLVertex bboxmax;
        public MDLVertex[] vertices;
    }
}