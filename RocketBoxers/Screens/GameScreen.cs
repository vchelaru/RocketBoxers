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
        #region Fields/Properties

        public static List<UiInputDevice> PlayerInputDevices = new List<UiInputDevice>();

        CollisionRelationship playerVsGround;

        #endregion

        #region Initialize Methods

        void CustomInitialize()
        {
            AddInputDevicesIfEmpty();

            InitializePlayers();

            InitializeCollisions();

            InitializeUi();

            SpriteManager.OrderedSortType = FlatRedBall.Graphics.SortType.ZSecondaryParentY;
        }

        private void InitializeUi()
        {
            GameHUDInstance.CurrentNumberOfPlayersState =
                (GumRuntimes.GameHUDRuntime.NumberOfPlayers)(PlayerList.Count - 1);

            for(int i = 0; i < PlayerInputDevices.Count; i++)
            {
                var icon = GameHUDInstance.PlayerGameIcons[i];
                icon.CurrentPlayerColorState = (GumRuntimes.PlayerGameIconRuntime.PlayerColor)(PlayerInputDevices[i].Color);

                icon.DisplayedPercentage = "0";
                icon.SetStockCount(PlayerList[i].StockCount);
            }
        }

        private void AddInputDevicesIfEmpty()
        {
            // this means the game is skipping the join screen
            if(PlayerInputDevices.Count == 0)
            {
                var inputDevice = UiInputDevice.FromKeyboard();
                inputDevice.Color = PlayerColor.Yellow;
                PlayerInputDevices.Add(inputDevice);

                inputDevice = UiInputDevice.FromGamepad(
                    InputManager.Xbox360GamePads[0]);
                inputDevice.Color = PlayerColor.Red;
                PlayerInputDevices.Add(inputDevice);

            }
        }

        private void InitializePlayers()
        {
            foreach (var device in PlayerInputDevices)
            {
                var player = new Player();

                player.RequestRespawn += HandleRequestRespawn;

                player.X = 1000;
                player.Y = -500 - 100;
                player.TeamIndex = PlayerList.Count;
                player.StockCount = MaxStockCount;

                player.SetAnimationsFromPlayerIndex((int)device.Color);

                player.InitializeInputFrom(device.BackingObject);

                player.MoveToLayer(PlayerLayer);

                this.PlayerList.Add(player);
            }
        }

        private void HandleRequestRespawn(Player player)
        {
            player.TryToRespawn();

            RefreshDamageDisplay(player);
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

        #endregion

        #region Activity

        void CustomActivity(bool firstTimeCalled)
		{
            CollisionActivity();

            DebugActivity();

            DoEndLevelActivity();
        }

        private void DoEndLevelActivity()
        {
            var numberOfLivingPlayers = PlayerList.Count(item => item.StockCount > 0);

            if(PlayerList.Count == 1 && numberOfLivingPlayers == 0)
            {
                DoEndLevel();
            }
            else if(PlayerList.Count > 1 && numberOfLivingPlayers < 2)
            {
                DoEndLevel();
            }
        }

        private void DoEndLevel()
        {
            MoveToScreen(typeof(WrapUpScreen));
        }

        private void DebugActivity()
        {
            string debugString = "";
            foreach(var player in PlayerList)
            {
                debugString += $"Player {player.TeamIndex} - health {player.DamageTaken}\n";
            }
            FlatRedBall.Debugging.Debugger.Write(debugString);
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
                if(player.IsOnGround == false && player.IsFalling == false)
                {
                    DoFallOff(player);
                }
            }
        }

        private void HandlePlayerVsDamageArea(Player player, DamageArea damageArea)
        {
            if(player.TeamIndex != damageArea.TeamIndex)
            {
                var dealtDamage = damageArea.TryToDamagePlayer(player);

                if(dealtDamage)
                {
                    var index = PlayerList.IndexOf(player);

                    var iconToUpdate = GameHUDInstance.PlayerGameIcons[index];

                    iconToUpdate.PercentDamageBounceAnimation.Play();

                    RefreshDamageDisplay(player);
                }
            }
        }

        private void RefreshDamageDisplay(Player player)
        {
            var index = PlayerList.IndexOf(player);
            var iconToUpdate = GameHUDInstance.PlayerGameIcons[index];

            iconToUpdate.DisplayedPercentage = ((int)(player.DamageTaken * 100)).ToString();
        }
        private void RefreshStockDisplay(Player player)
        {
            var index = PlayerList.IndexOf(player);
            var iconToUpdate = GameHUDInstance.PlayerGameIcons[index];

            iconToUpdate.SetStockCount(player.StockCount);
        }

        private void DoFallOff(Player player)
        {
            var randomSpawn = FlatRedBallServices.Random.In(RespawnList);

            player.PerformFallOff(randomSpawn);

            this.Call(() =>
            {
                player.StockCount--;
                RefreshDamageDisplay(player);

                RefreshStockDisplay(player);

            }).After(Player.FallingDuration);

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
