using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

public class FileLoader
{
    private List<VertexPositionNormalTexture> vertices;
    private List<ushort> indices;
    private VertexBuffer vertexBuffer;
    private IndexBuffer indexBuffer;
    private BasicEffect effect;
    private GraphicsDevice graphicsDevice;
    public bool isDataLoaded = false;

    public FileLoader(GraphicsDevice graphicsDevice, Texture2D texture)
    {
        this.graphicsDevice = graphicsDevice;
        effect = new BasicEffect(graphicsDevice)
        {
            TextureEnabled = true,
            Texture = texture,
            VertexColorEnabled = false,
            LightingEnabled = true
        };
    }

    public async Task LoadDataAsync(string filePath, Action onComplete)
    {
        if (Path.GetExtension(filePath).Equals(".obj", StringComparison.OrdinalIgnoreCase))
        {
            await Task.Run(() => LoadObj(filePath));
            onComplete?.Invoke(); // Callback after loading completes
            isDataLoaded = true;
        }
        else
        {
            Console.WriteLine("Unsupported file format.");
        }
    }

    private void LoadObj(string filePath)
    {
        vertices = new List<VertexPositionNormalTexture>();
        indices = new List<ushort>();
        List<Vector3> positions = new List<Vector3>();
        List<Vector2> textures = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        try
        {
            foreach (string line in File.ReadLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2) continue;

                switch (parts[0])
                {
                    case "v": // Vertex positions
                        Vector3 position = ParseVector3(parts);
                        positions.Add(position);
                        Console.WriteLine($"Vertex Position - X: {position.X}, Y: {position.Y}, Z: {position.Z}");
                        break;
                    case "vt": // Texture coordinates
                        textures.Add(ParseVector2(parts));
                        break;
                    case "vn": // Normals
                        normals.Add(ParseVector3(parts));
                        break;
                    case "f": // Faces
                        AddFace(parts, positions, textures, normals);
                        break;
                }
            }

            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertices.Count, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices.ToArray());
            indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Count, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices.ToArray());

            Debug.WriteLine($"OBJ file loaded and buffers created with {vertices.Count} vertices.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading .obj file: {ex.Message}");
            isDataLoaded = false;
        }
    }


    private static Vector3 ParseVector3(string[] parts)
    {
        return new Vector3(
            float.Parse(parts[1], CultureInfo.InvariantCulture),
            float.Parse(parts[2], CultureInfo.InvariantCulture),
            float.Parse(parts[3], CultureInfo.InvariantCulture)
        );
    }

    private static Vector2 ParseVector2(string[] parts)
    {
        return new Vector2(
            float.Parse(parts[1], CultureInfo.InvariantCulture),
            float.Parse(parts[2], CultureInfo.InvariantCulture)
        );
    }

    private void AddFace(string[] parts, List<Vector3> positions, List<Vector2> textures, List<Vector3> normals)
    {
        for (int i = 1; i < parts.Length; i++)
        {
            string[] indexParts = parts[i].Split('/');
            int positionIndex = int.Parse(indexParts[0]) - 1;
            int textureIndex = indexParts.Length > 1 && !string.IsNullOrEmpty(indexParts[1]) ? int.Parse(indexParts[1]) - 1 : -1;
            int normalIndex = indexParts.Length > 2 ? int.Parse(indexParts[2]) - 1 : -1;

            Vector3 position = positions[positionIndex];
            Vector2 texture = textureIndex >= 0 ? textures[textureIndex] : Vector2.Zero;
            Vector3 normal = normalIndex >= 0 ? normals[normalIndex] : Vector3.Zero;

            // Flip the normal
            //normal = -normal;

            vertices.Add(new VertexPositionNormalTexture(position, normal, texture));
            indices.Add((ushort)(vertices.Count - 1));
        }
    }


    public void ConfigureBasicEffect()
    {
        effect.LightingEnabled = true;  // Enable lighting
        effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);  // Add some ambient lighting
        effect.DirectionalLight0.Enabled = true;  // Enable a directional light
        effect.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);  // White light
        effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1, -1, -1));  // Light coming from above the model
        effect.DirectionalLight0.SpecularColor = Vector3.One;  // Specular highlights
    }

    public void Draw(Matrix world, Matrix view, Matrix projection)
    {
        if (!isDataLoaded || vertexBuffer == null || indexBuffer == null)
        {
            Debug.WriteLine("Attempted to draw without loaded or initialized data.");
            return;
        }
        ConfigureBasicEffect();
        graphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.None };
        effect.World = world;
        effect.View = view;
        effect.Projection = projection;

        graphicsDevice.SetVertexBuffer(vertexBuffer);
        graphicsDevice.Indices = indexBuffer;

        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, indices.Count / 3);
        }
    }

}
