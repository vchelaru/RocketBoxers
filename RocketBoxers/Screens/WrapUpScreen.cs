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
using Gum.Wireframe;
using RocketBoxers.GumRuntimes;
using RocketBoxers.Input;
using RocketBoxers.Gameplay;

namespace RocketBoxers.Screens
{
	public partial class WrapUpScreen
	{
        #region Fields/Properties

        List<WarpUpSatsRuntime> statsRuntime = new List<WarpUpSatsRuntime>();

        #endregion

        void CustomInitialize()
		{
            InitializeUi();

            // give it time to load:
            WrapUpInstance.CurrentScreenLocationState = GumRuntimes.WrapUpRuntime.ScreenLocation.Off;
            this.Call(() =>
            {
                WrapUpInstance.AnimateInStatsAnimation.Play();


            }).After(.6f);
		}

        private void InitializeUi()
        {
            var numberOfPlayers = GameScreen.PlayerInputDevices.Count;

            var children = WrapUpInstance.Children;

            statsRuntime = children
                .Select(item => item as WarpUpSatsRuntime)
                .ToList();

            Dictionary<UiInputDevice, double> deaths = new Dictionary<UiInputDevice, double>();


            for(int i = 0; i < statsRuntime.Count; i++)
            {
                statsRuntime[i].Visible =
                    i < numberOfPlayers;

                if(i < GameScreen.PlayerInputDevices.Count)
                {
                    statsRuntime[i].Device = GameScreen.PlayerInputDevices[i];
                    statsRuntime[i].CurrentCharacterColorState =
                        (WarpUpSatsRuntime.CharacterColor)GameScreen.PlayerInputDevices[i].Color;
                    statsRuntime[i].CurrentPlayerNumberState =
                        (WarpUpSatsRuntime.PlayerNumber)i;
                    var stats = AllStats.For(GameScreen.PlayerInputDevices[i].Color);
                    deaths.Add(GameScreen.PlayerInputDevices[i],
                        stats.TimeDied);
                }
            }

            var kvps = deaths
                .OrderBy(item => item.Value > 0 )
                .ThenByDescending(item => item.Value)
                .ToList();

            WarpUpSatsRuntime StatsFrom(UiInputDevice device) =>
                statsRuntime.FirstOrDefault(item => item.Device.Color == device.Color);

            if (kvps.Count > 0)
            {
                StatsFrom(kvps[0].Key).CurrentRankState = WarpUpSatsRuntime.Rank._1st;
            }
            if (kvps.Count > 1)
            {
                StatsFrom(kvps[1].Key).CurrentRankState = WarpUpSatsRuntime.Rank._2nd;
            }
            if (kvps.Count > 2)
            {
                StatsFrom(kvps[2].Key).CurrentRankState = WarpUpSatsRuntime.Rank._3rd;
            }
            if (kvps.Count > 3)
            {
                StatsFrom(kvps[3].Key).CurrentRankState = WarpUpSatsRuntime.Rank._4th;
            }
        }

        void CustomActivity(bool firstTimeCalled)
		{
            var areAllReady = statsRuntime
                .Where(item => item.Visible)
                .All(item => item.IsReady);

            var didAnyPush = statsRuntime.Any(item => item.Device?.Confirm.WasJustPressed == true);

            if(areAllReady && didAnyPush)
            {
                MoveToScreen(typeof(PlayerSelectScreen));
            }


            foreach(var item in statsRuntime)
            {
                item.CustomActivity();
            }
		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
