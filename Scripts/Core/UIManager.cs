using Microsoft.Xna.Framework;
using Myra;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.File;
using SpaceEngineersShipBuilder.Scripts.Core;
using System;
using System.Diagnostics;

namespace SpaceEngineersShipBuilder
{
    public class UIManager
    {
        private readonly Game _game;
        private readonly FileLoader _fileLoader;
        private readonly Desktop _desktop;

        public UIManager(Game game, FileLoader fileLoader)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _fileLoader = fileLoader ?? throw new ArgumentNullException(nameof(fileLoader));

            MyraEnvironment.Game = _game;
            _desktop = new Desktop();
            CreateUI();
        }

        private void CreateUI()
        {
            var grid = new Grid
            {
                RowSpacing = 8,
                ColumnSpacing = 8,
                ColumnsProportions = { new Proportion(ProportionType.Auto) },
                RowsProportions = { new Proportion(ProportionType.Auto) }
            };

            var menuButton = new TextButton
            {
                GridColumn = 0,
                GridRow = 0,
                Text = "    Menu    "
            };

            menuButton.Click += (s, a) => ShowContextMenu();
            grid.Widgets.Add(menuButton);

            _desktop.Root = grid;
        }

        private void ShowContextMenu()
        {
            if (_desktop.ContextMenu != null)
                return;

            var container = new VerticalStackPanel { Spacing = 10 };

            var menuItemLoad = new MenuItem { Text = "Load" };
            menuItemLoad.Selected += (s, a) => OpenFileDialog();

            var menuItemQuit = new MenuItem { Text = "Quit" };
            menuItemQuit.Selected += (s, a) => ShowQuitDialog();

            var verticalMenu = new VerticalMenu();
            verticalMenu.Items.Add(menuItemLoad);
            verticalMenu.Items.Add(menuItemQuit);
            container.Widgets.Add(verticalMenu);

            _desktop.ShowContextMenu(container, _desktop.TouchPosition);
        }

        private void OpenFileDialog()
        {
            var fileDialog = new FileDialog(FileDialogMode.OpenFile)
            {
                Filter = "*.obj",
                Folder = "C:/Your/Default/Folder"
            };

            fileDialog.Closed += async (sender, args) =>
            {
                if (fileDialog.Result)
                {
                    string selectedFilePath = fileDialog.FilePath;
                    await _fileLoader.LoadDataAsync(selectedFilePath, () => Debug.WriteLine("File loading complete!"));
                }
                else
                {
                    Console.WriteLine("File selection was cancelled.");
                }
            };

            fileDialog.ShowModal(_desktop);
        }

        private void ShowQuitDialog()
        {
            var dialog = new Dialog { Title = "Are you sure you want to quit?" };

            dialog.ButtonOk.Text = "Quit";
            dialog.ButtonCancel.Text = "Cancel";
            dialog.ButtonOk.Click += (sender, args) => Environment.Exit(0);
            dialog.ShowModal(_desktop);
        }

        public void Render()
        {
            _desktop.Render();
        }
    }
}
