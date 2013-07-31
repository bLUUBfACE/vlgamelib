using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameLibrary.Computing2D;
using GameLibrary.Other;

namespace GameLibrary.GUI
{
    public class Button : GUIElement
    {
        public bool Pressed
        {
            get { return this.innerPressed; }
            set
            {
                if (value != innerPressed)
                {
                    this.innerPressed = value;
                    if (value) this.OnPress(new EventArgs());
                }
            }
        }
        public event EventHandler Click;
        public event EventHandler Press;

        public string Text { get; set; }
        public string FontName
        {
            set
            {
                if (this.Root.Fonts.ContainsKey(value)) this.font = this.Root.Fonts[value];
            }
        }

        public virtual FloatRectangle TextureDrawingBounds
        {
            get { return this.textureDrawingBounds != FloatRectangle.Empty ? this.textureDrawingBounds : this.Bounds; }
            set
            {
                if (!this.Bounds.Contains(value)) throw new ArgumentException();
                this.textureDrawingBounds = value;
            }
        }
        public virtual Vector2 TextPosition
        {
            get
            {
                return useAutomaticTextPosition ?
                    new Vector2(this.Bounds.Width / 2, this.Bounds.Height / 2) + this.Bounds.Location - this.font.MeasureString(this.Text) / 2 :
                    this.innerTextPosition;
            }
            set
            {
                this.innerTextPosition = value;
                this.useAutomaticTextPosition = false;
            }
        }         
        
        protected SpriteFont font;
        protected Texture2D textureReleased, texturePressed, textureSelected;
        protected Color textColorReleased, textColorPressed, textColorSelected;
        protected Vector2 innerTextPosition;
        protected bool useAutomaticTextPosition = true;
        protected FloatRectangle textureDrawingBounds = FloatRectangle.Empty;

        private bool innerPressed;

        public Button(FloatRectangle bounds, Rectangle keyboardControlMapRectangle, GUIManager root, Texture2D textureReleased, Texture2D texturePressed = null, Texture2D textureSelected = null)
            : base(bounds, keyboardControlMapRectangle, root)
        {
            this.textureReleased = textureReleased;
            this.texturePressed = texturePressed != null ? texturePressed : textureReleased;
            this.textureSelected = textureSelected != null ? textureSelected : textureReleased;
            this.textColorReleased = this.textColorPressed = this.textColorSelected = Color.Black;
        }

        public Button(FloatRectangle bounds, Rectangle keyboardControlMapRectangle, GUIManager root, Texture2D textureReleased, Color textColorReleased, Color? textColorPressed = null, Color? textColorSelected = null)
            : base(bounds, keyboardControlMapRectangle, root)
        {
            this.textureReleased = this.texturePressed = this.textureSelected = textureReleased;
            this.textColorReleased = textColorReleased;
            this.textColorPressed = textColorPressed != null ? (Color)textColorPressed : textColorReleased;
            this.textColorSelected = textColorSelected != null ? (Color)textColorSelected : textColorReleased;
        }

        public Button(FloatRectangle bounds, Rectangle keyboardControlMapRectangle, GUIManager root, Texture2D textureReleased, Texture2D texturePressed = null, Texture2D textureSelected = null,
            Color? textColorReleased = null, Color? textColorPressed = null, Color? textColorSelected = null)
            : base(bounds, keyboardControlMapRectangle, root)
        {
            this.textureReleased = textureReleased;
            this.texturePressed = texturePressed != null ? texturePressed : textureReleased;
            this.textureSelected = textureSelected != null ? textureSelected : textureReleased;
            this.textColorReleased = textColorReleased != null ? (Color)textColorReleased : Color.Black;
            this.textColorPressed = textColorPressed != null ? (Color)textColorPressed : this.textColorReleased;
            this.textColorSelected = textColorSelected != null ? (Color)textColorSelected : this.textColorReleased;
        }        

        public override void Update(InputState input, float gameTimeSecons)
        {
            if (this.Pressed &&
                ((!input.IsMouseButtonPressed(MouseButtons.LeftButton) && this.Bounds.Contains(input.MousePosition)) || (this.selectionMode == SelectionMode.Keyboard && !input.IsKeyPressed(Keys.Enter))))
                this.OnClick(new EventArgs());

            if (this.Bounds.Contains(input.MousePosition))
            {
                this.selectionMode = SelectionMode.Mouse;
                this.Selected = true;
            }
            else if (this.selectionMode == SelectionMode.Mouse)
            {
                this.Selected = false;
                this.selectionMode = SelectionMode.Keyboard;
            }

            this.Pressed = this.Selected
                && ((this.selectionMode == SelectionMode.Mouse && input.IsMouseButtonPressed(MouseButtons.LeftButton))
                || (this.selectionMode == SelectionMode.Keyboard && input.IsKeyPressed(Keys.Enter)));
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if(textureReleased != null) spriteBatch.Draw(this.Pressed ? texturePressed : this.Selected ? textureSelected : textureReleased, this.TextureDrawingBounds.ToXnaRectangle(), Color.White);
            if(this.Text != null)
            {
                spriteBatch.DrawString(this.font, this.Text,
                    this.TextPosition,
                    this.Pressed ? textColorPressed : this.Selected ? textColorSelected : textColorReleased);
            }
        }

        protected void OnClick(EventArgs e)
        {
            if (this.Click != null) this.Click(this, e);
        }
        protected void OnPress(EventArgs e)
        {
            if (this.Press != null) this.Press(this, e);
        }
    }
    public class SizeAnimatedButton : Button, IPrerenderableElement
    {
        public float AnimationSpeed = 30;
        public override FloatRectangle TextureDrawingBounds
        {
            get { return this.textureDrawingBounds != FloatRectangle.Empty ? this.textureDrawingBounds : this.originalBounds; }
            set
            {
                if (!this.originalBounds.Contains(value)) throw new ArgumentException();
                this.textureDrawingBounds = value;
            }
        }

        private FloatRectangle originalBounds;
        private float releasedScale, pressedScale;
        private float targetScale, oldTargetScale, activeScale;

        RenderTarget2D renderTarget;

        public SizeAnimatedButton(FloatRectangle bounds, Rectangle keyboardControlMapRectangle, GUIManager root, float releasedScale, float pressedScale, Texture2D textureReleased, Texture2D texturePressed = null, Texture2D textureSelected = null)
            : base(bounds, keyboardControlMapRectangle, root, textureReleased, texturePressed, textureSelected)
        {
            Initialize(releasedScale, pressedScale);
        }
        public SizeAnimatedButton(FloatRectangle bounds, Rectangle keyboardControlMapRectangle, GUIManager root, float releasedScale, float pressedScale, Texture2D textureReleased, Color textColorReleased, Color? textColorPressed = null, Color? textColorSelected = null)
            : base(bounds, keyboardControlMapRectangle, root, textureReleased, textColorReleased, textColorPressed, textColorSelected)
        {
            Initialize(releasedScale, pressedScale);
        }
        public SizeAnimatedButton(FloatRectangle bounds, Rectangle keyboardControlMapRectangle, GUIManager root, float releasedScale, float pressedScale, Texture2D textureReleased, Texture2D texturePressed = null, Texture2D textureSelected = null,
            Color? textColorReleased = null, Color? textColorPressed = null, Color? textColorSelected = null)
            : base(bounds, keyboardControlMapRectangle, root, textureReleased, texturePressed, textureSelected, textColorReleased, textColorPressed, textColorSelected)
        {
            Initialize(releasedScale, pressedScale);
        }
        private void Initialize(float releasedScale, float pressedScale)
        {
            this.originalBounds = this.Bounds;
            this.releasedScale = releasedScale;
            this.pressedScale = pressedScale;
            this.activeScale = releasedScale;
            this.renderTarget = new RenderTarget2D(Root.spriteBatch.GraphicsDevice, (int)this.originalBounds.Width, (int)this.originalBounds.Height);
        }

        public override void Update(InputState input, float gameTimeSeconds)
        {
            this.Bounds = originalBounds;
            base.Update(input, gameTimeSeconds);

            float targetScaleLastFrame = targetScale;
            targetScale = this.Pressed ? pressedScale : this.Selected ? 1 : releasedScale;
            oldTargetScale = targetScale != targetScaleLastFrame ? targetScaleLastFrame : oldTargetScale;
            float newScale = activeScale + (targetScale - oldTargetScale) * gameTimeSeconds * this.AnimationSpeed;
            activeScale = oldTargetScale < targetScale ? Math.Min(newScale, targetScale) : Math.Max(newScale, targetScale);
            this.Bounds = new FloatRectangle(originalBounds.Location + new Vector2(originalBounds.Width, originalBounds.Height) * (1 - activeScale) * 0.5f, originalBounds.Width * activeScale, originalBounds.Height * activeScale);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.renderTarget, this.Bounds.ToXnaRectangle(), Color.White);
        }

        public void Prerender(SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.SetRenderTarget(this.renderTarget);
            spriteBatch.GraphicsDevice.Clear(new Color(Vector4.Zero));
            spriteBatch.Begin();
            if (textureReleased != null) spriteBatch.Draw(this.Pressed ? texturePressed : this.Selected ? textureSelected : textureReleased, new Rectangle((int)(this.TextureDrawingBounds.X - this.originalBounds.X), (int)(this.TextureDrawingBounds.Y - this.originalBounds.Y), (int)this.TextureDrawingBounds.Width, (int)this.TextureDrawingBounds.Height), Color.White);
            if (this.Text != null)
            {
                spriteBatch.DrawString(this.font, this.Text,
                    this.TextPosition - this.originalBounds.Location,
                    this.Pressed ? textColorPressed : this.Selected ? textColorSelected : textColorReleased);// 0, Vector2.Zero, activeScale, SpriteEffects.None, 0);
            }
            spriteBatch.End();
            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        public void Dispose()
        {
            this.renderTarget.Dispose();
        }
    }

    public class Label : GUIElement
    {
        public string Text { get; set; }
        public string FontName
        {
            set
            {
                if (this.Root.Fonts.ContainsKey(value)) this.font = this.Root.Fonts[value];
                else throw new ArgumentException();
            }
        }
        public Color TextColor = Color.Black;    
        public Texture2D BackgroundTexture;

        protected SpriteFont font;

        public Label(FloatRectangle bounds, GUIManager root) : base(bounds, root) { }
        public Label(FloatRectangle bounds, GUIManager root, string text, string fontName)
            : base(bounds, root)
        {
            if (text != default(string))
            {
                this.Text = text;
                this.FontName = fontName;
            }
        }
        public Label(FloatRectangle bounds, GUIManager root, string text, string fontName, Color textColor, Texture2D backgroundTexture)
            : base(bounds, root)
        {
            if (text != default(string))
            {
                this.Text = text;
                this.FontName = fontName;
                this.TextColor = textColor;
            }
            this.BackgroundTexture = backgroundTexture;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.BackgroundTexture != default(Texture2D)) spriteBatch.Draw(BackgroundTexture, this.Bounds.ToXnaRectangle(), Color.White);
            if (this.Text != default(string))
            {
                spriteBatch.DrawString(this.font, this.Text,
                    new Vector2(this.Bounds.X + this.Bounds.Width / 2, this.Bounds.Y + this.Bounds.Height / 2) - this.font.MeasureString(this.Text) / 2, this.TextColor);
            }
        }

        public override void Update(InputState input, float gameTimeSeconds) { }
    }
    public class GroupBox : GUIElement
    {
        public GUIElement[] Objects
        {
            get
            {
                return this.objects.ToArray();
            }
        }

        public override float Opacity
        {
            get
            {
                return base.Opacity;
            }
            set
            {
                base.Opacity = value;
                this.GroupBox_OpacityChanged();
            }
        }

        private List<GUIElement> objects;

        public GroupBox(GUIManager root, List<GUIElement> objects, FloatRectangle bounds = default(FloatRectangle))
            :base(bounds, root)
        {
            this.Enable += GroupBox_EnabledChanged;
            this.VisibleChanged += GroupBox_VisibleChanged;
            this.objects = new List<GUIElement>();
            foreach (GUIElement element in objects)
            {
                this.AddObject(element);
            }
        }

        public void AddObject(GUIElement element)
        {
            if (element.Root != this.Root || this.objects.Contains(element)) throw new ArgumentException();
            this.objects.Add(element);
            element.Enabled = this.innerEnabled;
            element.Visible = this.Visible;
            element.Opacity = this.Opacity;
        }
        public void RemoveObject(GUIElement element)
        {
            if (!this.objects.Contains(element)) throw new ArgumentException();
            this.objects.Remove(element);
        }

        public override void Update(InputState input, float gameTimeSeconds) { }
        public override void Draw(SpriteBatch spriteBatch) { }

        private void GroupBox_EnabledChanged(object sender, EventArgs e)
        {
            foreach (GUIElement element in this.objects)
            {
                element.Enabled = this.Enabled;
            }
        }
        private void GroupBox_VisibleChanged(object sender, EventArgs e)
        {
            foreach (GUIElement element in this.objects)
            {
                element.Visible = this.Visible;
            }
        }
        private void GroupBox_OpacityChanged()
        {
            foreach (GUIElement element in this.objects)
            {
                element.Opacity = this.Opacity;
            }
        }
    }
}
