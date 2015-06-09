using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework.Storage;

namespace Claw
{
    //experience system takes care of leveling and stuff

    class ExperienceSystem
    {
        public int level = 1;
        public int totalExperience = 0;
        public Dictionary<String, int> multDict;
        public Dictionary<String, int> pointDict; //keeps all the point value for the objects
        public List<DrawablePhysicsObject> hitList;
        public bool calcScore = false;
        public ExperienceSystem()
        {
            multDict = new Dictionary<String, int>();
            pointDict = new Dictionary<String, int>();
            hitList = new List<DrawablePhysicsObject>();
            multDict["rubble"] = 10;
            multDict["wall"] = 3;
            multDict["static"] = 3;
            pointDict["health"] = 20;
            pointDict["static"] = 10;
            totalExperience = 0;
        }

        public void calculateScore()
        {
            calcScore = true;
        }


        public void update()
        {
            //add to the total amount of points
            if (calcScore)
            {
                int totPoints = 1;
                for (int x = 0; x < hitList.Count; x++)
                {
                    
                    if (hitList[x].gameObjType == "health")
                    {
                        totPoints = totPoints *pointDict["health"];
                        totalExperience += totPoints;
                        totPoints = 1;
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
