using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.File;
using System.IO;
using System;
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
        public float sensitivity = 5;

        float frameRate = 1;

        public Vector3 transform;
        public Vector2 transform2D;

        public Model model;

        public Models models;

        //Camera
        Vector3 camTarget;
        Vector3 camPosition;

        public bool mouseInput = true;

        public Vector3 camLocalTrasform = Vector3.Forward;

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
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here


            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            UI();

            model = Content.Load<Model>("cube");
            otherTexture = Content.Load<Texture2D>("pixilart-drawing");

            base.LoadContent();

        }

        public void UI()
        {
            MyraEnvironment.Game = this;



           /* FileAssetResolver assetResolver = new FileAssetResolver(Path.Combine(PathUtils.ExecutingAssemblyDirectory, "Assets"));
            AssetManager assetManager = new AssetManager(GraphicsDevice, assetResolver);
            string data = File.ReadAllText(filePath);
            Project project = Project.LoadFromXml(data, assetManager);
*/
            var grid = new Grid
            {
                RowSpacing = 8,
                ColumnSpacing = 8
            };
            
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            /*var drawFPS = new Label
            {
                Id = "label",
                Text = frameRate.ToString(),
                TextColor = Color.GreenYellow
            };
            grid.Widgets.Add(drawFPS);
*/
            // ComboBox
            /*            var combo = new ComboBox
                        {
                            GridColumn = 1,
                            GridRow = 0
                        };

                        combo.Items.Add(new ListItem("Red", Color.Red));
                        combo.Items.Add(new ListItem("Green", Color.Green));
                        combo.Items.Add(new ListItem("Blue", Color.Blue));
                        grid.Widgets.Add(combo);

            */

            // Button
            var MenuButton = new TextButton
            {
                GridColumn = 0,
                GridRow = 0,
                Text = "    Menu    "
            };

            MenuButton.Click += (s, a) =>
            {
                /*var messageBox = Dialog.CreateMessageBox("Attention!", "Are you shure you want to quit!");
                messageBox.ShowModal(_desktop);
                messageBox.ButtonOk.Text = "Yes";
                messageBox.ButtonOk.Click += (s,a) => { Exit();};*/
                ShowContextMenu();
            };

            grid.Widgets.Add(MenuButton);

            /*if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                var messageBox = Dialog.CreateMessageBox("Message","Are you shure you want to quit");
                messageBox.ShowModal(_desktop);
            }*/


            // Add it to the desktop
            _desktop = new Desktop();
            {
                _desktop.Root = grid;
            };
        }

        public void ShowContextMenu()
        {
            if (_desktop.ContextMenu != null)
            {
                // Dont show if it's already shown
                return;
            }

            var container = new VerticalStackPanel
            {
                GridColumn = 0,
                GridRow = 0,
                Spacing = 10
            };

            var titleContainer = new Panel
            {
                Background = DefaultAssets.UITextureRegionAtlas["button"],
            };

            var titleLabel = new Label
            {
                //Text = "Choose Option",
                HorizontalAlignment = HorizontalAlignment.Center
            };

            //titleContainer.Widgets.Add(titleLabel);
            container.Widgets.Add(titleContainer);


            Project project = new Project();

            var menuItem1 = new MenuItem();
            {
                menuItem1.Text = "Save";
            };
            menuItem1.Selected += (s, a) =>
            {
                // "Start New Game" selected
                
            };

            var menuItem2 = new MenuItem
            {
                Text = "Load"
            };
            menuItem2.Selected += (s, a) => {
                FileDialog dialog = new FileDialog(FileDialogMode.OpenFile)
                {


                    Filter = "*.fbx",
                    //Folder = "E:/SteamLibrary/steamapps/common/SpaceEngineersModSDK/OriginalContent/Models/Cubes/large"
                    Folder = ""
                };

                dialog.Closed += (s, a) =>
                {
                    if (!dialog.Result)
                    {
                        // "Cancel" or Enter
                        return;
                    }

                    // "OK" or Enter
                    
                };
                dialog.ShowModal(_desktop);
            };

            var menuItem3 = new MenuItem();
            {
                menuItem3.Text = "Quit";
            };
            menuItem3.Selected += (s,a) => 
            {
                Dialog dialog = new Dialog
                {
                    Title = "ARE YOU SURE YOU WANT TO QUIT!"
                };

                dialog.ButtonOk.Text = "    Quit    ";
                dialog.ButtonCancel.Text = "    Cancel  ";
                dialog.ButtonOk.HorizontalAlignment = HorizontalAlignment.Center;
                dialog.ButtonCancel.HorizontalAlignment = HorizontalAlignment.Center;

                dialog.ButtonOk.Click += (s, a) => { Exit(); };


                dialog.ShowModal(_desktop);
            };


            var verticalMenu = new VerticalMenu();

            verticalMenu.Items.Add(menuItem1);
            verticalMenu.Items.Add(menuItem2);
            verticalMenu.Items.Add(menuItem3);

            container.Widgets.Add(verticalMenu);

            _desktop.ShowContextMenu(container, _desktop.TouchPosition);
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            /*if(Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }*/

            //mouse movement code
            mouseX = Mouse.GetState().X;
            mouseY = Mouse.GetState().Y;

            //xRotation = new Vector2(mouseX, mouseY);


            //UI controlls
            if(Keyboard.GetState().IsKeyDown(Keys.Tab) && mouseInput == false)
            {
                //mouseInput = false;   
            }
            else
            {

                //The Movement Code
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    camPosition.X -= camSpeed;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    camPosition.X += camSpeed;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    camPosition.Z += camSpeed;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    camPosition.Z -= camSpeed;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    camPosition.Y -= camSpeed;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                {
                    camPosition.Y += camSpeed;
                }

                if(Keyboard.GetState().IsKeyDown(Keys.E))
                {
                    world = Matrix.CreateRotationY(0);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Q))
                {
                }
                if(Keyboard.GetState().IsKeyDown(Keys.Tab) && mouseInput == true)
                {
                    mouseInput = !mouseInput;
                }

                //trasnlation for the camera;
                view = Matrix.CreateTranslation(camPosition) * Matrix.CreateRotationY(mouseX / GraphicsDevice.Viewport.Width);
                //view *= Matrix.CreateRotationX(mouseY /*/ GraphicsDevice.Viewport.Height * MathHelper.Clamp(1, -90, 90)*/);

                mouseInput = true;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //The color of the background
            GraphicsDevice.Clear(Color.Gray);

            DrawModel(model, world, view, projection);

            frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

            _desktop.Render();
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
}
