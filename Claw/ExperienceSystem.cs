using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Claw
{
    //experience system takes care of leveling and stuff

    class ExperienceSystem
    {
        public int level = 1;
        public double totalExperience = 0;
        public Dictionary<String, Double> multDict;
        public Dictionary<String, Double> pointDict; //keeps all the point value for the objects
        public List<DrawablePhysicsObject> hitList;
        public bool calcScore = false;
 
        public double totPoints = 0;
        public ExperienceSystem()
        {
            multDict = new Dictionary<String, Double>();
            pointDict = new Dictionary<String, Double>();
            hitList = new List<DrawablePhysicsObject>();
            multDict["rubble"] = 5;
            multDict["wall"] = 2;
            multDict["static"] = 3;
            pointDict["health"] = 20;
            pointDict["static"] = 100;
            totalExperience = 0;
        }

        public void calculateScore()
        {
            calcScore = true;
        }
        public void update()
        {
            //find how many collisions that claw hit

            //add to the total amount of points
            if (calcScore)
            {
                totPoints = 1;
                for (int x = 0; x < hitList.Count; x++)
                {
                    
                    if (hitList[x].gameObjType == "health")
                    {
                        totPoints = totPoints *pointDict["health"];
                        totalExperience += totPoints;
                        //totPoints = 1;
                    }
                    else if (hitList[x].gameObjType == "wall")
                    {
                        totPoints = totPoints * multDict["wall"];
                    }
                    else
                    {
                        String type = hitList[x].gameObjType;
                        totPoints = totPoints * multDict[type];
                    }

                }
                //clear the hit queue
                hitList = new List<DrawablePhysicsObject>();
                calcScore = false;
  
 
            }
        }
 
        public void addToHitList(DrawablePhysicsObject obj)
        {
            hitList.Add(obj);
        }

        public void staticResetPoints(int num)
        {
            totalExperience += num *  pointDict["static"];
        }
        public void resetHits()
        {
            hitList = new List<DrawablePhysicsObject>();
            calcScore = false;

        }
    }

   
}
