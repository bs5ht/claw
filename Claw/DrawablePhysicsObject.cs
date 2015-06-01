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
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics;

public class DrawablePhysicsObject
{
    // Because Farseer uses 1 unit = 1 meter we need to convert
    // between pixel coordinates and physics coordinates.
    // I've chosen to use the rule that 100 pixels is one meter.
    // We have to take care to convert between these two
    // coordinate-sets wherever we mix them!

    public const float unitToPixel = 100.0f;
    public const float pixelToUnit = 1 / unitToPixel;
    public World world;
    public Body body;
    public bool remove;
    public bool collideWithBall;
    public Vector2 Position
    {
        get { return body.Position * unitToPixel; }
        set { body.Position = value * pixelToUnit; }
    }

    public Texture2D texture;

    private Vector2 size;
    public Vector2 Size
    {
        get { return size * unitToPixel; }
        set { size = value * pixelToUnit; }
    }

    ///The farseer simulation this object should be part of
    ///The image that will be drawn at the place of the body
    ///The size in pixels
    ///The mass in kilograms
    public bool MyOnCollision(Fixture f1, Fixture f2, Contact contact)
    {
        Debug.WriteLine("collision!");
        Body claw, other;
        other = f2.Body;
        claw = f1.Body;
        f2.Body.Awake = true;
        f1.Body.Awake = true;
        //determine which one is claw and other is circle
        int shapeNum = (int)f1.Shape.ShapeType;
        int shapeNum2 = (int)f2.Shape.ShapeType;
        if (shapeNum == 0 || shapeNum2 == 0)
        {

            if (shapeNum == 0)
            {


                claw = f1.Body;
                other = f2.Body;
            }
            if (contact.IsTouching)
            {
                collideWithBall = true;
                return true;

            }
        }

        collideWithBall = false;
        return false;
    } 
    public DrawablePhysicsObject(Vector2 position, World world, Texture2D texture, Vector2 size, float mass, String type)
    {
        collideWithBall = false;
        this.world = world;
        this.remove = false;
        if (type == "rect")
        {
            body = BodyFactory.CreateRectangle(world, size.X * pixelToUnit, size.Y * pixelToUnit, 1);
        }
        if(type == "circle")
        {
            body = BodyFactory.CreateCircle(world, 5 * pixelToUnit, 10.0f, position*pixelToUnit);
        }
        this.Position = position;
        body.BodyType = BodyType.Dynamic;
        this.Size = size;
        this.texture = texture;
        body.OnCollision += MyOnCollision;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
       
        Vector2 scale = new Vector2(Size.X / (float)texture.Width, Size.Y / (float)texture.Height);
        spriteBatch.Draw(texture, Position, null, Color.White, body.Rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), scale, SpriteEffects.None, 0);
        
    }
    public void changePosition(Vector2 position)
    {
        Position = position;
        body.Position = position * pixelToUnit;
    }
    public void Destroy()
    {
        this.body.Dispose();
    }
    public object Clone() //clones an objet
    {
        return this.MemberwiseClone();
    }

 
}