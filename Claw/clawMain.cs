#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics;
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
        Texture2D healthText;

        
        World world;
        Body body;
        List<DrawablePhysicsObject> crateList;
        List<DrawablePhysicsObject> rubbleList;

        //floor and walls 
        DrawablePhysicsObject floor;
        DrawablePhysicsObject leftWall;
        DrawablePhysicsObject rightWall;

        Random random;

        //texture stuff
        //screen textures
        Texture2D mTitleScreenBackground;
        Texture2D crateImg;
        Texture2D rubbleImg;
        Texture2D floorImg;
        Texture2D texture;
        Texture2D mouseTex;
        Vector2 mouseCoords;

        //controls, player, and the claw
        Player player1;
        Controls controls;
        ClawObj claw;
        
        //constants to convert from physics engine to screen and vice-versa

        public const float unitToPixel = 100.0f;
        public const float pixelToUnit = 1 / unitToPixel;


        

        //Screen state variables to indicate what is the current screen;
        bool mIsTitleScreenShown;
        bool startGame = false;


        private Texture2D background;
        double spawnTimer;
        double spawnDelay = 0.0; //seconds

        double crateSpawnTimer;
        double crateSpawnDelay = 5.0; //seconds


        public bool checkBounds(Vector2 position)
        {
            if (position.X >= 0 && position.X <= GraphicsDevice.Viewport.Width && position.Y >= 0 && position.Y <= GraphicsDevice.Viewport.Height)
            {
                return true;
            }
            return false;
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

         

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            int viewWidth;
            viewWidth = GraphicsDevice.Viewport.Width;
            int viewHeight;
            viewHeight = GraphicsDevice.Viewport.Height;

            player1 = new Player(370, 400, 50, 50, viewWidth);

            base.Initialize();

            this.mouseTex = this.Content.Load<Texture2D>("targeting");
            controls = new Controls();
            Vector2 clawPos = new Vector2((float)player1.getX(), (float)player1.getY());
            claw = new ClawObj(clawPos, world, Content);

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
            healthText = Content.Load<Texture2D>("health text.png");
            mTitleScreenBackground = Content.Load<Texture2D>("startscreenop2.3.png");
            mIsTitleScreenShown = true;

            crateImg = Content.Load<Texture2D>("Crate.png");
            rubbleImg = Content.Load<Texture2D>("Rubble.png");
            floorImg = Content.Load<Texture2D>("Floor");
           
       
            random = new Random();

            Vector2 size = new Vector2(50, 50);
            random = new Random();
            //wall and ground stuff begin here
            Texture2D floorTex = Content.Load<Texture2D>("Floor");
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height - 20);
            floor = new DrawablePhysicsObject(position, world, floorTex, new Vector2(GraphicsDevice.Viewport.Width, 40.0f), 10.0f, "rect");

            floor.body.BodyType = BodyType.Static;
            floor.body.Restitution = 1f;
            crateList = new List<DrawablePhysicsObject>();
            //create left wall
            Vector2 pos = new Vector2(0f, GraphicsDevice.Viewport.Height / 2);
            leftWall = new DrawablePhysicsObject(pos, world, floorTex, new Vector2(10.0f, GraphicsDevice.Viewport.Height), 10.0f, "rect");

            leftWall.body.BodyType = BodyType.Static;
            leftWall.body.Friction = 0f;
            leftWall.body.Restitution = 1.00f;
            //create right wall
            pos = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 2);
            rightWall = new DrawablePhysicsObject(pos, world, floorTex, new Vector2(10.0f, GraphicsDevice.Viewport.Height), 10.0f, "rect");
            rightWall.body.BodyType = BodyType.Static;
            rightWall.body.Friction = 0f;
            rightWall.body.Restitution = 1.00f;
            crateList = new List<DrawablePhysicsObject>();
            rubbleList = new List<DrawablePhysicsObject>();
            //wall and ground stuff end here
        }

        private void SpawnRubble()
        {
            DrawablePhysicsObject rubble;
            Vector2 rubblePos = new Vector2(random.Next(50, GraphicsDevice.Viewport.Width - 50), 1);
            rubble = new DrawablePhysicsObject(rubblePos, world, rubbleImg, new Vector2(50.0f, 50.0f), 0.1f, "rect"); 
            rubble.body.LinearDamping = 30;
            // rubble.body.GravityScale = 0.00f;
            rubbleList.Add(rubble);

        }
        private void drawMouse() //draws the mouse pointer
        {
            this.spriteBatch.Draw(this.mouseTex, new Rectangle(0, 0, this.mouseTex.Width, this.mouseTex.Height), Color.White);
        }
        private void SpawnCrate()
        {
            DrawablePhysicsObject crate;
            Vector2 cratePos = new Vector2(random.Next(50, GraphicsDevice.Viewport.Width - 50), 1);
            crate = new DrawablePhysicsObject(cratePos, world, crateImg, new Vector2(50.0f, 50.0f), 0.1f, "rect");
            crate.body.LinearDamping = 30;
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
            var mouseState = Mouse.GetState();
            this.mouseCoords = new Vector2(mouseState.X, mouseState.Y);

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
                mCurrentHealth -= 0.005;

                spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                crateSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                //need to check bounds of the claw
                Vector2 pos = claw.clawHead.Position;
                if (!checkBounds(pos)) // if the claw is not within the boundary reset the claw
                {
                    Vector2 p = player1.getPosition();
                    claw.resetClaw(p);
                }
                if (mouseState.LeftButton == ButtonState.Pressed && !claw.clawInAction)
                {
                    claw.clawMoving = true;
                    claw.clawInAction = true;
                    claw.setClawVelocity(mouseCoords);
                }
                else if (mouseState.RightButton == ButtonState.Pressed)
                {
                    Vector2 p = player1.getPosition();
                    claw.resetClaw(p);
                }
                else if (gameTime.TotalGameTime.TotalMilliseconds - claw.lastClawTime > 100 && claw.clawMoving)
                {
                    claw.lastClawTime = gameTime.TotalGameTime.TotalMilliseconds;
                    claw.generateClawSegment(gameTime.TotalGameTime.TotalMilliseconds);
                }

                if (spawnTimer >= spawnDelay)
                {
                    
                    spawnTimer -= spawnDelay; //subtract used time
                    SpawnRubble();

                    double numgen = Shared.Random.NextDouble();
                    double delay = 10.0 * numgen;
                    if (delay > 3)
                    {
                        delay /= 2;
                    }
                    spawnDelay = delay;
                }

                
                if (crateSpawnTimer >= crateSpawnDelay)
                {

                    crateSpawnTimer -= crateSpawnDelay; //subtract used time
                    SpawnCrate();

                    double delay = 10.0;
                  
                    crateSpawnDelay = delay;
                }
                player1.Update(controls, gameTime);
                //update the ball's position with respect to the player
                if (!claw.clawInAction)
                {
                    claw.updatePosition(player1.getPosition());
                }
                //removes rubble
                for (int i = rubbleList.Count - 1; i >= 0; i--)
                {

                    if (rubbleList[i].Position.Y >= 410)
                    {
                        rubbleList[i].Destroy();
                        rubbleList.RemoveAt(i);
                    }
                    
                }

            

                //removes crates
                for (int j = crateList.Count - 1; j >= 0; j--)
                {
 
                    if (crateList[j].Position.Y >= 410)
                    {
                        crateList[j].Destroy();
                        crateList.RemoveAt(j);                    
                    }
                    if (crateList[j].collideWithBall)
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

               //background
               spriteBatch.Draw(background, new Rectangle(0, 0, 800, 480), Color.White);

               //health text
               spriteBatch.Draw(healthText, new Rectangle(10, 5, healthText.Bounds.Width, healthText.Bounds.Height), Color.White);

               //draw the negative space for the health bar
               spriteBatch.Draw(mHealthBar, new Rectangle(this.Window.ClientBounds.Width / 5 + 4 - mHealthBar.Width / 2,
                   30, mHealthBar.Width, 30), new Rectangle(0, 30, mHealthBar.Width, 30), Color.Gray);
               //draw the current health level based on the current Health
               spriteBatch.Draw(mHealthBar, new Rectangle((this.Window.ClientBounds.Width / 5 + 4 - mHealthBar.Width / 2),
                   30, (int)(mHealthBar.Width * ((double)mCurrentHealth / 100)), 30), new Rectangle(0, 30, mHealthBar.Width, 30), Color.Red);

               //draw box around health bar
               spriteBatch.Draw(mHealthBar, new Rectangle(this.Window.ClientBounds.Width / 5 + 4 - mHealthBar.Width / 2,
                   30, mHealthBar.Width, 30), new Rectangle(0, 0, mHealthBar.Width, 30), Color.White);



               foreach (DrawablePhysicsObject rubble in rubbleList)
               {
                   rubbleImg = Content.Load<Texture2D>("Rubble.png");
                   rubble.Draw(spriteBatch);
               }

               foreach (DrawablePhysicsObject crate in crateList)
               {
                   crateImg = Content.Load<Texture2D>("Crate.png");
                   crate.Draw(spriteBatch);
               }
               if (claw != null)
               {
                   claw.Draw(spriteBatch);
               }
               if (claw.clawMoving)
               {
                   foreach (DrawablePhysicsObject clawSeg in claw.clawSegmentList)
                   {
                       clawSeg.Draw(spriteBatch);
                       int length = claw.clawSegmentList.Count;
                   }
               }

               floor.Draw(spriteBatch);
               leftWall.Draw(spriteBatch);
               rightWall.Draw(spriteBatch);
               player1.Draw(spriteBatch);
               spriteBatch.Draw(this.mouseTex, this.mouseCoords, null, Color.White, 0.0f, this.mouseCoords, 0.05f, SpriteEffects.None, 0.0f);


               base.Draw(gameTime);
               spriteBatch.End();
           }
        }
    }

}

