

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

public class ClawObj
{
    public List<DrawablePhysicsObject> clawSegmentList;
    public List<Vector2> posQueue;
    double clawAfterImageFreq; //this the frequency to draw the objects on the screen
    double clawInterval = 200;//this is in milliseconds
    public bool clawMoving = false;
    public  bool clawInAction = false;
    public DrawablePhysicsObject clawHead;
    public double lastClawTime;
    public const float unitToPixel = 100.0f;
    public const float pixelToUnit = 1 / unitToPixel;
    public double ballSeparation = 20;
    public Vector2 lastBallPos = new Vector2(0.0001f, 0.00001f);
    public bool prevBallPosition = false;
    public bool FirstLaunch = true;
    public double generationDelay;
    public double lastGenTime;
    public bool firstClawSide;
    public Body body;
    ContentManager Content;
    World world;
	public ClawObj(Vector2 position, World world, ContentManager Content)
	{
        this.Content = Content;
        this.world = world;
        clawSegmentList = new List<DrawablePhysicsObject>();
        lastClawTime = 0;
        //generate the head of the claw
        Texture2D clawHeadClaw = Content.Load<Texture2D>("ball");
        Vector2 testPosition = new Vector2(400, 400);
        Vector2 clawHeadSize = new Vector2(10, 10);
        clawHead = new DrawablePhysicsObject(position, world, clawHeadClaw, clawHeadSize, 1.0f, "circle");
        clawHead.Position = position;
        clawHead.body.BodyType = BodyType.Dynamic;
        clawHead.body.IgnoreGravity = true;
        clawHead.body.Restitution = 1f;
        clawHead.body.Friction = 0f;
        clawHead.body.LinearVelocity = new Vector2(0.0001f, 0.0001f);
        body = clawHead.body;
        posQueue = new List<Vector2>();
        
	}
    private Vector2 getClawDirectionVector(Vector2 mouseCoords)
    {
        Vector2 direction = mouseCoords - clawHead.body.Position * unitToPixel;
        if (direction != Vector2.Zero)
            direction.Normalize();
        return direction;
    }

    public void updatePosition(Vector2 position)
    {
        clawHead.body.Position = position * pixelToUnit;
        clawHead.Position = position;

    }


    public void resetClaw(Vector2 player1Pos)
    {
        
            clawMoving = false;
            clawInAction = false;
            //remove all claw segments
            foreach (DrawablePhysicsObject clawSeg in clawSegmentList)
            {
                clawSeg.Destroy();
            }
            posQueue = new List<Vector2>();
            clawSegmentList = new List<DrawablePhysicsObject>();
            clawHead.changePosition(player1Pos);
        
    }

   
    public bool canGenerateStaticClawPart()
    {
        return true;
    }

    public void setClawVelocity(Vector2 mouseCoords)
    {
      
        Vector2 direction = getClawDirectionVector(mouseCoords);
        clawHead.body.LinearVelocity = direction * 2.0f;
        clawMoving = true;
        // ball.body.ApplyLinearImpulse(origVelocity);
    }
    public void generateClawSegment(double time)
    {
        double elapsedTime = time - lastGenTime;
        
        //store a snapshat of the ball at this position 
        if (posQueue.Count != 0)
        {
            DrawablePhysicsObject clawSegment;
            Texture2D ballClaw = Content.Load<Texture2D>("ball");
            Vector2 testPosition = clawHead.Position;
            Vector2 ballSize = new Vector2(10, 10);
            clawSegment = new DrawablePhysicsObject(testPosition, world, ballClaw, ballSize, 1.0f, "circle");
            clawSegment.Position = posQueue[0];
            clawSegment.body.BodyType = BodyType.Static;
            clawSegment.body.IgnoreGravity = true;
            clawSegment.body.Restitution = 0.000f;
            clawSegment.body.Friction = 0f;
            clawSegment.body.Position = posQueue[0]*pixelToUnit;
            clawSegment.body.LinearVelocity = new Vector2(0.0001f, 0.0001f);
            clawSegmentList.Add(clawSegment);
            posQueue.RemoveAt(0);
        }
        posQueue.Add(clawHead.Position + clawHead.Position * 0.01f);
        
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        clawHead.Draw(spriteBatch);

        
            
    }
}
