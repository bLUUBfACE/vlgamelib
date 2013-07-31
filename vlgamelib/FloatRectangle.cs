using Microsoft.Xna.Framework;

namespace GameLibrary.Computing2D
{

    public struct FloatRectangle
    {
        public static FloatRectangle Empty = new FloatRectangle(0, 0, 0, 0);
        public float X, Y, Width, Height;

        public FloatRectangle(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
        public FloatRectangle(Vector2 position, float width, float height)
        {
            this.X = position.X;
            this.Y = position.Y;
            this.Width = width;
            this.Height = height;
        }
        public FloatRectangle(Vector2 position, Vector2 size)
        {
            this.X = position.X;
            this.Y = position.Y;
            this.Width = size.X;
            this.Height = size.Y;
        }

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

        public Vector2 Location
        {
            get
            {
                return new Vector2(this.X, this.Y);
            }
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }
        public Vector2 Center
        {
            get
            {
                return new Vector2(this.X + this.Width / 2, this.Y + this.Height / 2);
            }
        }

        public void Offset(Vector2 amount)
        {
            this.X += amount.X;
            this.Y += amount.Y;
        }
        public void Offset(float offsetX, float offsetY)
        {
            this.X += offsetX;
            this.Y += offsetY;
        }
        public static FloatRectangle Offset(FloatRectangle value, Vector2 amount)
        {
            return new FloatRectangle(value.Location + amount, value.Width, value.Height);
        }

        public void Inflate(float horizontalAmount, float verticalAmount)
        {
            this.X -= horizontalAmount;
            this.Y -= verticalAmount;
            this.Width += horizontalAmount * 2f;
            this.Height += verticalAmount * 2f;
        }

        public bool Contains(float x, float y)
        {
            return this.X <= x && x < this.X + this.Width && this.Y <= y && y < this.Y + this.Height;
        }
        public bool Contains(Vector2 value)
        {
            return this.X <= value.X && value.X < this.X + this.Width && this.Y <= value.Y && value.Y < this.Y + this.Height;
        }
        public bool Contains(FloatRectangle value)
        {
            return this.X <= value.X && value.X + value.Width <= this.X + this.Width && this.Y <= value.Y && value.Y + value.Height <= this.Y + this.Height;
        }

        public bool Intersects(FloatRectangle value)
        {
            return value.X < this.X + this.Width && this.X < value.X + value.Width && value.Y < this.Y + this.Height && this.Y < value.Y + value.Height;
        }

        public override string ToString()
        {
            return "X:" + this.X + " Y:" + this.Y + " Width:" + this.Width + " Height:" + this.Height;
        }
        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.Y.GetHashCode() + this.Width.GetHashCode() + this.Height.GetHashCode();
        }

        public bool Equals(FloatRectangle other)
        {
            return this.X == other.X && this.Y == other.Y && this.Width == other.Width && this.Height == other.Height;
        }
        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is FloatRectangle)
            {
                result = this.Equals((FloatRectangle)obj);
            }
            return result;
        }

        public static bool operator ==(FloatRectangle a, FloatRectangle b)
        {
            return a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
        }
        public static bool operator !=(FloatRectangle a, FloatRectangle b)
        {
            return a.X != b.X || a.Y != b.Y || a.Width != b.Width || a.Height != b.Height;
        }
    }
}
