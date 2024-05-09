using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class PlayerMovement
{
    private Vector3 _position;
    private float _speed = 5f;
    private Matrix _cameraRotation; // Stores the latest camera rotation matrix
    private Vector3 _upVector;
    private int previousScrollValue;
    private MouseState originalMouseState;

    // Public property to access the player's position
    public Vector3 Position => _position;

    // Constructor to initialize player movement
    public PlayerMovement(Vector3 initialPosition, Vector3 upVec)
    {
        _position = initialPosition;
        _upVector = upVec;
        originalMouseState = Mouse.GetState();
        previousScrollValue = originalMouseState.ScrollWheelValue;
    }

    // Update method to handle movement logic
    public void Update(GameTime gameTime)
    {
        // Extract the forward and right vectors from the camera rotation matrix
        Vector3 forward = Vector3.Transform(Vector3.Forward, _cameraRotation);
        Vector3 right = Vector3.Transform(Vector3.Right, _cameraRotation);

        KeyboardState keyboardState = Keyboard.GetState();
        Vector3 moveVector = Vector3.Zero;

        // Adjust the movement vector based on keyboard input
        if (keyboardState.IsKeyDown(Keys.W)) moveVector += forward; // Move forward
        if (keyboardState.IsKeyDown(Keys.S)) moveVector -= forward; // Move backward
        if (keyboardState.IsKeyDown(Keys.A)) moveVector -= right;   // Move left
        if (keyboardState.IsKeyDown(Keys.D)) moveVector += right;   // Move right
        if (keyboardState.IsKeyDown(Keys.Space)) moveVector += _upVector; // Move up
        if (keyboardState.IsKeyDown(Keys.LeftControl)) moveVector -= _upVector; // Move down

        // Normalize movement vector to prevent faster diagonal movement
        if (moveVector != Vector3.Zero) moveVector.Normalize();

        // Adjust speed using the mouse scroll wheel (with Left Shift held)
        MouseState currentMouseState = Mouse.GetState();
        if (currentMouseState.ScrollWheelValue < previousScrollValue && keyboardState.IsKeyDown(Keys.LeftShift))
        {
            _speed += 1;
        }
        else if (currentMouseState.ScrollWheelValue > previousScrollValue && keyboardState.IsKeyDown(Keys.LeftShift))
        {
            _speed -= 1;
            if (_speed < 1) _speed = 1;
        }
        previousScrollValue = currentMouseState.ScrollWheelValue;

        // Update the player's position using the adjusted movement vector and speed
        _position += moveVector * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    // Method to update the camera's rotation matrix
    public void UpdateCameraRotation(Matrix newCameraRotation)
    {
        _cameraRotation = newCameraRotation;
    }
}
