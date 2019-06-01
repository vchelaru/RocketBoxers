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

namespace RocketBoxers.Screens
{
	public partial class WrapUpScreen
	{

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

            if(numberOfPlayers == 0)
            {
                numberOfPlayers = 2;// for debugging
            }

            var children = WrapUpInstance.Children;

            for(int i = 0; i < children.Count; i++)
            {
                (children[i] as GraphicalUiElement).Visible =
                    i < numberOfPlayers;
            }
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
