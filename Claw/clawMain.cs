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
        List<DrawablePhysicsObject> vitList;
        List<DrawablePhysicsObject> rubbleList;
        List<DrawablePhysicsObject> staticList;

        //floor and walls 
        DrawablePhysicsObject floor;
        DrawablePhysicsObject leftWall;
        DrawablePhysicsObject rightWall;

        DrawablePhysicsObject clawBody;

        Random random;

        //texture stuff
        //screen textures
        Texture2D mTitleScreenBackground;
        Texture2D gameOverScreen;
        Texture2D crateImg;
        Texture2D staticImg;
        Texture2D staticHit;
        Texture2D vitImg;
        Texture2D rubbleImg;
        Texture2D rubbleHit;
        Texture2D floorImg;
        Texture2D texture;
        Texture2D mouseTex;
        Texture2D clawRestImg;
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
        bool once = true;
        bool gameOver = false;


        private Texture2D background;
        double spawnTimer;
        double spawnDelay = 0.0; //seconds

        double healthSpawnTimer;
        double healthSpawnDelay = 0.0; //seconds


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
            background = Content.Load<Texture2D>("marsbg.jpg");
            gameOverScreen = Content.Load<Texture2D>("gameover.png");
            mHealthBar = Content.Load<Texture2D>("healthbar_temp3.png");
            healthText = Content.Load<Texture2D>("health text.png");
            mTitleScreenBackground = Content.Load<Texture2D>("startscreenop2.3.png");
            mIsTitleScreenShown = true;

            staticImg = Content.Load<Texture2D>("Static2.png");
            staticHit = Content.Load<Texture2D>("StaticHit.png");
            vitImg = Content.Load<Texture2D>("Health.png");
            rubbleImg = Content.Load<Texture2D>("Rubble.png");
            floorImg = Content.Load<Texture2D>("Floor");

            clawRestImg = Content.Load<Texture2D>("Claw_Idle.png");
       
            random = new Random();

            Vector2 size = new Vector2(50, 50);
            random = new Random();
            //wall and ground stuff begin here
            Texture2D floorTex = Content.Load<Texture2D>("Floor");
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height - 20);
            floor = new DrawablePhysicsObject(position, world, floorTex, new Vector2(GraphicsDevice.Viewport.Width, 40.0f), 10.0f, "rect");

            floor.body.BodyType = BodyType.Static;
            floor.body.Restitution = 1f;
            vitList = new List<DrawablePhysicsObject>();
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
            vitList = new List<DrawablePhysicsObject>();
            rubbleList = new List<DrawablePhysicsObject>();
            staticList = new List<DrawablePhysicsObject>();

            Vector2 spriteSize = new Vector2(player1.getWidth(), player1.getHeight());
            float playerMidPoint = player1.getPosition().X + player1.getWidth()/2;
            
            Vector2 clawBodyPos = new Vector2(playerMidPoint, 450); //450 is the player's height position
            Vector2 clawSize = new Vector2(50, 50);
            
            Vector2 clawPos = new Vector2(clawBodyPos.X, clawBodyPos.Y-10);
            claw = new ClawObj(clawPos, world, Content);
            foreach (Fixture fix in claw.body.FixtureList)
            {
                fix.CollisionCategories = Category.Cat1;
                fix.CollidesWith = Category.Cat2 | Category.Cat3;
            }
 
            clawBody = new DrawablePhysicsObject(clawBodyPos, world, clawRestImg, clawSize, 3.0f, "rect");
            clawBody.body.IgnoreGravity = true;
            clawBody.body.Rotation = 0;
            clawBody.body.CollisionCategories = Category.Cat10;
            clawBody.body.CollidesWith = Category.Cat10;
            foreach (Fixture fix in clawBody.body.FixtureList)
            {
                fix.CollisionCategories = Category.Cat20;
                fix.CollidesWith = Category.Cat20;
            }
 
            //wall and ground stuff end here
        }

        private void SpawnStatic()
        {
            DrawablePhysicsObject staticObject;
            int staticX = random.Next(50, GraphicsDevice.Viewport.Width - 50);
            int staticY = random.Next(100, GraphicsDevice.Viewport.Height / 3 + 50);
            Vector2 staticPosition = new Vector2(staticX, staticY);
            staticObject = new DrawablePhysicsObject(staticPosition, world, staticImg, new Vector2(60.0f, 60.0f), 0.1f, "rect");
            staticObject.body.BodyType = BodyType.Static;
            staticObject.body.CollisionCategories = Category.Cat3;
            staticObject.body.LinearDamping = 100;
            staticList.Add(staticObject);

        }

        private void SpawnRubble()
        {
            DrawablePhysicsObject rubble;
            Vector2 rubblePos = new Vector2(random.Next(50, GraphicsDevice.Viewport.Width - 50), 1);
            rubble = new DrawablePhysicsObject(rubblePos, world, rubbleImg, new Vector2(50.0f, 50.0f), 0.1f, "rect"); 
            rubble.body.LinearDamping = 20;
            rubble.body.CollisionCategories = Category.Cat2;
            // rubble.body.GravityScale = 0.00f;
            rubbleList.Add(rubble);

        }
        private void drawMouse() //draws the mouse pointer
        {
            this.spriteBatch.Draw(this.mouseTex, new Rectangle(0, 0, this.mouseTex.Width, this.mouseTex.Height), Color.White);
        }
        private void SpawnHealth()
        {
            DrawablePhysicsObject health;
            Vector2 healthPos = new Vector2(random.Next(50, GraphicsDevice.Viewport.Width - 50), 1);
            health = new DrawablePhysicsObject(healthPos, world, vitImg, new Vector2(50.0f, 50.0f), 0.1f, "rect");
            health.body.LinearDamping = 20;
            vitList.Add(health);
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

        private void updateClawBodyPosition()
        {
            Vector2 tempPosition = new Vector2( player1.getX() + player1.getWidth()/2 , player1.getY() - 30);
            clawBody.changePosition(tempPosition);
            float clawBodyAngle = clawBody.body.Rotation;
            Vector2 toMouse = this.mouseCoords - clawBody.body.Position* unitToPixel ;
            toMouse.Normalize();
            float newAngle = (float)(Math.Atan2(toMouse.Y, toMouse.X)) + (float)(3.14159/2);
            clawBody.body.SetTransform(clawBody.body.Position, newAngle);
            clawBody.Position = clawBody.body.Position * unitToPixel;
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
            if (Keyboard.GetState().IsKeyDown(Keys.Space) == true)
            {
                Vector2 multFactor = new Vector2(1.05f, 1.05f);
                claw.clawHead.body.LinearVelocity = Vector2.Multiply(multFactor, claw.clawHead.body.LinearVelocity);
            }
            else if (startGame)
            {
                 // TODO: Add your update logic here
                mCurrentHealth -= .05;

                //spawns static objects once at the start of hte game
                if (once)
                {
                    for (int setupStatic = 4; setupStatic >= 0; setupStatic--)
                    {
                        SpawnStatic();
                    } 
                }

                spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                healthSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                //need to check bounds of the claw
                Vector2 pos = claw.clawHead.Position;
                if (!checkBounds(pos)) // if the claw is not within the boundary reset the claw
                {
                    Vector2 p = new Vector2 (player1.getX() + 25, player1.getY() + 25);
                    claw.resetClaw(p);
                }
                if (mouseState.LeftButton == ButtonState.Pressed && !claw.clawInAction)
                {
                    claw.clawMoving = true;
                    claw.clawInAction = true;

                    //need to clean this up later

                    Vector2 direction = mouseCoords - clawBody.body.Position * unitToPixel;
                    if (direction != Vector2.Zero)
                        direction.Normalize();
              

                    claw.updatePosition(clawBody.Position + direction*10);
                    claw.setClawVelocity(mouseCoords);
                }
                else if (mouseState.RightButton == ButtonState.Pressed)
                {
                    Vector2 p = new Vector2(player1.getX() + 25, player1.getY() + 25);
                    claw.resetClaw(p);
                }
                else if (gameTime.TotalGameTime.TotalMilliseconds - claw.lastClawTime > 100 && claw.clawMoving)
                {
                    claw.lastClawTime = gameTime.TotalGameTime.TotalMilliseconds;
                    claw.generateClawSegment(gameTime.TotalGameTime.TotalMilliseconds);
                }

                if (once == false)
                {
                    if (spawnTimer >= spawnDelay)
                    {

                        spawnTimer -= spawnDelay; //subtract used time
                        SpawnRubble();

                        double numgen = Shared.Random.NextDouble();
                        double delay = 5.0 * numgen;
                        if (delay > 3)
                        {
                            delay /= 2;
                        }
                        spawnDelay = delay;
                    }
                }
                


                if (healthSpawnTimer >= healthSpawnDelay)
                {

                    healthSpawnTimer -= healthSpawnDelay; //subtract used time
                    SpawnHealth();

                    double delay = 2.0;
                  
                    healthSpawnDelay = delay;
                }
                player1.Update(controls, gameTime);
               updateClawBodyPosition();
                //update the ball's position with respect to the claw body
                if (!claw.clawInAction)
                {
                    Vector2 clawHeadPos = new Vector2(clawBody.Position.X, clawBody.Position.Y);

                    claw.updatePosition(clawHeadPos);
                }

                
                //removes rubble
                for (int i = rubbleList.Count - 1; i >= 0; i--)
                {
                    float rubbleX = rubbleList[i].Position.X;
                    float playerXMin = player1.getPosition().X;
                    float playerXMax = playerXMin + player1.getHeight();
                    if ( rubbleX >= playerXMin && rubbleX <= playerXMax)
                    {
                        //checks with collision with player so far, will probably need to be replaced
                        //with the physics version later
                        if (rubbleList[i].Position.Y >= player1.getPosition().Y)
                        {
                            rubbleList[i].Destroy();
                            rubbleList.RemoveAt(i);
                            mCurrentHealth -= 20;
                        }
                    }
                        
                    else if (rubbleList[i].hitSomething == true && rubbleList[i].Position.Y >= floor.Position.Y-floor.Size.Y)
                    {
                        
                        rubbleList[i].Destroy();
                        rubbleList.RemoveAt(i);
                    }
                    
                }

                for (int i = rubbleList.Count - 1; i >= 0; i--)
                {
                    float rubbleX = rubbleList[i].Position.X;
                    float playerXMin = player1.getPosition().X;
                    float playerXMax = playerXMin + player1.getHeight();
                    if (rubbleX >= playerXMin && rubbleX <= playerXMax)
                    {
                        //checks with collision with player so far, will probably need to be replaced
                        //with the physics version later
                        if (rubbleList[i].Position.Y >= player1.getPosition().Y)
                        {
                            rubbleList[i].Destroy();
                            rubbleList.RemoveAt(i);
                            mCurrentHealth -= 20;
                        }
                    }

                    else if (rubbleList[i].collideWithBall == true && rubbleList[i].Position.Y >= floor.Position.Y - floor.Size.Y)
                    {

                        rubbleList[i].Destroy();

                        rubbleList.RemoveAt(i);
                    }

                } 
                //check static objects hit
                for (int i = staticList.Count - 1; i >= 0; i--)
                {
                    
                    if (staticList[i].collideWithBall == true)
                    {
                        staticList[i].texture = staticHit;
                        //staticList[i].Destroy();
                       //staticList.RemoveAt(i);
                    }

                }

            

                //removes health packs
                for (int j = vitList.Count - 1; j >= 0; j--)
                {
                    
                    if (vitList[j].collideWithBall)
                    {
                        vitList[j].Destroy();
                        vitList.RemoveAt(j);
                        Vector2 direction = mouseCoords - clawBody.body.Position * unitToPixel;
                    if (direction != Vector2.Zero)
                        direction.Normalize();

                        claw.resetClaw(clawBody.body.Position * unitToPixel + direction * 10);
                        mCurrentHealth += 20;
                        if (mCurrentHealth > 100)
                            mCurrentHealth = 100;
                    }
                    else if (vitList[j].Position.Y >= (float) this.Window.ClientBounds.Center.Y)
                    {
                        vitList[j].Destroy();
                        vitList.RemoveAt(j);        
                    }    
                }

                }
                world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
                base.Update(gameTime);
                once = false;

            if (mCurrentHealth <= 0)
            {
                gameOver = true;
                startGame = false;
            }


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
           else if(gameOver)
           {
               spriteBatch.Draw(gameOverScreen, new Rectangle(0, 0, 800, 480), Color.White);
               spriteBatch.End();
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

               foreach (DrawablePhysicsObject staticObj in staticList)
               {
                   staticObj.Draw(spriteBatch);
               }

               foreach (DrawablePhysicsObject rubble in rubbleList)
               {

                   rubbleImg = Content.Load<Texture2D>("Rubble.png");
                   rubble.Draw(spriteBatch);
               }

               foreach (DrawablePhysicsObject health in vitList)
               {
                   vitImg = Content.Load<Texture2D>("Health.png");
                   health.Draw(spriteBatch);
               }
               if (claw != null && claw.clawInAction)
               {
                   claw.Draw(spriteBatch);
               }
               if (claw.clawInAction)
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
              
               clawBody.Draw(spriteBatch);
               player1.Draw(spriteBatch);
               spriteBatch.Draw(this.mouseTex, this.mouseCoords, null, Color.White, 0.0f, this.mouseCoords, 0.05f, SpriteEffects.None, 0.0f);


               base.Draw(gameTime);
               spriteBatch.End();
           }
        }
    }

}

