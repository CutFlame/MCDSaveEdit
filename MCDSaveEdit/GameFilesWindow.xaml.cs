using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Windows;
#nullable enable

namespace MCDSaveEdit
{
    /// <summary>
    /// Interaction logic for GameFilesWindow.xaml
    /// </summary>
    public partial class GameFilesWindow : Window
    {
        public string? selectedPath { get { return pathTextBox.Text; } private set { pathTextBox.Text = value; } }
        public bool? useSelectedPath { get; private set; }
        public Action? onClose;

        public GameFilesWindow()
        {
            InitializeComponent();
            setConstantStrings();
        }

        private void setConstantStrings()
        {
            Title = "Launch using game content";
            messageTextBlock.Text = "Could not find game content files in the default install location. Please provide the path to the game files or launch using no game content.";
            gameFilesGroupBox.Header = "Game Files";
            pathLabel.Content = "Path";
            pathTextBox.Text = "";
            exitButton.Content = "Exit";
            okButton.Content = "OK";
            noButton.Content = "No game content";
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            useSelectedPath = null;
            this.Close();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (isValidSelectedPath())
            {
                useSelectedPath = true;
                this.Close();
                return;
            }

            MessageBox.Show("Could not find game content files at the given path.", R.ERROR);
        }

        private bool isValidSelectedPath()
        {
            if (string.IsNullOrWhiteSpace(selectedPath))
            {
                return false;
            }

            var testPath = selectedPath!;
            if (File.Exists(Path.Combine(testPath, Constants.GAME_EXECUTABLE_FILENAME)))
            {
                testPath = Path.Combine(testPath, "Dungeons", "Content", "Paks");
            }

            if(File.Exists(Path.Combine(testPath, Constants.FIRST_PAK_FILENAME)))
            {
                selectedPath = testPath;
                return true;
            }
            return false;
        }

        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            useSelectedPath = false;
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            onClose?.Invoke();
        }

        private void pathBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.EnsurePathExists = true;
            dialog.ShowHiddenItems = true;
            var appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            dialog.InitialDirectory = appDataFolderPath;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                pathTextBox.Text = dialog.FileName;
            }            
        }
    }
}
