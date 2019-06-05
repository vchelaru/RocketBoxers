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
using RocketBoxers.Gameplay;
using FlatRedBall.TileCollisions;

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

            InitializeCamera();

            SpriteManager.OrderedSortType = FlatRedBall.Graphics.SortType.ZSecondaryParentY;

            StartCountdown();
        }

        private void InitializeCamera()
        {
            Camera.Main.X = 1000;
            Camera.Main.Y = -500;
        }

        private void StartCountdown()
        {
            foreach(var player in PlayerList)
            {
                player.InputEnabled = false;
            }

            GameCountdownInstance.CurrentAnimationStatesState = GumRuntimes.GameCountdownRuntime.AnimationStates.ThreeStart;
            this.Call(() =>
            {
                GameCountdownInstance.CountdownThreeAnimation.Play();
                GameCountdownInstance.CountdownTwoAnimation.PlayAfter(1);
                GameCountdownInstance.CountdownOneAnimation.PlayAfter(2);
                GameCountdownInstance.CountdownGoAnimation.PlayAfter(3);

                this.Call(() =>
                {
                    for(int i = 0; i < PlayerInputDevices.Count; i++)
                    {
                        var player = PlayerList[i];
                        player.InitializeInputFrom(
                            PlayerInputDevices[i]);
                        player.InputEnabled = true;
                    }
                }).After(3);

            }).After(1);



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
            AllStats.Reset();

            foreach (var device in PlayerInputDevices)
            {
                var player = new Player();

                player.RequestRespawn += HandleRequestRespawn;

                player.X = 1000;
                player.Y = -500 - 100;
                player.TeamIndex = PlayerList.Count;
                player.StockCount = MaxStockCount;

                player.SetAnimationsFromPlayerIndex((int)device.Color);

                // don't do it yet, do it after the game starts:
                //player.InitializeInputFrom(device.BackingObject);

                player.MoveToLayer(PlayerLayer);
                

                this.PlayerList.Add(player);
            }
        }

        private void HandleRequestRespawn(Player player)
        {
            player.StockCount--;
            if (player.StockCount == 0)
            {
                AllStats.For(player).TimeDied = this.PauseAdjustedCurrentTime;
            }
            else
            {
                RefreshDamageDisplay(player);

                RefreshStockDisplay(player);

                player.TryToRespawn();
            }
        }

        private void InitializeCollisions()
        {
            //GroundCollision.AddCollisionFromTilesWithProperty(TestLevel, "Ground");
            GroundCollision.AddMergedCollisionFromTilesWithProperty(MainTileMap, "Ground");
            WallCollision.AddCollisionFromTilesWithProperty(MainTileMap, "Wall");

            GroundCollision.Visible = false;

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

            float damageMultiplied = player.DamageTaken * 100;
            
            iconToUpdate.DisplayedPercentage = Math.Ceiling(damageMultiplied).ToString();
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
