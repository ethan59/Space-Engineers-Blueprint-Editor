using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using SpaceEngineersShipBuilder.Scripts.Player;
using Myra.Graphics2D.UI;

namespace SpaceEngineersShipBuilder.Scripts.Core
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private BuildGrid _grid;
        private PlayerMovement _playerMovement;
        private MouseMovement _mouseMovement;
        private UIManager _uiManager;
        private FileLoader _fileLoader;
        private BuildingSystem _buildingSystem;

        private bool _isMouseCaptured = true;
        private bool _escapeKeyPreviouslyPressed = false;
        private bool _gKeyPreviouslyPressed = false;

        private Vector3 _cameraTarget;
        private Vector3 _upVector;
        private Matrix _world, _view, _projection;

        private Texture2D _texture;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            _graphics.ApplyChanges();

            InitializeGameElements();
            SetInitialCamera();

            base.Initialize();
        }

        private void InitializeGameElements()
        {
            const int gridSize = 30;
            const float cellSize = 1.0f;
            const float gridHeight = -2.0f;
            Vector3 gridCenter = new Vector3(gridSize * cellSize / 2.0f, gridHeight, gridSize * cellSize / 2.0f);

            string modelPath = "path_to_obj_file.obj";

            _mouseMovement = new MouseMovement(0.1f, GraphicsDevice);
            _grid = new BuildGrid(GraphicsDevice, gridSize, cellSize, gridHeight, Color.Green);
            _playerMovement = new PlayerMovement(new Vector3(gridCenter.X, 0, gridCenter.Z), Vector3.Up);
            _fileLoader = new FileLoader(GraphicsDevice, _texture);
            _uiManager = new UIManager(this, _fileLoader);
            _buildingSystem = new BuildingSystem(GraphicsDevice, Content);
            _fileLoader.LoadDataAsync(modelPath, () => Debug.WriteLine("File loading completed!"));
        }

        private void SetInitialCamera()
        {
            _world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
            _view = Matrix.CreateLookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000f);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _texture = Content.Load<Texture2D>("pixilart-drawing");  // Load texture
        }

        protected override void Update(GameTime gameTime)
        {
            HandleInput();

            if (!_uiManager.IsItemPanelVisible)
            {
                if (_isMouseCaptured)
                {
                    _mouseMovement.Update(gameTime);
                    Matrix cameraRotation = _mouseMovement.GetCameraRotationMatrix();
                    _playerMovement.UpdateCameraRotation(cameraRotation);
                    _playerMovement.Update(gameTime);

                    Vector3 cameraForward = Vector3.Transform(Vector3.Forward, cameraRotation);
                    _cameraTarget = _playerMovement.Position + cameraForward;
                    _upVector = Vector3.Transform(Vector3.Up, cameraRotation);
                    _view = Matrix.CreateLookAt(_playerMovement.Position, _cameraTarget, _upVector);
                }

                _buildingSystem.Update(gameTime, _view, _mouseMovement.GetCameraRotationMatrix(), _playerMovement.Position);
            }

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            bool isEscapePressed = keyboardState.IsKeyDown(Keys.Escape);
            bool isGPressed = keyboardState.IsKeyDown(Keys.G);

            if (isEscapePressed && !_escapeKeyPreviouslyPressed)
            {
                _isMouseCaptured = !_isMouseCaptured;
                IsMouseVisible = !_isMouseCaptured;
                if (_isMouseCaptured)
                {
                    Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.DisplayMode.Height / 2);
                }
            }

            if (isGPressed && !_gKeyPreviouslyPressed)
            {
                _uiManager.ToggleItemPanel();
                IsMouseVisible = _uiManager.IsItemPanelVisible;
                _isMouseCaptured = !_uiManager.IsItemPanelVisible;

                if (!_isMouseCaptured)
                {
                    Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.DisplayMode.Height / 2);
                }
            }

            _escapeKeyPreviouslyPressed = isEscapePressed;
            _gKeyPreviouslyPressed = isGPressed;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(23, 45, 68));
            if (_fileLoader != null)
            {
                _fileLoader.Draw(_world, _view, _projection);
            }
            if (!_uiManager.IsItemPanelVisible)
            {
                _buildingSystem.Draw(_world, _view, _projection, _mouseMovement.GetCameraRotationMatrix());
                _grid.Draw(_view, _projection);
            }

            _uiManager.Render();
            base.Draw(gameTime);
        }
    }
}
