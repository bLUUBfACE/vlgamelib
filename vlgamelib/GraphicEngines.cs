using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameLibrary.Computing2D;

namespace GameLibrary.Graphics2D
{
    public enum ExplosionType { Fire, }

    public class ExplosionEngine
    {
        private struct ParticleData
        {
            public float BirthTime;
            public float MaxAge;
            public Vector2 OrginalPosition;
            public Vector2 Accelaration;
            public Vector2 Direction;
            public Vector2 Position;
            public float Scaling;
            public Color ModColor;
        }

        readonly ExplosionType explosionType;
        readonly Texture2D explosionTexture;
        List<ParticleData> particleList = new List<ParticleData>();
        Random randomizer = new Random();

        public ExplosionEngine(ExplosionType explosionType, Texture2D explosionTexture)
        {
            this.explosionType = explosionType; this.explosionTexture = explosionTexture;
        }

        public void AddExplosion(Vector2 position, int numberOfParticles, float size, float maxAge, GameTime gameTime)
        {
            for (int i = 0; i < numberOfParticles; i++)
                AddExplosionParticle(position, size, maxAge, gameTime);
        }
        private void AddExplosionParticle(Vector2 position, float size, float maxAge, GameTime gameTime)
        {
            ParticleData particle = new ParticleData();

            particle.OrginalPosition = position;
            particle.Position = particle.OrginalPosition;

            particle.BirthTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            particle.MaxAge = maxAge;
            particle.Scaling = 0.25f;
            particle.ModColor = Color.White;

            float particleDistance = (float)randomizer.NextDouble() * size;
            Vector2 displacement = new Vector2(particleDistance, 0);
            float angle = MathHelper.ToRadians(randomizer.Next(360));
            displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(angle));

            particle.Direction = displacement * 2.0f;
            particle.Accelaration = -particle.Direction;

            particleList.Add(particle);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            for (int i = 0; i < particleList.Count; i++)
            {
                ParticleData particle = particleList[i];
                spriteBatch.Draw(explosionTexture, particle.Position, null, particle.ModColor, i, new Vector2(256, 256), particle.Scaling, SpriteEffects.None, 1);
            }
            spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
            for (int i = particleList.Count - 1; i >= 0; i--)
            {
                ParticleData particle = particleList[i];
                float timeAlive = now - particle.BirthTime;

                if (timeAlive > particle.MaxAge)
                {
                    particleList.RemoveAt(i);
                }
                else
                {
                    float relAge = timeAlive / particle.MaxAge;
                    particle.Position = 0.5f * particle.Accelaration * relAge * relAge + particle.Direction * relAge + particle.OrginalPosition;

                    float invAge = 1.0f - relAge;
                    particle.ModColor = new Color(new Vector4(invAge, invAge, invAge, invAge));

                    Vector2 positionFromCenter = particle.Position - particle.OrginalPosition;
                    float distance = positionFromCenter.Length();
                    particle.Scaling = (50.0f + distance) / 200.0f;

                    particleList[i] = particle;
                }
            }
        }
    }

    public class PixelAnimationPathEngine
    {
        IPathable PixelPath;
        Vector2 offset = new Vector2(0, 0);

        int tailLength = 0;
        float minAlpha, maxAlpha;
        Queue<Vector2> tail = new Queue<Vector2>();

        public Vector2 Current
        {
            get { return PixelPath.Current() + offset; }
        }

        public PixelAnimationPathEngine(IPathable PixelPath)
        {
            this.PixelPath = PixelPath;
        }

        public PixelAnimationPathEngine(IPathable PixelPath, int TailLength, int MinimumAlpha, int MaximumAlpha)
        {
            this.PixelPath = PixelPath;
            this.tailLength = TailLength;
            this.minAlpha = Math.Max(0, MinimumAlpha);
            this.maxAlpha = Math.Min(255, MaximumAlpha);
        }

        public void Offset(Vector2 Offset)
        {
            offset.X += Offset.X; offset.Y += Offset.Y;
            for (int i = 0; i < tail.Count; i++)
            {
                tail.Enqueue(tail.Dequeue() + Offset);
            }

        }

        public Vector2 Animate(float speed)
        {
            if(tailLength != 0)
            {
                tail.Enqueue(Current);
                if (tail.Count > tailLength)
                    tail.Dequeue();
            }
            return PixelPath.MoveNext(speed) + offset;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D pixelTexture, Color color, int size)
        {
            spriteBatch.Draw(pixelTexture, Current.ToRectangle(size, size), color);
            List<Vector2> tailList = tail.ToList<Vector2>();
            float alphaInc = (maxAlpha - minAlpha) / tailLength;
            for (int i = 0; i < tail.Count; i++)
            {
                Color c = color; c.A = (byte)(minAlpha + i * alphaInc);
                spriteBatch.Draw(pixelTexture, tailList[i].ToRectangle(size, size), c);
            }
        }
    }

    public class ParticleEngine
    {
        List<Particle> Particles;
        FloatRectangle bounds;

        public ParticleEngine(FloatRectangle Bounds)
        {
            this.bounds = Bounds;
            Particles = new List<Particle>();
        }

        public void AddSprite(Particle sprite)
        {
            Particles.Add(sprite);
        }

        public void Update()
        {
            for (int i = Particles.Count - 1; i >= 0; i--)
            {
                Vector2 pos = Particles[i].Update();
                if (!bounds.Contains((int)pos.X,(int)pos.Y))
                    Particles.RemoveAt(i);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particle p in Particles)
            {
                p.Draw(spriteBatch);
            }
        }

        public class Particle
        {
            Texture2D texture;
            Vector2 position, speed, accel;
            Color color;
            float rotation, rotSpeed, alpha, alphaDec;
            int width, height;

            public Particle(Texture2D Texture, Vector2 Position, Vector2 Speed, Vector2 Acceleration, Color Color, float AlphaValue, float AlphaDecrease, float RotationSpeed, int Width, int Height)
            {
                this.texture = Texture;
                this.position = Position; this.speed = Speed; this.accel = Acceleration;
                this.color = Color; this.alpha = AlphaValue; this.alphaDec = AlphaDecrease;
                this.rotSpeed = RotationSpeed;
                this.width = Width; this.height = Height;
            }

            public virtual Vector2 Update()
            {
                alpha -= alphaDec;
                rotation += rotSpeed;
                return position += (speed += accel);
            }

            public virtual void Draw(SpriteBatch spriteBatch)
            {
                color.A = (byte)Math.Max(Math.Min(alpha, 255), 0);
                spriteBatch.Draw(texture, (position + new Vector2(width / 2, height / 2)).ToRectangle(width, height), null, color, rotation, new Vector2(texture.Width / 2, texture.Height / 2), SpriteEffects.None, 0);
            }
        }
    }

    public static class StaticDrawMethods
    {
        public static void DrawRectangle(Texture2D pixelTexture, SpriteBatch spriteBatch, Rectangle bounds, int lineWidth = 1, Color color = default(Color))
        {
            spriteBatch.Draw(pixelTexture, new Rectangle(bounds.X - lineWidth / 2, bounds.Y - lineWidth / 2, bounds.Width + lineWidth, lineWidth), color);
            spriteBatch.Draw(pixelTexture, new Rectangle(bounds.X - lineWidth / 2, bounds.Y - lineWidth / 2, lineWidth, bounds.Height + lineWidth), color);
            spriteBatch.Draw(pixelTexture, new Rectangle(bounds.X - lineWidth / 2, bounds.Bottom - lineWidth / 2, bounds.Width + lineWidth, lineWidth), color);
            spriteBatch.Draw(pixelTexture, new Rectangle(bounds.Right - lineWidth / 2, bounds.Y - lineWidth / 2, lineWidth, bounds.Height + lineWidth), color);
        }

        public static void DrawCCross(Texture2D lineTexture, SpriteBatch spriteBatch, FloatRectangle bounds, Vector2 lineSpacing, int lineWidth = 1, Color color = default(Color))
        {
            for (float x = bounds.X; x < bounds.Right; x += lineSpacing.X)
            {
                spriteBatch.Draw(lineTexture, new Rectangle((int)x, (int)bounds.Y, lineWidth, (int)bounds.Height), color);
            } 
            for (float y = bounds.Y; y < bounds.Bottom; y += lineSpacing.Y)
            {
                spriteBatch.Draw(lineTexture, new Rectangle((int)bounds.X, (int)y, (int)bounds.Width, lineWidth), color);
            }
        }

        //Alt bzw. noch nicht überarbeitet
        public static void DrawCCross(Texture2D lineTexture, SpriteBatch spriteBatch, int lineSpacing, int height, int width, float scale)
        {
            float realLength = 0;
            Vector2 positionVector = new Vector2(0, 0);
            while (positionVector.X <= width)
            {
                positionVector.X += lineSpacing;
                while (realLength < height)
                {
                    spriteBatch.Draw(lineTexture, positionVector, null, Color.White, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
                    realLength += lineTexture.Height * scale;
                    positionVector.Y += lineTexture.Height * scale;
                }
                positionVector.Y = 0;
                realLength = 0;
            }
            positionVector.X = 0;
            while (positionVector.Y <= height)
            {
                positionVector.Y += lineSpacing;
                while (realLength < width)
                {
                    spriteBatch.Draw(lineTexture, positionVector, null, Color.White, MathHelper.ToRadians(360 - 90), new Vector2(0, 0), scale, SpriteEffects.None, 0);
                    realLength += lineTexture.Height * scale;
                    positionVector.X += lineTexture.Height * scale;
                }
                positionVector.X = 0;
                realLength = 0;
            }
        }
    }
}

namespace GameLibrary.Graphics3D
{
    public class ArrowEngine
    {
        List<Vector3> pointList;
        bool pointListDirty;
        Vector3 direction, origin;
        float length, originDistance;

        public Vector3 Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                pointListDirty = true;
            }
        }

        public Vector3 Origin
        {
            get { return origin; }
            set
            {
                origin = value;
                pointListDirty = true;
            }
        }

        public float Length
        {
            get { return length; }
            set
            {
                length = value;
                pointListDirty = true;
            }
        }

        public float OriginDistance
        {
            get { return originDistance; }
            set
            {
                originDistance = value;
                pointListDirty = true;
            }
        }

        public List<Vector3> PointList
        {
            get
            {
                if (pointListDirty)
                    updatePointList();
                return pointList;
            }
        }

        public ArrowEngine(Vector3 direction, Vector3 origin, float lenght, float originDistance)
        {
            Direction = direction; Origin = origin; Length = lenght; OriginDistance = originDistance; pointList = new List<Vector3>();
        }

        private void updatePointList()
        {
            direction.Normalize();
            Vector3 startPoint = origin + direction * originDistance;
            Vector3 endPoint = startPoint + direction * length;
            pointList.Add(startPoint);
            pointList.Add(endPoint);
            Vector3 arrowLeft = endPoint + Vector3.Transform(direction, Matrix.CreateRotationZ(MathHelper.ToRadians(45)));
            Vector3 arrowRight = endPoint + Vector3.Transform(direction, Matrix.CreateRotationZ(MathHelper.ToRadians(-45)));
            pointList.Add(endPoint);
            pointList.Add(arrowLeft);
            pointList.Add(endPoint);
            pointList.Add(arrowRight);
            pointListDirty = false;
        }
    }
}
