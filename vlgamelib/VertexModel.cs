using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLibrary.Graphics3D
{
    public class VertexModel
    {
        private List<VertexPositionColor> baseVertices;
        private Vector3 position;
        private float rotationX, rotationY, rotationZ;
        private Matrix transformMatrix;
        private List<VertexPositionColor> transformedVertices;
        private List<uint> indicies;

        public List<VertexPositionColor> BaseVertices
        {
            get { return baseVertices; }
            set { baseVertices = value; UpdateModel(); }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; UpdateModel(); }
        }

        public float RotationX
        {
            get { return rotationX; }
            set { rotationX = value; UpdateModel(); }
        }

        public float RotationY
        {
            get { return rotationY; }
            set { rotationY = value; UpdateModel(); }
        }

        public float RotationZ
        {
            get { return rotationZ; }
            set { rotationZ = value; UpdateModel(); }
        }

        public VertexModel(List<VertexPositionColor> baseVertices, Vector3 position, float rotationX = 0, float rotationY = 0, float rotationZ = 0)
        {
            this.baseVertices = baseVertices; this.position = position; this.rotationX = rotationX; this.rotationY = rotationY; this.rotationZ = rotationZ;
            transformedVertices = new List<VertexPositionColor>();
            UpdateModel();
        }

        private void UpdateModel()
        {
            transformedVertices.Clear();
            Matrix transMat = Matrix.CreateTranslation(position);
            transformMatrix = transMat /** Matrix.CreateRotationX(rotationX) * Matrix.CreateRotationY(rotationY) * Matrix.CreateRotationZ(rotationZ) * transMat*/;
            foreach (VertexPositionColor v in baseVertices)
            {
                transformedVertices.Add(new VertexPositionColor(Vector3.Transform(v.Position, transformMatrix), v.Color));
            }
        }

        public VertexPositionColor[] GetTransformedModel()
        {
            return transformedVertices.ToArray();
        }

        public void SetIndicies(List<uint> indicies)
        {
            this.indicies = indicies;
        }

        public uint[] GetModelIndicies()
        {
            if (indicies != null)
                return indicies.ToArray();
            else throw new NullReferenceException();
        }

        public short[] GetShortModelIndicies()
        {
            if (indicies != null)
            {
                short[] sindicies = new short[indicies.Count];
                for (int i = 0; i < indicies.Count; i++)
                    sindicies[i] = (short)indicies[i];
                return sindicies;
            }
            else throw new NullReferenceException();
        }
    }
}
