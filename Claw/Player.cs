using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Claw
{
    class Player : Sprite
    {
        //private bool moving;
        private int speed;
        public int movedX;
        private int maxX;

        public Player(int x, int y, int width, int height, int viewWidth)
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
            maxX = viewWidth;
            //moving = false;

            // Movement
            speed = 5;
            movedX = 0;
        }

        public int getX()
        {
            return spriteX;
        }
        public int getY()
        {
            return spriteY;
        }

        public int getWidth()
        {
            return spriteWidth;
        }

        public int getHeight()
        {
            return spriteHeight;
        }

        public Vector2 getPosition()
        {
            return new Vector2(spriteX, spriteY);
        }
        public void setX(int x)
        {
            spriteX = x;
        }
        public void setY(int y)
        {
            spriteY = y;
        }

        public void LoadContent(ContentManager content)
        {
            image = content.Load<Texture2D>("Spaceman.png");
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White);
        }

        public void Update(Controls controls, GameTime gameTime, bool clawInAction)
        {
            if (!clawInAction)
            {

                Move(controls);
            }
        }

        public void Move(Controls controls)
        {

            if (controls.onPress(Keys.Right, Keys.D, Buttons.LeftThumbstickRight))
                movedX += speed;
            else if (controls.onRelease(Keys.Right, Keys.D, Buttons.LeftThumbstickRight))
                movedX -= speed;
            if (controls.onPress(Keys.Left, Keys.A, Buttons.LeftThumbstickLeft))
                movedX -= speed;
            else if (controls.onRelease(Keys.Left, Keys.A, Buttons.LeftThumbstickLeft))
                movedX += speed;

            //bounds sprite inside game window
            int move;
            move = spriteX + movedX;

            //window right edge
            int rightEdge = maxX - spriteWidth;

            if ((move < 0) || (move > rightEdge))
            {
                if (move < 0)
                    spriteX = 0;
                if (move > rightEdge)
                    spriteX = rightEdge;
            }
            else
                spriteX += movedX;


        }
    }
}
