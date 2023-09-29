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
        public Desktop _desktop;
        public UIManager(Game game) 
        {
            MyraEnvironment.Game = game;
            CreateUI();
        }
        public void CreateUI() 
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
                messageBox.ButtonOk.Click += (s, a) => { Exit(); };*/
                ShowContextMenu();
            };


            grid.Widgets.Add(MenuButton);

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


            Project project = new();

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
            menuItem2.Selected += (s, a) =>
            {
                FileDialog dialog = new(FileDialogMode.OpenFile)
                {


                    Filter = "*.fbx",
                    Folder = "E:/SteamLibrary/steamapps/common/SpaceEngineersModSDK/OriginalContent/Models/Cubes/large"
                    //Folder = ""
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
            menuItem3.Selected += (s, a) =>
            {
                Dialog dialog = new()
                {
                    Title = "ARE YOU SURE YOU WANT TO QUIT!"
                };

                dialog.ButtonOk.Text = "    Quit    ";
                dialog.ButtonCancel.Text = "    Cancel  ";
                dialog.ButtonOk.HorizontalAlignment = HorizontalAlignment.Center;
                dialog.ButtonCancel.HorizontalAlignment = HorizontalAlignment.Center;

                //dialog.ButtonOk.Click += (s, a) => { Exit(); };


                dialog.ShowModal(_desktop);
            };


            var verticalMenu = new VerticalMenu();

            verticalMenu.Items.Add(menuItem1);
            verticalMenu.Items.Add(menuItem2);
            verticalMenu.Items.Add(menuItem3);

            container.Widgets.Add(verticalMenu);

            _desktop.ShowContextMenu(container, _desktop.TouchPosition);
        }
    }
}
