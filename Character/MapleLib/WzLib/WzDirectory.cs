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

using System.Collections.Generic;
using System.Linq;
using Character.MapleLib.WzLib.Util;

namespace Character.MapleLib.WzLib
{
    /// <summary>
    /// A directory in the wz file, which may contain sub directories or wz images
    /// </summary>
    public class WzDirectory : WzObject
    {
        #region Fields

        private List<WzImage> _images = new List<WzImage>();
        private List<WzDirectory> _subDirs = new List<WzDirectory>();
        public WzBinaryReader Reader { get; private set; }
        private readonly uint _hash;
        internal byte[] WzIv;
        private readonly WzFile _wzFile;

        #endregion

        #region Inherited Members

        /// <summary>  
        /// The parent of the object
        /// </summary>
        public override WzObject Parent { get; internal set; }

        /// <summary>
        /// The WzObjectType of the directory
        /// </summary>
        public virtual WzObjectType ObjectType => WzObjectType.Directory;

        public override WzFile WzFileParent => _wzFile;

        /// <summary>
        /// Disposes the object
        /// </summary>
        public override void Dispose()
        {
            Name = null;
            Reader = null;
            foreach (var img in _images) img.Dispose();
            foreach (var dir in _subDirs) dir.Dispose();
            _images.Clear();
            _subDirs.Clear();
            _images = null;
            _subDirs = null;
        }

        public sealed override string Name { get; set; }

        #endregion

        #region Custom Members

        /// <summary>
        /// The wz images contained in the directory
        /// </summary>
        public IEnumerable<WzImage> WzImages => _images;

        /// <summary>
        /// The sub directories contained in the directory
        /// </summary>
        public IEnumerable<WzDirectory> WzDirectories => _subDirs;

        /// <summary>
        /// Offset of the folder
        /// </summary>
        private uint Offset { get; set; }

        /// <summary>
        /// Returns a WzImage or a WzDirectory with the given name
        /// </summary>
        /// <param name="name">The name of the img or dir to find</param>
        /// <returns>A WzImage or WzDirectory</returns>
        public new WzObject this[string name]
        {
            get
            {
                foreach (var i in _images.Where(i => i.Name.ToLower().Equals(name.ToLower()))) return i;
                return _subDirs.FirstOrDefault(d => d.Name.ToLower().Equals(name.ToLower()));
            }
        }


        /// <summary>
        /// Creates a blank WzDirectory
        /// </summary>
        public WzDirectory()
        {
        }

        /// <summary>
        /// Creates a WzDirectory
        /// </summary>
        /// <param name="reader">The BinaryReader that is currently reading the wz file</param>
        /// <param name="wzIv">wz Iv</param>
        /// <param name="wzFile">The parent Wz File</param>
        /// <param name="dirName">文件夹名称</param>
        /// <param name="verHash">版本hash</param>
        internal WzDirectory(WzBinaryReader reader, string dirName, uint verHash, byte[] wzIv, WzFile wzFile)
        {
            Reader = reader;
            Name = dirName;
            _hash = verHash;
            WzIv = wzIv;
            _wzFile = wzFile;
        }

        /// <summary>
        /// Parses the WzDirectory
        /// </summary>
        internal void ParseDirectory()
        {
            var entryCount = Reader.ReadCompressedInt();
            for (var i = 0; i < entryCount; i++)
            {
                var type = Reader.ReadByte();
                string fName = null;

                long rememberPos = 0;
                if (type == 1) //01 XX 00 00 00 00 00 OFFSET (4 bytes) 
                {
                    Reader.ReadInt32();
                    Reader.ReadInt16();
                    Reader.ReadOffset();
                    continue;
                }

                if (type == 2)
                {
                    var stringOffset = Reader.ReadInt32();
                    rememberPos = Reader.BaseStream.Position;
                    Reader.BaseStream.Position = Reader.FStart + stringOffset;
                    type = Reader.ReadByte();
                    fName = Reader.ReadString();
                }
                else if (type == 3 || type == 4)
                {
                    fName = Reader.ReadString();
                    rememberPos = Reader.BaseStream.Position;
                }

                Reader.BaseStream.Position = rememberPos;
                var fSize = Reader.ReadCompressedInt();
                var checksum = Reader.ReadCompressedInt();
                var offset = Reader.ReadOffset();
                if (type == 3)
                {
                    var subDir = new WzDirectory(Reader, fName, _hash, WzIv, _wzFile) {Offset = offset, Parent = this};
                    _subDirs.Add(subDir);
                }
                else
                {
                    var img = new WzImage(fName, Reader)
                    {
                        BlockSize = fSize, Offset = offset, Parent = this
                    };
                    _images.Add(img);
                }
            }

            foreach (var subDir in _subDirs)
            {
                Reader.BaseStream.Position = subDir.Offset;
                subDir.ParseDirectory();
            }
        }

        internal uint GetOffsets(uint curOffset)
        {
            Offset = curOffset;

            return _subDirs.Aggregate(curOffset, (current, dir) => dir.GetOffsets(current));
        }

        internal uint GetImgOffsets(uint curOffset)
        {
            foreach (var img in _images)
            {
                img.Offset = curOffset;
                curOffset += (uint) img.BlockSize;
            }

            return _subDirs.Aggregate(curOffset, (current, dir) => dir.GetImgOffsets(current));
        }

        /// <summary>
        /// Parses the wz images
        /// </summary>
        public void ParseImages()
        {
            foreach (var img in _images)
            {
                if (Reader.BaseStream.Position != img.Offset)
                {
                    Reader.BaseStream.Position = img.Offset;
                }

                img.ParseImage();
            }

            foreach (var subDir in _subDirs)
            {
                if (Reader.BaseStream.Position != subDir.Offset)
                {
                    Reader.BaseStream.Position = subDir.Offset;
                }

                subDir.ParseImages();
            }
        }

        /// <summary>
        /// Gets all child images of a WzDirectory
        /// </summary>
        /// <returns></returns>
        public List<WzImage> GetChildImages()
        {
            var imgFiles = new List<WzImage>();
            imgFiles.AddRange(_images);
            foreach (var subDir in _subDirs) imgFiles.AddRange(subDir.GetChildImages());
            return imgFiles;
        }

        #endregion
    }
}