#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.IO;
#endregion

namespace WallAll
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class WallAllScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont copyrightFont;
        SpriteFont instructionsFont;
        SpriteFont gameOverFont;
        SpriteFont titleFont;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Random random = new Random();

        Texture2D txNull;
        List<Wall> walls;
        Random rand;
        Player player;

        SoundEffect sndImpact;

        Vector2 gameOverPos;
        Vector2 titlePos;
        Vector2 scorePos;
        Vector2 scoreTitlePos;
        Vector2 pressStartPos;
        Vector2 copyRightPos;

        string msgGameOver = "GAME OVER";
        string msgTitle = "WALLCRASH";
        string msgCopyRight = "(c) COPYRIGHT 8BD 1984";
        string msgPressStart = "PRESS TO START";
        string msgHighScore = "HIGH SCORE:";
        string msgScore = "SCORE:";

        const int ScreenWidth = 480;
        const int ScreenHeight = 800;
        const int WallHeight = 15;
        const int PlayerSize = 11;
        const int PlayerSizeHalf = PlayerSize/2;
        const int GameOverDuration = 150;
        const int BlockGameStartDuration = 30;

        int score = 0;
        int highScore = 0;
        string strHighScore = "0";
        int wallTicks = 0;
        int gameOverTicks = 0;

        GameState gameState = GameState.Title;

        #endregion

        #region Initialization
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public WallAllScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            EnabledGestures = GestureType.Tap | GestureType.FreeDrag;
            IsSerializable = true;
        }
        
        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent(GraphicsDevice graphicsDevice)
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            copyrightFont = content.Load<SpriteFont>("Copyright");
            instructionsFont = content.Load<SpriteFont>("Instructions");
            titleFont = content.Load<SpriteFont>("Title");
            gameOverFont = content.Load<SpriteFont>("GameOver");

            txNull = new Texture2D(graphicsDevice, 1, 1);
            txNull.SetData<Color>(new Color[] { Color.White });

            player = new Player(txNull);

            sndImpact = content.Load<SoundEffect>("ouch");

            rand = new Random();
            walls = new List<Wall>();
            StartTitle();

            SetTextPositions();

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }

        private void SetTextPositions()
        {
            int widthTitle = GetFontWidth(titleFont, msgTitle);
            int widthCopyright = GetFontWidth(copyrightFont, msgCopyRight);
            int widthPressStart = GetFontWidth(instructionsFont, msgPressStart);
            int widthGameOver = GetFontWidth(gameOverFont, msgGameOver);
            
            titlePos = new Vector2(ScreenWidth / 2 - widthTitle/2, ScreenHeight/2 - 100);
            pressStartPos = new Vector2(ScreenWidth / 2 - 120, ScreenHeight - 300);
            copyRightPos = new Vector2(ScreenWidth / 2 - 150, ScreenHeight - 40);

            scorePos = new Vector2(195, 5);
            scoreTitlePos = new Vector2(5, 5);

            gameOverPos = new Vector2(ScreenWidth / 2 - widthGameOver / 2, ScreenHeight / 2 - 100);
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            
            // Add new walls
            if (wallTicks-- <= 0) AddWall();

            //Update position of the walls
            int updateSpeed = 0;
            if (gameOverTicks < 0 || gameOverTicks >= GameOverDuration) updateSpeed = 5 + score / 100;

            for (int i = walls.Count - 1; i >= 0; --i)
            {
                walls[i].Update(updateSpeed);
                if (walls[i].rect.Y > ScreenHeight + WallHeight)
                {
                    walls.RemoveAt(i);
                }
            }

            // Update player
            if (gameOverTicks < 0)
            {
                //Check out of bounds
                CheckOutOfBounds();

                bool playerCollision = player.Update(walls, lastTouch, PlayerSize);
                if (playerCollision)
                {
                    score++;
                }
                else
                {
                    //sndImpact.Play(); //TODO: REENABLE
                    StartGameOver();
                }
            }

            //Should we change state?
            if (gameOverTicks >= 0)
            {
                gameOverTicks++;
                if (gameOverTicks == GameOverDuration) StartTitle();
                if (isMouseClicked && gameOverTicks > BlockGameStartDuration) StartGame();
            }
        }


        private void CheckOutOfBounds()
        {
            if (lastTouch.X < PlayerSizeHalf) lastTouch.X = PlayerSizeHalf;
            if (lastTouch.Y < PlayerSizeHalf) lastTouch.Y = PlayerSizeHalf;

            if (lastTouch.X > (ScreenWidth - PlayerSize)) lastTouch.X = ScreenWidth - PlayerSize;
            if (lastTouch.Y > (ScreenHeight - PlayerSize)) lastTouch.Y = ScreenHeight - PlayerSize;
        }

        public Vector2 lastTouch = new Vector2(-1, -1);
        bool isMouseClicked = false;

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
#if WINDOWS
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new BackgroundScreen(), new MainMenuScreen());
            }

            //for (int i = 0; i < touchState.Count; ++i)
            {
                var mouseState = Mouse.GetState();
                //If finger touched screen for the first time
                //if (touchState[i].State == TouchLocationState.Pressed)
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    isMouseClicked = true;
                }
                //else if (touchState[i].State == TouchLocationState.Moved)
                else
                {
                    // TODO: This is a bug, it's very easy for the player to release the touch and move elsewhere
                    //lastTouch = touchState[i].Position;
                    lastTouch = new Vector2(mouseState.X, mouseState.Y);
                }
            }
#else
            PlayerIndex player;
            if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out player))
            {
                LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new BackgroundScreen(), new MainMenuScreen());
            }
            else
            {
                for (int i = 0; i < input.Gestures.Count; ++i)
                {
                    var gesture = input.Gestures[i];
                    if (gesture.GestureType == GestureType.Tap)
                    {
                        if (!isMouseClicked)
                        {
                            isMouseClicked = true;
                            lastTouch = input.Gestures[i].Position;
                        }
                    }
                    else
                    {
                        if (gesture.GestureType == GestureType.FreeDrag)
                        {
                            lastTouch.X += input.Gestures[i].Delta.X;
                            lastTouch.Y += input.Gestures[i].Delta.Y;
                        }                        
                    }
                }                
            }
#endif
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            
            ScreenManager.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            foreach (var wall in walls)
            {
                wall.Draw(spriteBatch);
            }

            if (gameOverTicks < 0 && gameOverTicks < GameOverDuration)
            {
                player.Draw(spriteBatch);
            }

            if (gameState == GameState.Title)
            {
                spriteBatch.DrawString(titleFont, msgTitle, titlePos, Color.Red);

                spriteBatch.DrawString(instructionsFont, msgPressStart, pressStartPos, Color.MintCream);
                spriteBatch.DrawString(copyrightFont, msgCopyRight, copyRightPos, Color.MintCream);

                // Draw highscore
                spriteBatch.DrawString(copyrightFont, msgHighScore, scoreTitlePos, Color.White);
                spriteBatch.DrawString(copyrightFont, strHighScore, scorePos, Color.White);
            }
            else if (gameState == GameState.GameOver)
            {
                spriteBatch.DrawString(gameOverFont, msgGameOver, gameOverPos, Color.Red);

                // Draw highscore
                spriteBatch.DrawString(copyrightFont, msgHighScore, scoreTitlePos, Color.White);
                spriteBatch.DrawString(copyrightFont, strHighScore, scorePos, Color.White);
            }
            else
            {
                // Draw current score
                spriteBatch.DrawString(copyrightFont, msgScore, scoreTitlePos, Color.White);
                spriteBatch.DrawString(copyrightFont, score.ToString(), scorePos, Color.White);
            }

            spriteBatch.End();
            
            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(1f - TransitionAlpha);
        }


        #endregion

        void StartTitle()
        {
            ClearActors();
            gameOverTicks = GameOverDuration;
            isMouseClicked = false;

            gameState = GameState.Title;
        }

        void StartGameOver()
        {
            gameOverTicks = 0;
            isMouseClicked = false;

            gameOverPos.Y = ScreenHeight / 2;

            gameState = GameState.GameOver;

            if (score > highScore)
            {
                highScore = score;
                strHighScore = highScore.ToString();
            }
        }

        void StartGame()
        {
            ClearActors();
            gameOverTicks = -1;
            score = 0;
            wallTicks = 0;

            gameState = GameState.Playing;
        }

        void AddWall()
        {
            int width = (int)((rand.NextDouble() / 2 + 0.1) * (float)ScreenWidth);
            int height = WallHeight;
            int x = rand.Next(ScreenWidth) - width / 2;
            int y = -WallHeight;

            var wall = new Wall(txNull, x, y, width, height);
            walls.Add(wall);

            wallTicks = (5 + rand.Next(15)) * 1000 / (1000 + score);
        }

        void ClearActors()
        {
            walls.Clear();
        }

        int GetFontWidth(SpriteFont font, string text)
        {
            return (int)(font.MeasureString(text).X);
        }

        public override void Serialize(Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(highScore);
            }

            base.Serialize(stream);
        }

        public override void Deserialize(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                highScore = reader.ReadInt32();
            }

            base.Deserialize(stream);
        }
    }

    enum GameState
    { 
        Title,
        GameOver,
        Playing
    }
}
