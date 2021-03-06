


namespace RocketBoxers.Entities
{
    public enum TopDownDirection
    {
        Right = 0,
        UpRight = 1,
        Up = 2,
        UpLeft = 3,
        Left = 4,
        DownLeft = 5,
        Down = 6,
        DownRight = 7
    }


    public static class TopDownDirectionExtensions
    {
        public static TopDownDirection FromDirection(Microsoft.Xna.Framework.Vector2 direction, PossibleDirections possibleDirections)
        {
            return FromDirection(direction.X, direction.Y, possibleDirections);
        }

        public static TopDownDirection FromDirection(Microsoft.Xna.Framework.Vector3 direction, PossibleDirections possibleDirections)
        {
            return FromDirection(direction.X, direction.Y, possibleDirections);
        }

        public static TopDownDirection FlipX(this TopDownDirection directionToFlip)
        {
            switch(directionToFlip)
            {
                case TopDownDirection.Left: return TopDownDirection.Right;
                case TopDownDirection.UpLeft: return TopDownDirection.UpRight;
                case TopDownDirection.Up: return TopDownDirection.Up;
                case TopDownDirection.UpRight: return TopDownDirection.UpLeft;
                case TopDownDirection.Right: return TopDownDirection.Left;
                case TopDownDirection.DownRight: return TopDownDirection.DownLeft;
                case TopDownDirection.Down: return TopDownDirection.Down;
                case TopDownDirection.DownLeft: return TopDownDirection.DownRight;
            }

            throw new System.Exception();
        }

        public static TopDownDirection FlipY(this TopDownDirection directionToFlip)
        {
            switch (directionToFlip)
            {
                case TopDownDirection.Left: return TopDownDirection.Left;
                case TopDownDirection.UpLeft: return TopDownDirection.DownLeft;
                case TopDownDirection.Up: return TopDownDirection.Down;
                case TopDownDirection.UpRight: return TopDownDirection.DownRight;
                case TopDownDirection.Right: return TopDownDirection.Right;
                case TopDownDirection.DownRight: return TopDownDirection.UpRight;
                case TopDownDirection.Down: return TopDownDirection.Up;
                case TopDownDirection.DownLeft: return TopDownDirection.UpLeft;
            }
            throw new System.Exception();
        }

        public static TopDownDirection Mirror(this TopDownDirection directionToFlip)
        {
            switch (directionToFlip)
            {
                case TopDownDirection.Left: return TopDownDirection.Right;
                case TopDownDirection.UpLeft: return TopDownDirection.DownRight;
                case TopDownDirection.Up: return TopDownDirection.Down;
                case TopDownDirection.UpRight: return TopDownDirection.DownLeft;
                case TopDownDirection.Right: return TopDownDirection.Left;
                case TopDownDirection.DownRight: return TopDownDirection.UpLeft;
                case TopDownDirection.Down: return TopDownDirection.Up;
                case TopDownDirection.DownLeft: return TopDownDirection.UpRight;
            }
            throw new System.Exception();
        }




        public static TopDownDirection FromDirection(float x, float y, PossibleDirections possibleDirections)
        {
            if(x == 0 && y == 0)
            {
                throw new System.Exception("Can't convert 0,0 to a direction");
            }

            switch (possibleDirections)
            {
                case PossibleDirections.LeftRight:
                    if (x > 0)
                    {
                        return TopDownDirection.Right;
                    }
                    else if (x < 0)
                    {
                        return TopDownDirection.Left;
                    }
                    break;
                case PossibleDirections.FourWay:
                    var absXVelocity = System.Math.Abs(x);
                    var absYVelocity = System.Math.Abs(y);

                    if (absXVelocity > absYVelocity)
                    {
                        if (x > 0)
                        {
                            return TopDownDirection.Right;
                        }
                        else if (x < 0)
                        {
                            return TopDownDirection.Left;
                        }
                    }
                    else if (absYVelocity > absXVelocity)
                    {
                        if (y > 0)
                        {
                            return TopDownDirection.Up;
                        }
                        else if (y < 0)
                        {
                            return TopDownDirection.Down;
                        }
                    }
                    break;
                case PossibleDirections.EightWay:
                    if (x != 0 || y != 0)
                    {
                        var angle = FlatRedBall.Math.MathFunctions.RegulateAngle(
                            (float)System.Math.Atan2(y, x));

                        var ratioOfCircle = angle / Microsoft.Xna.Framework.MathHelper.TwoPi;

                        var eights = FlatRedBall.Math.MathFunctions.RoundToInt(ratioOfCircle * 8) % 8;

                        return (TopDownDirection)eights;
                    }

                    break;
            }

            throw new System.Exception();
        }
    }





    public enum PossibleDirections
    {
        LeftRight,
        FourWay,
        EightWay
    }

}

