﻿﻿﻿/*  MapleLib - A general-purpose MapleStory library
 * Copyright (C) 2009, 2010, 2015 Snow and haha01haha01
   
 * This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

 * This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;
using System.IO;

namespace Character.MapleLib.Helpers
{
    public static class ErrorLogger
    {
        private static readonly List<Error> ErrorList = new List<Error>();

        public static void Log(ErrorLevel level, string message)
        {
            ErrorList.Add(new Error(level, message));
        }

        public static void SaveToFile(string filename)
        {
            using (var sw = new StreamWriter(File.Open(filename, FileMode.Append, FileAccess.Write, FileShare.Read)))
            {
                sw.WriteLine("Starting error log on " + DateTime.Today.ToShortDateString());
                foreach (var e in ErrorList)
                {
                    sw.WriteLine(e.Level + ":" + e.Message);
                }

                sw.WriteLine();
            }
        }
    }

    public class Error
    {
        internal readonly ErrorLevel Level;
        internal readonly string Message;

        internal Error(ErrorLevel level, string message)
        {
            this.Level = level;
            this.Message = message;
        }
    }

    public enum ErrorLevel
    {
        MissingFeature,
        IncorrectStructure,
        Critical,
        Crash
    }
}