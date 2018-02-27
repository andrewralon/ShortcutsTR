﻿using Shell32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShortcutsTR
{
    class Shortcut
    {
        public string Destination { get; private set; }

        public string Extension { get; private set; } = ".bat";

        public string Filename { get; private set; }

        public string FilenameWithExtension { get; private set; }

        public string Folder { get; private set; }

        public string FullPath { get; private set; }

        public ShortcutType Type { get; private set; } = ShortcutType.Unknown;

        // TODO Handle 2 args:
        //  - Case 1: 2nd arg is full path -> split into folder (w/ path) and name (w/o extension)
        //  - Case 2: 2nd arg is filename only -> use default shortcuts folder

        // TODO Handle 3 args:
        //  - destination, name, appToUse = null -> see above

        public Shortcut(string destination, string path)
        {
            // TODO Detect if 2nd arg is full path or filename only and handle both
            //  If filename only, create shortcut file in default shortcuts folder

            Destination = GetWindowsLinkTargetPath(destination);
            Extension = ".bat";
            Filename = Path.GetFileNameWithoutExtension(path);
            FilenameWithExtension = string.Format("{0}{1}", Filename, Extension);
            Folder = Path.GetDirectoryName(path);
            FullPath = Path.Combine(Folder, string.Format("{0}{1}", Filename, Extension));
            Type = GetShortcutType();
        }

        private ShortcutType GetShortcutType()
        {
            ShortcutType type = new ShortcutType();

            if (IsValidUrl() || IsValidUrlFile())
            {
                type = ShortcutType.Url;
            }
            else
            {
                var attributes = File.GetAttributes(Destination);

                if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    type = ShortcutType.Folder;
                }
                else if (File.Exists(Destination))
                {
                    if (Destination.ToLower() == @"C:\Windows\System32\drivers\etc\hosts".ToLower() ||
                        Destination.ToLower() == @"%windir%\System32\drivers\etc\hosts".ToLower())
                    {
                        type = ShortcutType.HostsFile;
                    }
                    else
                    {
                        type = ShortcutType.File;
                    }
                }
                else
                {
                    type = ShortcutType.Unknown;
                }
            }

            return type;
        }

        private bool IsValidUrl()
        {
            bool result = Uri.TryCreate(Destination, UriKind.Absolute, out Uri uriResult)
                && !uriResult.IsFile;

            if (!result)
            {
                // Check for an incomplete Uri without the scheme
                //  Complete:   https://www.amazon.com
                //  Incomplete: amazon.com

                var newUri = new UriBuilder(Destination);
                if (uriResult != null && !uriResult.IsFile)
                {
                    // Fix the incomplete Uri
                    Destination = newUri.Uri.AbsoluteUri;
                    result = true;
                }
            }

            return result;
        }

        private bool IsValidUrlFile()
        {
            bool result = false;

            if (Path.GetExtension(Destination).ToLower() == ".url")
            {
                // Check for a line starting with "URL="
                var line = File.ReadAllLines(Destination)
                    .ToList()
                    .Where(a => a.Substring(0, 4).ToUpper() == "URL=")
                    .SingleOrDefault();

                // Update the destination to use the URL from the link
                if (line != null)
                {
                    var url = line.Substring(4);
                    if (url != null)
                    {
                        Destination = url;
                        result = true;
                    }
                }
            }

            return result;
        }

        public static string GetWindowsLinkTargetPath(string shortcutFilename)
        {
            // TODO Handle resolving a relative
            //result = Path.GetFullPath(file);

            string result = shortcutFilename;

            // Code found here: http://stackoverflow.com/questions/310595/how-can-i-test-programmatically-if-a-path-file-is-a-shortcut
            string path = Path.GetDirectoryName(shortcutFilename);
            string file = Path.GetFileName(shortcutFilename);

            Shell shell = new Shell();
            Folder folder = shell.NameSpace(path);
            FolderItem folderItem = folder.ParseName(file);

            if (folderItem != null && folderItem.IsLink)
            {
                ShellLinkObject link = (ShellLinkObject)folderItem.GetLink;
                result = link.Path;
            }

            return result;
        }
    }
}
