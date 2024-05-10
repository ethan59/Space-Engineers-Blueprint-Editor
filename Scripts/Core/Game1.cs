using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
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
        private bool _isMouseCaptured = true;
        private bool _escapeKeyPreviouslyPressed = false;
        private Vector3 _cameraTarget;
        private Vector3 _upVector;
        private Model _model;
        private Matrix _world, _view, _projection;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = true
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
            // Initialize MouseMovement and Grid
            _mouseMovement = new MouseMovement(0.1f, GraphicsDevice);

            const int gridSize = 30;
            const float cellSize = 1.0f;
            const float gridHeight = -2.0f;

            Vector3 gridCenter = new Vector3(
                gridSize * cellSize / 2.0f,
                gridHeight,
                gridSize * cellSize / 2.0f
            );

            _grid = new BuildGrid(GraphicsDevice, gridSize, cellSize, gridHeight, Color.Green);
            _playerMovement = new PlayerMovement(new Vector3(gridCenter.X, 0, gridCenter.Z), Vector3.Up);
            // Inside your game's initialization code:
            FileLoader fileLoader = new FileLoader(GraphicsDevice);
            UIManager uiManager = new UIManager(this, fileLoader);
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
            _model = Content.Load<Model>("cube");

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
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

            KeyboardState keyboardState = Keyboard.GetState();
            bool isEscapePressed = keyboardState.IsKeyDown(Keys.Escape);

            if (isEscapePressed && !_escapeKeyPreviouslyPressed)
            {
                _isMouseCaptured = !_isMouseCaptured;
                IsMouseVisible = !_isMouseCaptured;

                if (_isMouseCaptured)
                {
                    Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
                }
            }

            _escapeKeyPreviouslyPressed = isEscapePressed;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);
            DrawModel(_model, _world, _view, _projection);
            _fileLoader.Draw(_world, _view, _projection);
            _uiManager.Render();

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.LightingEnabled = true;
                    effect.TextureEnabled = true;
                    _grid.Draw(view, projection);
                }
                mesh.Draw();
            }
        }
    }
}
