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
using Microsoft.Xna.Framework;

namespace RocketBoxers.Entities
{
    public enum AttackCollisionShapeType
    {
        Circle,
        Rectangle
    }

	public partial class Player
	{
        //SpriteInstance animation controller
        AnimationController animationController;
        AnimationLayer attackHoldAnimationLayer;
        AnimationLayer attackAnimationLayer;
        AnimationLayer getHitAnimationLayer;

        //AttackEffect animation controllerS
        AnimationController attackSpriteAnimationController;
        AnimationLayer attackEffectAnimationLayer;

        private Vector3 effectSpriteRelativeOffset;

        IPressableInput attackInput;
        IPressableInput specialAttackInpt;
        //IPressableInput dashInput;
        IPressableInput blockInput;

        DamageArea currentAttackDamageArea;

        public bool IsOnGround { get; set; }
        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{
            //DELETE THIS AFTER SETTING UP LAYERS
            this.Z = 10;
            this.InitializeInput();
            this.mCurrentMovement = TopDownValues[DataTypes.TopDownValues.Normal];
            this.PossibleDirections = PossibleDirections.EightWay;

            InitializeButtonInput();
            InitializeAnimations();
        }

        private void InitializeButtonInput()
        {
            if (InputManager.Xbox360GamePads[0].IsConnected)
            {
                var gamePad = InputManager.Xbox360GamePads[0];

                attackInput = gamePad.GetButton(Xbox360GamePad.Button.A);
                blockInput = gamePad.GetButton(Xbox360GamePad.Button.B);
                specialAttackInpt = gamePad.GetButton(Xbox360GamePad.Button.X);
            }
            else
            {
                attackInput = InputManager.Keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.A);
                blockInput = InputManager.Keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.LeftShift);
                specialAttackInpt = InputManager.Keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.S);
            }
        }

        private void InitializeAnimations()
        {
            InitilizeSpriteInstanceController();
            InitilizeAttackSpriteController();
        }

        private void InitilizeAttackSpriteController()
        {
            attackSpriteAnimationController = new AnimationController(AttackEffectSprite);

            var attackEffectDefaultLayer = new AnimationLayer();
            attackEffectDefaultLayer.EveryFrameAction = () => {return "NotActive";};
            attackSpriteAnimationController.Layers.Add(attackEffectDefaultLayer);

            attackEffectAnimationLayer = new AnimationLayer();
            attackSpriteAnimationController.Layers.Add(attackEffectAnimationLayer);
        }

        private void InitilizeSpriteInstanceController()
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
            attackAnimationLayer.OnAnimationFinished = () => BeginBasicAttack();
            animationController.Layers.Add(attackAnimationLayer);

            attackHoldAnimationLayer = new AnimationLayer();
            attackHoldAnimationLayer.OnAnimationFinished = () =>
            {
                mCurrentMovement = TopDownValues[DataTypes.TopDownValues.Normal];
            };
            animationController.Layers.Add(attackHoldAnimationLayer);

            getHitAnimationLayer = new AnimationLayer();
            animationController.Layers.Add(getHitAnimationLayer);

            var blockingLayer = new AnimationLayer();
            blockingLayer.EveryFrameAction = () =>
            {
                if (blockInput.IsDown)
                {
                    mCurrentMovement = TopDownValues[DataTypes.TopDownValues.Stopped];
                    return MakeAnimationChainName("Block");
                }
                else if(blockInput.WasJustReleased)
                {
                    mCurrentMovement = TopDownValues[DataTypes.TopDownValues.Normal];
                }

                return null;
            };

            animationController.Layers.Add(blockingLayer);
        }

        private void CustomActivity()
		{
            //Uncomment once animations are created.
            InputActivity();
            animationController.Activity();
            attackSpriteAnimationController.Activity();

            FlatRedBall.Debugging.Debugger.Write(mCurrentMovement.Name);
        }

        private void InputActivity()
        {
            if (attackInput.WasJustPressed && !attackHoldAnimationLayer.HasPriority)
            {
                attackAnimationLayer.PlayOnce(MakeAnimationChainName("Attack"));
                mCurrentMovement = TopDownValues[DataTypes.TopDownValues.Stopped];
            }
            if (specialAttackInpt.WasJustPressed && !attackHoldAnimationLayer.HasPriority)
            {
                BeginSpecialAttack();
                mCurrentMovement = TopDownValues[DataTypes.TopDownValues.Stopped];
            }
        }

        private void BeginBasicAttack()
        {
            var attackData = AttackData[DataTypes.AttackData.Attack];
            attackHoldAnimationLayer.PlayLoop(MakeAnimationChainName("Attack", "Hold"), attackData.AnimationLoopCount);

            var effectChainName = MakeAnimationChainName("Flame");
            attackEffectAnimationLayer.PlayOnce(effectChainName);

            SetAttackOffset(attackData.CollisionOffset);
            CreateAttackDamageArea(attackData, AttackAnimations[effectChainName]);

        }

        private void CreateAttackDamageArea(DataTypes.AttackData attackData, AnimationChain animationToRun, int loopCount = 1)
        {
            var newDamageArea = Factories.DamageAreaFactory.CreateNew();
            currentAttackDamageArea = newDamageArea;
            currentAttackDamageArea.AttackData = attackData;
            currentAttackDamageArea.TeamIndex = TeamIndex;
            
            this.Call(() =>
            {
                newDamageArea.Destroy();
            }).After(animationToRun.TotalLength * loopCount);
        }

        private void BeginSpecialAttack()
        {
            var attackData = AttackData[DataTypes.AttackData.Special];
            attackHoldAnimationLayer.PlayLoop("Special", attackData.AnimationLoopCount);
            attackEffectAnimationLayer.PlayLoop("FlameSpecial", attackData.AnimationLoopCount);
            SetAttackOffset(attackData.CollisionOffset);
            CreateAttackDamageArea(attackData, AttackAnimations["FlameSpecial"], attackData.AnimationLoopCount);
        }

        public void TakeHit()
        {
            getHitAnimationLayer.PlayOnce(MakeAnimationChainName("TakeHit"));
        }

		private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        public override void UpdateDependencies(double currentTime)
        {
            base.UpdateDependencies(currentTime);
            var frameOffset = new Vector3();
            frameOffset.X = AttackEffectSprite.CurrentChain[AttackEffectSprite.CurrentFrameIndex].RelativeX;
            frameOffset.Y = AttackEffectSprite.CurrentChain[AttackEffectSprite.CurrentFrameIndex].RelativeY;

            AttackEffectSprite.RelativePosition = effectSpriteRelativeOffset + frameOffset;
            if(currentAttackDamageArea != null)
            {
                currentAttackDamageArea.Position = AttackEffectSprite.Position;
            }
        }

        private string MakeAnimationChainName(string prefix, string suffix = "")
        {
            switch (this.DirectionFacing)
            {
                case TopDownDirection.Up:
                    return $"{prefix}Up{suffix}";
                case TopDownDirection.UpRight:
                    return $"{prefix}UpRight{suffix}";
                case TopDownDirection.Right:
                    return $"{prefix}Right{suffix}";
                case TopDownDirection.DownRight:
                    return $"{prefix}DownRight{suffix}";
                case TopDownDirection.Down:
                    return $"{prefix}Down{suffix}";
                case TopDownDirection.DownLeft:
                    return $"{prefix}DownLeft{suffix}";
                case TopDownDirection.Left:
                    return $"{prefix}Left{suffix}";
                case TopDownDirection.UpLeft:
                    return $"{prefix}UpLeft{suffix}";
            }

            return null;
        }

        private void SetAttackOffset(Vector3 attackOffsets)
        {
            var direction = new Vector3();

            switch(mDirectionFacing)
            {
                case TopDownDirection.Down:
                case TopDownDirection.DownLeft:
                case TopDownDirection.DownRight:
                    direction.Y = -1;
                    break;
                case TopDownDirection.Up:
                case TopDownDirection.UpLeft:
                case TopDownDirection.UpRight:
                    direction.Y = 1;
                    break;
            }

            switch(mDirectionFacing)
            {
                case TopDownDirection.Left:
                case TopDownDirection.UpLeft:
                case TopDownDirection.DownLeft:
                    direction.X = -1;
                    break;
                case TopDownDirection.Right:
                case TopDownDirection.UpRight:
                case TopDownDirection.DownRight:
                    direction.X = 1;
                    break;
            }

            direction.Normalize();
            effectSpriteRelativeOffset = direction * attackOffsets;
        }
	}
}
