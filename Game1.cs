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
        public Matrix localTransform = Matrix.CreateTranslation(new Vector3(20, -1, 20));


        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 1920f / 1080f, 0.1f, 10f);


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

            // Initialize the player movement with a starting position and up vector
            _playerMovement = new PlayerMovement(new Vector3(0, 0, 0), Vector3.Up);

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
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), aspectRatio, 0.1f, 1000f);


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
            // Update mouse movement to get the latest rotation angles
            _mouseMovement.Update(gameTime);

            // Get the current camera rotation matrix from MouseMovement
            Matrix cameraRotation = _mouseMovement.GetCameraRotationMatrix();

            // Update the player's camera rotation matrix in PlayerMovement
            _playerMovement.UpdateCameraRotation(cameraRotation);

            // Update player movement based on input
            _playerMovement.Update(gameTime);

            // Calculate the camera target using the camera's forward direction
            Vector3 cameraForward = Vector3.Transform(Vector3.Forward, cameraRotation);
            cameraTarget = _playerMovement.Position + cameraForward;

            // The camera's up vector remains aligned with the world
            upVector = Vector3.Transform(Vector3.Up, cameraRotation);

            // Create the view matrix based on the player's current position and the calculated camera target
            view = Matrix.CreateLookAt(_playerMovement.Position, cameraTarget, upVector);

            // Debug logs to track the player's position and camera view matrix
            Debug.WriteLine("Player position: " + _playerMovement.Position);
            Debug.WriteLine("Camera view matrix: " + view);

            // Call the base Update method to ensure other game logic updates
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
                    effect.FogEnabled = false;
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

