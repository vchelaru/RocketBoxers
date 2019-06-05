using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.TileGraphics;
using Microsoft.Xna.Framework.Graphics;
using RocketBoxers.Input;
using System;
using System.Collections.Generic;
using System.Linq;
    
namespace RocketBoxers.GumRuntimes
{
    public partial class LevelSelectorRuntime
    {
        public List<Layer> Levels = new List<Layer>();
        public int SelectedLevel { get; private set; }

        public UiInputDevice UiInput { get; set; }

        partial void CustomInitialize () 
        {
        }

        public void InitializeLevelList(List<LayeredTileMap> levels)
        {
            foreach (var level in levels)
            {
                Levels.Add(MakeRenderTargetFromTileMap(level));
            }

            if (levels.Count > 0)
            {
                SpriteInstance.Width = SpriteInstance.TextureWidth;
                SpriteInstance.Height = SpriteInstance.TextureHeight;

                SpriteInstance.TextureAddress = Gum.Managers.TextureAddress.EntireTexture;
                SpriteInstance.WidthUnits = Gum.DataTypes.DimensionUnitType.Absolute;
                SpriteInstance.HeightUnits = Gum.DataTypes.DimensionUnitType.Absolute;

                SpriteInstance.Texture = Levels[SelectedLevel].RenderTarget;
            }
        }

        private Layer MakeRenderTargetFromTileMap(LayeredTileMap tileMap)
        {
            RenderTarget2D renderTarget = new RenderTarget2D(FlatRedBall.FlatRedBallServices.GraphicsDevice, (int)tileMap.Width, (int)tileMap.Height);
            renderTarget.Name = tileMap.Name;
            var layer = new Layer();
            SpriteManager.AddLayer(layer);
            layer.UsePixelCoordinates();
            layer.LayerCameraSettings.Orthogonal = true;
            layer.LayerCameraSettings.OrthogonalWidth = tileMap.Width;
            layer.LayerCameraSettings.OrthogonalHeight = tileMap.Height;

            tileMap.X = Camera.Main.X - tileMap.Width / 2;
            tileMap.Y = Camera.Main.Y + tileMap.Height / 2;
            
            layer.RenderTarget = renderTarget;
            tileMap.AddToManagers(layer);

            return layer;
        }

        public void CustomActivity()
        {
            if (UiInput != null)
            {
                if (UiInput.LeftTrigger.WasJustPressed)
                {
                    SelectedLevel--;
                    if (SelectedLevel < 0)
                    {
                        SelectedLevel = Levels.Count - 1;
                    }
                    SpriteInstance.Texture = Levels[SelectedLevel].RenderTarget;
                }
                else if (UiInput.RightTrigger.WasJustPressed)
                {
                    SelectedLevel++;
                    if (SelectedLevel >= Levels.Count)
                    {
                        SelectedLevel = 0;
                    }
                    SpriteInstance.Texture = Levels[SelectedLevel].RenderTarget;
                }
            }
        }


        public void Destroy()
        {
            foreach(var level in Levels)
            {
                SpriteManager.RemoveLayer(level);
            }
        }
    }
}
