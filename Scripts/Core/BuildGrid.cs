using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
namespace SpaceEngineersShipBuilder.Scripts.Core
{
    public static class GridUtility
    {
        public static (VertexPositionColor[], int[]) CreateGrid(int gridSize, float cellSize, float yHeight, Color color)
        {
            try
            {
                int numVertices = (gridSize + 1) * 4;
                int numIndices = (gridSize + 1) * 4;
                VertexPositionColor[] vertices = new VertexPositionColor[numVertices];
                int[] indices = new int[numIndices];

                for (int i = 0; i < gridSize + 1; i++)
                {
                    vertices[i * 4] = new VertexPositionColor(new Vector3(i * cellSize, yHeight, 0), color);
                    vertices[i * 4 + 1] = new VertexPositionColor(new Vector3(i * cellSize, yHeight, gridSize * cellSize), color);
                    vertices[i * 4 + 2] = new VertexPositionColor(new Vector3(0, yHeight, i * cellSize), color);
                    vertices[i * 4 + 3] = new VertexPositionColor(new Vector3(gridSize * cellSize, yHeight, i * cellSize), color);

                    indices[i * 2] = i * 4;
                    indices[i * 2 + 1] = i * 4 + 1;
                    indices[(gridSize + 1) * 2 + i * 2] = i * 4 + 2;
                    indices[(gridSize + 1) * 2 + i * 2 + 1] = i * 4 + 3;
                }

                return (vertices, indices);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while creating the grid: {ex.Message}");
                return (Array.Empty<VertexPositionColor>(), Array.Empty<int>());
            }
        }
    }

    public class BuildGrid
    {
        private readonly VertexBuffer _vertexBuffer;
        private readonly IndexBuffer _indexBuffer;
        private readonly BasicEffect _effect;
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Matrix _worldPosition;

        public Vector3 Position => _worldPosition.Translation;

        public BuildGrid(GraphicsDevice graphicsDevice, int gridSize, float cellSize, float yPos, Color color)
        {
            _graphicsDevice = graphicsDevice;
            _effect = new BasicEffect(graphicsDevice)
            {
                VertexColorEnabled = true
            };

            // Create grid vertices and indices using the utility class
            var (vertices, indices) = GridUtility.CreateGrid(gridSize, cellSize, yPos, color);

            // Initialize buffers if there are valid vertices and indices
            if (vertices.Length > 0 && indices.Length > 0)
            {
                _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
                _vertexBuffer.SetData(vertices);

                _indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
                _indexBuffer.SetData(indices);
            }
            else
            {
                Console.WriteLine("Grid creation failed. Check your parameters.");
                throw new ArgumentException("Invalid grid parameters.");
            }

            _worldPosition = Matrix.Identity;
        }

        public void Draw(Matrix view, Matrix projection)
        {
            if (_vertexBuffer == null || _indexBuffer == null) return;

            _effect.View = view;
            _effect.Projection = projection;

            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                _graphicsDevice.SetVertexBuffer(_vertexBuffer);
                _graphicsDevice.Indices = _indexBuffer;

                _graphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.LineList,
                    0,
                    0,
                    _indexBuffer.IndexCount / 2
                );
            }
        }
    }
}
