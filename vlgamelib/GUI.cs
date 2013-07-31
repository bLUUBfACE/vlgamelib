using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameLibrary.Computing2D;
using GameLibrary.Other;

namespace GameLibrary.GUI
{
    public class GUIManager : IDisposable
    {
        public Vector2 Position
        {
            get { return this.innerPosition; }
            set { this.innerPosition = value; this.drawMatrix = Matrix.CreateTranslation(new Vector3(-this.Position, 0)); this.input.PositionOffset = value; }
        }

        internal Dictionary<string, SpriteFont> Fonts = new Dictionary<string,SpriteFont>();

        private readonly List<GUIElement> elements = new List<GUIElement>();
        private GUIElement selectedElement;
        private KeyboardControlMapManager keyboardControlMapManager = new KeyboardControlMapManager();
        private InputState input;

        private Vector2 innerPosition;
        private Matrix drawMatrix;
        internal SpriteBatch spriteBatch;
        private Effect opacityEffect;

        public GUIManager(GraphicsDevice graphics, InputState input)
        {
            this.spriteBatch = new SpriteBatch(graphics);
            this.input = input;
            this.Position = new Vector2(0);
        }
        public void AddGUIElement(GUIElement element)
        {
            if (!this.elements.Contains(element))
            {
                this.elements.Add(element);
                element.Select += this.anyElement_Select;
                if (element.IsKeyboardSelectable) this.keyboardControlMapManager.AddObject(element);
            }
        }
        public void LoadFont(string fontName, SpriteFont font)
        {
            this.Fonts.Add(fontName, font);
        }

        public void Update(float gameTimeSeconds)
        {
            this.input.UpdateMouse();
            this.input.UpdateKeyboard();

            if (this.input.IsNewKeyPressed(Keys.Up) || this.input.IsNewKeyPressed(Keys.Right) || this.input.IsNewKeyPressed(Keys.Down) || this.input.IsNewKeyPressed(Keys.Left))
            {
                this.selectElement(this.keyboardControlMapManager.MoveSelection(this.input.IsNewKeyPressed(Keys.Up) ? Direction.Top : this.input.IsNewKeyPressed(Keys.Right) ? Direction.Right : this.input.IsNewKeyPressed(Keys.Down) ? Direction.Bottom : Direction.Left));
            }

            foreach (GUIElement e in this.elements)
            {
                if(e.Enabled) e.Update(this.input, gameTimeSeconds);
            }
        }

        public void Prerender()
        {            
            foreach (GUIElement e in this.elements)
            {
                if (e is IPrerenderableElement && e.Visible) (e as IPrerenderableElement).Prerender(this.spriteBatch);
            }        
        }

        public void Draw()
        {
            this.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, new RasterizerState() { ScissorTestEnable = true }, this.opacityEffect, this.drawMatrix);
            foreach (GUIElement e in this.elements)
            {
                if (e.Visible)
                {
                    if (this.opacityEffect != null) this.opacityEffect.Parameters["alphaFactor"].SetValue(e.Opacity);
                    spriteBatch.GraphicsDevice.ScissorRectangle = (e.Bounds.Location - this.innerPosition).ToRectangle((int)e.Bounds.Width, (int)e.Bounds.Height)
                        .IntersectionArea(spriteBatch.GraphicsDevice.Viewport.Bounds);
                    e.Draw(this.spriteBatch);
                }
            }
            this.spriteBatch.End();
        }

        public void EnableOpacity(Effect opacityEffect)
        {
            try
            {
                opacityEffect.Parameters["alphaFactor"].SetValue(1);
                this.opacityEffect = opacityEffect;
            }
            catch
            {
                throw new Exception("Incorrect effect. Effect must provide alphaFactor-Parameter");
            }
        }

        private void anyElement_Select(GUIElement sender, SelectionEventArgs e)
        {
            if (e.IsSelected)
                this.selectElement(sender);
            else if(e.SelectionMode == SelectionMode.Mouse)
                this.selectElement(this.keyboardControlMapManager.CurrentSelection);
        }
        private void selectElement(GUIElement element)
        {
            if (this.selectedElement != element || (element != null && !element.Selected))
            {
                if (this.selectedElement != null && this.selectedElement.Selected)
                {
                    this.selectedElement.Select -= this.anyElement_Select;
                    this.selectedElement.Selected = false;
                    this.selectedElement.Select += this.anyElement_Select;
                }

                this.selectedElement = element;

                if (this.selectedElement != null && !this.selectedElement.Selected)
                {
                    this.selectedElement.Select -= this.anyElement_Select;
                    this.selectedElement.Selected = true;
                    this.selectedElement.Select += this.anyElement_Select;
                }
            }
        }

        private class KeyboardControlMapManager
        {
            public GUIElement CurrentSelection { get; private set; }

            private GUIElement[,] map = new GUIElement[0,0];
            private Point currentlySelectedPoint = new Point(0, 0);

            public void AddObject(GUIElement element)
            {
                if (!element.IsKeyboardSelectable
                    || element.KeyboardControlMapRectangle.X < 0 || element.KeyboardControlMapRectangle.Y < 0
                    || element.KeyboardControlMapRectangle.Width <= 0 || element.KeyboardControlMapRectangle.Height <= 0)
                    throw new InvalidOperationException();

                if (element.KeyboardControlMapRectangle.Right >= this.map.GetLength(0) || element.KeyboardControlMapRectangle.Bottom >= this.map.GetLength(1))
                {
                    GUIElement[,] newMap = new GUIElement[Math.Max(element.KeyboardControlMapRectangle.Right, this.map.GetLength(0)), Math.Max(element.KeyboardControlMapRectangle.Bottom, this.map.GetLength(1))];
                    for (int i = 0; i < this.map.GetLength(0); i++)
                    {
                        for (int a = 0; a < this.map.GetLength(1); a++)
                        {
                            newMap[i, a] = this.map[i, a];
                        }
                    }
                    this.map = newMap;
                }
                for (int i = element.KeyboardControlMapRectangle.X; i < element.KeyboardControlMapRectangle.Right; i++)
                {
                    for (int a = element.KeyboardControlMapRectangle.Y; a < element.KeyboardControlMapRectangle.Bottom; a++)
                    {
                        if (this.map[i, a] == null)
                            this.map[i, a] = element;
                        else throw new InvalidOperationException();
                    }
                }
            }

            public GUIElement MoveSelection(Direction direction)
            {
                if (this.map.LongLength != 0)
                {
                    Point oldCurrentlySelectedPoint = this.currentlySelectedPoint;
                    do
                    {
                        if (direction == Direction.Right || direction == Direction.Left)
                        {
                            currentlySelectedPoint.X += direction == Direction.Right ? 1 : -1;

                            if (currentlySelectedPoint.X < 0) currentlySelectedPoint.X = map.GetLength(0) - 1;
                            else if (currentlySelectedPoint.X == map.GetLength(0)) currentlySelectedPoint.X = 0;
                        }
                        else if (direction == Direction.Bottom || direction == Direction.Top)
                        {
                            currentlySelectedPoint.Y += direction == Direction.Bottom ? 1 : -1;

                            if (currentlySelectedPoint.Y < 0) currentlySelectedPoint.Y = map.GetLength(1) - 1;
                            else if (currentlySelectedPoint.Y == map.GetLength(1)) currentlySelectedPoint.Y = 0;
                        }
                        else throw new ArgumentException();
                    } while ((map[currentlySelectedPoint.X, currentlySelectedPoint.Y] == this.CurrentSelection || map[currentlySelectedPoint.X, currentlySelectedPoint.Y] == null || !map[currentlySelectedPoint.X, currentlySelectedPoint.Y].Enabled) && !this.currentlySelectedPoint.Equals(oldCurrentlySelectedPoint));

                    return this.CurrentSelection = map[this.currentlySelectedPoint.X, this.currentlySelectedPoint.Y];
                }
                //If the map is empty
                return null;
            }            
        }

        public void Dispose()
        {
            foreach (GUIElement e in this.elements)
            {
                if (e is IDisposable) (e as IDisposable).Dispose();
            }
            this.spriteBatch.Dispose();
        }
    }

    public abstract class GUIElement
    {
        public FloatRectangle Bounds {get; set;}

        public readonly GUIManager Root;

        public virtual bool Enabled 
        {
            get { return this.Visible ? this.innerEnabled : false; }
            set
            {
                if (this.innerEnabled != value)
                {
                    this.innerEnabled = value;
                    this.OnEnable(new EventArgs());
                    if (!value && this.innerSelected) this.Selected = false;
                }
            }
        }
        public virtual bool Selected
        {
            get { return this.innerSelected; }
            set
            {
                if (this.innerSelected != value)
                {
                    this.innerSelected = value;
                    this.OnSelect(new SelectionEventArgs(this.selectionMode, value));
                }
            }
        }
        public virtual bool Visible
        {
            get { return this.innerVisible; }
            set
            {
                if (this.innerVisible != value)
                {
                    this.innerVisible = value;
                    this.OnVisibleChanged(new EventArgs());
                    if (!value && this.innerSelected) this.Selected = false;
                }
            }

        }

        public virtual float Opacity 
        {
            get
            {
                return innerOpacity;
            }
            set
            {
                this.innerOpacity = value;
            }
        }

        public readonly bool IsKeyboardSelectable = false;        

        public event SelectionEventHandler Select;
        public event EventHandler Enable, VisibleChanged;

        public Rectangle KeyboardControlMapRectangle
        {
            get
            {
                if (this.IsKeyboardSelectable)
                    return innerKeyboardControlMapRectangle;
                throw new InvalidOperationException();
            } 
        }

        private readonly Rectangle innerKeyboardControlMapRectangle;        
        protected bool innerEnabled = true, innerSelected, innerVisible = true;
        protected float innerOpacity = 1;
        protected SelectionMode selectionMode = SelectionMode.Keyboard;

        internal GUIElement(FloatRectangle bounds, GUIManager root)
        {
            this.Bounds = bounds;
            this.Root = root;
        }
        internal GUIElement(FloatRectangle bounds, Rectangle keyboardControlMapRectangle, GUIManager root)
        {
            this.Bounds = bounds;
            this.Root = root;
            if (keyboardControlMapRectangle != Rectangle.Empty)
            {
                this.IsKeyboardSelectable = true;
                this.innerKeyboardControlMapRectangle = keyboardControlMapRectangle;
            }
        }

        public abstract void Update(InputState input, float gameTimeSeconds);
        public abstract void Draw(SpriteBatch spriteBatch);

        protected void OnEnable(EventArgs e)
        {
            if (this.Enable != null) this.Enable(this, e);
        }
        protected void OnSelect(SelectionEventArgs e)
        {
            if (this.Select != null) this.Select(this, e);
        }
        protected void OnVisibleChanged(EventArgs e)
        {
            if (this.VisibleChanged != null) this.VisibleChanged(this, e);
        }
    }

    public interface IPrerenderableElement : IDisposable
    {
        void Prerender(SpriteBatch spriteBatch);
    }

    #region SelectionEvent

    public class SelectionEventArgs : EventArgs
    {
        public readonly SelectionMode SelectionMode;
        public readonly bool IsSelected;


        public SelectionEventArgs(SelectionMode selectionMode, bool isSelected)
        {
            this.SelectionMode = selectionMode;
            this.IsSelected = isSelected;
        }
    }

    public delegate void SelectionEventHandler(GUIElement sender, SelectionEventArgs e);

    public enum SelectionMode
    {
        Mouse,
        Keyboard,
    }
    #endregion
}
