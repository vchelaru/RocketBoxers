using FlatRedBall.Input;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketBoxers.Input
{
    public enum PlayerColor
    {
        Yellow,
        Red,
        Green,
        Blue
    }

    public class UiInputDevice
    {
        /// <summary>
        /// The backing input object like a Keyboard or 
        /// Xbox360GamePad
        /// </summary>
        public object BackingObject { get; set; }

        public PlayerColor Color { get; set; }

        public IPressableInput Up { get; set; }
        public IPressableInput Down { get; set; }
        public IPressableInput Left { get; set; }
        public IPressableInput Right { get; set; }

        public IPressableInput LeftTrigger { get; set; }
        public IPressableInput RightTrigger { get; set; }

        public IPressableInput Confirm { get; set; }
        public IPressableInput Back { get; set; }

        // do we need a "start"?

        public static UiInputDevice FromKeyboard()
        {
            var toReturn = new UiInputDevice();
            var keyboard = InputManager.Keyboard;

            toReturn.BackingObject = keyboard;

            toReturn.Up = keyboard.GetKey(Keys.Up);
            toReturn.Down = keyboard.GetKey(Keys.Down);
            toReturn.Left= keyboard.GetKey(Keys.Left);
            toReturn.Right= keyboard.GetKey(Keys.Right);

            toReturn.Confirm = keyboard.GetKey(Keys.Enter);
            toReturn.Back = keyboard.GetKey(Keys.Escape)
                .Or(keyboard.GetKey(Keys.Back));

            toReturn.LeftTrigger = keyboard.GetKey(Keys.A);
            toReturn.RightTrigger = keyboard.GetKey(Keys.D);

            return toReturn;
        }

        public static UiInputDevice FromGamepad(Xbox360GamePad gamePad)
        {
            var toReturn = new UiInputDevice();

            toReturn.BackingObject = gamePad;

            toReturn.Up = gamePad.GetButton(Xbox360GamePad.Button.DPadUp)
                .Or(gamePad.LeftStick.UpAsButton);

            toReturn.Down = gamePad.GetButton(Xbox360GamePad.Button.DPadDown)
                .Or(gamePad.LeftStick.DownAsButton);

            toReturn.Left = gamePad.GetButton(Xbox360GamePad.Button.DPadLeft)
                .Or(gamePad.LeftStick.LeftAsButton);

            toReturn.Right = gamePad.GetButton(Xbox360GamePad.Button.DPadRight)
                .Or(gamePad.LeftStick.RightAsButton);

            toReturn.Confirm = gamePad.GetButton(Xbox360GamePad.Button.Start)
                .Or(gamePad.GetButton(Xbox360GamePad.Button.A));
            toReturn.Back = gamePad.GetButton(Xbox360GamePad.Button.Back)
                .Or(gamePad.GetButton(Xbox360GamePad.Button.B));

            toReturn.LeftTrigger = gamePad.GetButton(Xbox360GamePad.Button.LeftTrigger);
            toReturn.RightTrigger = gamePad.GetButton(Xbox360GamePad.Button.RightTrigger);
            return toReturn;

        }

        public void Clear()
        {
            if(BackingObject is FlatRedBall.Input.Keyboard keyboard)
            {
                keyboard.Clear();
            }
            else if(BackingObject is Xbox360GamePad gamePad)
            {
                gamePad.Clear();
            }
        }

    }
}
