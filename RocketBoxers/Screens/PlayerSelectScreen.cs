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
using Gum.Wireframe;
using RocketBoxers.GumRuntimes;

namespace RocketBoxers.Screens
{
	public partial class PlayerSelectScreen
	{
        #region Fields/Properties

        // It's easier to just make them all
        // up front and check if they've joined
        // to not duplicate confirm/back logic:
        List<UiInputDevice> AllInputDevices;
        List<UiInputDevice> JoinedInputDevices;

        List<SelectionMarkerRuntime> SelectionMarkers;

        #endregion

        #region Initialize

        void CustomInitialize()
		{
            CreateAllInputDevices();

            InitializeUi();

            JoinedInputDevices = new List<UiInputDevice>();

		}

        private void InitializeUi()
        {
            // So we can access them regardless of parent:
            SelectionMarkers = MarkerContainer
                .Children
                .Select(item => (SelectionMarkerRuntime)item).ToList();
            foreach(GraphicalUiElement marker in SelectionMarkers)
            {
                marker.Visible = false;
                marker.X = 0;
                marker.Y = 0;
            }

            foreach(SelectedCharacterFrameRuntime item in BottomFrameContainer.Children)
            {
                item.CurrentJoinStateState = SelectedCharacterFrameRuntime.JoinState.NotJoined;
            }
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

        #endregion

        #region Activity

        void CustomActivity(bool firstTimeCalled)
		{

            DoCursorMoveActivity();

            // check for locking in before checking for joining or else joining will do both join and lock in
            CheckForLockingIn();

            CheckForJoiningInputDevices();
		}

        private void DoCursorMoveActivity()
        {
            foreach(var marker in SelectionMarkers)
            {
                if(marker.CurrentSelectionState == SelectionMarkerRuntime.SelectionState.Selecting)
                {
                    marker.CustomActivity();
                }
            }
        }

        private void CheckForJoiningInputDevices()
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
            var firstAvailableMarker = SelectionMarkers
                .FirstOrDefault(item => 
                    item.CurrentSelectionState == SelectionMarkerRuntime.SelectionState.Invisible);

            var firstAvailableFrame = BottomFrameContainer
                .Children
                .FirstOrDefault(item => ((SelectedCharacterFrameRuntime)item).CurrentJoinStateState == SelectedCharacterFrameRuntime.JoinState.NotJoined)
                as SelectedCharacterFrameRuntime;

            // If there are no markers available, the player can't join because we only support 4 characters
            if (firstAvailableMarker != null)
            {
                firstAvailableMarker.CurrentSelectionState = SelectionMarkerRuntime.SelectionState.Selecting;
                JoinedInputDevices.Add(device);
                firstAvailableMarker.InputDevice = device;

                firstAvailableMarker.Parent = CharacterGridInstance.Children[0];
                firstAvailableMarker.CharacterFrame = firstAvailableFrame;

                firstAvailableFrame.CurrentJoinStateState = SelectedCharacterFrameRuntime.JoinState.Joined;
                firstAvailableFrame.CurrentCharacterSelectAnimationStatesState = SelectedCharacterFrameRuntime.CharacterSelectAnimationStates.NoSelection;
                firstAvailableFrame.CharacterSelectionAnimation.Play();
            }

        }

        private void CheckForLockingIn()
        {
            foreach (var marker in SelectionMarkers)
            {
                if (marker.CurrentSelectionState == SelectionMarkerRuntime.SelectionState.Selecting &&
                    marker.InputDevice.Confirm.WasJustPressed)
                {
                    LockInCharacter(marker);
                }
            }
        }

        private void LockInCharacter(SelectionMarkerRuntime marker)
        {
            marker.CurrentSelectionState = SelectionMarkerRuntime.SelectionState.LockedIn;
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
