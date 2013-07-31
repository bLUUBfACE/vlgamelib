using Microsoft.Xna.Framework;

namespace GameLibrary.Computing3D
{
    public struct Cube
    {
        public float X, Y, Z;
        public float Width, Height, Depth;

        public float Left
        {
            get
            {
                return this.X;
            }
        }
        public float Right
        {
            get
            {
                return this.X + this.Width;
            }
        }
        public float Top
        {
            get
            {
                return this.Y;
            }
        }
        public float Bottom
        {
            get
            {
                return this.Y + this.Height;
            }
        }
        public float Front
        {
            get
            {
                return this.Z;
            }
        }
        public float Back
        {
            get
            {
                return this.Z + this.Depth;
            }
        }
        public Vector3 Position
        {
            get
            {
                return new Vector3(X, Y, Z);
            }
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }

        public Cube(float x, float y, float z, float width, float height, float depth)
        {
            X = x; Y = y; Z = z; Width = width; Height = height; Depth = depth;
        }

        public Cube(Vector3 position, float width, float height, float depth)
        {
            X = position.X; Y = position.Y; Z = position.Z; Width = width; Height = height; Depth = depth;
        }

        public bool Contains(Vector3 point)
        {
            return (X < point.X && point.X < X + Width) && (Y < point.Y && point.Y < Y + Height) && (Z < point.Z && point.Z < Z + Depth);
        }

        public bool Intersects(Cube value)
        {
            return value.X < this.X + this.Width && this.X < value.X + value.Width && value.Y < this.Y + this.Height && this.Y < value.Y + value.Height && value.Z < this.Z + this.Depth && this.Z < value.Z + value.Depth;
        }
    }
}
