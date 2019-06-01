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
        struct PlayerHitData
        {
            public int RemainingHits;
            public double LastHitTime;
        }

        Dictionary<Player, PlayerHitData> damagedPlayers = new Dictionary<Player, PlayerHitData>();
        public Player OwningPlayer;
        bool isActive = true;
        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{
		}

		private void CustomActivity()
		{

		}

		private void CustomDestroy()
		{
            // Deal launching damage on any remaining hits
            foreach(var hitPlayers in damagedPlayers)
            {
                //Only perform last hit if a plyer is still colliding with the collision
                if (this.CollideAgainst(hitPlayers.Key))
                {
                    if (hitPlayers.Value.RemainingHits > 0)
                    {
                        hitPlayers.Key.TakeHit(AttackData, OwningPlayer.Position, true);
                    }
                }
            }

		}

        public bool TryToDamagePlayer(Player player)
        {
            bool canDealDamage = false;

            if (isActive)
            {
                if (!player.IsInvincible && !player.IsFalling)
                {

                    //Check if the player has been hit
                    double lastHitTime = FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedCurrentTime;
                    PlayerHitData currentHit = new PlayerHitData();
                    if (!damagedPlayers.ContainsKey(player))
                    {
                        canDealDamage = true;
                        int remainingHits = AttackData.HitsAllowed;
                        currentHit = new PlayerHitData() { RemainingHits = remainingHits, LastHitTime = lastHitTime };
                        damagedPlayers.Add(player, currentHit);

                    }
                    else if (CanDealDamage(player))
                    {
                        canDealDamage = true;
                        currentHit = damagedPlayers[player];
                        currentHit.LastHitTime = lastHitTime;
                    }

                    if (canDealDamage)
                    {
                        currentHit.RemainingHits--;
                        player.TakeHit(AttackData, OwningPlayer.Position, currentHit.RemainingHits <= 0);
                        damagedPlayers[player] = currentHit;
                    }
                }
            }
            
            return canDealDamage;
        }

        private bool CanDealDamage(Player player)
        {
            return FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedSecondsSince(damagedPlayers[player].LastHitTime) > AttackData.DelayBetweenHits;
        }

        public void DisableDamageArea()
        {
            isActive = false;
        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
