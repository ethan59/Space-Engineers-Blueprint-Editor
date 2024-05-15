using Myra;
using Myra.Graphics2D.UI;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.Text;

namespace SpaceEngineersShipBuilder.Scripts.Core
{
    public class ItemPanel
    {
        private Desktop _desktop;
        private Panel _rootPanel;

        public void Itempanel()
        {
            _rootPanel = new Panel
            {
                Background = new SolidBrush(new Color(0, 0, 0, 128)) // Semi-transparent background
            };

            var mainGrid = new Grid
            {
                RowSpacing = 8,
                ColumnSpacing = 8,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Toolbar Config Label
            var toolbarLabel = new Label
            {
                Text = "Toolbar Config",
                GridColumn = 1,
                GridRow = 0,
                GridColumnSpan = 2,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Search field
            var searchLabel = new Label
            {
                Text = "Search:",
                GridColumn = 0,
                GridRow = 0,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            var searchField = new TextBox
            {
                GridColumn = 0,
                GridRow = 1,
                Width = 200,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Tabs of tools and objects list
            var toolsList = new ListBox
            {
                GridColumn = 0,
                GridRow = 2,
                Width = 200,
                Height = 500,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Adding items to the tools list
            var tools = new string[]
            {
                "All Blocks",
                "Weapons and Tools",
                "Character Tools",
                "Character Animations",
                "Advanced Reactors",
                "Armor Blocks",
                "Cockpit Blocks",
                "Conv./Cargo Blocks",
                "Large Blocks",
                "Nanites Blocks",
                "Power Blocks",
                "Production Blocks",
                "Small Blocks",
                "Tiered Hydrogen Arm...",
                "Voxel Hands",
                "Window Blocks"
            };

            foreach (var tool in tools)
            {
                toolsList.Items.Add(new ListItem(tool));
            }

            // Tab content grid
            var tabContentGrid = new Grid
            {
                GridColumn = 1,
                GridRow = 1,
                RowSpan = 2,
                ColumnSpacing = 8,
                RowSpacing = 8,
                ShowGridLines = true,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };

            for (int i = 0; i < 10; i++)
            {
                tabContentGrid.ColumnsProportions.Add(new Proportion(ProportionType.Part, 1));
            }
            for (int i = 0; i < 3; i++)
            {
                tabContentGrid.RowsProportions.Add(new Proportion(ProportionType.Part, 1));
            }

            // Add items to the tab content grid (with extra blank squares)
            int totalItems = 25 + 5; // 25 items + 5 blank squares
            for (int i = 0; i < totalItems; i++)
            {
                int column = i % 10;
                int row = i / 10;

                tabContentGrid.Widgets.Add(new ImageTextButton
                {
                    Text = (i < 25) ? $"T{i + 1}" : "",
                    GridColumn = column,
                    GridRow = row,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 100, // Increase item size
                    Height = 100  // Increase item size
                });
            }

            // Add elements to main grid
            mainGrid.Widgets.Add(toolbarLabel);
            mainGrid.Widgets.Add(searchLabel);
            mainGrid.Widgets.Add(searchField);
            mainGrid.Widgets.Add(toolsList);
            mainGrid.Widgets.Add(tabContentGrid);

            _rootPanel.Widgets.Add(mainGrid);
            _desktop = new Desktop
            {
                Root = _rootPanel
            };
        }

        public void Render()
        {
            _desktop.Render();
        }

        public Widget GetRootWidget()
        {
            return _rootPanel;
        }
    }
}
