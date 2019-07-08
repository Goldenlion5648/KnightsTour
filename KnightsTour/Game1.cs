﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Linq;
using System;

namespace KnightsTour
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState kb, oldkb;
        MouseState mouse, oldmouse;
        Point mousePos;
        SpriteFont testFont;
        Random rand = new Random(20);



        //Character player, playerCopyTopAndBottom, playerCopyLeftAndRight;
        Character[,] board;
        Character[,] tokens;
        Character knight;
        int playerWidth = 64, playerHeight = 64;

        List<int[]> validSpaces = new List<int[]>(8);
        List<int[]> computerValidSpacesGlobal = new List<int[]>(8);

        int boardXDim = 8, boardYDim = 8;

        int numMovesMade = 0;

        int screenWidth = 800;
        int screenHeight = 800;


        int knightX = 0;
        int knightY = 0;

        int gameClock = 0;

        bool hasDoneOneTimeCode = false;
        bool didWork = false;

        bool isPressingKey = false;
        bool hasWon = false;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.graphics.PreferredBackBufferHeight = screenHeight;
            this.graphics.PreferredBackBufferWidth = screenWidth;
            this.IsMouseVisible = true;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            testFont = Content.Load<SpriteFont>("testFont");

            board = new Character[boardYDim, boardXDim];
            tokens = new Character[boardYDim, boardXDim];

            knight = new Character(Content.Load<Texture2D>("chessKnight"),
                        new Rectangle(0, 0, (screenWidth / boardXDim), (screenHeight / boardYDim)));

            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    board[y, x] = new Character(Content.Load<Texture2D>("blankSquare"),
                        new Rectangle(x * (screenWidth / boardXDim), y * (screenHeight / boardYDim), (screenWidth / boardXDim), (screenHeight / boardYDim)));
                    tokens[y, x] = new Character(Content.Load<Texture2D>("whiteCircle"),
                        new Rectangle(x * (screenWidth / boardXDim), y * (screenHeight / boardYDim), (screenWidth / boardXDim), (screenHeight / boardYDim)));
                }
            }

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            kb = Keyboard.GetState();
            mouse = Mouse.GetState();
            mousePos.X = mouse.X;
            mousePos.Y = mouse.Y;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (hasDoneOneTimeCode == false)
            {
                int x = 0;
                int y = 0;
                knight.setPos(board[y, x].getRec());
                knightX = x;
                knightY = y;

                findValidSpaces();
                //computerFindValidSpaces(ref computerValidSpacesGlobal);
                hasDoneOneTimeCode = true;
            }

            userControls();

            checkWins();

            oldmouse = mouse;
            oldkb = kb;
            gameClock++;
            base.Update(gameTime);
        }

        public void userControls()
        {
            if (kb.IsKeyDown(Keys.R) && oldkb.IsKeyUp(Keys.R))
            {
                numMovesMade = 0;
                for (int y = 0; y < boardYDim; y++)
                {
                    for (int x = 0; x < boardXDim; x++)
                    {
                        board[y, x].moveNumber = 0;

                    }
                }
                findValidSpaces();
            }

            if (kb.IsKeyDown(Keys.U) && oldkb.IsKeyUp(Keys.U))
            {
                if (numMovesMade > 0)
                {
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (board[y, x].moveNumber == numMovesMade - 1)
                            {
                                board[knightY, knightX].moveNumber = 0;
                                numMovesMade--;
                                knight.setPos(board[y, x].getRec());
                                knightX = x;
                                knightY = y;
                                findValidSpaces();
                                x = boardXDim;
                                y = boardYDim;
                            }

                        }
                    }
                }
            }

            //if (kb.IsKeyDown(Keys.C) && oldkb.IsKeyUp(Keys.C))
            //{
            //    for (int i = 0; i < computerValidSpacesGlobal.Count; i++)
            //    {
            //        var a = computerValidSpacesGlobal[i];
            //        computerMoveToSpot(a[0], a[1], ref computerValidSpacesGlobal);
            //    }
            //}

            if (numMovesMade == 0)
            {
                if (mouse.RightButton == ButtonState.Pressed)
                {
                    for (int y = 0; y < boardYDim; y++)
                    {
                        for (int x = 0; x < boardXDim; x++)
                        {
                            if (board[y, x].getRec().Contains(mousePos))
                            {
                                knight.setPos(board[y, x].getRec());
                                knightX = x;
                                knightY = y;

                                findValidSpaces();
                                y = boardYDim;
                                break;
                            }

                        }
                    }
                }
            }

            if (mouse.LeftButton == ButtonState.Pressed && oldmouse.LeftButton == ButtonState.Released)
            {
                for (int i = 0; i < validSpaces.Count; i++)
                {
                    var a = validSpaces[i];
                    if (board[a[0], a[1]].getRec().Contains(mousePos))
                    {
                        if (numMovesMade == 0)
                        {
                            numMovesMade++;
                            board[knightY, knightX].moveNumber = numMovesMade;
                        }
                        knight.setPos(board[a[0], a[1]].getRec());
                        knightX = a[1];
                        knightY = a[0];
                        numMovesMade++;
                        board[knightY, knightX].moveNumber = numMovesMade;
                        findValidSpaces();

                        break;
                    }
                }
            }
        }

        public void makeRandomLevel()
        {

        }

        //public void computerMoveToSpot(int y, int x, ref List<int[]> computerValidSpaces)
        //{
        //    knight.setPos(board[y, x].getRec());
        //    knightX = x;
        //    knightY = y;

        //    computerFindValidSpaces(ref computerValidSpaces);

        //    for (int i = 0; i < computerValidSpacesGlobal.Count; i++)
        //    {
        //        var a = computerValidSpacesGlobal[i];
        //        computerMoveToSpot(a[0], a[1], ref computerValidSpacesGlobal);
        //    }
        //}

        public void findValidSpaces()
        {
            for (int i = 0; i < validSpaces.Count;)
            {
                validSpaces.RemoveAt(i);
            }

            addValidSpace(knightX - 2, knightY - 1);
            addValidSpace(knightX - 2, knightY + 1);

            addValidSpace(knightX + 1, knightY - 2);
            addValidSpace(knightX - 1, knightY - 2);

            addValidSpace(knightX + 2, knightY + 1);
            addValidSpace(knightX + 2, knightY - 1);

            addValidSpace(knightX - 1, knightY + 2);
            addValidSpace(knightX + 1, knightY + 2);
        }

        //public void computerFindValidSpaces(ref List<int[]> computerValidSpaces)
        //{
        //    for (int i = 0; i < computerValidSpaces.Count;)
        //    {
        //        computerValidSpaces.RemoveAt(i);
        //    }

        //    computerAddValidSpace(knightX - 2, knightY - 1, ref computerValidSpaces);
        //    computerAddValidSpace(knightX - 2, knightY + 1, ref computerValidSpaces);

        //    computerAddValidSpace(knightX + 1, knightY - 2, ref computerValidSpaces);
        //    computerAddValidSpace(knightX - 1, knightY - 2, ref computerValidSpaces);

        //    computerAddValidSpace(knightX + 2, knightY + 1, ref computerValidSpaces);
        //    computerAddValidSpace(knightX + 2, knightY - 1, ref computerValidSpaces);

        //    computerAddValidSpace(knightX - 1, knightY + 2, ref computerValidSpaces);
        //    computerAddValidSpace(knightX + 1, knightY + 2, ref computerValidSpaces);
        //}

        public void checkWins()
        {
            //hasWon = true;
            //for (int y = 0; y < boardYDim; y++)
            //{
            //    for (int x = 0; x < boardXDim; x++)
            //    {
            //        if (board[y, x].moveNumber == 0)
            //        {
            //            hasWon = false;
            //            y = boardYDim;
            //            x = boardXDim;
            //        }
            //    }
            //}

            if (numMovesMade == boardXDim * boardYDim - 1)
                hasWon = true;
        }

        public void addValidSpace(int xPos, int yPos)
        {
            int[] tempArray = new int[2];

            if (xPos >= 0 && xPos < boardXDim && yPos >= 0 && yPos < boardYDim && board[yPos, xPos].moveNumber == 0)
            {
                tempArray[0] = yPos;
                tempArray[1] = xPos;
                validSpaces.Add(tempArray);
            }
        }

        //public void computerAddValidSpace(int xPos, int yPos, ref List<int[]> computerValidSpaces)
        //{
        //    int[] tempArray = new int[2];

        //    if (xPos >= 0 && xPos < boardXDim && yPos >= 0 && yPos < boardYDim && board[yPos, xPos].moveNumber == 0)
        //    {
        //        tempArray[0] = yPos;
        //        tempArray[1] = xPos;
        //        computerValidSpaces.Add(tempArray);
        //    }
        //}

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();



            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    if ((x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0))
                        board[y, x].drawCharacter(spriteBatch, Color.Green);
                    else
                        board[y, x].drawCharacter(spriteBatch);
                    for (int i = 0; i < validSpaces.Count; i++)
                    {
                        var a = validSpaces[i];
                        //var b = computerValidSpacesGlobal[i];



                        //if (b[0] == y && b[1] == x && gameClock % 100 > 50)
                        //{
                        //    board[y, x].drawCharacter(spriteBatch, Color.Salmon);

                        //}

                        if (a[0] == y && a[1] == x && gameClock % 100 > 50)
                        {
                            board[y, x].drawCharacter(spriteBatch, Color.BlueViolet);

                        }
                    }

                    if (board[y, x].moveNumber != 0)
                    {
                        if (numMovesMade - board[y, x].moveNumber > 0)
                            tokens[y, x].drawCharacter(spriteBatch, Color.Black);

                        spriteBatch.DrawString(testFont, board[y, x].moveNumber.ToString(),
                            new Vector2(board[y, x].getRec().Center.X - 5, board[y, x].getRec().Center.Y - 10), Color.Red);
                    }
                }
            }





            knight.drawCharacter(spriteBatch);
            if (board[knightY, knightX].moveNumber != 0)
                spriteBatch.DrawString(testFont, board[knightY, knightX].moveNumber.ToString(),
                            new Vector2(board[knightY, knightX].getRec().Center.X - 5, board[knightY, knightX].getRec().Center.Y - 10), Color.Red);

            //spriteBatch.DrawString(testFont, "MouseX: " + mousePos.X + "\nMouseY: " + mousePos.Y, new Vector2(200, 200), Color.Pink);
            //spriteBatch.DrawString(testFont, "didWork: " + didWork, new Vector2(200, 280), Color.Pink);
            //spriteBatch.DrawString(testFont, "didWork: " + didWork, new Vector2(200, 280), Color.Pink);

            if (numMovesMade == 0)
            {
                spriteBatch.DrawString(testFont, "Choose a starting \nsquare with right click", new Vector2(200, 400), Color.Black);

            }

            if (hasWon)
            {
                spriteBatch.DrawString(testFont, "You did it!", new Vector2(screenWidth / 2 - 75, screenHeight / 2 - 30), Color.Black);

            }
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
