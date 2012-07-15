#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
#endregion

namespace WallAll
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        ContentManager content;
        Song songBackground;
        bool isMusicTemporarilyOff = true; //TODO: REENABLE

        #region Initialization
        
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Retro Pixels")
        {
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("Play WallCrash !") { Font = ScreenManager.FontSmall };
            MenuEntry optionsMenuEntry = new MenuEntry("About") { Font = ScreenManager.FontSmall };

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
        }

        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            songBackground = content.Load<Song>("Songzille1");

            if (MediaPlayer.GameHasControl && !isMusicTemporarilyOff)
            {
                try
                {
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Play(songBackground);
                }
                catch (UnauthorizedAccessException)
                {
                    // WP7 is connected to the computer, disable music since we cannot play
                    isMusicTemporarilyOff = true;
                }
            }
        }

        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new WallAllScreen());
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new AboutMenuScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, we exit the game.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
