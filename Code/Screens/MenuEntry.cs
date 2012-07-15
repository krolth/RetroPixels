#region File Description
//-----------------------------------------------------------------------------
// MenuEntry.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace WallAll
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    class MenuEntry
    {
        #region Fields & Properties

        /// <summary>
        /// The text rendered for this entry.
        /// </summary>
        string text;

        public float SeparationScale { get; private set; }

        public SpriteFont Font { get; set; }

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        float selectionFade;

        /// <summary>
        /// The position at which the entry is drawn. This is set by the MenuScreen
        /// each frame in Update.
        /// </summary>
        Vector2 position;

        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public bool IsLink { get; set; }
        public bool IsTitle { get; set; }

        #endregion

        #region Events


        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;


        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }


        #endregion

        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry(string text, float scale)
        {
            this.text = text;
            this.SeparationScale = scale;

            Font = ScreenManager.FontNormal;
        }

        public MenuEntry(string text)
        {
            this.text = text;
            this.SeparationScale = 1f;

            Font = ScreenManager.FontNormal;
        }

        #region Update and Draw

        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif

            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }

        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false

#if WINDOWS_PHONE
            isSelected = false;
#endif

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;

            float pulsate = (float)Math.Sin(time * 6) + 1;

            //float scale = SeparationScale + pulsate * 0.05f * selectionFade;
            float scale = 1f + pulsate * 0.05f * selectionFade;
            if (IsLink || IsTitle)
            {
                isSelected = true;
                scale *= 1.2f;
            }

            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Color.Yellow : Color.White;

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            //SpriteFont font = UseSmallFont ? screenManager.FontSmall : screenManager.FontNormal;

            Vector2 origin = new Vector2(0, Font.LineSpacing / 2);

            spriteBatch.DrawString(Font, text, new Vector2(position.X + 2, position.Y + 2), Color.Black, 0,
                                   origin, scale, SpriteEffects.None, 0);

            spriteBatch.DrawString(Font, text, position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(MenuScreen screen)
        {
            System.Diagnostics.Debug.Assert(Font != null, "Font is null!");
            if (Font != null)
            {
                return (int)(Font.MeasureString(Text).Y * SeparationScale);
            }
            else
            {
                System.Diagnostics.Debug.Assert(ScreenManager.FontNormal != null, "Font Normal is null!");
                return (int)(ScreenManager.FontNormal.MeasureString(Text).Y * SeparationScale);
            }
            
        }

        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public virtual int GetWidth(MenuScreen screen)
        {
            System.Diagnostics.Debug.Assert(Font != null, "Font is null!");
            if (Font != null)
            {
                return (int)(Font.MeasureString(Text).X * SeparationScale);
            }
            else
            {
                System.Diagnostics.Debug.Assert(ScreenManager.FontNormal != null, "Font Normal is null!");
                return (int)(ScreenManager.FontNormal.MeasureString(Text).X * SeparationScale);
            }

            
        }

        #endregion
    }
}