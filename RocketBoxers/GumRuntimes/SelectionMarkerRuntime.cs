using Gum.Wireframe;
using RocketBoxers.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RocketBoxers.GumRuntimes
{
    public partial class SelectionMarkerRuntime
    {

        public enum SelectionState
        {
            Invisible,
            Selecting,
            LockedIn
        }

        private SelectionState currentSelectionStateButUseProperty;
        public SelectionState CurrentSelectionState
        {
            get => currentSelectionStateButUseProperty;
            set
            {
                currentSelectionStateButUseProperty = value;
                this.Visible = currentSelectionStateButUseProperty != SelectionState.Invisible;
            }
        }

        public UiInputDevice InputDevice { get; set; }

        public SelectedCharacterFrameRuntime CharacterFrame { get; set; }

        partial void CustomInitialize()
        {
        }

        public void CustomActivity()
        {
            var index = this.Parent.Parent.Children.IndexOf(this.Parent);

            const int columnCount = 2;

            const int rowCount = 2;

            var row = index / columnCount;
            var column = index % columnCount;

            var changed = false;
            if(InputDevice.Up.WasJustPressed)
            {
                row--;
                changed = true;
            }
            if (InputDevice.Down.WasJustPressed)
            {
                row++;
                changed = true;

            }
            if (InputDevice.Left.WasJustPressed)
            {
                column--;
                changed = true;

            }
            if (InputDevice.Right.WasJustPressed)
            {
                column++;
                changed = true;
            }

            if(changed)
            {
                if(column < 0)
                {
                    column = columnCount - 1;
                }
                if(row < 0)
                {
                    row = rowCount - 1;
                }
                row = row % rowCount;
                column = column % columnCount;

                var newIndex = column + row * columnCount;

                this.Parent = this.Parent.Parent.Children[newIndex];

                switch(newIndex)
                {
                    case 0:
                        CharacterFrame.CurrentCharacterNumberState = SelectedCharacterFrameRuntime.CharacterNumber.Character1;
                        break;
                    case 1:
                        CharacterFrame.CurrentCharacterNumberState = SelectedCharacterFrameRuntime.CharacterNumber.Character2;
                        break;
                    case 2:
                        CharacterFrame.CurrentCharacterNumberState = SelectedCharacterFrameRuntime.CharacterNumber.Character3;
                        break;
                    case 3:
                        CharacterFrame.CurrentCharacterNumberState = SelectedCharacterFrameRuntime.CharacterNumber.Character4;
                        break;
                }

                CharacterFrame.CharacterSelectionAnimation.Play();
            }


        }
    }
}
