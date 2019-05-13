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
    public enum AttackCollisionShapeType
    {
        Circle,
        Rectangle
    }

	public partial class Player
	{

        AnimationController animationController;

        AnimationLayer getHitAnimationLayer;
        AnimationLayer attackAnimationLayer;

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
            InitializeAnimationInstructions();
        }

        private void InitializeAnimationInstructions()
        {
            foreach(var attack in AttackData.Values)
            {
                foreach(var chain in SpriteInstance.AnimationChains.FindAll((x) => x.Name.Contains(attack.Name)))
                {
                    if (chain.TotalLength > attack.CollisionSpawnFrame && chain.TotalLength > attack.CollisionDestroyFrame)
                    {
                        chain[attack.CollisionSpawnFrame].Instructions.Add(new DelegateInstruction(() =>
                        {
                            var damageArea = Factories.DamageAreaFactory.CreateNew();
                            SetupAttackDamageArea(damageArea, attack);

                            chain[attack.CollisionDestroyFrame].Instructions.Clear();
                            chain[attack.CollisionDestroyFrame].Instructions.Add(new DelegateInstruction(() =>
                            {
                                damageArea.Destroy();
                            }));
                        }));
                    }
                    else
                    {
                        throw new Exception("");
                    }
                }
            }
        }

        private void InitializeAnimations()
        {
            animationController = new AnimationController(SpriteInstance);

            var idleAnimationLayer = new AnimationLayer();
            idleAnimationLayer.EveryFrameAction = () =>
            {
                return MakeAnimationChainName("Idle");
            };
            animationController.Layers.Add(idleAnimationLayer);

            const float WalkingDeadzone = .1f;
            var walkAnimationLayer = new AnimationLayer();
            walkAnimationLayer.EveryFrameAction = () =>
            {
                if (MovementInput.Magnitude > WalkingDeadzone)
                {
                    return MakeAnimationChainName("Walk");
                }
                return null;
            };
            animationController.Layers.Add(walkAnimationLayer);


            attackAnimationLayer = new AnimationLayer();

            animationController.Layers.Add(attackAnimationLayer);


            getHitAnimationLayer = new AnimationLayer();

            animationController.Layers.Add(getHitAnimationLayer);
        }

        private void CustomActivity()
		{
            //Uncomment once animations are created.
            //animationController.Activity();
        }

        public void TakeHit()
        {
            getHitAnimationLayer.PlayOnce(MakeAnimationChainName("TakeHit"));
        }

        private void BeginAttack()
        {
            attackAnimationLayer.PlayOnce(MakeAnimationChainName("Attack"));
        }

        private void SetupAttackDamageArea(DamageArea newDamageArea, DataTypes.AttackData attackData)
        {

        }

		private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        private string MakeAnimationChainName(string baseName)
        {
            switch (this.DirectionFacing)
            {
                case TopDownDirection.Up:
                    return $"{baseName}Up";
                case TopDownDirection.UpRight:
                    return $"{baseName}UpRight";
                case TopDownDirection.Right:
                    return $"{baseName}Right";
                case TopDownDirection.DownRight:
                    return $"{baseName}DownRight";
                case TopDownDirection.Down:
                    return $"{baseName}Down";
                case TopDownDirection.DownLeft:
                    return $"{baseName}DownLeft";
                case TopDownDirection.Left:
                    return $"{baseName}Left";
                case TopDownDirection.UpLeft:
                    return $"{baseName}UpLeft";
            }

            return null;
        }
	}
}
