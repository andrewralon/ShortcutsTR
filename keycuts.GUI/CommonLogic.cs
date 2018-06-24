﻿using keycuts.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace keycuts.GUI
{
    public class CommonLogic
    {
        public static int CreateShortcut(string destination, string shortcutName, bool force = false)
        {
            var result = 0;

            if (!string.IsNullOrEmpty(destination) && 
                !string.IsNullOrEmpty(shortcutName))
            {
                var args = new KeycutArgs(
                    destination,
                    shortcutName,
                    force: force);

                result = new Runner().Run(args);
            }
            else
            {
                result = (int)ExitCode.BadArguments;
            }

            return result;
        }

        public static void SetOutputFolder(string outputFolder)
        {
            var runner = new Runner();
            runner.SetOutputFolder(outputFolder);
        }
    }
}
