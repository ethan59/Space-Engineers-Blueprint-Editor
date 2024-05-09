using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

public class PlayerMovement
{
    private Vector3 _position;
    private float _speed = 5f;
    private Matrix _cameraRotation;
    private Vector3 _upVector;
    private int previousScrollValue;
    private MouseState originalMouseState;

    public Vector3 Position => _position;

    public PlayerMovement(Vector3 initialPosition, Vector3 upVec)
    {
        _position = initialPosition;
        _upVector = upVec;
        originalMouseState = Mouse.GetState();
        previousScrollValue = originalMouseState.ScrollWheelValue;
    }

    public void Update(GameTime gameTime)
    {
        // Extract the forward and right vectors from the camera rotation matrix
        Vector3 forward = Vector3.Transform(Vector3.Forward, _cameraRotation);
        Vector3 right = Vector3.Transform(Vector3.Right, _cameraRotation);

        KeyboardState keyboardState = Keyboard.GetState();
        Vector3 moveVector = Vector3.Zero;

        // Adjust the movement vector based on keyboard input
        if (keyboardState.IsKeyDown(Keys.W)) moveVector += forward;
        if (keyboardState.IsKeyDown(Keys.S)) moveVector -= forward;
        if (keyboardState.IsKeyDown(Keys.A)) moveVector -= right;
        if (keyboardState.IsKeyDown(Keys.D)) moveVector += right;
        if (keyboardState.IsKeyDown(Keys.Space)) moveVector += _upVector;
        if (keyboardState.IsKeyDown(Keys.LeftControl)) moveVector -= _upVector;

        //if (keyboardState.IsKeyDown(Keys.Escape));

        // Normalize movement vector to prevent faster diagonal movement
        if (moveVector != Vector3.Zero) moveVector.Normalize();

        // Get the current mouse state
        MouseState currentMouseState = Mouse.GetState();

        float speedAmount = 1f;
        // Check if Left Shift is pressed and the mouse scroll is up
        if (keyboardState.IsKeyDown(Keys.LeftShift) && currentMouseState.ScrollWheelValue > previousScrollValue)
        {
            _speed += speedAmount;
        }

        // Prevent speed from dropping below a minimum
        if (keyboardState.IsKeyDown(Keys.LeftShift) && currentMouseState.ScrollWheelValue < previousScrollValue)
        {
            _speed -= speedAmount;
            if (_speed < 0.1f)
            {
                _speed = speedAmount; // Minimum speed limit
            }
        }

        // Update the scroll value for comparison in the next update cycle
        previousScrollValue = currentMouseState.ScrollWheelValue;

        // Update the player's position using the adjusted movement vector and speed
        _position += moveVector * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public void UpdateCameraRotation(Matrix newCameraRotation)
    {
        _cameraRotation = newCameraRotation;
    }
}
