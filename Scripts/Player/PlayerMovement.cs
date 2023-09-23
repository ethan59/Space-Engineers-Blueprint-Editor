using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using System.Diagnostics;

namespace SpaceEngineersShipBuilder.Scripts.Player
{
    public class PlayerMovement
    {
        private Vector3 _position;
        private float _speed = 5;
        private Matrix _camLocalPos;

        private int previousScrollValue;
        MouseState originalMouseState;

        public Vector3 Position => _position;

        public PlayerMovement(Matrix camPos)
        {
            _camLocalPos = camPos;
            previousScrollValue = originalMouseState.ScrollWheelValue;
        }

        public void Update(GameTime gameTime)
        {
            Vector3 forward = _camLocalPos.Forward;
            Vector3 right = _camLocalPos.Right;

            KeyboardState keyboardState = Keyboard.GetState();
            Vector3 moveVector = Vector3.Zero;


            if (keyboardState.IsKeyDown(Keys.W))
            {
                moveVector += forward;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                moveVector -= forward;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                moveVector -= right;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                moveVector += right;
            }

            // Normalize the move vector to prevent faster diagonal movement
            if (moveVector != Vector3.Zero)
            {
                moveVector.Normalize();
            }

            //Does Not work need to be fixed
            if (originalMouseState.ScrollWheelValue < previousScrollValue && keyboardState.IsKeyDown(Keys.LeftShift))
            {
                _speed += 1;
            }
            else if (originalMouseState.ScrollWheelValue > previousScrollValue && keyboardState.IsKeyDown(Keys.LeftShift))
            {
                _speed -= 1;
            }
            previousScrollValue = originalMouseState.ScrollWheelValue;
            // Update the player's position
            _position += moveVector * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
