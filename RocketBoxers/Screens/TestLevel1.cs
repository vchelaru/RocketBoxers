using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Localization;
using FlatRedBall.TileCollisions;

namespace RocketBoxers.Screens
{
	public partial class TestLevel1
	{

		void CustomInitialize()
		{
            Camera.Main.X = 1000;
            Camera.Main.Y = -500;

            

            GroundCollision.AddCollisionFromTilesWithProperty(TestLevel, "Ground");
            WallCollision.AddCollisionFromTilesWithProperty(TestLevel, "Wall");
            GroundCollision.Visible = true;
        }

		void CustomActivity(bool firstTimeCalled)
		{


		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
