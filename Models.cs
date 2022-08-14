using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace SpaceEngineersShipBuilder
{
    public class Models
    {
        public Model cube;
        public Game1 main;

        public void DrawModel(Model model,  Matrix worldSpace, Matrix playerView, Matrix projectionSpace)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = worldSpace;
                    effect.View = playerView;
                    effect.Projection = projectionSpace;

                    effect.LightingEnabled = true;
                    effect.DirectionalLight0.Enabled = true;
                    effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
                    //effect.EmissiveColor = new Vector3(0, 0, 0);
                    //effect.Texture = default;
                    //effect.TextureEnabled = true;
                    effect.FogEnabled = true;
                    effect.FogColor = Color.White.ToVector3();
                    effect.FogStart = 20f;
                    effect.FogEnd = 25f;

                    //effect.Texture = otherTexture;
                }
                mesh.Draw();
            }
        }
    }
}