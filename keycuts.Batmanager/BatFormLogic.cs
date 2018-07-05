﻿using keycuts.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace keycuts.Batmanager
{
    public class BatFormLogic
    {
        #region Fields

        private static readonly string explorer = "\\explorer.exe\"";
        private static readonly string start = "START ";
        //START "" \/[BD] .+ "([A - Za - z0 - 9:\\ \/\-\.\(\)\%]+)"$
        //private static readonly string startBD = "START \"\" \\/[BD] .+ \"([A-Za-z0-9:\\ \\/\\-\\.\\(\\)\\%]+)\"$";

        private static readonly string startBD = "START \"\" \\/[BD] \".+\" \"([A-Za-z0-9:\\ \\/\\-\\.\\(\\)\\%]+)\"$";



        private static readonly string startB = $"{start} \"\" /B \"";
        private static readonly string startD = $"{start} \"\" /D \"";

        private static readonly string alphanumeric = "A-Za-z0-9";
        private static readonly string alphanumericspecial = $"{alphanumeric}:\\.";
        private static readonly string alphanumericurl = $@"{alphanumeric}:/.";

        public static string URL = $"START [{alphanumericurl}]+";
        public static string FILE = $"START \"\" /B \"[{alphanumericspecial}]+";
        public static string FOLDER = $"\"%SystemRoot%\\explorer.exe\" \"[{alphanumericspecial}]+\"";
        public static string OPENWITHAPP = $"\"[{alphanumericspecial}]+\" \"[{alphanumericspecial}]+\"";

        public static List<BatParseArg> BatList = new List<BatParseArg>()
        {
            new BatParseArg(URL, ShortcutType.Url),
            new BatParseArg(FILE, ShortcutType.File),
            new BatParseArg(FOLDER, ShortcutType.Folder)
        };

        #endregion Fields

        #region Public Methods

        public static void PopulateDataGrid(DataGrid dataGrid)
        {
            var outputFolder = RegistryStuff.GetOutputFolder(@"C:\Shortcuts");
            var batFiles = Directory.GetFiles(outputFolder, "*.bat").ToList();
            var bats = ParseBats(batFiles);
            dataGrid.ItemsSource = bats;
        }

        public static List<Bat> ParseBats(List<string> batFiles)
        {
            var bats = new List<Bat>();

            foreach (var batFile in batFiles)
            {
                var allLines = File.ReadAllLines(batFile);
                var lines = allLines
                    .Where(x => x.Contains(explorer) ||
                                x.StartsWith(start, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();

                var bat = new Bat();

                foreach (var line in lines)
                {
                    bat.Shortcut = Path.GetFileNameWithoutExtension(batFile);

                    if (line.Contains(explorer))
                    {
                        // It's a Folder!
                        bat.Command = line;
                        bat.ShortcutType = ShortcutType.Folder;
                        
                        //var asdf = line.IndexOfAny('"',)
                        //var secondToLastQuotes = line.IndexOf("\"", )

                        break;
                    }
                    else if (line.StartsWith(start, StringComparison.CurrentCultureIgnoreCase))
                    {
                        // It's NOT a folder! -- Could be File, Url, HostsFile, or CLSIDKey
                        bat.Command = line;

                        Console.WriteLine($"Line: {line}");

                        if (!line.Contains("\""))
                        {
                            // No quotes -- It's a Url!
                            bat.Destination = line.Substring(6);
                            bat.ShortcutType = ShortcutType.Url;
                        }
                        //else if (line.StartsWith(startB) || line.StartsWith(startD))
                        //{
                        //    //var 
                        //    bat.ShortcutType = ShortcutType.File;
                        //}
                        else
                        {
                            var regex = new Regex(startBD, RegexOptions.IgnoreCase);
                            var match = regex.Match(line);
                            //var matchCount = 0;

                            if (match.Success)
                            {
                                bat.Destination = match.Groups[1].Value;
                                bat.ShortcutType = ShortcutType.File;

                                //Console.WriteLine($"Match: {++matchCount}");
                                //for (int i = 1; i <= 2; i++)
                                //{
                                //    var group = match.Groups[i];
                                //    Console.WriteLine($"Group: {i}='{group}'");

                                //    CaptureCollection cc = group.Captures;
                                //    for (int j = 0; j < cc.Count; j++)
                                //    {
                                //        Capture capture = cc[j];
                                //        Console.WriteLine($"Capture: {j}='{cc}', Position={capture.Index}");
                                //    }
                                //}
                                //match = match.NextMatch();
                            }
                            else
                            {
                                bat.ShortcutType = ShortcutType.CLSIDKey;
                            }
                        }

                        break;
                    }
                }

                if (!string.IsNullOrEmpty(bat.Shortcut) &&
                    !string.IsNullOrEmpty(bat.Command) &&
                    bat.ShortcutType != ShortcutType.Unknown)
                {
                    bats.Add(bat);
                }


                //if (lines.Any())
                //{
                //    var shortcut = Path.GetFileNameWithoutExtension(batFile);
                //    var command = lines[0];
                //    var destination = "";
                //    var shortcutType = ShortcutType.Unknown;
                //    var openWithApp = "";

                //    if (command.Contains(explorer))
                //    {
                //        // It's a folder!
                //        shortcutType = ShortcutType.Folder;
                //    }
                //    else if (command.Substring(0, 6).ToUpper() == start)
                //    {
                //        // It's NOT a folder! -- Could be File, Url, HostsFile, or CLSIDKey


                //        shortcutType = ShortcutType.File;
                //    }

                //    if (!string.IsNullOrEmpty(shortcut) && 
                //        !string.IsNullOrEmpty(command))
                //    {
                //        bats.Add(new Bat(shortcut, command, destination, shortcutType, openWithApp));
                //    }
                //}
            }

            return bats;
        }

        #endregion Public Methods
    }
}
