using FlatRedBall.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RocketBoxers.GumRuntimes
{
    public partial class TitleScreenGumRuntime
    {
        public int? SelectedOption
        {
            get
            {
                for (int i = 0; i < OptionsContainer.Children.Count; i++)
                {
                    var optionItem = (MainMenuSelectableTextRuntime)OptionsContainer.Children[i];

                    if (optionItem.CurrentSelectionCategoryState == MainMenuSelectableTextRuntime.SelectionCategory.Selected)
                    {
                        return i;
                    }
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException("Can't set the selected value to null");
                }


                for (int i = 0; i < OptionsContainer.Children.Count; i++)
                {
                    var optionItem = (MainMenuSelectableTextRuntime)OptionsContainer.Children[i];

                    if(i == value)
                    {
                        optionItem.CurrentSelectionCategoryState = MainMenuSelectableTextRuntime.SelectionCategory.Selected;
                    }
                    else
                    {
                        optionItem.CurrentSelectionCategoryState = MainMenuSelectableTextRuntime.SelectionCategory.Normal;
                    }
                }
            }
        }
        partial void CustomInitialize()
        {
        }

        public void CustomActivity()
        {
            if(PushedDown())
            {
                SelectedOption = (SelectedOption+1) % 3;
            }
            if(PushedUp())
            {
                var newValue = SelectedOption - 1;
                if(newValue < 0)
                {
                    newValue = 2; // hardcode this? Sure, prob won't change...
                }
            }
        }

        private bool PushedDown()
        {
            var keyboard = InputManager.Keyboard;

            if(keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                return true;
            }
            else
            {
                return InputManager.Xbox360GamePads
                    .Any(item => 
                        item.LeftStick.AsDPadPushed(Xbox360GamePad.DPadDirection.Down) || 
                        item.ButtonPushed(Xbox360GamePad.Button.DPadDown));
            }

        }

        private bool PushedUp()
        {
            var keyboard = InputManager.Keyboard;

            if (keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                return true;
            }
            else
            {
                return InputManager.Xbox360GamePads
                    .Any(item =>
                        item.LeftStick.AsDPadPushed(Xbox360GamePad.DPadDirection.Up) ||
                        item.ButtonPushed(Xbox360GamePad.Button.DPadUp));
            }
        }
    }
}
