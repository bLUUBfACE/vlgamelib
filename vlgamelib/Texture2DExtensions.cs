using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLibrary.Graphics2D
{

    public static class Texture2DExtensions
    {
        public static Texture2D CutTexture(this Texture2D sourceTexture, Rectangle sourceRectangle)
        {
            if (!sourceTexture.Bounds.Contains(sourceRectangle)) throw new ArgumentException("The rectangle doesn't fit the texture");
            Color[] data = new Color[sourceRectangle.Width * sourceRectangle.Height];
            sourceTexture.GetData<Color>(0, sourceRectangle, data, 0, data.Length);
            Texture2D retTex = new Texture2D(sourceTexture.GraphicsDevice, sourceRectangle.Width, sourceRectangle.Height);
            retTex.SetData<Color>(data);
            return retTex;
        }

        public static Texture2D FlipTexture(this Texture2D sourceTexture, SpriteEffects flipDirection)
        {
            bool horizontally = flipDirection == SpriteEffects.FlipHorizontally || flipDirection == (SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically),
                vertically = flipDirection == SpriteEffects.FlipVertically || flipDirection == (SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically);
            Color[] data = new Color[sourceTexture.Width * sourceTexture.Height];
            Color[] newData = new Color[sourceTexture.Width * sourceTexture.Height];
            sourceTexture.GetData<Color>(data);
            for (int i = 0; i < data.Length; i++)
            {
                int x = i % sourceTexture.Width, y = i / sourceTexture.Width;
                newData[(horizontally ? sourceTexture.Width - x - 1: x) + (vertically ? sourceTexture.Height - y - 1: y) * sourceTexture.Width] // target index
                    = data[i];
            }
            Texture2D retTex = new Texture2D(sourceTexture.GraphicsDevice, sourceTexture.Width, sourceTexture.Height);
            retTex.SetData<Color>(newData);
            return retTex;
        }

        //public static Texture2D RotateTexture90Degrees(this Texture2D sourceTexture, bool clockwise)
        //{
        //    Texture2D retTex = new Texture2D(sourceTexture.GraphicsDevice, sourceTexture.Height, sourceTexture.Width);


        //}

        public static Texture2D TintTexture(this Texture2D sourceTexture, Color color, float percentage)
        {
            percentage = Math.Min(percentage, 1);
            Color[] data = new Color[sourceTexture.Width * sourceTexture.Height];
            sourceTexture.GetData<Color>(data);
            for (int i = 0; i < data.Length; i++)
            {
                Color c = data[i];
                c.R = (byte)((float)c.R * (1f - percentage) + (float)color.R * percentage);
                c.G = (byte)((float)c.G * (1f - percentage) + (float)color.G * percentage);
                c.B = (byte)((float)c.B * (1f - percentage) + (float)color.B * percentage);
                c.A = (byte)((float)c.A * (1f - percentage) + (float)color.A * percentage);
                data[i] = c;
            }
            Texture2D retTex = new Texture2D(sourceTexture.GraphicsDevice, sourceTexture.Width, sourceTexture.Height); 
            retTex.SetData<Color>(data);
            return retTex;
        }
        //buggt rum: TintTextureUnsafe

        //public unsafe static Texture2D TintTextureUnsafe(Texture2D sourceTexture, Color color, float percentage)
        //{
        //    percentage = Math.Min(percentage, 1);
        //    Color[] data = new Color[sourceTexture.Width * sourceTexture.Height];
        //    Texture2D retTex = new Texture2D(sourceTexture.GraphicsDevice, sourceTexture.Width, sourceTexture.Height); ;
        //    sourceTexture.GetData<Color>(data);
        //    fixed (Color* c = data)
        //    {
        //        Color* p = c;
        //        for (int i = 0; i < data.Length; i++)
        //        {
        //            p->R &= (byte)((float)p->R * (1f - percentage) + (float)color.R * percentage);
        //            p->G &= (byte)((float)p->G * (1f - percentage) + (float)color.G * percentage);
        //            p->B &= (byte)((float)p->B * (1f - percentage) + (float)color.B * percentage);
        //            p++->A &= (byte)((float)p->A * (1f - percentage) + (float)color.A * percentage);                    
        //        }                
        //    }
        //    retTex.SetData<Color>(data);
        //    return retTex;
        //}

        public static Texture2D BrightenTexture(this Texture2D sourceTexture, float factor)
        {
            Color[] data = new Color[sourceTexture.Width * sourceTexture.Height];
            sourceTexture.GetData<Color>(data);
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = new Color(new Vector4(data[i].ToVector3() * factor, data[i].A));
            }
            Texture2D retTex = new Texture2D(sourceTexture.GraphicsDevice, sourceTexture.Width, sourceTexture.Height);
            retTex.SetData<Color>(data);
            return retTex;
        }

        public static Texture2D CreateTextureFromColor(Color color, GraphicsDevice graphicsDevice, int width, int height)
        {           
            Color[] colors = new Color[width * height];
            for (int x = 0; x < width * height; x++)
            {
                colors[x] = color;
            }
            Texture2D returnTexture = new Texture2D(graphicsDevice, width, height);
            returnTexture.SetData<Color>(colors);
            return returnTexture;
        }
    }

}
