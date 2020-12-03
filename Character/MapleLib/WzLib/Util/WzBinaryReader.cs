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

using System.IO;
using System.Text;
using Character.MapleLib.MapleCryptoLib;

   namespace Character.MapleLib.WzLib.Util
{
    public class WzBinaryReader : BinaryReader
    {
        #region Properties

        public WzMutableKey WzKey { get; }

        public uint Hash { get; set; }
        // public WzHeader Header { get; set; }


        public uint FStart { get; set; }

        #endregion

        #region Constructors

        public WzBinaryReader(Stream input, byte[] wzIv)
            : base(input)
        {
            WzKey = CryptoConstants.GenerateWzKey(wzIv);
        }

        #endregion

        #region Methods

        public string ReadStringAtOffset(long offset, bool readByte = false)
        {
            var currentOffset = BaseStream.Position;
            BaseStream.Position = offset;
            if (readByte)
            {
                ReadByte();
            }

            var returnString = ReadString();
            BaseStream.Position = currentOffset;
            return returnString;
        }

        public override string ReadString()
        {
            var smallLength = base.ReadSByte();

            if (smallLength == 0)
            {
                return string.Empty;
            }

            int length;
            var retString = new StringBuilder();
            if (smallLength > 0) // Unicode
            {
                ushort mask = 0xAAAA;
                length = smallLength == sbyte.MaxValue ? ReadInt32() : smallLength;
                if (length <= 0) return string.Empty;

                for (var i = 0; i < length; i++)
                {
                    var encryptedChar = ReadUInt16();
                    encryptedChar ^= mask;
                    encryptedChar ^= (ushort) ((WzKey[i * 2 + 1] << 8) + WzKey[i * 2]);
                    retString.Append((char) encryptedChar);
                    mask++;
                }
            }
            else
            {
                // ASCII
                byte mask = 0xAA;
                if (smallLength == sbyte.MinValue)
                {
                    length = ReadInt32();
                }
                else
                {
                    length = -smallLength;
                }

                if (length <= 0)
                {
                    return string.Empty;
                }

                for (var i = 0; i < length; i++)
                {
                    var encryptedChar = ReadByte();
                    encryptedChar ^= mask;
                    encryptedChar ^= WzKey[i];
                    retString.Append((char) encryptedChar);
                    mask++;
                }
            }

            return retString.ToString();
        }

        /// <summary>
        /// Reads an ASCII string, without decryption
        /// </summary>
        /// <param name="length">length</param>
        public string ReadString(int length)
        {
            return Encoding.ASCII.GetString(ReadBytes(length));
        }

        public string ReadNullTerminatedString()
        {
            var retString = new StringBuilder();
            var b = ReadByte();
            while (b != 0)
            {
                retString.Append((char) b);
                b = ReadByte();
            }

            return retString.ToString();
        }

        public int ReadCompressedInt()
        {
            var sb = base.ReadSByte();
            return sb == sbyte.MinValue ? ReadInt32() : sb;
        }

        public long ReadLong()
        {
            var sb = base.ReadSByte();
            return sb == sbyte.MinValue ? ReadInt64() : sb;
        }

        public uint ReadOffset()
        {
            var offset = (uint) BaseStream.Position;
            offset = (offset - FStart) ^ uint.MaxValue;
            offset *= Hash;
            offset -= CryptoConstants.WzOffsetConstant;
            offset = CryptoConstants.RotateLeft(offset, (byte) (offset & 0x1F));
            var encryptedOffset = ReadUInt32();
            offset ^= encryptedOffset;
            offset += FStart * 2;
            return offset;
        }

        public string DecryptString(char[] stringToDecrypt)
        {
            var outputString = "";
            for (var i = 0; i < stringToDecrypt.Length; i++)
                outputString += (char) (stringToDecrypt[i] ^ ((char) ((WzKey[i * 2 + 1] << 8) + WzKey[i * 2])));
            return outputString;
        }

        public string DecryptNonUnicodeString(char[] stringToDecrypt)
        {
            var outputString = "";
            for (var i = 0; i < stringToDecrypt.Length; i++)
                outputString += (char) (stringToDecrypt[i] ^ WzKey[i]);
            return outputString;
        }

        public string ReadStringBlock(uint offset)
        {
            switch (ReadByte())
            {
                case 0:
                case 0x73:
                    return ReadString();
                case 1:
                case 0x1B:
                    return ReadStringAtOffset(offset + ReadInt32());
                default:
                    return "";
            }
        }

        #endregion
    }
}