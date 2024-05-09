using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SpaceEngineersShipBuilder.Scripts.Player
{
    public class MouseMovement
    {
        private Vector2 _rotation; // Stores the yaw (Y) and pitch (X) rotations
        private float _sensitivity;
        private GraphicsDevice _graphicsDevice;

        // Constants for fine-tuning
        private const float maxRotationSpeed = 0.1f;  // Limit on how fast rotation changes
        private const float deadZoneThreshold = 0.002f; // Threshold to ignore slight movements

        // Public property to access the current rotation (yaw and pitch)
        public Vector2 Rotation => _rotation;

        // Constructor to initialize sensitivity and graphics device
        public MouseMovement(float sensitivity, GraphicsDevice graphicsDevice)
        {
            _sensitivity = sensitivity;
            _graphicsDevice = graphicsDevice;
        }

        // Method to update the rotation based on mouse movement
        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            // Calculate mouse movement differences with dead zone filtering
            float deltaX = (mouseState.X - _graphicsDevice.Viewport.Width / 2) * _sensitivity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float deltaY = (mouseState.Y - _graphicsDevice.Viewport.Height / 2) * _sensitivity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Math.Abs(deltaX) < deadZoneThreshold) deltaX = 0f;
            if (Math.Abs(deltaY) < deadZoneThreshold) deltaY = 0f;

            // Cap the rotation speed to avoid excessive movement
            deltaX = MathHelper.Clamp(deltaX, -maxRotationSpeed, maxRotationSpeed);
            deltaY = MathHelper.Clamp(deltaY, -maxRotationSpeed, maxRotationSpeed);

            // Update yaw (horizontal) and pitch (vertical) based on mouse movement
            _rotation.Y -= deltaX; // Yaw (horizontal rotation)
            _rotation.X = MathHelper.Clamp(_rotation.X - deltaY, -MathHelper.PiOver2, MathHelper.PiOver2); // Pitch (vertical)

            // Re-center the mouse cursor
            Mouse.SetPosition(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2);
        }

        // Method to calculate the camera rotation matrix
        public Matrix GetCameraRotationMatrix()
        {
            // Apply the pitch rotation (around the X-axis)
            Matrix pitchMatrix = Matrix.CreateRotationX(_rotation.X);

            // Apply the yaw rotation (around the Y-axis)
            Matrix yawMatrix = Matrix.CreateRotationY(_rotation.Y);

            // Combine yaw and pitch to create the final rotation matrix
            return pitchMatrix * yawMatrix;
        }
    }
}
