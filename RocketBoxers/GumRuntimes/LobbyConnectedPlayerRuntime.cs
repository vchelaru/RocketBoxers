using System;
using System.Collections.Generic;
using System.Linq;

namespace RocketBoxers.GumRuntimes
{
    public partial class LobbyConnectedPlayerRuntime
    {
        public Lidgren.Network.NetConnection Connection
        {
            get; set;
        }

        public string OverridingName
        {
            get; set;
        }

        partial void CustomInitialize()
        {
        }

        public void UpdateText()
        {
            if (!string.IsNullOrEmpty(OverridingName))
            {
                Text = OverridingName;
            }
            else
            {
                Text = Connection?.RemoteEndPoint.Address.ToString();
            }
        }
    }
}
