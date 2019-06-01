using System;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Specialized;
using FlatRedBall.Audio;
using FlatRedBall.Screens;
using RocketBoxers.Entities;
using RocketBoxers.Screens;
using FlatRedBall.Math.Geometry;

namespace RocketBoxers.Entities
{
    public partial class DamageArea
    {
        void OnAfterAttackDataSet (object sender, EventArgs e) 
        {
            switch(AttackData.CollsionShapeType)
            {
                case AttackCollisionShapeType.Circle:
                    //Only use the x value for the radius
                    var circle = new Circle() { Radius = AttackData.CollisionDimensions.X };
                    this.Collision.Add(circle);
                    ShapeManager.AddCircle(circle);
                    circle.AttachTo(this);
                    break;
                case AttackCollisionShapeType.Rectangle:
                    var rectangle = new AxisAlignedRectangle() { Width = AttackData.CollisionDimensions.X, Height = AttackData.CollisionDimensions.Y };
                    this.Collision.Add(rectangle);
                    ShapeManager.AddAxisAlignedRectangle(rectangle);
                    rectangle.AttachTo(this);
                    break;
            }
        }
    }
}
