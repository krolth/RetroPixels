using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WallAll
{
    class SplashScreen : MenuScreen
    {
        ContentManager content;
        Texture2D txLogo;
        Rectangle screenRect;

        double elapsedMilliseconds = 0;
        double ReceiveInputTime = 500;
        double MaxTimeScreen = 2000;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SplashScreen()
            : base(String.Empty)
        {
        }

        public override void LoadContent(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice)
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            txLogo = content.Load<Texture2D>("8BigDogs");
            screenRect = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            elapsedMilliseconds += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (elapsedMilliseconds >= MaxTimeScreen)
            {
                GotoNextScreen();
            }
        }

        private void GotoNextScreen()
        {
            LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new BackgroundScreen(), new MainMenuScreen());
        }

        /// <summary>
        ///Go to main manu on cancel
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }

        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
#if WINDOWS_PHONE
            PlayerIndex player;
            if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out player))
            {
                ScreenManager.Game.Exit();
            }
            else
            {
                if (elapsedMilliseconds < ReceiveInputTime)
                {
                    //Show the splash for a little while
                    return;
                }

                for (int i = 0; i < input.Gestures.Count; ++i)
                {
                    if (input.Gestures[i].GestureType == GestureType.Tap)
                    {
                        GotoNextScreen();
                    }
                }
            }
#else
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                ScreenManager.Game.Exit();
            }

            if (elapsedMilliseconds < ReceiveInputTime)
            {
                //Show the splash for a little while
                return;
            }
            var keys = Keyboard.GetState().GetPressedKeys();

            bool noKeysPressed = (keys.Length == 1 && keys[0] == Keys.None) || keys.Length == 0;
            if (!noKeysPressed)
            {
                GotoNextScreen();
            }
#endif
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.Black);

            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(
                txLogo,
                screenRect,
                Color.White);
            ScreenManager.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
