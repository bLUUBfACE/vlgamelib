using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace GameLibrary.Other
{
    public enum MouseButtons
    {
        LeftButton,
        RightButton,
        MiddleButton
    }

    public class InputState
    {
        #region Mouse members
        public Vector2 MousePosition { get { return new Vector2(mouseState.X, mouseState.Y) + this.PositionOffset; } }
        public Vector2 PositionOffset = Vector2.Zero;

        private MouseState mouseState = new MouseState();
        private MouseState mouseStateLastFrame = new MouseState();
        #endregion

        #region Keyboard members
        private List<Keys> inputKeys = new List<Keys>();
        private List<Keys> inputKeysLastFrame = new List<Keys>();
        #endregion

        #region Touch members
        public List<Vector2> FingerPositions = new List<Vector2>();
        public List<Vector2> FingerMovement = new List<Vector2>();
        private TouchCollection touchCollection;
        private TouchCollection touchCollectionLastFrame = new TouchCollection();
        #endregion

        #region process Keyboard
        public void UpdateKeyboard()
        {
            inputKeysLastFrame.Clear();
            foreach (Keys k in inputKeys)
            {
                inputKeysLastFrame.Add(k);
            }

            inputKeys.Clear();
            inputKeys = Keyboard.GetState().GetPressedKeys().ToList<Keys>();
        }
        public bool IsNewKeyPressed(Keys k)
        {
            return inputKeys.Contains(k) && !inputKeysLastFrame.Contains(k);
        }
        public bool IsKeyPressed(Keys k)
        {
            return inputKeys.Contains(k);
        }
        public bool IsCombinationPressed(Keys k1, Keys k2)
        {
            return inputKeys.Contains(k1) && IsNewKeyPressed(k2);
        }
        public bool IsCombinationPressed(List<Keys> keys)
        {
            for (int i = 0; i < keys.Count - 1; i++)
            {
                if (!inputKeys.Contains(keys[i]))
                    return false;
            }
            return IsNewKeyPressed(keys[keys.Count - 1]);
        }
        #endregion

        #region process Mouse
        public void UpdateMouse()
        {
            mouseStateLastFrame = mouseState;
            mouseState = Mouse.GetState();
        }
        public bool IsNewMouseButtonPressed(MouseButtons b)
        {
            return b == MouseButtons.LeftButton ? mouseState.LeftButton == ButtonState.Pressed && mouseStateLastFrame.LeftButton == ButtonState.Released :
                b == MouseButtons.RightButton ? mouseState.RightButton == ButtonState.Pressed && mouseStateLastFrame.RightButton == ButtonState.Released :
                mouseState.MiddleButton == ButtonState.Pressed && mouseStateLastFrame.MiddleButton == ButtonState.Released;
        }
        public bool IsMouseButtonPressed(MouseButtons b)
        {
            return b == MouseButtons.LeftButton ? mouseState.LeftButton == ButtonState.Pressed:
                b == MouseButtons.RightButton ? mouseState.RightButton == ButtonState.Pressed:
                mouseState.MiddleButton == ButtonState.Pressed;        
        }
        public bool IsNewMouseButtonReleased(MouseButtons b)
        {
            return b == MouseButtons.LeftButton ? mouseState.LeftButton == ButtonState.Released && mouseStateLastFrame.LeftButton == ButtonState.Pressed :
                b == MouseButtons.RightButton ? mouseState.RightButton == ButtonState.Released && mouseStateLastFrame.RightButton == ButtonState.Pressed :
                mouseState.MiddleButton == ButtonState.Released && mouseStateLastFrame.MiddleButton == ButtonState.Pressed;
        }
        #endregion

        #region process Touch
        public void UpdateTouch()
        {
            touchCollectionLastFrame = touchCollection;
            touchCollection = TouchPanel.GetState();
            FingerPositions.Clear(); FingerMovement.Clear();

            foreach (TouchLocation touchLocation in touchCollection)
            {
                if (touchLocation.State == TouchLocationState.Moved || touchLocation.State == TouchLocationState.Pressed)
                    FingerPositions.Add(touchLocation.Position);

                TouchLocation prevLoc;
                bool moved = touchLocation.TryGetPreviousLocation(out prevLoc);
                FingerMovement.Add(moved ? Vector2.Zero : touchLocation.Position - prevLoc.Position);
            }
        }

        public bool IsNewFingerPressed()
        {
            return touchCollection.Count > touchCollectionLastFrame.Count;
        }
        #endregion
    }
}
