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
using RocketBoxers.Input;

namespace RocketBoxers.Screens
{
	public partial class PlayerSelectScreen
	{

        // It's easier to just make them all
        // up front and check if they've joined
        // to not duplicate confirm/back logic:
        List<UiInputDevice> AllInputDevices;
        List<UiInputDevice> JoinedInputDevices;

		void CustomInitialize()
		{
            CreateAllInputDevices();

            JoinedInputDevices = new List<UiInputDevice>();

		}

        private void CreateAllInputDevices()
        {
            AllInputDevices = new List<UiInputDevice>();

            AllInputDevices.Add(UiInputDevice.FromKeyboard());

            AllInputDevices.Add(UiInputDevice.FromGamepad(InputManager.Xbox360GamePads[0]));
            AllInputDevices.Add(UiInputDevice.FromGamepad(InputManager.Xbox360GamePads[1]));
            AllInputDevices.Add(UiInputDevice.FromGamepad(InputManager.Xbox360GamePads[2]));
            AllInputDevices.Add(UiInputDevice.FromGamepad(InputManager.Xbox360GamePads[3]));
        }

        void CustomActivity(bool firstTimeCalled)
		{
            CheckForJoiningCharacters();

		}

        private void CheckForJoiningCharacters()
        {
            var possibleList = AllInputDevices.Except(JoinedInputDevices).ToList();
            foreach (var possible in possibleList)
            {
                if(possible.Confirm.WasJustPressed)
                {
                    JoinWith(possible);
                }
            }
        }

        private void JoinWith(UiInputDevice device)
        {
            // todo: turn on the matching object:


            JoinedInputDevices.Add(device);
        }

        void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
