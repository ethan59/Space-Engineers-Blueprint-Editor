using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.File;
using System.IO;
using System;
using Myra.Assets;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using SpaceEngineersShipBuilder.Scripts.Player;
/*using AssetManagementBase;
using AssetManagementBase.Utility;
*/
namespace SpaceEngineersShipBuilder
{
    public class Game1 : Game
    {
        public GraphicsDeviceManager _graphics;

        public Desktop _desktop;

        public float camSpeed = 0.1f;
        public float mouseX;
        public float mouseY;
        public float sensitivity = 0.5f;


        private PlayerMovement _playerMovement;
        private MouseMovement _mouseMovement;
        private UIManager _uiManager;


        public Vector3 transform;
        public Vector2 transform2D;

        public Model model;

        public Models models;

        //Camera
        Vector3 camPosition;

        public bool mouseInput = true;

        public Vector3 camLocalTrasform = Vector3.Forward;

        //items tranform
        private Matrix localTransform = Matrix.CreateTranslation(new Vector3(0, 0, -10));


        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1920f / 1080f, 0.1f, 100f);


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            transform = new Vector3(0, 0, 0);
            transform2D = new Vector2(0, 0);
            _playerMovement = new PlayerMovement(this,camLocalTrasform, 0.1f);
            // Initialize the GraphicsDevice first
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            _uiManager = new UIManager(this);


            // Create an instance of MouseMovement after initializing _graphics
            _mouseMovement = new MouseMovement(0.5f, GraphicsDevice);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            // Inside your Initialize method in Game1
            float aspectRatio = (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), aspectRatio, 0.1f, 100f);



            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            //AssetHotloader hotloader = new AssetHotloader(Content, Content.RootDirectory);
            model = Content.Load<Model>("cube");
            otherTexture = Content.Load<Texture2D>("pixilart-drawing");

            base.LoadContent();

        }        

        public Vector2 camRotation = Vector2.Zero;


        protected override void Update(GameTime gameTime)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                _uiManager.ShowContextMenu();
            }


            // Inside your Update method in Game1
            _playerMovement.Update(gameTime);
            _mouseMovement.Update(gameTime);

            // Calculate the elapsed time since the last frame
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;


            //float deltaX = (Mouse.GetState().X - GraphicsDevice.Viewport.Width / 2) * sensitivity * elapsed;
            //float deltaY = (Mouse.GetState().Y - GraphicsDevice.Viewport.Height / 2) * sensitivity * elapsed;

            // Adjust the camera's rotation based on mouse input
            //camRotation.Y -= deltaX;
            //camRotation.X = MathHelper.Clamp(camRotation.X - deltaY, -MathHelper.PiOver2, MathHelper.PiOver2);

            // Reset the mouse position to the center of the screen
            //Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            //mouseInput = true;
            // Calculate camera position and view matrix based on player input
            Vector3 cameraTarget = _playerMovement.Position + Vector3.Transform(Vector3.Forward, Matrix.CreateRotationX(_mouseMovement.Rotation.X) * Matrix.CreateRotationY(_mouseMovement.Rotation.Y));
            Vector3 upVector = Vector3.Transform(Vector3.Up, Matrix.CreateRotationX(_mouseMovement.Rotation.X) * Matrix.CreateRotationY(_mouseMovement.Rotation.Y));
            //view = Matrix.CreateLookAt(_playerMovement.Position, cameraTarget, upVector);


            // Update the view matrix with the new camera position and rotation
            view = Matrix.CreateLookAt(_playerMovement.Position, cameraTarget, upVector);
            Debug.WriteLine("player pos: " + _playerMovement.Position);
            Debug.WriteLine("player rot: " + view);
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            //The color of the background
            GraphicsDevice.Clear(Color.Gray);

            DrawModel(model, localTransform, view, projection);

            //_desktop.Render();

            base.Draw(gameTime);
        }

        private Texture2D otherTexture;

        public void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;

                    effect.LightingEnabled = false;
                    effect.DirectionalLight0.Enabled = true;
                    effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
                    effect.EmissiveColor = new Vector3(0, 0, 0);
                    effect.Texture = otherTexture;
                    effect.TextureEnabled = true;
                    effect.FogEnabled = true;
                    effect.FogColor = Color.White.ToVector3();
                    effect.FogStart = 20f;
                    effect.FogEnd = 25f;

                    //effect.Texture = otherTexture;
                }
                mesh.Draw();
            }
        }

    }
    public class AssetHotloader
    {
        private readonly ContentManager contentManager;
        private readonly FileSystemWatcher watcher;

        public AssetHotloader(ContentManager contentManager, string contentPath)
        {
            this.contentManager = contentManager;

            // Set up a FileSystemWatcher to monitor the content folder for changes
            watcher = new FileSystemWatcher(contentPath);
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.*";
            watcher.Changed += OnFileChanged;
            watcher.EnableRaisingEvents = true;
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            // Handle the asset change event
            string assetName = Path.GetFileNameWithoutExtension(e.Name);

            // Reload the asset
            ReloadTexture(assetName);
        }

        private void ReloadTexture(string assetName)
        {
            try
            {
                // Unload the existing asset
                contentManager.Unload();

                // Reload the asset
                Texture2D reloadedTexture = contentManager.Load<Texture2D>(assetName);

                // Do something with the reloaded texture (e.g., update game objects)
            }
            catch (ContentLoadException)
            {
                // Handle the exception (e.g., asset not found)
            }
        }
    }
}

