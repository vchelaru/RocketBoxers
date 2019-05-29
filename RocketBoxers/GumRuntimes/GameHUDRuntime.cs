using System;
using System.Collections.Generic;
using System.Linq;

namespace RocketBoxers.GumRuntimes
{
    public partial class GameHUDRuntime
    {
        public IList<PlayerGameIconRuntime> PlayerGameIcons =>
            this.ContainerInstance.Children
            .Select(item => (PlayerGameIconRuntime)item)
            .ToList();
        partial void CustomInitialize()
        {
        }
    }
}
