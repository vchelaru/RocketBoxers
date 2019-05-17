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



namespace RocketBoxers.Screens
{
	public partial class TitleScreen
	{
        RocketBoxers.GumRuntimes.TitleScreenGumRuntime castedGumScreen;

        void CustomInitialize()
		{
            castedGumScreen = (RocketBoxers.GumRuntimes.TitleScreenGumRuntime)TitleScreenGum;
            castedGumScreen.OptionSelected += HandleOptionSelected;

        }

        private void HandleOptionSelected(object sender, EventArgs e)
        {
            switch(castedGumScreen.SelectedOption)
            {
                case 0:
                    MoveToScreen(typeof(PlayerSelectScreen));
                    break;
                case 1:

                    break;

                case 2:

                    break;
            }
        }

        void CustomActivity(bool firstTimeCalled)
		{
            castedGumScreen.CustomActivity();

		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
