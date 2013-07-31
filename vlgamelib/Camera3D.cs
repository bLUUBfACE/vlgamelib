using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameLibrary.Other;

namespace GameLibrary.Computing3D
{
    public interface ICameraController
    {
        void Initialize(Camera camera);
        void Update(Camera camera, GameTime gameTime);
    }

    public interface ICameraConstraint
    {
        void Initialize(Camera camera);
        void Update(Camera camera, GameTime gameTime);
    }

    public class Camera
    {
        #region Members
        private Matrix projectionMatrix;
        private Matrix viewMatrix;
        private bool viewMatrixDirty;
        private Vector3 position;
        private Vector3 target;
        private List<ICameraConstraint> constraints = new List<ICameraConstraint>();
        private List<ICameraController> controllers = new List<ICameraController>();
        private InputState input;
        #endregion

        #region Propeties
        public Matrix ProjectionMatrix
        {
            get
            {
                return this.projectionMatrix;
            }
        }
        public Matrix ViewMatrix
        {
            get
            {
                if (viewMatrixDirty)
                {
                    UpdateViewMatrix();
                }

                return this.viewMatrix;
            }
        }

        public Vector3 Position
        {
            get
            {
                return position;
            }

            set
            {
                if (position != value)
                {
                    position = value;
                    viewMatrixDirty = true;
                }
            }
        }
        public Vector3 Target
        {
            get
            {
                return target;
            }
            set
            {
                if (target != value)
                {
                    target = value;
                    viewMatrixDirty = true;
                }
            }
        }

        public InputState Input
        {
            get
            {
                return input;
            }
        }
        #endregion

        public Camera(float fieldOfView, float aspectRatio, float nearPlane, float farPlane, Vector3 position, Vector3 target)
        {
            this.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlane, farPlane);

            this.position = position;
            this.target = target;
            this.viewMatrixDirty = true;
            input = new InputState();
        }

        public void AddController(ICameraController controller)
        {
            this.controllers.Add(controller);
            controller.Initialize(this);
        }
        public void AddConstraint(ICameraConstraint constraint)
        {
            this.constraints.Add(constraint);
            constraint.Initialize(this);
        }

        public void Update(GameTime gameTime)
        {
            input.UpdateKeyboard();
            input.UpdateMouse();
            input.UpdateTouch();

            foreach (ICameraController controller in this.controllers)
            {
                controller.Update(this, gameTime);
            }

            foreach (ICameraConstraint constraint in this.constraints)
            {
                constraint.Update(this, gameTime);
            }
        }

        private void UpdateViewMatrix()
        {
            this.viewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);

            viewMatrixDirty = false;
        }
    }
}
