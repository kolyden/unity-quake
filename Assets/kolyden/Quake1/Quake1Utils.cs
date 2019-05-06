using UnityEngine;

namespace kolyden.Quake1
{
    public static class Quake1Utils
    {
        public static Texture2D LoadTexture(byte[] data, Vector2Int size)
        {
            var tex = new Texture2D(size.x, size.y);
            for (int y = 0; y < size.y; y++)
            for (int x = 0; x < size.x; x++)
            {
                var index = x + y * size.x;
                var value = data[index];
                var color = Quake1Const.Palette[value];
                tex.SetPixel(x, y, color);
            }

            return tex;
        }
    }
}