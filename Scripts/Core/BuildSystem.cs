using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

public class BuildingSystem
{
    private GraphicsDevice graphicsDevice;
    private ContentManager content;
    private List<Model> blocks;
    private Model selectedBlock;
    private Vector3 blockOffset;
    private float blockRotationX;
    private float blockRotationY;
    private float blockRotationZ;
    private bool buildingEnabled;
    private BasicEffect holographicEffect;
    private float offsetAdjustmentSpeed = 0.1f;
    private int previousScrollValue;
    private float rotationSpeed = 0.1f; // Speed of rotation
    private float distanceFromCamera = 5.0f; // Distance from camera to object

    public BuildingSystem(GraphicsDevice graphicsDevice, ContentManager content)
    {
        this.graphicsDevice = graphicsDevice;
        this.content = content;
        blocks = new List<Model>();
        LoadBlocks();
        holographicEffect = new BasicEffect(graphicsDevice)
        {
            Alpha = 0.5f,
            LightingEnabled = false,
            VertexColorEnabled = false,
            TextureEnabled = true,
            DiffuseColor = new Vector3(0, 1, 0) // Green for holographic effect
        };
        buildingEnabled = false;
        previousScrollValue = Mouse.GetState().ScrollWheelValue;
        blockRotationX = 0.0f; // Initialize rotation around X-axis
        blockRotationY = 0.0f; // Initialize rotation around Y-axis
        blockRotationZ = 0.0f; // Initialize rotation around Z-axis
    }

    private void LoadBlocks()
    {
        // Load your block models here
        blocks.Add(content.Load<Model>("cube"));
        // blocks.Add(content.Load<Model>("triangle"));
        // ... Add up to 9 blocks
    }

    public void Update(GameTime gameTime, Matrix cameraViewMatrix, Matrix cameraRotationMatrix, Vector3 cameraPosition)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        MouseState mouseState = Mouse.GetState();

        // Handle block selection (1-9)
        for (int i = 0; i < 9; i++)
        {
            if (keyboardState.IsKeyDown(Keys.D1 + i))
            {
                if (i < blocks.Count)
                {
                    selectedBlock = blocks[i];
                    buildingEnabled = true;
                }
                else
                {
                    // Handle case where block index is out of range
                    Console.WriteLine($"Block {i + 1} is not loaded.");
                }
            }
        }

        // Disable building mode (0)
        if (keyboardState.IsKeyDown(Keys.D0))
        {
            buildingEnabled = false;
        }

        // Adjust offset (left Alt + mouse wheel)
        if (keyboardState.IsKeyDown(Keys.LeftAlt))
        {
            int currentScrollValue = mouseState.ScrollWheelValue;
            int scrollDelta = currentScrollValue - previousScrollValue;
            if (scrollDelta != 0)
            {
                blockOffset += new Vector3(0, offsetAdjustmentSpeed * Math.Sign(scrollDelta), 0);
            }
            previousScrollValue = currentScrollValue;
        }

        // Rotate block around Y-axis (Insert/Delete)
        if (keyboardState.IsKeyDown(Keys.Insert))
        {
            blockRotationY -= rotationSpeed;
        }
        if (keyboardState.IsKeyDown(Keys.Delete))
        {
            blockRotationY += rotationSpeed;
        }

        // Rotate block around X-axis (Home/End)
        if (keyboardState.IsKeyDown(Keys.Home))
        {
            blockRotationX -= rotationSpeed;
        }
        if (keyboardState.IsKeyDown(Keys.End))
        {
            blockRotationX += rotationSpeed;
        }

        // Rotate block around Z-axis (PageUp/PageDown)
        if (keyboardState.IsKeyDown(Keys.PageUp))
        {
            blockRotationZ -= rotationSpeed;
        }
        if (keyboardState.IsKeyDown(Keys.PageDown))
        {
            blockRotationZ += rotationSpeed;
        }

        // Update the block position to stay in front of the camera
        Vector3 cameraForwardDirection = Vector3.Transform(Vector3.Forward, cameraRotationMatrix);
        Vector3 blockPosition = cameraPosition + cameraForwardDirection * distanceFromCamera + blockOffset;

        // Update the block's world matrix
        Matrix rotationMatrix = Matrix.CreateRotationX(blockRotationX) *
                                Matrix.CreateRotationY(blockRotationY) *
                                Matrix.CreateRotationZ(blockRotationZ);
        Matrix translationMatrix = Matrix.CreateTranslation(blockPosition);
        Matrix holographicWorld = rotationMatrix * translationMatrix;

        DrawBlock(holographicWorld, cameraViewMatrix, Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), graphicsDevice.Viewport.AspectRatio, 0.1f, 1000f));
    }

    public void Draw(Matrix world, Matrix view, Matrix projection, Matrix cameraRotationMatrix)
    {
        if (buildingEnabled && selectedBlock != null)
        {
            // Ensure the block follows the camera's position and rotation
            Vector3 cameraForwardDirection = Vector3.Transform(Vector3.Forward, cameraRotationMatrix);
            Vector3 blockPosition = cameraForwardDirection * distanceFromCamera + blockOffset;

            Matrix rotationMatrix = Matrix.CreateRotationX(blockRotationX) *
                                    Matrix.CreateRotationY(blockRotationY) *
                                    Matrix.CreateRotationZ(blockRotationZ);
            Matrix translationMatrix = Matrix.CreateTranslation(blockPosition);
            Matrix holographicWorld = rotationMatrix * translationMatrix;

            DrawBlock(holographicWorld, view, projection);
        }
    }

    private void DrawBlock(Matrix world, Matrix view, Matrix projection)
    {
        if (buildingEnabled && selectedBlock != null)
        {
            foreach (ModelMesh mesh in selectedBlock.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = holographicEffect;
                    holographicEffect.World = world;
                    holographicEffect.View = view;
                    holographicEffect.Projection = projection;
                }

                graphicsDevice.BlendState = BlendState.AlphaBlend;
                graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                mesh.Draw();
                graphicsDevice.BlendState = BlendState.Opaque;
                graphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
        }
    }
}
