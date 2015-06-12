#region Using Statements

using System.Collections.Generic;
using System.Diagnostics;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics;
using System.IO;
using System.Text;
using System;
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
        
        //Timers begin here
        double staticResetTimer = 30000; // the static rubble resets every 30 seconds
        double lastStaticResetTime = 0;

        int increases = 0;
        double rubbleFallTimer = 10000; //every 
        double lastRubbleIncreaseTime = 0;
        double rubbleDampingDelta = .2;

        int healthDecreases = 1;
        double healthDecTimer = 20000;
        double lastHealthDecreaseTime = 0;
        double healthDecDelta = 0.05f;

        double keyboardPullRate = 100;
        double lastKeyboardTime = 0; 
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
        ExperienceSystem expSys;

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
        Texture2D introScreen;
        Texture2D controlScreen;
        Texture2D chainImg;
        SpriteFont font;

        string  scoreName = ""; //Start with no text
        int cursorPos = 0;
        //These are set in the constructor:
        Rectangle backRect;

        Vector2 mouseCoords;
        //try to use these coordinates globally
        Vector2 rubbleSize = new Vector2(50.0f, 50.0f);
        Vector2 staticSize = new Vector2(40.0f, 40.0f);
        Vector2 staticDrawFactor = new Vector2(1.6f, 1.6f);
        Vector2 healthSize;
        //audio
        private SoundEffect bgmusic;

        //controls, player, and the claw
        Player player1;
        Controls controls;
        ClawObj claw;

        KeyboardState prevKeyBoardState;
        KeyboardState curKeyBoardState;
        //constants to convert from physics engine to screen and vice-versa

        public const float unitToPixel = 100.0f;
        public const float pixelToUnit = 1 / unitToPixel;
        String highScorerName = "";
        int staticGenNum = 5;

        

        //Screen state variables to indicate what is the current screen;
        bool mIsTitleScreenShown;
        bool startGame = false;
        bool once = true;
        bool gameOver = false;
        bool spawnStatic = false;
        bool isIntro = false;
        bool isControl = false;
        bool nameIsCompleted = false;
        private Texture2D background;
        double spawnTimer;
        double spawnDelay = 0.0; //seconds

        double healthSpawnTimer;
        double healthSpawnDelay = 0.0; //seconds
        double highScore;

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
            graphics.PreferredBackBufferHeight = 550;
            graphics.PreferredBackBufferWidth = 800;
            graphics.ApplyChanges();
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

            player1 = new Player(viewWidth/2, 480, 50, 50, viewWidth);


            this.mouseTex = this.Content.Load<Texture2D>("targeting");
            controls = new Controls();

            expSys = new ExperienceSystem();
            string scoreData = null;
            //load high score from score.txt 
            
            
            String path = @"Content/score.txt";
            
            using (var stream = TitleContainer.OpenStream(path))
            {
                using (var reader = new StreamReader(stream))
                {
                    string[] separators = {","};
                    scoreData = reader.ReadToEnd();
                    string[] scoreAndName = scoreData.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    highScore = Convert.ToDouble(scoreAndName[0]);
                    if(scoreData.Length > 1){
                    highScorerName = "" + scoreData[1];
                    }
                    reader.Close();
                }
                stream.Close();
                
            }
            base.Initialize();

        }


        public void CharEntered(String c, GameTime gameTime)
        {

            /*
            string newText = scoreName.Insert(cursorPos, c.ToString()); //Insert the char

            //Check if the text width is shorter than the back rectangle
            
            if (font.MeasureString(newText).X < GraphicsDevice.PresentationParameters.BackBufferWidth)
            {
                scoreName = newText; //Set the text
                cursorPos++; //Move the text cursor
            }*/
          
                Keys[] PressedKeys = curKeyBoardState.GetPressedKeys();
                for (int i = 0; i < PressedKeys.Length; i++)
                {
                    if (PressedKeys[i].ToString().Length == 1)
                        scoreName += PressedKeys[i].ToString();
                    if (PressedKeys[i] == Keys.Back && scoreName.Length > 0)
                        scoreName = scoreName.Remove(scoreName.Length - 1);
                }
                if (curKeyBoardState.IsKeyDown(Keys.Enter))
                {
                    nameIsCompleted = true;

                }
            
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
            background = Content.Load<Texture2D>("marsbg.png");
            gameOverScreen = Content.Load<Texture2D>("gameover.png");
            mHealthBar = Content.Load<Texture2D>("healthbar_temp3.png");
            healthText = Content.Load<Texture2D>("health text.png");
            mTitleScreenBackground = Content.Load<Texture2D>("startscreen.png");
            mIsTitleScreenShown = true;
            introScreen = Content.Load<Texture2D>("intro text.png");
            controlScreen = Content.Load<Texture2D>("controlscreen.png");

            //audio
            bgmusic = Content.Load<SoundEffect>("fairytailark.wav");
            //audio start
            SoundEffectInstance bgMusic = bgmusic.CreateInstance();
            bgMusic.IsLooped = true;
            bgMusic.Play();
            
            staticImg = Content.Load<Texture2D>("Static2.png");
            staticHit = Content.Load<Texture2D>("StaticHit.png");
            vitImg = Content.Load<Texture2D>("Health.png");
            rubbleImg = Content.Load<Texture2D>("Rubble.png");
            floorImg = Content.Load<Texture2D>("Floor");
            chainImg = Content.Load<Texture2D>("chain.png");
            clawRestImg = Content.Load<Texture2D>("Claw_Idle.png");
            font = Content.Load<SpriteFont>("Font"); // Use the name of your sprite font file here instead of 'Score'.
            random = new Random();

            Vector2 size = new Vector2(50, 50);
            random = new Random();
            //wall and ground stuff begin here
            Texture2D floorTex = Content.Load<Texture2D>("Floor");
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height - 20);
            floor = new DrawablePhysicsObject(position, world, floorTex, new Vector2(GraphicsDevice.Viewport.Width, 40.0f), 10.0f, "floor");

            floor.body.BodyType = BodyType.Static;
            floor.body.Restitution = 1f;
            vitList = new List<DrawablePhysicsObject>();
            //create left wall
            Vector2 pos = new Vector2(0f, GraphicsDevice.Viewport.Height / 2);
            leftWall = new DrawablePhysicsObject(pos, world, floorTex, new Vector2(10.0f, GraphicsDevice.Viewport.Height), 10.0f, "wall");

            leftWall.body.BodyType = BodyType.Static;
            leftWall.body.Friction = 0f;
            leftWall.body.Restitution = 1.00f;
            //update collision category
            foreach (Fixture fix in leftWall.body.FixtureList)
            {
                fix.CollisionCategories = Category.Cat4; //category 4 is the wall 
                fix.CollidesWith = Category.Cat1; //can collide with catagory 2(rubble) , or category 3(statics), or wall
            }
            //create right wall
            pos = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 2);
            rightWall = new DrawablePhysicsObject(pos, world, floorTex, new Vector2(10.0f, GraphicsDevice.Viewport.Height), 10.0f, "wall");
            rightWall.body.BodyType = BodyType.Static;
            rightWall.body.Friction = 0f;
            rightWall.body.Restitution = 1.00f;
            //update collision category
            foreach (Fixture fix in rightWall.body.FixtureList)
            {
                fix.CollisionCategories = Category.Cat4; //category 4 is the wall 
                fix.CollidesWith = Category.Cat1; //can collide with catagory 2(rubble) , or category 3(statics), or wall
            }
            vitList = new List<DrawablePhysicsObject>();
            rubbleList = new List<DrawablePhysicsObject>();
            staticList = new List<DrawablePhysicsObject>();

            Vector2 spriteSize = new Vector2(player1.getWidth(), player1.getHeight());
            float playerMidPoint = player1.getPosition().X + player1.getWidth()/2;
            
            Vector2 clawBodyPos = new Vector2(playerMidPoint, 450); //450 is the player's height position
            Vector2 clawSize = new Vector2(50, 50);
            
            Vector2 clawPos = new Vector2(clawBodyPos.X, clawBodyPos.Y-20);
            claw = new ClawObj(clawPos, world, Content);
            //update collision category
            claw.turnOffCollision();
 
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


        public bool checkBoxIntersections(List<DrawablePhysicsObject> list, Vector2 pos, float xWidth, float yWidth){
             for(int x = 0; x < list.Count; x++){
                 Vector2 listBoxPos = list[x].Position;
                 Vector2 listBoxSize = list[x].Size;       
                 bool intersect = (Math.Abs(listBoxPos.X - pos.X) * 2 < (listBoxSize.X + xWidth)) &&
                     (Math.Abs(listBoxPos.Y - pos.Y) * 2 < (listBoxSize.Y + yWidth));
                 if (intersect)
                 {
                     return true;
                 }
             }
             return false;
        }
        private void SpawnStatic()
        {
            Vector2 staticPosition;
            DrawablePhysicsObject staticObject;
            bool inter= true;
            do
            {
                
                int staticX = random.Next(50, GraphicsDevice.Viewport.Width - 50);
                int staticY = random.Next(100, GraphicsDevice.Viewport.Height / 3 + 50);
                staticPosition = new Vector2(staticX, staticY);
                staticObject = new DrawablePhysicsObject(staticPosition, world, staticImg, new Vector2(40.0f, 40.0f), 1000.0f, "static");
                staticObject.body.BodyType = BodyType.Dynamic;
                staticObject.body.CollisionCategories = Category.Cat3;
                staticObject.body.LinearDamping = 100;
                //changing this to make sure that there are no intersections
                inter = checkBoxIntersections(staticList, staticPosition, 40.0f * staticDrawFactor.X, 40.0f * staticDrawFactor.Y);
                if (inter)
                {
                    staticObject.Destroy();
                }
            }
       while (inter); //as long as it is intersecting
            
                
            staticList.Add(staticObject);
           
        }

        private void SpawnRubble()
        {
            DrawablePhysicsObject rubble;
            Vector2 rubblePos;
            bool inter = true;
            do{
                rubblePos = new Vector2(random.Next(50, GraphicsDevice.Viewport.Width - 50), 1);
                rubble = new DrawablePhysicsObject(rubblePos, world, rubbleImg, new Vector2(30.0f, 30.0f), 10f, "rubble"); 
                rubble.body.LinearDamping = 20 - (float)rubbleDampingDelta * (float)increases;
               
                rubble.body.CollisionCategories = Category.Cat2;
                inter = checkBoxIntersections(rubbleList, rubblePos, 50.0f * staticDrawFactor.X, 50.0f * staticDrawFactor.Y);
                if (inter)
                {
                    rubble.Destroy();
                }
            }
            while (inter); //as long as it is intersecting
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
            health = new DrawablePhysicsObject(healthPos, world, vitImg, new Vector2(50.0f, 50.0f), 0.1f, "health");
            health.body.LinearDamping = 20;
            health.body.CollisionCategories = Category.Cat5;
            foreach (Fixture fix in health.body.FixtureList)
            {
                fix.CollisionCategories = Category.Cat5;
                fix.CollidesWith = Category.Cat1;
            }
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

        private void updateControlScreen()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) == true)
            {
                isControl = false;
                isIntro = false;
                mIsTitleScreenShown = false;
                startGame = true;
            }
            return;
        }

        private void updateIntroScreen()
        {

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) == true || Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                isIntro = false;
                mIsTitleScreenShown = false;
                isControl = true;
            }
            return;
        }

        private void UpdateTitleScreen()
        {
             
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) == true || Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                isIntro = true;
                mIsTitleScreenShown = false;
            }
            return;
        }

        private void updateClawBodyPosition()
        {
            Vector2 tempPosition = new Vector2( player1.getX() + player1.getWidth()/2 , player1.getY() - 20);
            clawBody.changePosition(tempPosition);
            float clawBodyAngle = clawBody.body.Rotation;
            Vector2 toMouse = this.mouseCoords - clawBody.body.Position* unitToPixel;
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
            prevKeyBoardState = curKeyBoardState;
            curKeyBoardState = Keyboard.GetState();
            
            //set our keyboardstate tracker update can change the gamestate on every cycle
            if (!claw.clawInAction)
            {
                controls.Update();
                player1.Update(controls, gameTime);
            }
            var mouseState = Mouse.GetState();
            this.mouseCoords = new Vector2(mouseState.X, mouseState.Y);

            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            

            if (mIsTitleScreenShown)
            {
                UpdateTitleScreen();
                return;
            }
            //if (Keyboard.GetState().IsKeyDown(Keys.Space) == true)
            //{
            //   Vector2 multFactor = new Vector2(1.05f, 1.05f);
            //   claw.clawHead.body.LinearVelocity = Vector2.Multiply(multFactor, claw.clawHead.body.LinearVelocity);
            //}
            else if(isIntro)
            {
                updateIntroScreen();
                return;
            }
            else if(isControl)
            {
                updateControlScreen();
                return;
            }
            else if (gameOver)
            {
                Keys[] keys = Keyboard.GetState().GetPressedKeys();
                //if (gameTime.TotalGameTime.TotalMilliseconds - lastKeyboardTime > 3000)
                //{
                    curKeyBoardState = Keyboard.GetState();

                    lastKeyboardTime = gameTime.TotalGameTime.TotalMilliseconds;
                    foreach (Keys k in keys)
                    {
                        if (curKeyBoardState.IsKeyDown(k) && !prevKeyBoardState.IsKeyDown(k))
                        {
                            CharEntered(k.ToString(), gameTime);
                        }

                        
                    }
                    prevKeyBoardState = curKeyBoardState;
            //    }


            }
            else if (startGame)
            {
                 // TODO: Add your update logic here
                mCurrentHealth -= healthDecDelta*healthDecreases;

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
                    expSys.resetHits();
                }
                if (mouseState.LeftButton == ButtonState.Pressed && !claw.clawInAction)
                {
                    claw.clawMoving = true;
                    claw.clawInAction = true;
                    claw.turnOnCollision();
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
                    expSys.resetHits();
                }
                else if (gameTime.TotalGameTime.TotalMilliseconds - claw.lastClawTime > 100 && claw.clawMoving)
                {
                    claw.lastClawTime = gameTime.TotalGameTime.TotalMilliseconds;
                    claw.generateClawSegment(gameTime.TotalGameTime.TotalMilliseconds);
                }

                if (gameTime.TotalGameTime.TotalMilliseconds - lastStaticResetTime > 20000) //10 seconds
                {
                    for (int i = staticList.Count - 1; i >= 0; i--)
                    {
                        staticList[i].body.Awake = true;
                        staticList[i].Destroy();
                        world.RemoveBody(staticList[i].body);
                        staticList.RemoveAt(i);
                    }
                    for (int i = 0; i < staticGenNum; i++)
                    {
                        SpawnStatic();
                    }
                    lastStaticResetTime = gameTime.TotalGameTime.TotalMilliseconds;

                }

                if (gameTime.TotalGameTime.TotalMilliseconds - lastRubbleIncreaseTime > 10000)
                {
                    increases++;
                    lastRubbleIncreaseTime = gameTime.TotalGameTime.TotalMilliseconds;
                }
                if (gameTime.TotalGameTime.TotalMilliseconds - lastHealthDecreaseTime > 10000)
                {
                    healthDecreases++;
                    lastHealthDecreaseTime = gameTime.TotalGameTime.TotalMilliseconds;
                }



                if (once == false)
                {
                    if (spawnTimer >= spawnDelay)
                    {

                        spawnTimer -= spawnDelay; //subtract used time
                        SpawnRubble();

                        double numgen = Shared.Random.NextDouble();
                        double delay = 10.0 * numgen;
                        if (delay > 6)
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

                    double delay = 3.0;
                  
                    healthSpawnDelay = delay;
                }
              
                   
                
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
                    //checks collision with the player sprite 
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

                //SCORE UPDATES BEGIN HERE

                for (int i = rubbleList.Count - 1; i >= 0; i--)
                {
                    if (rubbleList[i].collideWithBall)
                    {
                        expSys.addToHitList(rubbleList[i]);
                        rubbleList[i].collideWithBall = false;
                    }
                }

                //check static objects hit
                for (int i = staticList.Count - 1; i >= 0; i--)
                {
                    
                    if (staticList[i].collideWithBall == true)
                    {
                        staticList[i].texture = staticHit;
                        expSys.addToHitList(staticList[i]);
                        staticList[i].collideWithBall = false;
                        staticList[i].hasBeenHitOnce = true;
                    }
                }


                //update experience system by checking collisions with left and right wall, may add floor later
                if (leftWall.collideWithBall)
                {
                    expSys.addToHitList(leftWall);
                    leftWall.collideWithBall = false;
                }
                if (rightWall.collideWithBall)
                {
                    expSys.addToHitList(rightWall);
                    rightWall.collideWithBall = false;
                }

                //UPDATE score system. DO NOT COMBINE LOOP with one below, there are indexing issues.

                for (int j = vitList.Count - 1; j >= 0; j--)
                {
                    if (vitList[j].collideWithBall)
                    {
                        //update score system
                        expSys.addToHitList(vitList[j]);
                        expSys.calculateScore();
                        expSys.update(); //update the experience system 
                    }
                }

                //SCORE UPDATE ENDS HERE 

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
                    else if (vitList[j].Position.Y >= (float) this.Window.ClientBounds.Center.Y+100)
                    {
                        vitList[j].Destroy();
                        vitList.RemoveAt(j);        
                    }    
                 
                }
                  if (spawnStatic)
                {

                    for (int i = staticList.Count - 1; i >= 0; i--)
                    {
                        staticList[i].Destroy();
                        staticList[i].texture = staticImg;
                        staticList.RemoveAt(i);
                    }

                    for (int i = 0; i < staticGenNum; i++)
                    {
                            SpawnStatic();
                    }
                    spawnStatic = false;
                }
                //check if all rubble are hit to regenerate them at random, and add points to the score
                bool allRubbleHit = true;
                for (int i = 0; i < staticList.Count; i++)
                {
                    if (!staticList[i].hasBeenHitOnce)
                        allRubbleHit = false;
                }
                if (staticList.Count == 0)
                {
                    allRubbleHit = false;
                }
                if (allRubbleHit)
                {
                    //grants small health bonus for getting all static objects
                    mCurrentHealth += 40;
                    if (mCurrentHealth > 100)
                    {
                        mCurrentHealth = 100;
                    }

                    expSys.staticResetPoints(staticList.Count);
                    for (int i = staticList.Count -1; i >= 0; i--){
                        staticList[i].body.Awake = true;
                        staticList[i].Destroy();
                        staticList.RemoveAt(i);
                    }
                    spawnStatic = true;
                    lastStaticResetTime = gameTime.TotalGameTime.TotalMilliseconds;
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
            spriteBatch.Draw(mTitleScreenBackground, new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
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
               spriteBatch.Draw(this.mouseTex, this.mouseCoords, null, Color.White, 0.0f, this.mouseCoords, 0.05f, SpriteEffects.None, 0.0f);
               spriteBatch.End();
               return;
           }
             
           else if(isIntro)
           {
               spriteBatch.Draw(introScreen, new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
               spriteBatch.End();
               return;
           }
           else if(isControl)
           {
               spriteBatch.Draw(controlScreen, new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
               spriteBatch.End();
               return;
           }
           else if (startGame)
           {

               //background
               spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);

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
                   float transparency = 1 - (float)((float)gameTime.TotalGameTime.TotalMilliseconds - (float)lastStaticResetTime) / (float)staticResetTimer;
                   staticObj.Draw(spriteBatch, staticDrawFactor.X, staticDrawFactor.Y, transparency, true);
               }

               foreach (DrawablePhysicsObject rubble in rubbleList)
               {

                   rubbleImg = Content.Load<Texture2D>("Rubble.png");
                   rubble.Draw(spriteBatch, staticDrawFactor.X, staticDrawFactor.Y);
               }

               foreach (DrawablePhysicsObject health in vitList)
               {
                   vitImg = Content.Load<Texture2D>("Health.png");
                   //need to calculate transparency value based on its location and height
                   float transparency = 2f * ((float)this.Window.ClientBounds.Center.Y+100 - health.Position.Y) / (float)this.Window.ClientBounds.Center.Y;

                   health.Draw(spriteBatch, transparency);
               }
               if (claw != null && claw.clawInAction)
               {
                   claw.Draw(spriteBatch);
                   Texture2D SimpleTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                   spriteBatch.Draw(SimpleTexture, new Rectangle(100, 100, 100, 1), Color.Blue);

                   float clawBodyAngle = clawBody.body.Rotation;
                   Vector2 toMouse = this.mouseCoords - clawBody.body.Position * unitToPixel;
                   toMouse.Normalize();
                   float newAngle = (float)(Math.Atan2(toMouse.Y, toMouse.X)) + (float)(3.14159 / 2);
                   clawBody.body.SetTransform(clawBody.body.Position, newAngle);
                   clawBody.Position = clawBody.body.Position * unitToPixel;
               }
               if (claw.clawInAction)
               {
                   
                   DrawablePhysicsObject prevObject = null;
                   Vector2 scaleDim = new Vector2(0, 0);
                   for (int y = 1; y < claw.clawSegmentList.Count; y++)
                   {
                       Vector2 newDim = claw.clawSegmentList[y].Position - claw.clawSegmentList[y-1].Position;
                       
                       if (newDim.Length() > scaleDim.Length() ){
                           scaleDim = newDim;
                           scaleDim.X = Math.Abs(newDim.X);
                           scaleDim.Y = Math.Abs(newDim.Y);
                       }


                   }
                   for(int y = 1; y < claw.clawSegmentList.Count; y++){
                       Vector2 prevVector = claw.clawSegmentList[y - 1].Position;
                       Vector2 curVector = claw.clawSegmentList[y].Position;


                       Vector2 rotVector= curVector - prevVector;
                   rotVector.Normalize();
                   float newAngle = (float)(Math.Atan2(rotVector.Y, rotVector.X)) + (float)(3.14159 / 2);

                   
                   
                   Rectangle rect = new Rectangle(0, 0, 150, 600);
                  // Vector2 pos = new Vector2(curVector, curVector.Y);

                   Vector2 origin = new Vector2(chainImg.Width / 2, chainImg.Height / 2);
                   Vector2 scale2 = (curVector - prevVector) / scaleDim;
                   scale2.X = Math.Abs(scale2.X);
                   scale2.Y = Math.Abs(scale2.Y);

                   scale2.Normalize();
                   Vector2 scale = new Vector2(0.15f, 0.15f);
                   scale = scale2 * scale;
                   Vector2 posVector =  (curVector + prevVector)/2;
                   spriteBatch.Draw(chainImg, posVector, null, Color.White, newAngle, origin, scale, SpriteEffects.None, 0);
                  // spriteBatch.Draw(chainImg, curVector, rect, Color.White, newAngle, origin, 0.15f, SpriteEffects.None, 1);
                   }
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

               spriteBatch.DrawString(font, "Score:" + expSys.totalExperience, new Vector2(20, 70), Color.White);

               
               spriteBatch.End();
           }
           else if(gameOver)
           {
               spriteBatch.Draw(gameOverScreen, new Rectangle(0, 0,  GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);

               if (Keyboard.GetState().IsKeyDown(Keys.Enter))
               {
                   if (expSys.totalExperience > highScore)
                   {
                       highScore = expSys.totalExperience;
                       highScorerName = scoreName;
                       string txtScore = Convert.ToString(highScore);

                       //write new high score to text doc
                       using (System.IO.StreamWriter file = new System.IO.StreamWriter("Content/score.txt"))
                       {
                           file.WriteLine(txtScore +"," + scoreName);
                       }
                   }
               }

               spriteBatch.Draw(gameOverScreen, new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
               //spriteBatch.Draw(staticHit, new Rectangle(110, 300, 270, 125), Color.Black);
               
               spriteBatch.DrawString(font, "High Score:" + highScore, new Vector2(135, 370), Color.Purple);
               spriteBatch.DrawString(font, "High Scorer:" + highScorerName, new Vector2(135, 390), Color.Purple);
               spriteBatch.DrawString(font, "Your Score:" + expSys.totalExperience, new Vector2(135, 405), Color.Green);
               spriteBatch.DrawString(font, "Your Name:" + scoreName, new Vector2(135, 430), Color.Red);
               spriteBatch.End();
           }
           base.Draw(gameTime);
        }
    }

}

