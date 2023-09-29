using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
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
        private SpriteBatch spriteBatch;
        private grid _grid;
        private PlayerMovement _playerMovement;
        private MouseMovement _mouseMovement;
        private UIManager _uiManager;


        public Vector3 cameraTarget;
        public Vector3 upVector;

        public Model model;

        //items tranform
        public Matrix localTransform = Matrix.CreateTranslation(new Vector3(0, 0, -10));


        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1920f / 1080f, 0.1f, 100f);


        public Game1()
        {

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;


            // Initialize the GraphicsDevice first
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();



            // Create an instance of MouseMovement after initializing _graphics
            _mouseMovement = new MouseMovement(0.5f, GraphicsDevice);
            _playerMovement = new PlayerMovement(view, upVector);
            _grid = new grid(GraphicsDevice, 30, 1.0f, -2.0f); // Adjust gridSize and cellSize as needed
            _uiManager = new UIManager(this);

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
            spriteBatch = new SpriteBatch(GraphicsDevice);

            base.LoadContent();

        }        

        public Vector3 camRotation = Vector3.Zero;


        protected override void Update(GameTime gameTime)
        {

            // Inside your Update method in Game1
            _mouseMovement.Update(gameTime);
            _playerMovement.Update(gameTime);

            // Calculate the elapsed time since the last frame
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Calculate camera position and view matrix based on player input
            cameraTarget = _playerMovement.Position + Vector3.Transform(Vector3.Forward, Matrix.CreateRotationX(_mouseMovement.Rotation.X) * Matrix.CreateRotationY(_mouseMovement.Rotation.Y));
            upVector = Vector3.Transform(Vector3.Up, Matrix.CreateRotationX(_mouseMovement.Rotation.X) * Matrix.CreateRotationY(_mouseMovement.Rotation.Y));


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

            _uiManager._desktop.Render();

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

                    _grid.Draw(view, projection);

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

