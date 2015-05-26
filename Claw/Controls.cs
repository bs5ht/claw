using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Claw
{
    public class Controls
    {
        public KeyboardState kb;
        public KeyboardState kbo;
        public GamePadState gp;
        public GamePadState gpo;

        public Controls()
        {
            this.kb = Keyboard.GetState();
            this.kbo = Keyboard.GetState();
            this.gp = GamePad.GetState(PlayerIndex.One);
            this.gpo = GamePad.GetState(PlayerIndex.One);

        }

        public void Update()
        {
            kbo = kb;
            gpo = gp;
            kb = Keyboard.GetState();
            this.gp = GamePad.GetState(PlayerIndex.One);
        }

        public bool isPressed(Keys key, Keys key2, Buttons button)
        {
            //Console.WriteLine (button);
            return kb.IsKeyDown(key) || kb.IsKeyDown(key2) || gp.IsButtonDown(button);
        }

        public bool onPress(Keys key, Keys key2, Buttons button)
        {
            if ((gp.IsButtonDown(button) && gpo.IsButtonUp(button)))
            {
                Console.WriteLine(button);
            }
            return (kb.IsKeyDown(key) && kbo.IsKeyUp(key)) ||
                (kb.IsKeyDown(key2) && kbo.IsKeyUp(key2)) ||
                (gp.IsButtonDown(button) && gpo.IsButtonUp(button));
        }

        public bool onRelease(Keys key, Keys key2, Buttons button)
        {
            //Console.WriteLine (button);
            return (kb.IsKeyUp(key) && kbo.IsKeyDown(key)) ||
                (kb.IsKeyUp(key2) && kbo.IsKeyDown(key2)) ||
                (gp.IsButtonUp(button) && gpo.IsButtonDown(button));
        }

        public bool isHeld(Keys key, Keys key2, Buttons button)
        {
            //Console.WriteLine (button);
            return (kb.IsKeyDown(key) && kbo.IsKeyDown(key)) ||
                (kb.IsKeyDown(key2) && kbo.IsKeyDown(key2)) ||
                (gp.IsButtonDown(button) && gpo.IsButtonDown(button));
        }

    }
}

