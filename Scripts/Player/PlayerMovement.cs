using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace SpaceEngineersShipBuilder.Scripts.Player
{
    public class PlayerMovement
    {
        private Vector3 _position;
        private float _speed;
        
        Game1 _game1;

        public Vector3 Position => _position;

        public PlayerMovement(Vector3 initialPosition, float speed)
        {
            _game1 = new Game1();
            _position = initialPosition;
            _speed = speed;

            Vector2 camRotation = _game1.camRotation;

            // Calculate the forward direction based on the camera's rotation
            Vector3 forward = Vector3.Normalize(Vector3.Transform(Vector3.Forward, Matrix.CreateRotationX(camRotation.X) * Matrix.CreateRotationY(camRotation.Y)));
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, Vector3.Up));
        }

        public void Update(GameTime gameTime)
        {

            KeyboardState keyboardState = Keyboard.GetState();
            Vector3 moveVector = Vector3.Zero;
            moveVector = _game1.camLocalTrasform;
            if (keyboardState.IsKeyDown(Keys.W))
            {
                moveVector += Vector3.Forward;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                moveVector += Vector3.Backward;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                moveVector += Vector3.Left;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                moveVector += Vector3.Right;
            }

            // Normalize the move vector to prevent faster diagonal movement
            if (moveVector != Vector3.Zero)
            {
                moveVector.Normalize();
            }

            // Update the player's position
            _position += moveVector * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}

