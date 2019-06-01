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

namespace RocketBoxers.Screens
{
	public partial class WrapUpScreen
	{
        List<WarpUpSatsRuntime> statsRuntime = new List<WarpUpSatsRuntime>();

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

            for(int i = 0; i < statsRuntime.Count; i++)
            {
                statsRuntime[i].Visible =
                    i < numberOfPlayers;

                if(i < GameScreen.PlayerInputDevices.Count)
                {
                    statsRuntime[i].Device = GameScreen.PlayerInputDevices[i];
                }
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
