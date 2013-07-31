using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;

namespace GameLibrary.Computing3D
{
    public interface IOcttreeable
    {
        List<Octtree.Partition> getOcttreePartitions();
        void updateOcttree();
        Cube getCube();
    }

    public abstract class Octtreeable : IOcttreeable, IDisposable
    {
        protected List<Octtree.Partition> Partitions = new List<Octtree.Partition>();
        protected Octtree Tree;
        protected Cube Cube;

        public Octtreeable(Octtree tree, Cube cube)
        {
            this.Tree = tree;
            this.Cube = cube;
            this.updateOcttree();
        }

        public virtual List<Octtree.Partition> getOcttreePartitions()
        {
            return Partitions;
        }

        public virtual void removeFromOcttree()
        {
            if (this.Partitions != null)
                foreach (Octtree.Partition p in this.Partitions)
                {
                    p.RemoveObject(this);
                }
        }

        public virtual void updateOcttree()
        {
            this.Partitions = this.Tree.UpdateOcttreeablePosition(this);
        }

        public virtual Cube getCube()
        {
            return Cube;
        }

        public virtual List<IOcttreeable> getAllOcttreeObjects<T>()
        {
            List<IOcttreeable> objects = new List<IOcttreeable>();
            foreach (Octtree.Partition p in this.Partitions)
            {
                foreach (IOcttreeable q in p.GetObjects())
                {
                    if (q is T && !objects.Contains(q))
                        objects.Add(q);
                }
            }
            return objects;
        }

        public virtual void Dispose()
        {
            this.removeFromOcttree();
        }
    }


    public class Octtree
    {
        private bool clearupNeeded = false;
        private List<Partition> canBeClearedUp;

        public readonly Cube Area; //Area of the Octtree
        public readonly Partition MainPartition;

        internal readonly int maxObjectsPerPartition;
        internal readonly int maxLevel;

        internal readonly float smallestPartitionWidth, smallestPartitionHeight, smallestPartitionDepth; //Callculated by Area and maxLevel

        public readonly Partition[, ,] PartitionGrid;

        public Octtree(Cube area, int maxLevel, int maxObjectsPerPartition)
        {
            this.Area = area;
            this.maxLevel = maxLevel;
            this.maxObjectsPerPartition = maxObjectsPerPartition;

            int gridSideCount = (int)Math.Pow(2, maxLevel); //The max count of partitions per side of the partition grid
            this.PartitionGrid = new Partition[gridSideCount, gridSideCount, gridSideCount];

            this.smallestPartitionWidth = Area.Width / gridSideCount;
            this.smallestPartitionHeight = Area.Height / gridSideCount;
            this.smallestPartitionDepth = Area.Depth / gridSideCount;

            this.MainPartition = new Partition(this.Area, 1, null, this);
            
            //Set up grid
            for (int x = 0; x < gridSideCount; x++)
            {
                for (int y = 0; y < gridSideCount; y++)
                {
                    for (int z = 0; z < gridSideCount; z++)
                    {
                        PartitionGrid[x, y, z] = this.MainPartition;
                    }
                }
            }
        }

        public List<Partition> UpdateOcttreeablePosition(IOcttreeable obj)
        {
            if (clearupNeeded) //Clears up Octtree after calling Optimize()
            {
                foreach (Partition partition in canBeClearedUp)
                {
                    partition.Merge();
                }
                clearupNeeded = false;
            }

            foreach (Partition p in obj.getOcttreePartitions())
                p.RemoveObject(obj);

            List<Partition> newPartitions = new List<Partition>();

            Cube area = obj.getCube();
            if (this.Area.Intersects(area))
            {
                //Defining the upper-left coordinate of the rectangle in the grid
                Vector3 upperLeftCoord = new Vector3(area.Left < this.Area.Left ? 0 : (int)((area.Left - area.Left % this.smallestPartitionWidth) / this.smallestPartitionWidth), //X-Coordinate
                    area.Top < this.Area.Top ? 0 : (int)((area.Top - area.Top % this.smallestPartitionHeight) / this.smallestPartitionHeight),                                    //Y-Coordinate
                    area.Front < this.Area.Front ? 0 : (int)((area.Front - area.Front % this.smallestPartitionDepth) / this.smallestPartitionDepth));                             //Z-Coordinate

                //Defining the lower-right coordinate of the rectangle in the grid
                Vector3 lowerRightCoord = new Vector3(area.Right >= this.Area.Right ? this.PartitionGrid.GetLength(0) - 1 : (int)((area.Right - area.Right % this.smallestPartitionWidth) / this.smallestPartitionWidth), //X-Coordinate
                    area.Bottom >= this.Area.Bottom ? this.PartitionGrid.GetLength(1) - 1 : (int)((area.Bottom - area.Bottom % this.smallestPartitionHeight) / this.smallestPartitionHeight),                             //Y-Coordinate
                    area.Back >= this.Area.Back ? this.PartitionGrid.GetLength(2) - 1 : (int)((area.Back - area.Back % this.smallestPartitionDepth) / this.smallestPartitionDepth));                                      //Z-Coordinate

                for (int x = (int)upperLeftCoord.X; x < (int)lowerRightCoord.X + 1; x++)
                {
                    for (int y = (int)upperLeftCoord.Y; y < (int)lowerRightCoord.Y + 1; y++)
                    {
                        for (int z = (int)upperLeftCoord.Z; z < (int)lowerRightCoord.Z + 1; z++)
                        {
                            if (!newPartitions.Contains(this.PartitionGrid[x, y, z]))
                            {
                                this.PartitionGrid[x, y, z].AddObject(obj);
                                newPartitions.Add(this.PartitionGrid[x, y, z]);

                            }
                        }
                    }
                }
            }
            return newPartitions;
        }

        public void Optimize() //Removes empty partitions, can take some time
        {
            Thread clearUp = new Thread(getEmptyPartitions);
            clearUp.Start();
        }

        private void getEmptyPartitions()
        {
            canBeClearedUp = MainPartition.getEmptyChildren();
            if (canBeClearedUp.Contains(MainPartition))
                canBeClearedUp.Remove(MainPartition);
            clearupNeeded = true;
            Thread.CurrentThread.Abort();
        }

        public class Partition
        {
            public readonly Cube Area;
            public readonly Vector3 GridCoordinate; //Represents the position in thr grid NOT THE REAL POSITION!!

            public readonly int Level;
            public readonly Octtree Root;
            public Partition[] ChildNodes;
            Partition parent;

            internal List<IOcttreeable> objects = new List<IOcttreeable>();

            public Partition(Cube area, int level, Partition parent, Octtree root)
            {
                this.Level = level;
                this.parent = level > 1 ? parent : null;
                this.Root = root;

                this.Area = area;
                this.GridCoordinate = new Vector3((int)((area.X - area.X % root.smallestPartitionWidth) / root.smallestPartitionWidth),
                    (int)((area.Y - area.Y % root.smallestPartitionHeight) / root.smallestPartitionHeight),
                    (int)((area.Z - area.Z % root.smallestPartitionDepth) / root.smallestPartitionDepth));

                //Update root.PartitionGrid
                int gridSideCount = (int)Math.Pow(2, root.maxLevel - this.Level + 1);

                for (int x = 0; x < gridSideCount; x++)
                {
                    for (int y = 0; y < gridSideCount; y++)
                    {
                        for (int z = 0; z < gridSideCount; z++)
                        {
                            root.PartitionGrid[(int)this.GridCoordinate.X + x, (int)this.GridCoordinate.Y + y, (int)this.GridCoordinate.Z + z] = this;
                        }
                    }
                }
            }

            public void Split()
            {
                if (Root.maxLevel > this.Level)
                {
                    ChildNodes = new Partition[8];
                    ChildNodes[0] = new Partition(new Cube(this.Area.X, this.Area.Y, this.Area.Z, this.Area.Width / 2, this.Area.Height / 2, this.Area.Depth / 2), this.Level + 1, this, Root);
                    ChildNodes[1] = new Partition(new Cube(this.Area.X + this.Area.Width / 2, this.Area.Y, this.Area.Z, this.Area.Width / 2, this.Area.Height / 2, this.Area.Depth / 2), this.Level + 1, this, Root);
                    ChildNodes[2] = new Partition(new Cube(this.Area.X, this.Area.Y, this.Area.Z + this.Area.Depth / 2, this.Area.Width / 2, this.Area.Height / 2, this.Area.Depth / 2), this.Level + 1, this, Root);
                    ChildNodes[3] = new Partition(new Cube(this.Area.X + this.Area.Width / 2, this.Area.Y, this.Area.Z + this.Area.Depth / 2, this.Area.Width / 2, this.Area.Height / 2, this.Area.Depth / 2), this.Level + 1, this, Root);
                    ChildNodes[4] = new Partition(new Cube(this.Area.X, this.Area.Y + this.Area.Height / 2, this.Area.Z, this.Area.Width / 2, this.Area.Height / 2, this.Area.Depth / 2), this.Level + 1, this, Root);
                    ChildNodes[5] = new Partition(new Cube(this.Area.X + this.Area.Width / 2, this.Area.Y + this.Area.Height / 2, this.Area.Z, this.Area.Width / 2, this.Area.Height / 2, this.Area.Depth / 2), this.Level + 1, this, Root);
                    ChildNodes[6] = new Partition(new Cube(this.Area.X, this.Area.Y + this.Area.Height / 2, this.Area.Z + this.Area.Depth / 2, this.Area.Width / 2, this.Area.Height / 2, this.Area.Depth / 2), this.Level + 1, this, Root);
                    ChildNodes[7] = new Partition(new Cube(this.Area.X + this.Area.Width / 2, this.Area.Y + this.Area.Height / 2, this.Area.Z + this.Area.Depth / 2, this.Area.Width / 2, this.Area.Height / 2, this.Area.Depth / 2), this.Level + 1, this, Root);

                    IOcttreeable[] tmpObjects = new IOcttreeable[objects.Count];
                    objects.CopyTo(tmpObjects);
                    foreach (IOcttreeable obj in tmpObjects)
                    {
                        obj.updateOcttree();
                    }
                }

                else throw new InvalidOperationException();
            }

            public void Merge()
            {
                this.ChildNodes = null;

                IOcttreeable[] tmpObjects = new IOcttreeable[objects.Count];
                objects.CopyTo(tmpObjects);
                foreach (IOcttreeable obj in tmpObjects)
                {
                    obj.updateOcttree();
                }
            }

            public void AddObject(IOcttreeable obj)
            {
                if (!this.objects.Contains(obj))
                    this.objects.Add(obj);
                if (this.objects.Count > Root.maxObjectsPerPartition && Root.maxLevel > this.Level)
                    this.Split();
            }

            public void RemoveObject(IOcttreeable obj)
            {
                if (this.objects.Contains(obj))
                    this.objects.Remove(obj);
            }

            internal List<Partition> getEmptyChildren()
            {
                List<Partition> emptyChildren = new List<Partition>();
                if (!(this.objects.Count == 0))
                    foreach (Partition p in ChildNodes)
                        emptyChildren.AddRange(p.getEmptyChildren());

                else emptyChildren.Add(this);
                return emptyChildren;
            }

            public List<IOcttreeable> GetObjects()
            {
                //Copy to a new list to prevent other classes from changing the list
                return this.objects.ToList<IOcttreeable>();
            }
        }

        #if DEBUG
        public override string ToString()
        {
            return Area.ToString() + "\r\n" + MainPartition.objects.Count.ToString();
        }
        #endif
    }
}
