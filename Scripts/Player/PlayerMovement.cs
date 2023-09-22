using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace SpaceEngineersShipBuilder.Scripts.Player
{
    public class PlayerMovement
    {
        //local pos
        private Vector3 _position;
        
        private float _speed = 100;
        
        private Game1 _game1;
        
        private bool showUI = false;
        
        public Vector3 Position => _position;


        private Vector3 moveVector = new Vector3();

        public PlayerMovement(Game1 game1,Vector3 camLocalPosition, float speed)
        {
            _game1 = game1;
            _position = camLocalPosition;
            _speed = speed;
        }

        public void Update(GameTime gameTime)
        {

            Vector3 forward = Vector3.Transform(Vector3.Forward, Matrix.CreateTranslation(_game1.camLocalTrasform.X,0,0) * Matrix.CreateTranslation(0,0, _game1.camLocalTrasform.Z));
            //Vector3 right = Vector3.Normalize(Vector3.Cross(forward, Vector3.Up));

            KeyboardState keyboardState = Keyboard.GetState();
            // Calculate the forward direction based on the camera's rotation


            if (keyboardState.IsKeyDown(Keys.W))
            {
                moveVector += forward * _speed;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                moveVector += Vector3.Backward * _speed;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                moveVector += Vector3.Left * _speed;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                moveVector += Vector3.Right * _speed;
            }

            // Normalize the move vector to prevent faster diagonal movement
            if (moveVector != Vector3.Zero)
            {
                moveVector.Normalize();
            }
            showUI = true;
            if(keyboardState.IsKeyDown(Keys.OemTilde) && showUI)
            {
                Debug.WriteLine("-------------Im working-------------");
                showUI = !showUI;
            }
            else
            {
                Debug.WriteLine("False");
                showUI = !showUI;
            }


            // Update the player's position
            _position += moveVector * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}

