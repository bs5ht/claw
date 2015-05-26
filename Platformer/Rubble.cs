using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;



namespace Claw
{
    class Rubble
    {
        static Texture2D rubbleTexture;
        Vector2 position;
        const float rubbleSpeed = 25;

        
        public Rubble(Vector2 pos)
        {
            this.position = pos;
        }

        public Vector2 pos
        {
            get
            {
                return position;
            }
        }
        

        public static void LoadContent(ContentManager content)
        {
            rubbleTexture = content.Load < Texture2D > ("rubbleplaceholder2.png");
        }

     
        public void Update(float elapsedSeconds)
        {
            position.Y += rubbleSpeed * elapsedSeconds;
  

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(rubbleTexture, position, Color.White);
            spriteBatch.End();
        }

    }
    static class Shared
    {
        public static readonly Random Random = new Random();
    }
}
