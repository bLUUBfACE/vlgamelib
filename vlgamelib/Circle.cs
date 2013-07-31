using System;
using Microsoft.Xna.Framework;

namespace GameLibrary.Computing2D
{
    public class Circle
    {
        public Vector2 Position { get; set; }
        public float Radius { get; set; }

        public Circle(Vector2 position, float radius)
        {
            Position = position;
            Radius = radius;
        }

        public bool Intersect(Circle circle)
        {
            double distance = Math.Sqrt((int)(this.Position.X - circle.Position.X) ^ 2 + (int)(this.Position.Y - circle.Position.Y) ^ 2);
            return (this.Radius + circle.Radius) > distance;
        }

        public bool Intersect(Vector2 point)
        {
            //double distance = Math.Sqrt((int)(this.Position.X - point.X) ^ 2 + (int)(this.Position.Y - point.Y) ^ 2);
            double distanceSquared = this.Position.DistanceSquared(point);
            return this.Radius * this.Radius > distanceSquared;
        }

        public bool Intersect(Polygon polygon)
        {
            foreach (Line l in polygon.Lines)
                if (Intersect(l))
                    return true;
            return false;
        }

        public bool Intersect(Polygon polygon, out Line intersectionLine)
        {
            foreach (Line l in polygon.Lines)
                if (Intersect(l))
                {
                    intersectionLine = l;
                    return true;
                }
            intersectionLine = null;
            return false;
        }

        public bool Intersect(Line line)
        {
            float invDSq = 1.0f / line.LineVector.LengthSquared();
            float p = (Vector2.Dot(line.LineVector, Position) - Vector2.Dot(line.PointA, line.LineVector)) * invDSq;
            float discriminant = p * p - ((line.PointA - Position).LengthSquared() - Radius * Radius) * invDSq;

            if (discriminant < 0)
                return false;
            else
            {
                float squareRoot = (float)Math.Sqrt(discriminant);
                float t1 = p + squareRoot;
                float t2 = p - squareRoot;
                Vector2 pointA = line.PointA + t1 * line.LineVector;
                Vector2 pointB = line.PointA + t1 * line.LineVector;

                if (line.Contains2(pointA) || line.Contains2(pointB))
                    return true;

                return false;
            }
        }

        public bool Intersect(Line line, out Vector2 pointA, out Vector2 pointB)
        {
            float invDSq = 1.0f / line.LineVector.LengthSquared();
            float p = (Vector2.Dot(line.LineVector, Position) - Vector2.Dot(line.PointA, line.LineVector)) * invDSq;
            float discriminant = p * p - ((line.PointA - Position).LengthSquared() - Radius * Radius) * invDSq;

            if (discriminant < 0)
            {
                pointA = pointB = new Vector2();
                return false;
            }
            else
            {
                float squareRoot = (float)Math.Sqrt(discriminant);
                float t1 = p + squareRoot;
                float t2 = p - squareRoot;
                pointA = line.PointA + t1 * line.LineVector;
                pointB = line.PointA + t1 * line.LineVector;

                if (line.Contains2(pointA) || line.Contains2(pointB))
                    return true;

                return false;
            }
        }
    }

    public class CirclePath : IPathable
    {
        protected Vector2 position;
        protected float radius;
        public readonly Circle Circle;

        //true: upper semicircle
        //false: lower semicircle
        protected bool positiveYSign = false;
        protected readonly bool clockwise;
        protected float currentX, YDistortion = 1;
        protected LinePath currentPath;

        protected Vector2 currentVector;

        public CirclePath(Circle circle, bool clockwise, float startingX, bool positiveYSign)
        {
            this.Circle = circle;
            this.clockwise = clockwise;
            this.Initialize(startingX, positiveYSign);
        }

        public CirclePath(Circle circle, bool clockwise, Vector2 startingDirection)
        {
            this.Circle = circle;
            this.clockwise = clockwise;
            startingDirection = Vector2.Normalize(startingDirection) * circle.Radius;
            this.Initialize(startingDirection.X,
                startingDirection.Y > 0 ? true : startingDirection.Y > 0 ? false :
                (clockwise && startingDirection.X > 0) || (!clockwise && startingDirection.X < 0) ? true : false );
        }

        public CirclePath(FloatRectangle rectangle, bool clockwise, float startingX, bool positiveYSign)
        {
            this.Circle = new Circle(rectangle.Center,  rectangle.Width / 2);
            this.YDistortion = rectangle.Height / rectangle.Width;
            this.clockwise = clockwise;
            this.Initialize(startingX, positiveYSign);
        }

        private void Initialize(float startingX, bool positiveYSign)
        {
            this.position = this.Circle.Position;
            this.radius = Math.Max(this.Circle.Radius, 0);
            this.currentX = startingX;
            this.positiveYSign = positiveYSign;

            if (Math.Abs(this.currentX) < this.radius)
            {
                this.currentPath = new LinePath(new Line(new Vector2(this.currentX, calculateYCoordinate(this.currentX)), new Vector2(calculateNextX(), calculateYCoordinate(calculateNextX()))));
            }
            else if (Math.Abs(this.currentX) == this.radius)
            {
                if ((this.clockwise && this.currentX > 0) || (!this.clockwise && this.currentX < 0))
                    positiveYSign = !positiveYSign;
                this.currentPath = new LinePath(new Line(new Vector2(this.currentX, 0), new Vector2(calculateNextX(), calculateYCoordinate(calculateNextX()))));
            }
            else throw new ArgumentException();
            this.currentVector = this.currentPath.Current() + this.position;
        }

        protected float calculateYCoordinate(float xCoord)
        {
            return (float)Math.Sqrt(this.radius * this.radius - xCoord * xCoord) * (positiveYSign ? 1 : -1) * YDistortion;
        }

        protected float calculateNextX()
        {
            bool l = false;
            return calculateNextX(out l);
        }

        protected float calculateNextX(out bool xLimitReached)
        {
            float nextX = currentX + (positiveYSign ? -1 : 1) * (clockwise ? 1 : -1);
            xLimitReached = Math.Abs(nextX) < this.radius ? false : true;
            return Math.Abs(nextX) < this.radius ? nextX : this.radius * (positiveYSign ? -1 : 1) * (clockwise ? 1 : -1);
        }


        public virtual Vector2 MoveNext(float distance)
        {
            bool f = false; float s = 0;
            return this.MoveNext(distance, out f, out s);
        }

        public virtual Vector2 MoveNext(float distance, out bool isFinished, out float surplus)
        {
            surplus = 0;
            isFinished = false;
            if (this.radius == 0)
                return this.position;

            bool lineFinished = false;
            float lineSurplus = 0;
            Vector2 returnVector;
            do
            {
                returnVector = currentPath.MoveNext(distance, out lineFinished, out lineSurplus);
                if (lineFinished)
                {
                    distance = lineSurplus;
                    bool xlimitReached = false;
                    this.currentX = calculateNextX(out xlimitReached);
                    if(xlimitReached)
                        positiveYSign = !positiveYSign;
                    this.currentPath = new LinePath(new Line(new Vector2(this.currentX, calculateYCoordinate(this.currentX)), new Vector2(calculateNextX(), calculateYCoordinate(calculateNextX()))));
                }
            } while (lineFinished);

            this.currentVector = returnVector + this.position;
            return returnVector + this.position;
        }

        public Vector2 Current()
        {
            return this.currentVector;
        }
    }

    public enum SpiralType
    {
        Default,
        Archimedean,
    }

    public class SpiralPath : CirclePath
    {
        
        public readonly SpiralType SpiralType = SpiralType.Default;
        private float contractionPerDistanceOrAngle, totalContraction,
            lastAngle;
        private Vector2 lastCirclePosition;

        public SpiralPath(Circle startingCircle, bool clockwise, float startingX, bool positiveYSign, float contractionPerDistance)
            : base(startingCircle, clockwise, startingX, positiveYSign)
        {
            this.contractionPerDistanceOrAngle = contractionPerDistance;
            this.SpiralType = SpiralType.Default;
        }

        public SpiralPath(Circle startingCircle, bool clockwise, Vector2 startingDirection, float contractionPerDistance)
            : base(startingCircle, clockwise, startingDirection)
        {
            this.contractionPerDistanceOrAngle = contractionPerDistance;
            this.SpiralType = SpiralType.Default;
        }

        public SpiralPath(Circle startingCircle, bool clockwise, Vector2 startingDirection, float contractionPerDistanceOrAngle, SpiralType type)
            :base(startingCircle, clockwise, startingDirection)
        {
            this.contractionPerDistanceOrAngle = contractionPerDistanceOrAngle;
            this.SpiralType = type;
        }

        public override Vector2 MoveNext(float distance)
        {
            bool f = false; float s = 0;
            return this.MoveNext(distance, out f, out s);
        }

        public override Vector2 MoveNext(float distance, out bool isFinished, out float surplus)
        {
            switch(this.SpiralType){
                case SpiralType.Default:
                    totalContraction += contractionPerDistanceOrAngle * distance;
                    break;
                case SpiralType.Archimedean:
                        totalContraction += contractionPerDistanceOrAngle * lastAngle;
                    break;
            }
            if (totalContraction < this.radius)
            {
                Vector2 circlePosition = base.MoveNext(distance * (this.radius / (this.radius - this.totalContraction)), out isFinished, out surplus) - this.position;
                calculateLastAngle(circlePosition);
                return currentVector = circlePosition + Vector2.Normalize(Vector2.Zero - circlePosition) * totalContraction + this.position;
            }
            else
            {
                isFinished = true; surplus = 0;
                return this.position;
            }
        }

        private void calculateLastAngle(Vector2 circlePosition)
        {
            if (this.SpiralType != SpiralType.Default)
            {
                lastAngle = lastCirclePosition.AngleInRadiansAntiClockwise(circlePosition);
                lastAngle = this.clockwise ?  lastAngle : MathHelper.TwoPi - lastAngle;
                lastCirclePosition = circlePosition;
            }
        }

    }
}
