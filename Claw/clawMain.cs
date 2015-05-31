#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
 
#endregion

namespace Claw
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>



    public class clawMain : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
 

        //health bar
        Texture2D mHealthBar;
        double mCurrentHealth = 100.0;

        //farseer variables
        World world;
        Body body;
        List<DrawablePhysicsObject> crateList;
        DrawablePhysicsObject floor;
        Random random;
        Texture2D texture;

        //screen textures
        Texture2D mTitleScreenBackground;

        //Screen state variables to indicate what is the current screen;
        bool mIsTitleScreenShown;
        bool startGame = false;

 
        Player player1;
        Controls controls;
        private Texture2D background;
        double rubbleSpawnTimer;
        double rubbleSpawnDelay = 3.0; //seconds


        void NewRubble()
        {
            int viewWidth = GraphicsDevice.Viewport.Width;
            float xPosition = Shared.Random.Next(viewWidth - 50);
            rubble.Add(new Rubble(new Vector2(xPosition, 0)));

        }

        public clawMain()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 

        List<Rubble> rubble = new List<Rubble>();

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            int viewWidth;
            viewWidth = GraphicsDevice.Viewport.Width;
            int viewHeight;
            viewHeight = GraphicsDevice.Viewport.Height;

            player1 = new Player(370, 400, 50, 50, viewWidth);

            NewRubble();

            base.Initialize();


            controls = new Controls();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player1.LoadContent(this.Content);
            
            //farseer world
            world = new World(new Vector2(0, 9.8f));

            // TODO: use this.Content to load your game content here
            background = Content.Load<Texture2D>("spacebg.jpg");
            mHealthBar = Content.Load<Texture2D>("healthbar_temp3.png");
            mTitleScreenBackground = Content.Load<Texture2D>("startscreenop2.3.png");

            mIsTitleScreenShown = true;
 
           
            Rubble.LoadContent(Content);
            Vector2 size = new Vector2(50, 50);
            random = new Random();
            Texture2D floorTex = Content.Load<Texture2D>("Floor");
            floor = new DrawablePhysicsObject(world, floorTex, new Vector2(GraphicsDevice.Viewport.Width, 40.0f), 1000.0f);
            floor.Position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height-20);
            floor.body.BodyType = BodyType.Static;
            crateList = new List<DrawablePhysicsObject>();
        }
        private void SpawnCrate()
        {
            DrawablePhysicsObject crate;
            crate = new DrawablePhysicsObject(world, Content.Load<Texture2D>("Crate"), new Vector2(50.0f, 50.0f), 0.1f);
            crate.Position = new Vector2(random.Next(50, GraphicsDevice.Viewport.Width - 50), 1);
            crate.body.LinearDamping = 20;
            // crate.body.GravityScale = 0.00f;
            crateList.Add(crate);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

  
        private void UpdateTitleScreen()
        {
            
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                startGame = true;
                mIsTitleScreenShown = false;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //set our keyboardstate tracker update can change the gamestate on every cycle
            controls.Update();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (mIsTitleScreenShown)
            {
                UpdateTitleScreen();
                return;
            }

            else if (startGame)
            {
                 // TODO: Add your update logic here
                mCurrentHealth -= 0.015;

                rubbleSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (rubbleSpawnTimer >= rubbleSpawnDelay)
                {
                    rubbleSpawnTimer -= rubbleSpawnDelay; //subtract used time
                    SpawnCrate();
                    NewRubble();
                    double numgen = Shared.Random.NextDouble();
                    double delay = 10.0 * numgen;
                    if (delay > 3)
                    {
                        delay /= 2;
                    }
                    rubbleSpawnDelay = delay;
                }

                player1.Update(controls, gameTime);

                //moves falling rubble
                foreach (Rubble piece in rubble)
                {
                    piece.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
                }

                //removes rubble
                for (int i = rubble.Count - 1; i >= 0; i--)
                {
                    if (rubble[i].pos.Y > GraphicsDevice.Viewport.Height - 100)
                        rubble.RemoveAt(i);
                }

                //removes crates
                for (int j = crateList.Count - 1; j >= 0; j--)
                {
 
                    if (crateList[j].Position.Y >= 413)
                    {
                        crateList[j].Destroy();
                        crateList.RemoveAt(j);                    
                    }
                    
                }


            
            
                }
                world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
                base.Update(gameTime);           
        }
 
        private void DrawTitleScreen()
        {
            spriteBatch.Draw(mTitleScreenBackground, Vector2.Zero, Color.White);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            
            // TODO: Add your drawing code here
            

           //based on screen state variables, call the Draw method associated with the current screen
           if(mIsTitleScreenShown)
           {
               DrawTitleScreen();
               spriteBatch.End();
               return;
           }
           else if(startGame)
           {

               texture = Content.Load<Texture2D>("Crate");

               //background
               spriteBatch.Draw(background, new Rectangle(0, 0, 800, 480), Color.White);

               //draw the negative space for the health bar
               spriteBatch.Draw(mHealthBar, new Rectangle(this.Window.ClientBounds.Width / 5 + 10 - mHealthBar.Width / 2,
                   20, mHealthBar.Width, 30), new Rectangle(0, 30, mHealthBar.Width, 30), Color.Gray);
               //draw the current health level based on the current Health
               spriteBatch.Draw(mHealthBar, new Rectangle((this.Window.ClientBounds.Width / 5 + 10 - mHealthBar.Width / 2),
                   20, (int)(mHealthBar.Width * ((double)mCurrentHealth / 100)), 30), new Rectangle(0, 30, mHealthBar.Width, 30), Color.Red);

               //ignore - old health bar; this does not decrement
               //draw health for health bar
               //spriteBatch.Draw(mHealthBar, new Rectangle(this.Window.ClientBounds.Width / 5 + 10 - mHealthBar.Width / 2,
               //     20, mHealthBar.Width, 30), new Rectangle(0, 30, mHealthBar.Width, 30), Color.Red);
               //ignore - old health bar; this does not decrement

               //draw box around health bar
               spriteBatch.Draw(mHealthBar, new Rectangle(this.Window.ClientBounds.Width / 5 + 10 - mHealthBar.Width / 2,
                   20, mHealthBar.Width, 30), new Rectangle(0, 0, mHealthBar.Width, 30), Color.White);


               Vector2 scale = new Vector2(50 / (float)texture.Width, 50 / (float)texture.Height);
                
               foreach (DrawablePhysicsObject crate in crateList)
               {
                   crate.Draw(spriteBatch);
               }

               floor.Draw(spriteBatch);
               player1.Draw(spriteBatch);
              
               foreach (Rubble piece in rubble)
               {
                   piece.Draw(spriteBatch);
               }

               base.Draw(gameTime);
               spriteBatch.End();
           }
        }
    }

}

