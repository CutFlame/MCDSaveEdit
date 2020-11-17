using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
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
            Title = R.GAME_FILES_WINDOW_TITLE;
            messageTextBlock.Text = R.GAME_FILES_WINDOW_MESSAGE;
            gameFilesGroupBox.Header = R.GAME_FILES_WINDOW_GROUPBOX_HEADER;
            pathLabel.Content = R.GAME_FILES_WINDOW_TEXTBOX_LABEL;
            pathTextBox.Text = string.Empty;
            exitButton.Content = R.EXIT;
            okButton.Content = R.OK;
            noButton.Content = R.GAME_FILES_WINDOW_NO_CONTENT_BUTTON;
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            EventLogger.logEvent("exitButton_Click");
            useSelectedPath = null;
            this.Close();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            bool isValidPath = isValidSelectedPath();
            EventLogger.logEvent("okButton_Click", new Dictionary<string, object> { { "isValidPath", isValidPath } });
            if (isValidPath)
            {
                useSelectedPath = true;
                this.Close();
                return;
            }

            MessageBox.Show(R.NO_GAME_FILES_FOUND_ERROR_MESSAGE, R.ERROR);
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
            EventLogger.logEvent("noButton_Click");
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
            EventLogger.logEvent("pathBrowseButton_Click");
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.EnsurePathExists = true;
            dialog.ShowHiddenItems = true;
            var appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            dialog.InitialDirectory = appDataFolderPath;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                selectedPath = dialog.FileName;
            }            
        }
    }
}
