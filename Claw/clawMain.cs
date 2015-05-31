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

        //farseer variables
        World world;
        Body body;
        List<DrawablePhysicsObject> crateList;
        List<DrawablePhysicsObject> clawSegmentList; 
        double clawAfterImageFreq; //this the frequency to draw the objects on the screen
        double clawInterval = 200;//this is in milliseconds
        bool clawMoving = false;
        double lastClawTime;
        DrawablePhysicsObject floor;
        DrawablePhysicsObject ball;
        Random random;
        Texture2D texture;
        Texture2D mouseTex;
        Vector2 mouseCoords;
        Player player1;
        Controls controls;
        
        public const float unitToPixel = 100.0f;
        public const float pixelToUnit = 1 / unitToPixel;

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

            this.mouseTex = this.Content.Load<Texture2D>("targeting");
            controls = new Controls();
            clawSegmentList = new List<DrawablePhysicsObject>();
            lastClawTime = 0;
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

            Rubble.LoadContent(Content);

            Vector2 size = new Vector2(50, 50);
            random = new Random();
            Texture2D floorTex = Content.Load<Texture2D>("Floor");
            floor = new DrawablePhysicsObject(world, floorTex, new Vector2(GraphicsDevice.Viewport.Width, 40.0f), 1000.0f, "rect");
            floor.Position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height-20);
            floor.body.BodyType = BodyType.Static;
            crateList = new List<DrawablePhysicsObject>();
            
            
        }
        private void SpawnCrate()
        {
            DrawablePhysicsObject crate;
            crate = new DrawablePhysicsObject(world, Content.Load<Texture2D>("Crate"), new Vector2(50.0f, 50.0f), 0.1f, "rect");
            crate.Position = new Vector2(random.Next(50, GraphicsDevice.Viewport.Width - 50), 1);
            crate.body.LinearDamping = 20;
            // crate.body.GravityScale = 0.00f;
            crateList.Add(crate);
        }

        private void spawnBall()
        {
            Texture2D ballClaw = Content.Load<Texture2D>("ball");
            Vector2 testPosition = new Vector2(400, 400);
            Vector2 ballSize = new Vector2(20, 20);
            ball = new DrawablePhysicsObject(world, ballClaw, ballSize, 1.0f, "circle");
            ball.Position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height - 200);
            ball.body.BodyType = BodyType.Dynamic;
            ball.body.IgnoreGravity = true;
            ball.body.Restitution = 1f;
            ball.body.Friction = 0f;
            ball.body.LinearVelocity = new Vector2(0.0001f, 0.0001f);
           //Vector2 origVelocity = new  Vector2(0.0f, -0.01f);
            //ball.body.ApplyLinearImpulse(origVelocity);
        }
          private Vector2 getClawDirectionVector()
        {
            Vector2 spriteCoord = new Vector2(player1.getX(), player1.getY());
            Vector2 direction = mouseCoords - ball.body.Position*unitToPixel;
            if (direction != Vector2.Zero)
             direction.Normalize();
            return direction;
        }
        private void setBallVelocity()
        {
            float xMag = 0.1f;
            float yMag = 0.1f;
            Vector2 direction = getClawDirectionVector();
            ball.body.LinearVelocity = direction * 2.0f;
            clawMoving = true;
           // ball.body.ApplyLinearImpulse(origVelocity);
          
        }

        private void generateClawSegment()
        {
            if (ball != null) { 
            DrawablePhysicsObject clawSegment;
            Texture2D ballClaw = Content.Load<Texture2D>("ball");
            Vector2 testPosition = new Vector2(400, 400);
            Vector2 ballSize = new Vector2(20, 20);
            clawSegment = new DrawablePhysicsObject(world, ballClaw, ballSize, 1.0f, "circle");
            clawSegment.Position = ball.body.Position;
            clawSegment.body.BodyType = BodyType.Static;
            clawSegment.body.IgnoreGravity = true;
            clawSegment.body.Restitution = 1f;
            clawSegment.body.Friction = 0f;
            clawSegment.body.Position = ball.body.Position;
            clawSegment.body.LinearVelocity = new Vector2(0.0001f, 0.0001f);
            clawSegmentList.Add(clawSegment);
        }
        }
        private void drawMouse() //draws the mouse pointer
        {
            this.spriteBatch.Draw(this.mouseTex, new Rectangle(0, 0, this.mouseTex.Width, this.mouseTex.Height), Color.White);


        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            //set our keyboardstate tracker update can change the gamestate on every cycle
            controls.Update();
            var mouseState = Mouse.GetState();
            this.mouseCoords = new Vector2(mouseState.X, mouseState.Y);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                spawnBall();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.V))
            {
                setBallVelocity();
            }



            //generate the blocks of the claw
            double var = gameTime.ElapsedGameTime.TotalSeconds;
            double millis = gameTime.ElapsedGameTime.TotalMilliseconds;
            if (gameTime.TotalGameTime.TotalMilliseconds - lastClawTime > 100 && clawMoving)
            {
                Debug.WriteLine("hellooo");
                lastClawTime = gameTime.TotalGameTime.TotalMilliseconds;
                generateClawSegment();
            }


            rubbleSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            Debug.WriteLine(rubbleSpawnTimer);
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




            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            texture = Content.Load<Texture2D>("Crate");
            // TODO: Add your drawing code here
            
            spriteBatch.Draw(background, new Rectangle(0, 0, 800, 480), Color.White);
            //ball.Draw(spriteBatch);
            Vector2 scale = new Vector2(50 / (float)texture.Width, 50 / (float)texture.Height);
            foreach (DrawablePhysicsObject crate in crateList)
            {
                crate.Draw(spriteBatch);
            }
            if(ball != null){
                ball.Draw(spriteBatch);
            }
            
            floor.Draw(spriteBatch);
            player1.Draw(spriteBatch);
            spriteBatch.Draw(this.mouseTex, this.mouseCoords, null, Color.White, 0.0f, this.mouseCoords, 0.05f, SpriteEffects.None, 0.0f);
            foreach (DrawablePhysicsObject clawSeg in clawSegmentList)
            {
                clawSeg.Draw(spriteBatch);
            }

            
            spriteBatch.End();

            foreach (Rubble piece in rubble)
            {
                piece.Draw(spriteBatch);
            }
        

           
            base.Draw(gameTime);
        }
    }

}

