using RocketBoxers.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RocketBoxers.GumRuntimes
{
    public partial class WarpUpSatsRuntime
    {

        public bool IsReady { get; set; }

        public UiInputDevice Device { get; set; }

        partial void CustomInitialize()
        {
        }

        public void CustomActivity()
        {
            if(this.Visible && Device != null)
            {
                if(IsReady == false && Device.Confirm.WasJustPressed)
                {
                    IsReady = true;
                    this.CurrentReadyOrNotState = ReadyOrNot.Ready;
                    Device.Clear();
                }
            }
        }
    }




}
