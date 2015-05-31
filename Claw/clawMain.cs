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
        
        DrawablePhysicsObject floor;
        DrawablePhysicsObject leftWall;
        DrawablePhysicsObject rightWall;
        Random random;
        Texture2D texture;
        Texture2D mouseTex;
        Vector2 mouseCoords;
        Player player1;
        Controls controls;
        ClawObj claw;
        
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
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height - 200);
            base.Initialize();

            this.mouseTex = this.Content.Load<Texture2D>("targeting");
            controls = new Controls();
            Vector2 clawPos = new Vector2((float)player1.getX(), (float)player1.getY());
            claw = new ClawObj(clawPos, world, Content);
            
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

            //generate the ground and side walls
            Vector2 size = new Vector2(50, 50);
            random = new Random();
            Texture2D floorTex = Content.Load<Texture2D>("Floor");
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width / 2.0f, GraphicsDevice.Viewport.Height - 20);
            floor = new DrawablePhysicsObject(position, world, floorTex, new Vector2(GraphicsDevice.Viewport.Width, 40.0f), 100000.0f, "rect");
            
            floor.body.BodyType = BodyType.Static;
            floor.body.Restitution = 1f;
            crateList = new List<DrawablePhysicsObject>();
            //create left wall
           Vector2 pos = new Vector2(0f, GraphicsDevice.Viewport.Height / 2);
            leftWall = new DrawablePhysicsObject(pos, world, floorTex, new Vector2(10.0f, GraphicsDevice.Viewport.Height), 100000.0f, "rect");
            
            leftWall.body.BodyType = BodyType.Static;
            leftWall.body.Friction = 0f;
            leftWall.body.Restitution = 1.00f;
            //create right wall
            pos = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 2);
            rightWall = new DrawablePhysicsObject(pos, world, floorTex, new Vector2(10.0f, GraphicsDevice.Viewport.Height), 100000.0f, "rect");

            rightWall.body.BodyType = BodyType.Static;
            rightWall.body.Friction = 0f;
            rightWall.body.Restitution = 1.00f;
        }

        private void SpawnCrate()
        {
            DrawablePhysicsObject crate;
            Vector2 position = new Vector2(random.Next(50, GraphicsDevice.Viewport.Width - 50), 1);
            crate = new DrawablePhysicsObject(position, world, Content.Load<Texture2D>("Crate"), new Vector2(50.0f, 50.0f), 0.1f, "rect");
            
            crate.body.LinearDamping = 20;
            // crate.body.GravityScale = 0.00f;
            crateList.Add(crate);
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
            if(mouseState.LeftButton == ButtonState.Pressed && !claw.clawInAction)
            {
                claw.clawMoving = true;
                claw.clawInAction = true;
                claw.setClawVelocity(mouseCoords);
            }
           


            //generate the blocks of the claw
            double var = gameTime.ElapsedGameTime.TotalSeconds;
            double millis = gameTime.ElapsedGameTime.TotalMilliseconds;
            if (gameTime.TotalGameTime.TotalMilliseconds - claw.lastClawTime > 100 && claw.clawMoving)
            {
                Debug.WriteLine("hellooo");
                claw.lastClawTime = gameTime.TotalGameTime.TotalMilliseconds;
                claw.generateClawSegment(gameTime.TotalGameTime.TotalMilliseconds);
            }


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
            //update the ball's position with respect to the player
            if (!claw.clawInAction)
            {
                claw.updatePosition(player1.getPosition());
            }

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
            if(claw != null){
                claw.Draw(spriteBatch);
            }
            foreach (DrawablePhysicsObject clawSeg in claw.clawSegmentList)
            {
                clawSeg.Draw(spriteBatch);
                int length = claw.clawSegmentList.Count;
            }

            floor.Draw(spriteBatch);
            leftWall.Draw(spriteBatch);
            rightWall.Draw(spriteBatch);
            player1.Draw(spriteBatch);
            spriteBatch.Draw(this.mouseTex, this.mouseCoords, null, Color.White, 0.0f, this.mouseCoords, 0.05f, SpriteEffects.None, 0.0f);
           
            
            
            spriteBatch.End();

            foreach (Rubble piece in rubble)
            {
                piece.Draw(spriteBatch);
            }

           
           
            base.Draw(gameTime);
        }
    }

}

