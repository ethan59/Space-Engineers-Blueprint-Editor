using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class PlayerMovement
{
    private Vector3 _position;
    private float _speed = 5;
    private Matrix _camLocalPos;
    private Matrix _camRotation; // Store the camera's rotation matrix

    private int previousScrollValue;
    MouseState originalMouseState;

    public Vector3 Position => _position;

    public PlayerMovement(Matrix camPos, Matrix camRotation)
    {
        _camLocalPos = camPos;
        _camRotation = camRotation; // Store the camera's rotation matrix
        previousScrollValue = originalMouseState.ScrollWheelValue;
    }

    public void Update(GameTime gameTime)
    {
        // Use the camera's rotation matrix to transform the forward and right vectors
        Vector3 forward = Vector3.Transform(Vector3.Forward, _camRotation);
        Vector3 right = Vector3.Transform(Vector3.Right, _camRotation);

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

        // Scrolling with Left Shift key (assuming you want to adjust speed)
        if (originalMouseState.ScrollWheelValue < previousScrollValue && keyboardState.IsKeyDown(Keys.LeftShift))
        {
            _speed += 1;
        }
        else if (originalMouseState.ScrollWheelValue > previousScrollValue && keyboardState.IsKeyDown(Keys.LeftShift))
        {
            _speed -= 1;
            if (_speed < 1)
                _speed = 1; // Ensure speed doesn't go below 1
        }
        previousScrollValue = originalMouseState.ScrollWheelValue;

        // Update the player's position
        _position += moveVector * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
}
