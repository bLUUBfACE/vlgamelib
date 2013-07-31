using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameLibrary.Computing2D
{
    public class Line
    {
        public float m { get; private set; }
        public float t { get; private set; }

        public Vector2 PointA { get; private set; }
        public Vector2 PointB { get; private set; }
        public Vector2 LineVector { get; private set; }
        public bool vertical = false;

        public Line(Vector2 a, Vector2 b)
        {
            if (a.X == b.X)
                vertical = true;
            m = (b.Y - a.Y) / (b.X - a.X);
            t = a.Y - m * a.X;
            PointA = a; PointB = b;
            LineVector = b - a;
        }

        public bool Intersect(Line Line2, out Vector2 intersection)
        {
            intersection = Vector2.Zero;
            if (PointA == Line2.PointA && PointB == Line2.PointB)
                return true;
            else if (m == Line2.m)
                return false;
            if (vertical)
                if (Math.Min(Line2.PointA.X, Line2.PointB.X) < PointA.X && Math.Max(Line2.PointB.X, Line2.PointA.X) > PointA.X)
                    if (Line2.PointB.Y - (Line2.PointB.X - PointA.X) * Line2.m < Math.Max(PointA.Y, PointB.Y) && Line2.PointB.Y - (Line2.PointB.X - PointA.X) * Line2.m > Math.Min(PointA.Y, PointB.Y))
                    { intersection = new Vector2(PointA.X, Line2.PointB.Y - (Line2.PointB.X - PointA.X) * Line2.m); return true; }
                    else return false;
                else return false;
            if (Line2.vertical)
                if (Math.Min(PointA.X, PointB.X) < Line2.PointA.X && Math.Min(PointB.X, PointA.X) > Line2.PointA.X)
                    if (PointB.Y - (PointB.X - Line2.PointA.X) * m < Math.Max(Line2.PointA.Y, Line2.PointB.Y) && PointB.Y - (PointB.X - Line2.PointA.X) * m > Math.Min(Line2.PointA.Y, Line2.PointB.Y))
                    { intersection = new Vector2(Line2.PointA.X, PointB.Y - (PointB.X - Line2.PointA.X) * m); return true; }
                    else return false; 
                else return false;
            float slopeDifference = m - Line2.m;
            float YpostponementDifference = Line2.t - t;
            float XIntersection = YpostponementDifference / slopeDifference;
            if (XIntersection > Math.Min(PointA.X, PointB.X) && XIntersection < Math.Max(PointA.X, PointB.X) && XIntersection > Math.Min(Line2.PointA.X, Line2.PointB.X) && XIntersection < Math.Max(Line2.PointA.X, Line2.PointB.X))
            { intersection = new Vector2(XIntersection, XIntersection * m + t); return true; }
            else return false;
        }

        public bool Intersect(Line Line2)
        {
            Vector2 intersection; return Intersect(Line2, out intersection);
        }

        public bool Contains(Vector2 point)
        {
            if ((!vertical && point.Y == m * point.X + t)
                || (vertical && point.X == PointA.X))
            {
                return Contains2(point);
            }
            else return false;
        }

        internal bool Contains2(Vector2 point)
        {
            Vector2 tempPointA = PointA;
            Vector2 tempPointB = PointB;
            Vector2 tempPointPlaceholder;

            if (PointB.Y > PointA.Y)
            {
                tempPointPlaceholder = tempPointA;
                tempPointA = tempPointB;
                tempPointB = tempPointPlaceholder;
            }

            if (!vertical)
            {
                if (point.Y != PointA.Y || point.Y != PointB.Y)
                {
                    if (point.Y >= Math.Min(PointA.Y, PointB.Y) && point.Y <= Math.Max(PointA.Y, PointB.Y))
                        return true;
                }
                else
                    if (point.X >= Math.Min(PointA.X, PointB.X) && point.X <= Math.Max(PointA.X, PointB.X))
                        return true;
            }
            else
                if (point.Y > Math.Min(PointA.Y, PointB.Y) && point.Y < Math.Max(PointA.Y, PointB.Y))
                    return true;

            return false;
        }

        public float getLength()
        {
            return LineVector.Length();
        }

        public float getLengthSqared()
        {
            return LineVector.LengthSquared();
        }

    }

    public class Polygon
    {
        public Line[] Lines;
        public Vector2[] PolygonPoints;
        public readonly bool autocomplete;

        public Polygon(Vector2[] Points, bool autocomplete)
        {
            bool endPointsEqual = false;
            if (Points[Points.Length - 1] == Points[0]) { autocomplete = true; endPointsEqual = true; }
            this.autocomplete = autocomplete;
            if (Points.Length < 2)
                throw new Exception("No valid count of points");
            Lines = new Line[autocomplete && !endPointsEqual ? Points.Length : Points.Length - 1];
            for (int i = 0; i < Points.Length; i++)
            {
                if (i < Points.Length - 1)
                    Lines[i] = new Line(Points[i], Points[i + 1]);
                else if (autocomplete) if (!endPointsEqual) Lines[i] = new Line(Points[i], Points[0]);
            }
            PolygonPoints = Points;
        }

        public bool Intersect(Polygon Polygon2)
        {
            foreach (Line Line1 in Lines)
            {
                foreach (Line Line2 in Polygon2.Lines)
                {
                    if (Line1.Intersect(Line2))
                        return Line1.Intersect(Line2);
                }
            }
            return false;
        }
    }


    public class LinePath : IPathable
    {
        public readonly Line Line;
        Vector2 normDir, curPos;
        float moves;

        public LinePath(Line l)
        {
            this.Line = l;
            curPos = l.PointA;
            normDir = l.LineVector; normDir.Normalize();
            moves = l.LineVector.Length() / normDir.Length();
        }

        public Vector2 MoveNext(float speed, out bool isFinished, out float surplus)
        {
            moves -= speed;
            if (moves < 0)
            {
                isFinished = true;
                surplus = moves * -1;
                moves = 0;
                curPos = Line.PointB;
            }
            else
            {
                isFinished = false;
                surplus = 0;
                curPos += normDir * speed;
            }
            return curPos;
        }

        public Vector2 MoveNext(float speed) 
        {
            moves -= speed;
            if (moves < 0)
            {
                moves = 0;
                curPos = Line.PointB;
            }
            else
            {
                curPos += normDir * speed;
            }
            return curPos;
        }

        public Vector2 Current()
        {
            return curPos;
        }

        public void Reset()
        {
            curPos = Line.PointA;
            moves = Line.LineVector.Length() / normDir.Length();
        }
    }

    public class PolygonPath : IPathable
    {
        private LinePath[] LinePathes;
        private bool loop, finished;
        private int curindex;
        private Vector2 current;

        public PolygonPath(Polygon polygon, bool loop)
        {
            LinePathes = new LinePath[polygon.Lines.Length];
            for (int i = 0; i < LinePathes.Length; i++) { LinePathes[i] = new LinePath(polygon.Lines[i]); }
            this.loop = loop;
            this.current = polygon.Lines[0].PointA;
        }

        public Vector2 MoveNext(float speed)
        {
            bool f = false; float s = 0;
            return this.MoveNext(speed, out f, out s);
        }

        public Vector2 MoveNext(float speed, out bool isFinished, out float surplus)
        {
            isFinished = true;
            surplus = 0;
            if (!this.finished)
            {
                bool lineFinished = false;
                float lineSurplus = 0;
                Vector2 returnVector = LinePathes[curindex].MoveNext(speed, out lineFinished, out lineSurplus);
                while (lineFinished == true)
                {
                    LinePathes[curindex].Reset();
                    if (curindex == LinePathes.Length - 1)
                    {
                        if (loop) curindex = -1;
                        else
                        {
                            this.finished = true;
                            surplus = lineSurplus;
                            this.current = returnVector;
                            return returnVector;
                        }
                    }
                    //returnVector = LinePathes[++curindex].MoveNext(LinePathes[curindex == 0 ? LinePathes.Length - 1 : curindex - 1].rest, out lineFinished);
                    returnVector = LinePathes[++curindex].MoveNext(lineSurplus, out lineFinished, out lineSurplus);
                }
                isFinished = false;
                this.current = returnVector;
                return returnVector;
            }
            else return this.current;
        }

        public Vector2 Current()
        {
            return current;
        }
    }

}
