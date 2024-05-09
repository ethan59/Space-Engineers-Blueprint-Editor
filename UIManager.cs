using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Myra;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.File;
using System;

namespace SpaceEngineersShipBuilder
{
    public class UIManager
    {
        private Desktop _mainDesktop;

        public UIManager(Game game)
        {
            MyraEnvironment.Game = game;
            _mainDesktop = new Desktop();
            CreateUI();
        }

        private void CreateUI()
        {
            var grid = new Grid
            {
                RowSpacing = 8,
                ColumnSpacing = 8
            };

            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.ColumnsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));
            grid.RowsProportions.Add(new Proportion(ProportionType.Auto));

            var menuButton = new TextButton
            {
                GridColumn = 0,
                GridRow = 0,
                Text = "Menu"
            };

            menuButton.Click += (s, a) => { ShowContextMenu(); };

            grid.Widgets.Add(menuButton);
            _mainDesktop.Root = grid;
        }

        public void ShowContextMenu()
        {
            if (_mainDesktop.ContextMenu != null)
            {
                return;
            }

            var verticalMenu = new VerticalMenu();

            var saveItem = new MenuItem
            {
                Text = "Save"
            };

            saveItem.Selected += (s, a) => { Console.WriteLine("Save Selected"); };

            var loadItem = new MenuItem
            {
                Text = "Load"
            };

            loadItem.Selected += (s, a) =>
            {
                var dialog = new FileDialog(FileDialogMode.OpenFile)
                {
                    Filter = "*.fbx",
                    Folder = "E:/SteamLibrary/steamapps/common/SpaceEngineersModSDK/OriginalContent/Models/Cubes/large"
                };

                dialog.Closed += (dialogSender, dialogArgs) =>
                {
                    if (!dialog.Result) return;
                    Console.WriteLine($"Loaded: {dialog.FilePath}");
                };

                dialog.ShowModal(_mainDesktop);
            };

            var quitItem = new MenuItem
            {
                Text = "Quit"
            };

            quitItem.Selected += (s, a) =>
            {
                var dialog = Dialog.CreateMessageBox("Confirm Exit", "Are you sure you want to quit?");
                dialog.ButtonOk.Text = "Quit";
                dialog.ButtonCancel.Text = "Cancel";
                dialog.ButtonOk.Click += (dialogSender, dialogArgs) =>
                {
                    MyraEnvironment.Game.Exit();
                };

                dialog.ShowModal(_mainDesktop);
            };

            verticalMenu.Items.Add(saveItem);
            verticalMenu.Items.Add(loadItem);
            verticalMenu.Items.Add(quitItem);

            _mainDesktop.ShowContextMenu(verticalMenu, _mainDesktop.TouchPosition);
        }

        public void Render()
        {
            _mainDesktop.Render();
        }
    }

}
