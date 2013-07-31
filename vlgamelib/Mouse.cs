using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameLibrary.Other
{
    public delegate void DrawMouse(List<Texture2D> mouseTextures, int selectedMouseTexture, Vector2 position, SpriteBatch spriteBatch);
    public delegate void UpdateMouse(ref Vector2 position);

    public class PositionMouse
    {
        private Vector2 position;
        private List<Texture2D> mouseTextures;
        private int selectedMouseTexture = 0;
        private DrawMouse drawMouseMethod;
        private UpdateMouse updateMouseMethod;
        private bool visible = true;
        private Color color;
        private Vector2 screenCoordinates;
        private bool paused = false;
        private InputState input;

        public PositionMouse(Vector2 position, List<Texture2D> mouseTextures, Color color, Vector2 screenCoordinates, int selectedMouseTexture = 0)
        {
            this.position = position; this.mouseTextures = mouseTextures; this.selectedMouseTexture = selectedMouseTexture; this.color = color; this.screenCoordinates = screenCoordinates;
            drawMouseMethod = drawMouse;
            updateMouseMethod = updateMouse;
            Mouse.SetPosition((int)screenCoordinates.X / 2, (int)screenCoordinates.Y / 2);
            input = new InputState();
        }

        public PositionMouse(Vector2 position, List<Texture2D> mouseTextures, DrawMouse drawMouse, UpdateMouse updateMouse, Color color, Vector2 screenCoordinates, int selectedMouseTexture = 0)
        {
            this.position = position; this.mouseTextures = mouseTextures; this.selectedMouseTexture = selectedMouseTexture; this.color = color; this.screenCoordinates = screenCoordinates;
            this.updateMouseMethod = updateMouse; this.drawMouseMethod = drawMouse;
            Mouse.SetPosition((int)screenCoordinates.X / 2, (int)screenCoordinates.Y / 2);
            input = new InputState();
        }

        private void drawMouse(List<Texture2D> mouseTextures, int selectedMouseTexture, Vector2 position, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(mouseTextures[selectedMouseTexture], position, color);
        }

        private void updateMouse(ref Vector2 position)
        {
            input.UpdateKeyboard();
            if (!paused)
            {
                MouseState m = Mouse.GetState();
                position += new Vector2(m.X, m.Y) - new Vector2(screenCoordinates.X / 2, screenCoordinates.Y / 2);
                
                if (position.X < 0)
                {
                    position.X = 0;
                }
                if (position.Y < 0)
                {
                    position.Y = 0;
                }
                if (position.X > screenCoordinates.X - mouseTextures[selectedMouseTexture].Width)
                {
                    position.X = screenCoordinates.X - mouseTextures[selectedMouseTexture].Width;
                }
                if (position.Y > screenCoordinates.Y - mouseTextures[selectedMouseTexture].Height)
                {
                    position.Y = screenCoordinates.Y - mouseTextures[selectedMouseTexture].Height;
                }

                Mouse.SetPosition((int)screenCoordinates.X / 2, (int)screenCoordinates.Y / 2);
                if (input.IsNewKeyPressed(Keys.X))
                    paused = true;
            }

            else
                if (input.IsNewKeyPressed(Keys.X))
                    paused = false;
        }

        public void Update()
        {
            updateMouseMethod(ref position);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
                drawMouseMethod(mouseTextures, selectedMouseTexture, position, spriteBatch);
        }

        public void SetVisibility(bool visible)
        {
            this.visible = visible;
        }

        public void SetSelectedMouseTexture(int selectedMouseTexture)
        {
            this.selectedMouseTexture = selectedMouseTexture;
        }

        public MouseState GetMouseState()
        {
            MouseState a = Mouse.GetState();
            MouseState m = new MouseState((int)position.X, (int)position.Y, a.ScrollWheelValue, a.LeftButton, a.MiddleButton, a.RightButton, a.XButton1, a.XButton2);
            return m;
        }
    }
}
