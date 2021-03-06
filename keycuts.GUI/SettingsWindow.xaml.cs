﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace keycuts.GUI
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window, INotifyPropertyChanged
    {
        #region Fields

        private MainFormLogic mainFormLogic;

        private Settings settings;

        private string outputFolder;

        private bool forceOverwrite;

        private bool rightClickContextMenus;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Fields

        #region Properties

        public string LabelOutputFolder { get; } = "Output Folder";

        public string LabelForceOverwrite { get; } = "Force Overwrite (if shortcut exists)";

        public string LabelRightClickContextMenus { get; } = "Right Click Context Menus";

        public string OutputFolder
        {
            get { return outputFolder; }
            set
            {
                outputFolder = value;
                NotifyPropertyChanged("OutputFolder");
            }
        }

        public bool ForceOverwrite
        {
            get { return forceOverwrite; }
            set
            {
                forceOverwrite = value;
                NotifyPropertyChanged("ForceOverwrite");
            }
        }

        public bool RightClickContextMenus
        {
            get { return rightClickContextMenus; }
            set
            {
                rightClickContextMenus = value;
                NotifyPropertyChanged("RightClickContextMenu");
            }
        }

        #endregion Properties

        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = this;

            settings = new Settings();
            mainFormLogic = new MainFormLogic();
            LoadSettings();
        }

        private void LoadSettings()
        {
            settings.LoadSettings();

            OutputFolder = settings.OutputFolder;
            ForceOverwrite = settings.ForceOverwrite;
            RightClickContextMenus = settings.RightClickContextMenus;
        }

        private void CloseWindow()
        {
            Close();
        }

        #region UI Handlers

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new Settings()
            {
                OutputFolder = OutputFolder,
                ForceOverwrite = (bool)CheckboxForceOverwrite.IsChecked,
                RightClickContextMenus = RightClickContextMenus
            };
            
            mainFormLogic.SaveSettings(settings);
        }

        private void Settings_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CloseWindow();
            }
        }

        #endregion UI Handlers

        #region OnPropertyChanged Handler

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion OnPropertyChanged Handler
    }
}
