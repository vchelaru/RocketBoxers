using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;

namespace RocketBoxers.Entities
{
	public partial class DamageArea
	{
        List<Player> damagedPlayers = new List<Player>();
        public Player OwningPlayer;
        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{
            Collision.Visible = true;
		}

		private void CustomActivity()
		{

		}

		private void CustomDestroy()
		{


		}

        public bool TryToDamagePlayer(Player player)
        {
            bool toReturn = false;

            if(!damagedPlayers.Contains(player))
            {
                toReturn = true;
                damagedPlayers.Add(player);
                player.TakeHit(AttackData, OwningPlayer.Position);
            }

            return toReturn;
        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
