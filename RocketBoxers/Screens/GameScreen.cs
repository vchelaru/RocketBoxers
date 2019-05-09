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
using FlatRedBall.Math.Collision;
using RocketBoxers.Entities;

namespace RocketBoxers.Screens
{
	public partial class GameScreen
	{
        CollisionRelationship playerVsGround;

        void CustomInitialize()
        {
            Player1.X = 1000;
            Player1.Y = -500;

            InitializeCollisions();

        }

        private void InitializeCollisions()
        {
            var playerVsGroundCasted =
                CollisionManager.Self.CreateTileRelationship(PlayerList, GroundCollision);
            playerVsGroundCasted.CollisionOccurred += (player, ground) => player.IsOnGround = true;
            playerVsGround = playerVsGroundCasted;
            // inactivate so we can manually call collision
            playerVsGround.IsActive = false;

            var playerVsDamageArea = CollisionManager.Self.CreateRelationship(PlayerList, DamageAreaList);
            playerVsDamageArea.CollisionOccurred += HandlePlayerVsDamageArea;

        }


        #region Activity

        void CustomActivity(bool firstTimeCalled)
		{
            CollisionActivity();

		}

        private void CollisionActivity()
        {
            foreach(var player in PlayerList)
            {
                player.IsOnGround = false;
            }

            playerVsGround.DoCollisions();

            foreach(var player in PlayerList)
            {
                if(player.IsOnGround == false)
                {
                    DoFallOff(player);
                }
            }
        }

        private void HandlePlayerVsDamageArea(Player player, DamageArea damageArea)
        {

        }

        private void DoFallOff(Player player)
        {
            var randomSpawn = FlatRedBallServices.Random.In(RespawnList);

            player.XVelocity = 0;
            player.YVelocity = 0;
            player.XAcceleration = 0;
            player.YAcceleration = 0;

            player.X = randomSpawn.X;
            player.Y = randomSpawn.Y;
        }

        #endregion

        void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
