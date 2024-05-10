using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceEngineersShipBuilder.Scripts.Core
{
    public class FileLoader
    {
        private List<VertexPositionNormalTexture> vertices;
        private List<ushort> indices;
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private BasicEffect effect;
        private GraphicsDevice graphicsDevice;

        public FileLoader(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            effect = new BasicEffect(graphicsDevice)
            {
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
                            positions.Add(ParseVector3(parts));
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

                lock (graphicsDevice)
                {
                    // Create buffers
                    vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertices.Count, BufferUsage.WriteOnly);
                    vertexBuffer.SetData(vertices.ToArray());

                    indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Count, BufferUsage.WriteOnly);
                    indexBuffer.SetData(indices.ToArray());
                }

                Console.WriteLine("OBJ file loaded and buffers created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading .obj file: {ex.Message}");
            }
        }

        private void AddFace(string[] parts, List<Vector3> positions, List<Vector2> textures, List<Vector3> normals)
        {
            try
            {
                for (int i = 1; i <= 3; i++)
                {
                    string[] indexParts = parts[i].Split('/');
                    int positionIndex = int.Parse(indexParts[0]) - 1;
                    int textureIndex = indexParts.Length > 1 && indexParts[1] != "" ? int.Parse(indexParts[1]) - 1 : -1;
                    int normalIndex = indexParts.Length > 2 ? int.Parse(indexParts[2]) - 1 : -1;

                    Vector3 position = positions[positionIndex];
                    Vector2 texture = textureIndex >= 0 ? textures[textureIndex] : Vector2.Zero;
                    Vector3 normal = normalIndex >= 0 ? normals[normalIndex] : Vector3.Zero;

                    vertices.Add(new VertexPositionNormalTexture(position, normal, texture));
                    indices.Add((ushort)(vertices.Count - 1));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing face data: {ex.Message}");
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

        public void Draw(Matrix world, Matrix view, Matrix projection)
        {
            // Check if the buffers are properly initialized
            if (vertexBuffer == null || indexBuffer == null)
            {
                Console.WriteLine("Warning: Attempted to draw without loaded data.");
                return;
            }

            lock (graphicsDevice)
            {
                // Set effect parameters
                effect.World = world;
                effect.View = view;
                effect.Projection = projection;

                // Set the graphics device buffers
                graphicsDevice.SetVertexBuffer(vertexBuffer);
                graphicsDevice.Indices = indexBuffer;

                // Draw using the effect passes
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    graphicsDevice.DrawIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        0,
                        0,
                        indices.Count / 3
                    );
                }
            }
        }

    }
}
