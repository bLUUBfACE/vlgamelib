using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLibrary.OldStuff
{
//    [Serializable]
//    public class GameObjectData : NewIQuadtreeable
//    {
//        public readonly string Name, Tag;

//        public readonly Polygon Polygon;
//        public readonly List<NewQuadtree.Partition> QuadtreePartitions;
//        public readonly Rectangle QuadtreeRectangle;
//        public readonly Dictionary<string, object> Data;

//        public GameObjectData(string Name, string Tag, Polygon Polygon, List<NewQuadtree.Partition> QuadtreePartitions, Rectangle QuadtreeRectangle, Dictionary<string, object> Data)
//        {
//            this.Name = Name; this.Tag = Tag;
//            this.Polygon = Polygon; this.QuadtreePartitions = QuadtreePartitions; this.QuadtreeRectangle = QuadtreeRectangle;
//            this.Data = Data;
//        }

//        public override string ToString()
//        {
//            return Name;
//        }

//        #region IQuadtreeable Member

//        public List<NewQuadtree.Partition> getQuadtreePartitions()
//        {
//            return QuadtreePartitions;
//        }

//        public void updateQuadtree()
//        {

//        }

//        public FloatRectangle getRectangle()
//        {
//            return QuadtreeRectangle;
//        }

//        public void removeFromQuadtree()
//        {

//        }

//        #endregion
//    }

//    [Serializable]
//    public class GameData
//    {
//        public readonly int RENDERWIDTH, RENDERHEIGHT;

//        public readonly NewQuadtree Tree;
//        public readonly List<GameObjectData> GameObjects;

//        public readonly string GameName, SnapshotInfo;

//        public GameData(int RENDERWIDTH, int RENDERHEIGHT, NewQuadtree Tree, List<GameObjectData> GameObjects, string GameName, string SnapshotInfo)
//        {
//            this.RENDERWIDTH = RENDERWIDTH; this.RENDERHEIGHT = RENDERHEIGHT;
//            this.Tree = Tree; this.GameObjects = GameObjects;
//            this.GameName = GameName; this.SnapshotInfo = SnapshotInfo;
//        }

//        public GameData()
//        { RENDERWIDTH = 0; RENDERHEIGHT = 0; Tree = new NewQuadtree(new Rectangle(0, 0, 0, 0)); this.GameObjects = new List<GameObjectData>(); this.GameName = ""; this.SnapshotInfo = ""; }
//    }

//    public interface IGameObject
//    {
//        GameObjectData getGameObject();
//    }

//    public static partial class ExtensionMethods
//    {
//        //public static Quadtree getTreeWithGameObjects(this Quadtree oldTree)
//        //{
//        //    Quadtree newTree = new Quadtree(oldTree.MainArea.Area);
//        //    newTree.MainArea = oldTree.MainArea.getPartitionWithGameObjects(null) as Quadtree.MainPartition;
//        //    return newTree;
//        //}

//        //public static Quadtree.Partition getPartitionWithGameObjects(this Quadtree.Partition oldPartition, Quadtree.Partition parent)
//        //{
//        //    Quadtree.Partition newPartition;
//        //    if (oldPartition is Quadtree.MainPartition)
//        //        newPartition = new Quadtree.MainPartition(oldPartition.Area);
//        //    else newPartition = new Quadtree.Partition(oldPartition.Area, oldPartition.Level, parent);
//        //    foreach (IQuadtreeable iq in oldPartition.getAllOwnObjects<IGameObject>())
//        //    {
//        //        newPartition.AddObject((iq as IGameObject).getGameObject());
//        //    }
//        //    if (oldPartition.Partitions != null)
//        //    {
//        //        newPartition.Divide();
//        //        for (int i = 0; i < newPartition.Partitions.Length; i++)
//        //        {
//        //            newPartition.Partitions[i] = oldPartition.Partitions[i].getPartitionWithGameObjects(newPartition);
//        //        }
//        //    }
//        //    return newPartition;
//        //}
//    }
}
