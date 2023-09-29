using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

public class PlayerMovement
{
    private bool isMouseMovementEnabled = false;


    private Vector3 _position;
    private float _speed = 5;
    private Matrix _camRotation; // Store the camera's rotation matrix
    //private Vector3 _camRotation;
    private int previousScrollValue;
    MouseState originalMouseState;
    private Vector3 _camUpVector;
    public Vector3 Position => _position;

    public PlayerMovement(Matrix camRotation, Vector3 upVec)
    {
        _camUpVector = upVec;
        _camRotation = camRotation; // Store the camera's rotation matrix
        previousScrollValue = originalMouseState.ScrollWheelValue;
    }

    public void Update(GameTime gameTime)
    {
        // Use the camera's rotation matrix to transform the forward and right vectors
        Vector3 forward = _camRotation.Forward;
        Vector3 right = _camRotation.Right;

        KeyboardState keyboardState = Keyboard.GetState();
        Vector3 moveVector = Vector3.Zero;

        if (keyboardState.IsKeyDown(Keys.W))
        {
            moveVector += forward;
            //moveVector = Vector3.Cross(forward, right);
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
        if(keyboardState.IsKeyDown(Keys.Space))
        {
            moveVector -= Vector3.Cross(forward, right);
        }
        if (keyboardState.IsKeyDown(Keys.LeftControl))
        {
            moveVector += Vector3.Cross(forward, right);
        }
        // Normalize the move vector to prevent faster diagonal movement
        if (moveVector != Vector3.Zero)
        {
            moveVector.Normalize();
        }

        if(keyboardState.IsKeyDown(Keys.Escape) && isMouseMovementEnabled)
        {
            Debug.WriteLine("Im Down");
        }
        else
        {

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
