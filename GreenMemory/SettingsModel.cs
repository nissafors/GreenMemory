using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Globalization;

namespace GreenMemory
{
    class SettingsModel
    {
        private const string SETTINGSFILEPATH = "memorySettings.cfg";

        public enum SettingsType {Rows, Columns, AgainstAI, AIDifficulty, Sound, Music, Theme};

        private static int rows;
        private static int columns;
        private static bool hasAi;
        private static volatile AIModel.Difficulty aiLevel;
        private static bool sound;
        private static bool music;
        private static int theme;
        /*
        static public int Rows { get; set; }
        static public int Columns { get; set; }
        static public bool AgainstAI { get; set; }
        static public AIModel.Difficulty AILevel { get; set; }
        static public bool Sound { get; set; }
        static public bool Music { get; set; }
        static public int Theme { get; set; }
        */
        //*
        static public int Rows
        {
            get
            {
                return rows;
            }
            set
            {
                rows = value;
                alertListeners(SettingsType.Rows);

            }
        }

        static public int Columns
        {
            get
            {
                return columns;
            }

            set 
            {
                columns = value;
                alertListeners(SettingsType.Columns);
            }
        }

        static public bool AgainstAI
        {
            get
            {
                return hasAi;
            }

            set
            {
                hasAi = value;
                alertListeners(SettingsType.AgainstAI);
            }
        }

        static public AIModel.Difficulty AILevel
        {
            get
            {
                return aiLevel;
            }

            set
            {
                aiLevel = value;
                alertListeners(SettingsType.AIDifficulty);
            }
        }

        static public bool Sound
        {
            get
            {
                return sound;
            }

            set
            {
                sound = value;
                alertListeners(SettingsType.Sound);
            }
        }

        static public bool Music
        {
            get
            {
                return music;
            }

            set
            {
                music = value;
                alertListeners(SettingsType.Music);
            }
        }

        static public int Theme
        {
            get
            {
                return theme;
            }

            set
            {
                theme = value;
                alertListeners(SettingsType.Theme);
            }
        } 
        // List of listeners 
        private static List<Action<SettingsType>> changeSettingsListeners = new List<Action<SettingsType>>();

        /// Alert all listeners
        private static void alertListeners(SettingsType type)
        {
            foreach(Action<SettingsType> f in changeSettingsListeners)
            {
                f(type);
            }
        }
       
        /// <summary>
        /// Add a listener for when a setting is changed
        /// The (Action) callback must take a SettingsType enum as argument
        /// </summary>
        /// <param name="Callback"></param>
        public static void AddChangeSettingsListener(Action<SettingsType> callback)
        {
            changeSettingsListeners.Add(callback);
        }

        /// <summary>
        /// Remove a specific callback, NOT RECOMMENDED! Use: ClearListeners
        /// </summary>
        /// <param name="callback"></param>
        public static void RemoveChangeSettingsListener(Action<SettingsType> callback)
        {
            changeSettingsListeners.Remove(callback);
        }

        /// <summary>
        /// Clear all Listeners
        /// </summary>
        public static void ClearListeners()
        {
            changeSettingsListeners.Clear();
        }


        static public string SoundPath
        {
            get
            {
                string path = string.Empty;
                switch (Theme)
                {
                    case 0:
                        path = "Game/Sounds/Poker/";
                        break;

                    case 1:
                        path = "Game/Sounds/Pokemon/";
                        break;

                    case 2:
                        path = "Game/Sounds/Nerd/";
                        break;

                    case 3:
                        path = "Game/Sounds/Neon/";
                        break;

                    default:
                        path = "Game/Sounds/Common/";
                        break;
                }

                return path;
            }
        }

        static public string CardImagePath
        {
            get
            {
                string path = string.Empty;
                switch(Theme)
                {
                    case 0:
                        path = "Game\\Poker\\";
                        break;

                    case 1:
                        path = "Game\\Pokemon\\";
                        break;

                    case 2:
                        path = "Game\\Nerd\\";
                        break;

                    case 3:
                        path = "Game\\Neon\\";
                        break;
                }

                return path;
            }
        }
        static public string GameviewBackgroundPath { 
            get
            {
                string path = string.Empty;
                switch (Theme)
                {
                    case 0:
                        path = "Game\\Backgrounds\\Filt Background.png";
                        break;

                    case 1:
                        path = "Game\\Backgrounds\\pokemon background.png";
                        break;

                    case 2:
                        path = "Game\\Backgrounds\\Background Nerd.png";
                        break;

                    case 3:
                        path = "Game\\Backgrounds\\Neonbackground.png";
                        break;
                }

                return path;
            }
            }
        static public string TopPlayerName { get; set; }
        static public string BottomPlayerName { get; set; }

        /// <summary>
        /// Gets settings from an xml file. 
        /// File path specified in constant SETTINGSFILEPATH.
        /// </summary>
        /// <returns></returns>
        public static bool readSettingsFromFile()
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
                                AIModel.Difficulty tmpAILevel;
                                if (Enum.TryParse<AIModel.Difficulty>(reader.ReadElementContentAsString(), out tmpAILevel))
                                    SettingsModel.AILevel = tmpAILevel;
                                else
                                    SettingsModel.AILevel = AIModel.Difficulty.Medium;
                                break;

                            case "Sound":
                                SettingsModel.Sound = reader.ReadElementContentAsBoolean();
                                break;

                            case "Music":
                                SettingsModel.Music = reader.ReadElementContentAsBoolean();
                                break;

                            case "Theme":
                                SettingsModel.Theme = reader.ReadElementContentAsInt();
                                break;

                            case "TopPlayer":
                                SettingsModel.TopPlayerName = reader.ReadElementContentAsString();
                                break;

                            case "BottomPlayer":
                                SettingsModel.BottomPlayerName = reader.ReadElementContentAsString();
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
        public static void writeSettingsToFile()
        {
            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true, NewLineOnAttributes = true };

            using (XmlWriter writer = XmlWriter.Create(SETTINGSFILEPATH, settings))
            {
                writer.WriteStartDocument();
                writer.WriteComment("This file was autogenerated by Memory. Do not change!");
                writer.WriteStartElement("Settings");

                writer.WriteStartElement("GeneralSettings");
                writer.WriteElementString("Music", SettingsModel.Music.ToString().ToLower());
                writer.WriteElementString("Sound", SettingsModel.Sound.ToString().ToLower());
                writer.WriteEndElement();

                writer.WriteStartElement("BoardSettings");
                writer.WriteElementString("Rows", SettingsModel.Rows.ToString());
                writer.WriteElementString("Columns", SettingsModel.Columns.ToString());
                writer.WriteElementString("Theme", SettingsModel.Theme.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("PlayerSettings");
                writer.WriteElementString("TopPlayer", SettingsModel.TopPlayerName);
                writer.WriteElementString("BottomPlayer", SettingsModel.BottomPlayerName);
                writer.WriteElementString("AgainstAI", SettingsModel.AgainstAI.ToString().ToLower());
                writer.WriteElementString("AILevel", SettingsModel.AILevel.ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }
}
