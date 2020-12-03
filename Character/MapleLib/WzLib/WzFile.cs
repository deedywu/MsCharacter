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
using System.IO;
using Character.MapleLib.Helpers;
using Character.MapleLib.MapleCryptoLib;
using Character.MapleLib.WzLib.Util;

namespace Character.MapleLib.WzLib
{
    /// <summary>
    /// A class that contains all the information of a wz file
    /// </summary>
    public sealed class WzFile : WzObject
    {
        #region Fields

        private short _version;
        private uint _versionHash;
        private readonly byte[] _wzIv;

        #endregion

        /// <summary>
        /// The parsed IWzDir after having called ParseWzDirectory(), this can either be a WzDirectory or a WzListDirectory
        /// </summary>
        public WzDirectory WzDirectory { get; private set; }

        /// <summary>
        /// Name of the WzFile
        /// </summary>
        public override string Name { get; set; }

        /// <summary>
        /// Returns WzDirectory[name]
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>WzDirectory[name]</returns>
        public new WzObject this[string name] => WzDirectory[name];

        private short Version { get; set; }

        private string FilePath { get; set; }

        private WzMapleVersion MapleVersion { get; }

        public override WzObject Parent
        {
            get => null;
            internal set { }
        }

        public override WzFile WzFileParent => this;

        public override void Dispose()
        {
            if (WzDirectory.Reader == null) return;
            WzDirectory.Reader.Close();
            FilePath = null;
            Name = null;
            WzDirectory.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Open a wz file from a file on the disk
        /// </summary>
        /// <param name="filePath">Path to the wz file</param>
        /// <param name="gameVersion">游戏版本</param>
        /// <param name="version">版本</param>
        public WzFile(string filePath, short gameVersion, WzMapleVersion version)
        {
            Name = Path.GetFileName(filePath);
            FilePath = filePath;
            Version = gameVersion;
            MapleVersion = version;
            _wzIv = CryptoConstants.GetIvByMapleVersion(version);
        }

        /// <summary>
        /// Parses the wz file, if the wz file is a list.wz file, WzDirectory will be a WzListDirectory, if not, it'll simply be a WzDirectory
        /// </summary>
        public void ParseWzFile()
        {
            if (MapleVersion == WzMapleVersion.Generate)
                throw new InvalidOperationException("Cannot call ParseWzFile() if WZ file type is GENERATE");
            ParseMainWzDirectory();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void ParseMainWzDirectory()
        {
            if (FilePath == null)
            {
                ErrorLogger.Log(ErrorLevel.Critical, "[Error] Path is null");
                return;
            }

            var reader = new WzBinaryReader(File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read), _wzIv);

            reader.ReadString(4);
            reader.ReadUInt64();
            reader.FStart = reader.ReadUInt32();
            reader.ReadNullTerminatedString();
            reader.ReadBytes((int) (reader.FStart - reader.BaseStream.Position));
            _version = reader.ReadInt16();
            if (Version == -1)
            {
                for (var j = 0; j < short.MaxValue; j++)
                {
                    Version = (short) j;
                    _versionHash = GetVersionHash(_version, Version);
                    if (_versionHash == 0) continue;
                    reader.Hash = _versionHash;
                    var position = reader.BaseStream.Position;
                    WzDirectory testDirectory;
                    try
                    {
                        testDirectory = new WzDirectory(reader, Name, _versionHash, _wzIv, this);
                        testDirectory.ParseDirectory();
                    }
                    catch
                    {
                        reader.BaseStream.Position = position;
                        continue;
                    }

                    var testImage = testDirectory.GetChildImages()[0];

                    try
                    {
                        reader.BaseStream.Position = testImage.Offset;
                        var checkByte = reader.ReadByte();
                        reader.BaseStream.Position = position;
                        testDirectory.Dispose();
                        switch (checkByte)
                        {
                            case 0x73:
                            case 0x1b:
                            {
                                var directory = new WzDirectory(reader, Name, _versionHash, _wzIv, this);
                                directory.ParseDirectory();
                                WzDirectory = directory;
                                return;
                            }
                        }

                        reader.BaseStream.Position = position;
                    }
                    catch
                    {
                        reader.BaseStream.Position = position;
                    }
                }

                throw new Exception(
                    "Error with game version hash : The specified game version is incorrect and WzLib was unable to determine the version itself");
            }

            _versionHash = GetVersionHash(_version, Version);
            reader.Hash = _versionHash;
            var dir = new WzDirectory(reader, Name, _versionHash, _wzIv, this);
            dir.ParseDirectory();
            WzDirectory = dir;
        }

        private static uint GetVersionHash(int encVer, int realVer)
        {
            var encryptedVersionNumber = encVer;
            var versionNumber = realVer;
            var versionHash = 0;

            var versionNumberStr = versionNumber.ToString();

            var l = versionNumberStr.Length;
            for (var i = 0; i < l; i++) versionHash = (32 * versionHash) + versionNumberStr[i] + 1;

            var a = (versionHash >> 24) & 0xFF;
            var b = (versionHash >> 16) & 0xFF;
            var c = (versionHash >> 8) & 0xFF;
            var d = versionHash & 0xFF;
            var decryptedVersionNumber = (0xff ^ a ^ b ^ c ^ d);

            return encryptedVersionNumber == decryptedVersionNumber ? Convert.ToUInt32(versionHash) : 0;
        }
    }
}