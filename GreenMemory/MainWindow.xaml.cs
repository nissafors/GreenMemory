﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace GreenMemory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string SETTINGSFILEPATH = "memorySettings.cfg";

        public enum View { Start, Settings, Game };

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Tests.Run();

            if (!readSettingsFromFile())
            {
                SettingsModel.Columns = 4;
                SettingsModel.Rows = 4;
                SettingsModel.AgainstAI = true;
                SettingsModel.AILevel = 0.1;
            }
        }

        /// <summary>
        /// Changes the view being displayed.
        /// </summary>
        /// <param name="view"></param>
        public void ChangeView(View view)
        {
            switch (view)
            {
                case View.Start:
                    mainGrid.Children.Clear();
                    mainGrid.Children.Add(new StartView());
                    break;
                case View.Settings:
                    mainGrid.Children.Clear();
                    mainGrid.Children.Add(new SettingsView());
                    break;
                case View.Game:
                    mainGrid.Children.Clear();
                    mainGrid.Children.Add(new GameView());
                    break;
            }
        }

        /// <summary>
        /// Gets settings from an xml file. 
        /// File path specified in constant SETTINGSFILEPATH.
        /// </summary>
        /// <returns></returns>
        private bool readSettingsFromFile()
        {
            try
            {
                using (XmlReader reader = XmlReader.Create(SETTINGSFILEPATH))
                {
                    while (reader.Read())
                    {
                        switch (reader.Name)
                        {
                            case "Rows":
                                SettingsModel.Rows = reader.ReadElementContentAsInt();
                                break;

                            case "Columns":
                                SettingsModel.Columns = reader.ReadElementContentAsInt();
                                break;

                            case "AgainstAI":
                                SettingsModel.AgainstAI = reader.ReadElementContentAsBoolean();
                                break;

                            case "AILevel":
                                SettingsModel.AILevel = reader.ReadElementContentAsDouble();
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Writes settings to an xml file.
        /// File path specified in constant SETTINGSFILEPATH.
        /// </summary>
        private void writeSettingsToFile()
        {
            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true, NewLineOnAttributes = true };

            using (XmlWriter writer = XmlWriter.Create(SETTINGSFILEPATH, settings))
            {
                writer.WriteStartDocument();
                writer.WriteComment("This file was autogenerated by Memory. Do not change!");
                writer.WriteStartElement("Settings");

                writer.WriteStartElement("BoardSettings");
                writer.WriteElementString("Rows", SettingsModel.Rows.ToString());
                writer.WriteElementString("Columns", SettingsModel.Columns.ToString());
                writer.WriteEndElement();

                // TODO: Get names for players
                writer.WriteStartElement("PlayerSettings");
                writer.WriteElementString("PlayerOne", "");
                writer.WriteElementString("PlayerTwo", "");
                writer.WriteElementString("AgainstAI", SettingsModel.AgainstAI.ToString().ToLower());
                writer.WriteElementString("AILevel", SettingsModel.AILevel.ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        /// <summary>
        /// Handler for closing the main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainWindowClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            writeSettingsToFile();
        }
    }
}
