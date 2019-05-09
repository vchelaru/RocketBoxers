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
	public partial class Player
	{

        AnimationController animationController;

        AnimationLayer getHitAnimationLayer;

        public bool IsOnGround { get; set; }
        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{
            this.InitializeInput();
            this.mCurrentMovement = TopDownValues[DataTypes.TopDownValues.Normal];

            InitializeAnimations();
            


		}

        private void InitializeAnimations()
        {
            animationController = new AnimationController(SpriteInstance);

            var idleAnimationLayer = new AnimationLayer();
            idleAnimationLayer.EveryFrameAction = () =>
            {
                switch(this.DirectionFacing)
                {
                    case TopDownDirection.Up:
                        return "IdleUp";
                    case TopDownDirection.UpRight:
                        return "IdleUpRight";
                    case TopDownDirection.Right:
                        return "IdleRight";
                        // todo - continue here...
                    default:
                        return null;
                }
            };
            animationController.Layers.Add(idleAnimationLayer);

            const float WalkingDeadzone = .1f;
            var walkAnimationLayer = new AnimationLayer();
            walkAnimationLayer.EveryFrameAction = () =>
            {
                if (MovementInput.Magnitude > WalkingDeadzone)
                {
                    switch (this.DirectionFacing)
                    {
                        case TopDownDirection.Up:
                            return "WalkUp";
                        case TopDownDirection.UpRight:
                            return "WalkUpRight";
                        case TopDownDirection.Right:
                            return "WalkRight";
                        // todo - continue here...
                        default:
                            return null;
                    }
                }
                else
                {
                    return null;
                }
            };
            animationController.Layers.Add(walkAnimationLayer);


            var attackAnimationLayer = new AnimationLayer();

            animationController.Layers.Add(attackAnimationLayer);


            getHitAnimationLayer = new AnimationLayer();

            animationController.Layers.Add(getHitAnimationLayer);
        }

        private void CustomActivity()
		{


		}


        public void TakeHit()
        {
            string animationToPlay = null;

            switch(DirectionFacing)
            {
                case TopDownDirection.Right:
                    animationToPlay = "GetHitFacingRight";
                    break;
            }

            getHitAnimationLayer.PlayOnce(animationToPlay);
        }

		private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
