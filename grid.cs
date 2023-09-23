using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceEngineersShipBuilder
{
    public class grid
    {
        private VertexPositionColor[] vertices;
        private int[] indices;
        private BasicEffect effect;
        private GraphicsDevice graphicsDevice;
        private Color color;
        private Matrix worldPosition;


        public Vector3 Position 
        {
            get { return worldPosition.Translation; }
            set { worldPosition.Translation = value; }
        }

        public grid(GraphicsDevice graphicsDevice, int gridSize, float cellSize, float yPos)
        {
            this.graphicsDevice = graphicsDevice;
            int numVertices = (gridSize + 1) * 4;
            int numIndices = (gridSize + 1) * 4;

            vertices = new VertexPositionColor[numVertices];
            indices = new int[numIndices];
            effect = new BasicEffect(graphicsDevice);
            color = new Color(Color.DarkGray,0);

            // Create the grid vertices and indices
            CreateGrid(gridSize, cellSize, yPos);

            // Initialize the world matrix with an identity matrix
            worldPosition = Matrix.Identity;

        }


        private void CreateGrid(int gridSize, float cellSize, float yHeight)
        {
            // Generate grid vertices with the specified Y height
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
        }



        public void Draw(Matrix view, Matrix projection)
        {
            effect.View = view;
            effect.Projection = projection;
            effect.VertexColorEnabled = true;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    vertices,
                    0,
                    vertices.Length,
                    indices,
                    0,
                    indices.Length / 2
                );
            }
        }
    }
}
