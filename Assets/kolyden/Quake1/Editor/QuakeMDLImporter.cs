using System.IO;
using UnityEngine;
using UnityEditor.Experimental.AssetImporters;

namespace kolyden.Quake1
{
    [ScriptedImporter(1, "mdl")]
	public class QuakeMDLLoader : ScriptedImporter
    {
	    public override void OnImportAsset(AssetImportContext ctx)
	    {
            using (var file = File.OpenRead(ctx.assetPath))
            using (var reader = new BinaryReader(file))
            {
                var id = reader.ReadInt32();
                if (id != MDLHeader.Id)
                    return;

                var version = reader.ReadInt32();
                if (version != MDLHeader.Version)
                    return;

                var model = ScriptableObject.CreateInstance<Quake1Model>();
                ctx.AddObjectToAsset("Root", model);
                ctx.SetMainObject(model);

                model.scale = ReadVector3(reader);
                model.translate = ReadVector3(reader);
                model.boundingradius = reader.ReadSingle();
                model.eyeposition = ReadVector3(reader);

                var numskins = reader.ReadInt32();
                var skinwidth = reader.ReadInt32();
                var skinheight = reader.ReadInt32();
                model.skinSize = new Vector2Int(skinwidth, skinheight);

                var num_verts = reader.ReadInt32();
                var num_tris = reader.ReadInt32();
                var num_frames = reader.ReadInt32();

                var synctype = reader.ReadInt32();
                var flags = reader.ReadInt32();
                var size = reader.ReadSingle();

                // SKINS
                model.skins = new Texture2D[numskins];
                for (int i = 0; i < numskins; i++)
                {
                    var type = reader.ReadInt32();
                    var dataSize = skinwidth * skinheight;
                    byte[] data;

                    switch (type)
                    {
                        case 0:
                            data = reader.ReadBytes(dataSize);
                            break;

                        case 1:
                            var count = reader.ReadInt32();
                            file.Position += sizeof(float) * count;
                            data = reader.ReadBytes(dataSize);
                            file.Position += dataSize * (count - 1);
                            break;

                        default:
                            return;
                    }

                    var skin = Quake1Utils.LoadTexture(data, model.skinSize);
                    skin.name = $"Skin{i + 1}";
                    skin.wrapMode = TextureWrapMode.Clamp;
                    skin.filterMode = FilterMode.Point;
                    skin.Apply(true);

                    ctx.AddObjectToAsset(skin.name, skin);
                    model.skins[i] = skin;
                }

                // Texture coordinates
                model.texs = new MDLTexcoord[num_verts];
                for (int i = 0; i < num_verts; i++)
                {
                    var tex = new MDLTexcoord();
                    tex.onseam = reader.ReadInt32();
                    tex.s = reader.ReadInt32();
                    tex.t = reader.ReadInt32();
                    model.texs[i] = tex;
                }

                // Triangles
                model.tris = new MDLTriangle[num_tris];
                for (int i = 0; i < num_tris; i++)
                {
                    var tri = new MDLTriangle();
                    tri.facesfront = reader.ReadInt32();
                    tri.vertex0 = reader.ReadInt32();
                    tri.vertex1 = reader.ReadInt32();
                    tri.vertex2 = reader.ReadInt32();
                    model.tris[i] = tri;
                }

                // Frames
                model.frames = new MDLSimpleFrame[num_frames];
                for (int i = 0; i < num_frames; i++)
                {                    
                    var type = reader.ReadInt32();
                    if (type != 0)
                        return;

                    var frame = new MDLSimpleFrame();
                    frame.vertices = new MDLVertex[num_verts];
                    frame.bboxmin = ReadVertex(reader);
                    frame.bboxmax = ReadVertex(reader);
                    frame.name = ReadString(reader, 16);

                    for (int j = 0; j < num_verts; j++)
                        frame.vertices[j] = ReadVertex(reader);
                    model.frames[i] = frame;
                }
            }
	    }

	    private static string ReadString(BinaryReader reader, int count)
	    {
	        var bytes = reader.ReadBytes(count);
	        return System.Text.Encoding.ASCII.GetString(bytes);
	    }

	    private static Vector3 ReadVector3(BinaryReader reader)
	    {
	        var x = reader.ReadSingle();
	        var y = reader.ReadSingle();
	        var z = reader.ReadSingle();
	        return new Vector3(x, y, z);
	    }

	    private static MDLVertex ReadVertex(BinaryReader reader)
	    {
	        var v0 = reader.ReadByte();
	        var v1 = reader.ReadByte();
	        var v2 = reader.ReadByte();
	        var normal = reader.ReadByte();

	        return new MDLVertex
	        {
	            v0 = v0,
	            v1 = v1,
	            v2 = v2,
	            normalIndex = normal
	        };
	    }
	}
}