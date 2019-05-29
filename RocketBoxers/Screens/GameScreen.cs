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
using RocketBoxers.Input;

namespace RocketBoxers.Screens
{
	public partial class GameScreen
	{
        public static List<UiInputDevice> PlayerInputDevices = new List<UiInputDevice>();

        CollisionRelationship playerVsGround;

        void CustomInitialize()
        {
            AddInputDevicesIfEmpty();

            InitializePlayers();

            InitializeCollisions();

        }

        private void AddInputDevicesIfEmpty()
        {
            // this means the game is skipping the join screen
            if(PlayerInputDevices.Count == 0)
            {
                var inputDevice = new UiInputDevice();
                inputDevice.BackingObject = InputManager.Keyboard;
                PlayerInputDevices.Add(inputDevice);

                inputDevice = new UiInputDevice();
                inputDevice.BackingObject = InputManager.Xbox360GamePads[0];
                PlayerInputDevices.Add(inputDevice);

            }
        }

        private void InitializePlayers()
        {
            int index = 0;
            foreach (var device in PlayerInputDevices)
            {
                var player = new Player();

                player.X = 1000;
                player.Y = -500 - 100;

                player.SetAnimationsFromPlayerIndex(index);

                player.InitializeInputFrom(device.BackingObject);

                this.PlayerList.Add(player);



                index++;
            }
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

            var playerVsPlayerCollision = CollisionManager.Self.CreateRelationship(PlayerList, PlayerList);
            playerVsPlayerCollision.SetMoveCollision(1, 1);
        }


        #region Activity

        void CustomActivity(bool firstTimeCalled)
		{
            CollisionActivity();

            DebugActivity();
		}

        private void DebugActivity()
        {
            //FlatRedBall.Debugging.Debugger.Write(DamageAreaList.Count);
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
            if(player.TeamIndex != damageArea.TeamIndex)
            {
                damageArea.TryToDamagePlayer(player);
            }
        }

        private void DoFallOff(Player player)
        {
            var randomSpawn = FlatRedBallServices.Random.In(RespawnList);

            player.PerformFallOff(randomSpawn);
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
