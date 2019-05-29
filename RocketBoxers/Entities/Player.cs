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
    #region Enums

    public enum AttackCollisionShapeType
    {
        Circle,
        Rectangle
    }

    #endregion

    public partial class Player
	{
        #region Fields/Properties

        //SpriteInstance animation controller
        AnimationController animationController;
        AnimationLayer attackHoldAnimationLayer;
        AnimationLayer attackAnimationLayer;
        AnimationLayer getHitAnimationLayer;
        AnimationLayer fallingAnimationLayer;

        //AttackEffect animation controllerS
        AnimationController attackSpriteAnimationController;
        AnimationLayer attackEffectAnimationLayer;

        private Vector3 effectSpriteRelativeOffset;

        IPressableInput attackInput;
        IPressableInput specialAttackInpt;
        IPressableInput dashInput;
        IPressableInput blockInput;

        DamageArea currentAttackDamageArea;
        Respawn currentSpawnArea;

        static AnimationChainList P2Animations;
        static AnimationChainList P3Animations;
        static AnimationChainList P4Animations;

        public bool IsInvincible => isInvincible;
        bool isInvincible = false;
        double fallStart = 0;
        Vector2 fallingSpriteScale;

        static List<AnimationChainList> AllAnimationChains;

        public bool IsOnGround { get; set; }
        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>

        #endregion

        #region Initialize Methods

        private void CustomInitialize()
		{
            //DELETE THIS AFTER SETTING UP LAYERS
            this.Z = 10;
            this.InitializeInput();
            this.mCurrentMovement = TopDownValues[DataTypes.TopDownValues.Normal];
            this.PossibleDirections = PossibleDirections.EightWay;

            InitializeAnimations();

            InputEnabled = !DEBUG_IgnoreInput;
        }

        public void InitializeInputFrom(object inputDevice)
        {
            if(inputDevice is Keyboard keyboard)
            {
                InitializeInputFromKeyboard();
            }
            else if(inputDevice is Xbox360GamePad gamePad)
            {
                InitializeInputFromGamePad(gamePad);
            }
        }

        private void InitializeInputFromKeyboard()
        {
            this.MovementInput = FlatRedBall.Input.InputManager.Keyboard.Get2DInput(
                Microsoft.Xna.Framework.Input.Keys.Left,
                Microsoft.Xna.Framework.Input.Keys.Right,
                Microsoft.Xna.Framework.Input.Keys.Up,
                Microsoft.Xna.Framework.Input.Keys.Down);

            attackInput = InputManager.Keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.A);
            blockInput = InputManager.Keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.LeftShift);
            specialAttackInpt = InputManager.Keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.S);
            dashInput = InputManager.Keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.D);
        }

        private void InitializeInputFromGamePad(Xbox360GamePad gamePad)
        {
            this.MovementInput = gamePad.LeftStick;

            attackInput = gamePad.GetButton(Xbox360GamePad.Button.A);
            blockInput = gamePad.GetButton(Xbox360GamePad.Button.B);
            specialAttackInpt = gamePad.GetButton(Xbox360GamePad.Button.X);
            dashInput = gamePad.GetButton(Xbox360GamePad.Button.Y);
        }

        private void InitializeAnimations()
        {
            InitilizeSpriteInstanceController();
            InitilizeAttackSpriteController();
            //To manually scale the sprite to simulate falling we calculate the scale of the first frame of the falling animation.
            //This is kinda gross but has to be done for the monthly timeframe.
            var fallingFirstFrame = YellowCharacterAnimations["Falling"][0];
            fallingSpriteScale.X = (fallingFirstFrame.RightCoordinate - fallingFirstFrame.LeftCoordinate) * fallingFirstFrame.Texture.Width * .5f;
            fallingSpriteScale.Y = (fallingFirstFrame.BottomCoordinate - fallingFirstFrame.TopCoordinate)* fallingFirstFrame.Texture.Height * .5f;
        }

        public void SetAnimationsFromPlayerIndex(int index)
        {
            switch(index)
            {
                case 0:
                    SpriteInstance.AnimationChains = YellowCharacterAnimations;
                    break;
                case 1:
                    SpriteInstance.AnimationChains = P2Animations;
                    break;
                case 2:
                    SpriteInstance.AnimationChains = P3Animations;
                    break;
                case 3:
                    SpriteInstance.AnimationChains = P4Animations;
                    break;
            }
            TeamIndex = index;
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
                SetMovement(DataTypes.TopDownValues.Normal);
            };
            animationController.Layers.Add(attackHoldAnimationLayer);

            var blockingLayer = new AnimationLayer();
            blockingLayer.EveryFrameAction = () =>
            {
                if (blockInput.IsDown)
                {
                    if (blockInput.WasJustPressed)
                    {
                        SetMovement(DataTypes.TopDownValues.Blocking);
                    }
                    return MakeAnimationChainName("Block");
                }
                else if(blockInput.WasJustReleased)
                {
                   SetMovement(DataTypes.TopDownValues.Normal);
                }

                return null;
            };
            animationController.Layers.Add(blockingLayer);

            getHitAnimationLayer = new AnimationLayer();
            getHitAnimationLayer.OnAnimationFinished = () =>
            {
                SetMovement(DataTypes.TopDownValues.Normal);
                if (DEBUG_IgnoreInput)
                    InputEnabled = false;
            };
            animationController.Layers.Add(getHitAnimationLayer);

            fallingAnimationLayer = new AnimationLayer();
            fallingAnimationLayer.OnAnimationFinished = () =>
            {
                TryToRespawn();
                SetMovement(DataTypes.TopDownValues.Normal);
            };
            animationController.Layers.Add(fallingAnimationLayer);

        }

        #endregion

        #region Activity Methods

        private void CustomActivity()
		{
            //Uncomment once animations are created.
            if (!DEBUG_IgnoreInput)
                InputActivity();
            animationController.Activity();
            attackSpriteAnimationController.Activity();

        }

        private void InputActivity()
        {
            if (!blockInput.IsDown && !getHitAnimationLayer.HasPriority)
            {
                if (attackInput.WasJustPressed && !attackHoldAnimationLayer.HasPriority)
                {
                    attackAnimationLayer.PlayOnce(MakeAnimationChainName("Attack"));
                    SetMovement(DataTypes.TopDownValues.Stopped);
                }
                else if (dashInput.WasJustPressed && !attackHoldAnimationLayer.HasPriority)
                {
                    BeginDashAttack();
                    SetMovement(DataTypes.TopDownValues.DashAttack);
                }
                else if (specialAttackInpt.WasJustPressed && !attackHoldAnimationLayer.HasPriority)
                {
                    BeginSpecialAttack();
                    SetMovement(DataTypes.TopDownValues.Stopped);
                }
            }
        }

        private void BeginDashAttack()
        {
            var attackData = AttackData[DataTypes.AttackData.Dash];
            var chainName = MakeAnimationChainName("Dash");
            var movement = TopDownValues[DataTypes.TopDownValues.DashAttack];
            attackHoldAnimationLayer.PlayDuration(chainName, movement.DecelerationTime);

            SetAttackOffset(attackData.CollisionOffset);
            CreateAttackDamageArea(attackData, movement.DecelerationTime);
        }

        private void BeginBasicAttack()
        {
            var attackData = AttackData[DataTypes.AttackData.Attack];
            attackHoldAnimationLayer.PlayLoop(MakeAnimationChainName("Attack", "Hold"), attackData.AnimationLoopCount);

            var effectChainName = MakeAnimationChainName("Flame");
            attackEffectAnimationLayer.PlayOnce(effectChainName);

            SetAttackOffset(attackData.CollisionOffset);
            CreateAttackDamageArea(attackData, AttackAnimations[effectChainName].TotalLength);

        }

        private void CreateAttackDamageArea(DataTypes.AttackData attackData, float animationDuration, int loopCount = 1)
        {
            var newDamageArea = Factories.DamageAreaFactory.CreateNew();
            currentAttackDamageArea = newDamageArea;
            currentAttackDamageArea.AttackData = attackData;
            currentAttackDamageArea.TeamIndex = TeamIndex;
            currentAttackDamageArea.OwningPlayer = this;
            
            this.Call(() =>
            {
                newDamageArea.Destroy();
            }).After(animationDuration * loopCount);
        }

        private void BeginSpecialAttack()
        {
            var attackData = AttackData[DataTypes.AttackData.Special];
            attackHoldAnimationLayer.PlayLoop("Special", attackData.AnimationLoopCount);
            attackEffectAnimationLayer.PlayLoop("FlameSpecial", attackData.AnimationLoopCount);
            AttackEffectSprite.CurrentFrameIndex = 0;
            SetAttackOffset(attackData.CollisionOffset);
            CreateAttackDamageArea(attackData, AttackAnimations["FlameSpecial"].TotalLength, attackData.AnimationLoopCount);
        }

        public void TakeHit(DataTypes.AttackData attackData, Vector3 attackerLocation, bool shouldLaunch)
        {
            var damageToDeal = blockInput.IsDown ? attackData.DamageToDeal * BaseBlockDamageReduction : attackData.DamageToDeal;
            DamageTaken += damageToDeal;
            if(currentAttackDamageArea != null)
            {
                currentAttackDamageArea.DisableDamageArea();
            }
            ReactToDamage( attackData, attackerLocation, shouldLaunch);
        }

        private void ReactToDamage(DataTypes.AttackData attackData, Vector3 colliderLocation, bool shouldLaunch)
        {
            if (!blockInput.IsDown)
            {
                var launchDuration = shouldLaunch ? OnDamageLaunchDuration * DamageTaken : attackData.HitReactMin;
                if (shouldLaunch)
                {
                    var launchVector = Position - colliderLocation;
                    launchVector.Normalize();
                    Velocity = launchVector;
                    SetMovement(DataTypes.TopDownValues.Damaged);

                    var launchDirection = TopDownDirectionExtensions.FromDirection(new Vector2(launchVector.X, launchVector.Y), PossibleDirections.EightWay);
                    mDirectionFacing = TopDownDirectionExtensions.Mirror(launchDirection);
                }
                else
                {
                    SetMovement(DataTypes.TopDownValues.Stopped);
                }
                getHitAnimationLayer.PlayDuration(MakeAnimationChainName("Damage"), launchDuration);
            }
        }

        public void PerformFallOff(Respawn respawnLocation)
        {
            if (currentSpawnArea == null)
            {
                currentSpawnArea = respawnLocation;
                foreach (var layer in animationController.Layers)
                {
                    layer.StopPlay();
                }
                fallStart = FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedCurrentTime;
                
                XVelocity = 0;
                YVelocity = 0;
                XAcceleration = 0;
                YAcceleration = 0;

                SetMovement(DataTypes.TopDownValues.Stopped);
                fallingAnimationLayer.PlayDuration("Falling", FallingDuration);
            }
        }

        private void TryToRespawn()
        {
            DamageTaken = 0;
            fallStart = 0;
            X = currentSpawnArea.X;
            Y = currentSpawnArea.Y;
            isInvincible = true;
            currentSpawnArea = null;
            IsOnGround = true;

            ForceUpdateDependenciesDeep();

            this.Call(() => 
            {
                isInvincible = false;
            }).After(RespawnInvincibilityTime);
        }

        #endregion

        private void CustomDestroy()
		{


		}

        #region Custom Load Static Content

        private static void CustomLoadStaticContent(string contentManagerName)
        {
            if(P2Animations == null)
            {
                P2Animations = YellowCharacterAnimations.Clone();
                foreach(var animation in P2Animations)
                {
                    foreach(var frame in animation)
                    {
                        frame.TopCoordinate += 256.0f / frame.Texture.Height;
                        frame.BottomCoordinate += 256.0f / frame.Texture.Height;
                    }
                }

                P3Animations = YellowCharacterAnimations.Clone();
                foreach (var animation in P3Animations)
                {
                    foreach (var frame in animation)
                    {
                        frame.TopCoordinate += 2 * 256.0f / frame.Texture.Height;
                        frame.BottomCoordinate += 2 * 256.0f / frame.Texture.Height;
                    }
                }

                P4Animations = YellowCharacterAnimations.Clone();
                foreach (var animation in P4Animations)
                {
                    foreach (var frame in animation)
                    {
                        frame.TopCoordinate += 3 * 256.0f / frame.Texture.Height;
                        frame.BottomCoordinate += 3 * 256.0f / frame.Texture.Height;
                    }
                }
            }

        }

        #endregion

        public override void UpdateDependencies(double currentTime)
        {
            base.UpdateDependencies(currentTime);
            var frameOffset = new Vector3();
            frameOffset.X = AttackEffectSprite.CurrentChain[AttackEffectSprite.CurrentFrameIndex].RelativeX;
            frameOffset.Y = AttackEffectSprite.CurrentChain[AttackEffectSprite.CurrentFrameIndex].RelativeY;

            var attackOffset = effectSpriteRelativeOffset + frameOffset;
            if(currentAttackDamageArea != null)
            {
                currentAttackDamageArea.Position = attackOffset + Position;
            }

            AttackEffectSprite.RelativePosition = attackOffset + SpriteInstance.RelativePosition;

            if(fallingAnimationLayer.HasPriority)
            {
                var seconds = FlatRedBall.Screens.ScreenManager.CurrentScreen.PauseAdjustedSecondsSince(fallStart);
                var firstRatio = seconds / FallingDuration;
                var ratio = (float)Math.Max(1 - firstRatio, 0);
                SpriteInstance.ScaleX = fallingSpriteScale.X * ratio;
                SpriteInstance.ScaleY = fallingSpriteScale.Y * ratio;
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

        private void SetMovement(string movement)
        {
            mCurrentMovement = TopDownValues[movement];

            InputEnabled = !mCurrentMovement.DisablesInput;

            if(mCurrentMovement.ShouldSetToMaxSpeed)
            {
                if(Velocity.X == 0 && Velocity.Y == 0)
                {
                    // apply velocity in the given direction. Even though diagonals
                    // result in length > 1, normalize will handle it
                    switch(this.DirectionFacing)
                    {
                        case TopDownDirection.Left:
                            Velocity.X = -1;
                            break;
                        case TopDownDirection.UpLeft:
                            Velocity.X = -1;
                            Velocity.Y = 1;
                            break;
                        case TopDownDirection.Up:
                            YVelocity = 1;
                            break;
                        case TopDownDirection.UpRight:
                            Velocity.X = 1;
                            Velocity.Y = 1;
                            break;
                        case TopDownDirection.Right:
                            XVelocity = 1;
                            break;
                        case TopDownDirection.DownRight:
                            XVelocity = 1;
                            YVelocity = -1;
                            break;
                        case TopDownDirection.Down:
                            YVelocity = -1;
                            break;
                        case TopDownDirection.DownLeft:
                            YVelocity = -1;
                            XVelocity = -1;
                            break;
                    }
                }
                Velocity.Normalize();
                Velocity *= mCurrentMovement.MaxSpeed;
            }
        }
	}
}
