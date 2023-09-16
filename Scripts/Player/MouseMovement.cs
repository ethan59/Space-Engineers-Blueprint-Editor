using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceEngineersShipBuilder
{
    public class MouseMovement
    {
        private Vector2 _rotation;
        private float _sensitivity;
        private GraphicsDevice _graphicsDevice;

        public Vector2 Rotation => _rotation;

        public MouseMovement(float sensitivity, GraphicsDevice graphicsDevice)
        {
            _sensitivity = sensitivity;
            _graphicsDevice = graphicsDevice;
        }

        public void Update(GameTime gameTime)
        {

            MouseState mouseState = Mouse.GetState();

            float deltaX = (mouseState.X - _graphicsDevice.Viewport.Width / 2) * _sensitivity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float deltaY = (mouseState.Y - _graphicsDevice.Viewport.Height / 2) * _sensitivity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            _rotation.X -= deltaX;
            _rotation.Y = MathHelper.Clamp(_rotation.Y - deltaY, -MathHelper.PiOver2, MathHelper.PiOver2);

            Mouse.SetPosition(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2);
        }
    }
}
