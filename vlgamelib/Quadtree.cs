using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace GameLibrary.Computing2D
{
    public interface IQuadtreeable
    {
        List<Quadtree.Partition> getQuadtreePartitions();
        void updateQuadtree();
        FloatRectangle getRectangle();
    }

    public abstract class Quadtreeable : IQuadtreeable, IDisposable
    {
        protected List<Quadtree.Partition> Partitions = new List<Quadtree.Partition>();
        protected Quadtree Tree;
        protected FloatRectangle Rectangle;

        public Quadtreeable(Quadtree tree, FloatRectangle rectangle)
        {
            this.Tree = tree;
            this.Rectangle = rectangle;
            this.updateQuadtree();
        }

        public virtual void updateQuadtree()
        {
            this.Partitions = this.Tree.UpdateQuadtreeablePosition(this);
        }

        public virtual List<Quadtree.Partition> getQuadtreePartitions()
        {
            return Partitions;
        }

        public virtual void removeFromQuadtree()
        {
            if (this.Partitions != null)
                foreach (Quadtree.Partition p in this.Partitions)
                {
                    p.RemoveObject(this);
                }
        }

        public virtual FloatRectangle getRectangle()
        {
            return Rectangle;
        }

        public virtual List<T> getAllQuadtreeObjects<T>() where T : IQuadtreeable
        {
            List<T> objects = new List<T>();
            foreach (Quadtree.Partition p in this.Partitions)
            {
                foreach (IQuadtreeable q in p.GetObjects())
                {
                    if (q is T && !objects.Contains((T)q))
                        objects.Add((T)q);
                }
            }
            return objects;
        }

        public virtual void Dispose()
        {
            this.removeFromQuadtree();
        }
    }

    public class Quadtree
    {
        public readonly Partition MainPartition;
        public readonly FloatRectangle Area;

        internal readonly int MaxLevel;
        internal readonly int MaxObjects;
        //Size of the smallest possible partition, based on the maximum level
        internal readonly float SmallestPartitionWidth, SmallestPartitionHeight;

        public readonly Partition[,] Grid;

        public Quadtree(FloatRectangle area, int maxLevel, int maxObjects)
        {
            this.Area = area;
            this.MaxLevel = maxLevel;
            this.MaxObjects = maxObjects;

            int gridSize = (int)Math.Pow(2, maxLevel);
            this.Grid = new Partition[gridSize, gridSize];

            //Calculate smallest possible sizes
            SmallestPartitionWidth = this.Area.Width / Grid.GetLength(0);
            SmallestPartitionHeight = this.Area.Height / Grid.GetLength(1);

            this.MainPartition = new Partition(area, 1, null, this);

            //Initialize Grid
            for (int i = 0; i < this.Grid.GetLength(0); i++)
            {
                for (int a = 0; a < this.Grid.GetLength(1); a++)
                {
                    this.Grid[i, a] = this.MainPartition;
                }
            }
        }

        public List<Partition> UpdateQuadtreeablePosition(IQuadtreeable obj)
        {
            foreach (Partition p in obj.getQuadtreePartitions())
            {
                p.RemoveObject(obj);
            }

            List<Partition> newPartitions = new List<Partition>();

            FloatRectangle area = obj.getRectangle();
            if (this.Area.Intersects(area))
            {
                //Defining the upper-left coordinate of the rectangle in the grid
                Point upperLeftCoord = new Point(area.Left < this.Area.Left ? 0 : (int)((area.Left - area.Left % this.SmallestPartitionWidth) / this.SmallestPartitionWidth), //X-Coordinate
                    area.Top < this.Area.Top ? 0 : (int)((area.Top - area.Top % this.SmallestPartitionHeight) / this.SmallestPartitionHeight));                               //Y-Coordinate

                //Defining the lower-right coordinate of the rectangle in the grid
                Point lowerRightCoord = new Point(area.Right >= this.Area.Right ? this.Grid.GetLength(0) - 1 : (int)((area.Right - area.Right % this.SmallestPartitionWidth) / this.SmallestPartitionWidth), //X-Coordinate
                    area.Bottom >= this.Area.Bottom ? this.Grid.GetLength(1) - 1 : (int)((area.Bottom - area.Bottom % this.SmallestPartitionHeight) / this.SmallestPartitionHeight));                        //Y-Coordinate

                for (int i = upperLeftCoord.X; i < lowerRightCoord.X + 1; i++)
                {
                    for (int a = upperLeftCoord.Y; a < lowerRightCoord.Y + 1; a++)
                    {
                        if (!newPartitions.Contains(this.Grid[i, a]))
                        {
                            this.Grid[i, a].AddObject(obj);
                            newPartitions.Add(this.Grid[i, a]);                            
                        }
                    }                    
                }
            }
            return newPartitions;
        }

        public List<T> GetIntersectionObjects<T>(FloatRectangle area) where T : IQuadtreeable
        {
            if (this.Area.Intersects(area))
            {
                List<T> objects = new List<T>();

                //Defining the upper-left coordinate of the rectangle in the grid
                Point upperLeftCoord = new Point(area.Left < this.Area.Left ? 0 : (int)((area.Left - area.Left % this.SmallestPartitionWidth) / this.SmallestPartitionWidth), //X-Coordinate
                    area.Top < this.Area.Top ? 0 : (int)((area.Top - area.Top % this.SmallestPartitionHeight) / this.SmallestPartitionHeight));                               //Y-Coordinate

                //Defining the lower-right coordinate of the rectangle in the grid
                Point lowerRightCoord = new Point(area.Right >= this.Area.Right ? this.Grid.GetLength(0) - 1 : (int)((area.Right - area.Right % this.SmallestPartitionWidth) / this.SmallestPartitionWidth), //X-Coordinate
                    area.Bottom >= this.Area.Bottom ? this.Grid.GetLength(1) - 1 : (int)((area.Bottom - area.Bottom % this.SmallestPartitionHeight) / this.SmallestPartitionHeight));                        //Y-Coordinate

                for (int i = upperLeftCoord.X; i < lowerRightCoord.X + 1; i++)
                {
                    for (int a = upperLeftCoord.Y; a < lowerRightCoord.Y + 1; a++)
                    {
                        foreach (IQuadtreeable q in Grid[i, a].GetObjects())
                        {
                            if (q is T && !objects.Contains((T)q))
                                objects.Add((T)q);
                        }
                    }
                }
                return objects;
            }
            throw new ArgumentException();
        }   

        public class Partition
        {
            public readonly FloatRectangle Area;
            //GridCoordinate is defined as a Point because it represent a position within the int-based grid
            public readonly Point GridCoordinate;

            public readonly int Level;
            public readonly Quadtree Root;

            public Partition[] ChildNodes;
            Partition parent;

            private List<IQuadtreeable> Objects = new List<IQuadtreeable>();

            public Partition(FloatRectangle area, int level, Partition parent, Quadtree root)
            {                
                this.Root = root;
                this.Level = level;
                this.parent = (Level > 1) ? parent : null;

                this.Area = area;
                this.GridCoordinate = new Point((int)((area.X - area.X % root.SmallestPartitionWidth) / root.SmallestPartitionWidth),
                    (int)((area.Y - area.Y % root.SmallestPartitionHeight) / root.SmallestPartitionHeight));

                
                int partitionGridSize = (int)Math.Pow(2, root.MaxLevel - this.Level + 1);
                //Update the PartitionGrid in the root
                for (int i = 0; i < partitionGridSize; i++)
                {
                    for (int a = 0; a < partitionGridSize; a++)
                    {
                        root.Grid[GridCoordinate.X + i, GridCoordinate.Y + a] = this;
                    }
                }
            }

            public void Split()
            {
                if (Root.MaxLevel > this.Level)
                {
                    ChildNodes = new Partition[4];
                    ChildNodes[0] = new Partition(new FloatRectangle(this.Area.Location, this.Area.Width / 2f, this.Area.Height / 2f), this.Level + 1, this, this.Root);
                    ChildNodes[1] = new Partition(new FloatRectangle(this.Area.Location + new Vector2(this.Area.Width / 2f, 0), this.Area.Width / 2f, this.Area.Height / 2f), this.Level + 1, this, this.Root);
                    ChildNodes[2] = new Partition(new FloatRectangle(this.Area.Location + new Vector2(0, this.Area.Height / 2f), this.Area.Width / 2f, this.Area.Height / 2f), this.Level + 1, this, this.Root);
                    ChildNodes[3] = new Partition(new FloatRectangle(this.Area.Location + new Vector2(this.Area.Width / 2f, this.Area.Height / 2f), this.Area.Width / 2f, this.Area.Height / 2f), this.Level + 1, this, this.Root);

                    IQuadtreeable[] tempObjects = new IQuadtreeable[this.Objects.Count];
                    this.Objects.CopyTo(tempObjects);
                    foreach (IQuadtreeable iq in tempObjects)
                    {
                        iq.updateQuadtree();
                    }
                }
                else throw new InvalidOperationException();              
            }

            public void AddObject(IQuadtreeable obj)
            {
                if(!this.Objects.Contains(obj))
                    this.Objects.Add(obj);
                if (this.Objects.Count > Root.MaxObjects && Root.MaxLevel > this.Level)
                    this.Split();
            }

            public void RemoveObject(IQuadtreeable obj)
            {
                if (this.Objects.Contains(obj))
                    this.Objects.Remove(obj);
            }

            public List<IQuadtreeable> GetObjects()
            {
                //Copy to a new list to prevent other classes from changing the list
                return this.Objects.ToList<IQuadtreeable>();
            }
        }
    }
}
