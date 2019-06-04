using RocketBoxers.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketBoxers.Gameplay
{
    class AllStats
    {
        public static List<IndividualStats> Stats { get; private set; } = new List<IndividualStats>();

        public static IndividualStats For(Player player)
        {
            var color = player.InputDevice.Color;

            return Stats[(int)color];
        }

        public static void Reset()
        {
            Stats.Clear();

            Stats.Add(new IndividualStats());
            Stats.Add(new IndividualStats());
            Stats.Add(new IndividualStats());
            Stats.Add(new IndividualStats());
        }
    }
}
