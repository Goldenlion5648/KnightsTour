using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        

        //Character player, playerCopyTopAndBottom, playerCopyLeftAndRight;
        Character[,] board;
        Character knight;
        int playerWidth = 64, playerHeight = 64;

        int boardXDim = 8, boardYDim = 8;

        int numMovesMade = 0;

        int screenWidth = 800;
        int screenHeight = 800;

        int gameClock = 0;

        bool didWork = false;

        bool isPressingKey = false;

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

            knight = new Character(Content.Load<Texture2D>("chessKnight"),
                        new Rectangle(0, 0, (screenWidth / boardXDim), (screenHeight / boardYDim)));

            for (int y = 0; y < boardYDim; y++)
            {
                for (int x = 0; x < boardXDim; x++)
                {
                    board[y, x] = new Character(Content.Load<Texture2D>("blankSquare"),
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

            userControls();


            oldmouse = mouse;
            oldkb = kb;
            gameClock++;
            base.Update(gameTime);
        }

        public void userControls()
        {
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
                                numMovesMade++;
                                board[y, x].moveNumber = numMovesMade;
                                knight.setPos(board[y, x].getRec());

                                y = boardYDim;
                                break;
                            }

                        }
                    }
                }
            }

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                for (int y = 0; y < boardYDim; y++)
                {
                    for (int x = 0; x < boardXDim; x++)
                    {
                        if (board[y, x].getRec().Contains(mousePos))
                        {
                            numMovesMade++;
                            board[y, x].moveNumber = numMovesMade;
                            knight.setPos(board[y, x].getRec());

                            y = boardYDim;
                            break;
                        }

                    }
                }
            }
        }

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

                    spriteBatch.DrawString(testFont, board[y,x].moveNumber.ToString(), board[y,x].getRec().Center.ToVector2(), Color.Purple);
                }
            }



            knight.drawCharacter(spriteBatch);


            //spriteBatch.DrawString(testFont, "MouseX: " + mousePos.X + "\nMouseY: " + mousePos.Y, new Vector2(200, 200), Color.Pink);
            //spriteBatch.DrawString(testFont, "didWork: " + didWork, new Vector2(200, 280), Color.Pink);
            //spriteBatch.DrawString(testFont, "didWork: " + didWork, new Vector2(200, 280), Color.Pink);

            if (numMovesMade == 0)
            {
                spriteBatch.DrawString(testFont, "Choose a starting \nsquare with right click", new Vector2(200, 400), Color.Black);

            }
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
