/*  MapleLib - A general-purpose MapleStory library
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

using Character.MapleLib.WzLib;
using Character.MapleLib.WzLib.Util;

namespace Character.MapleLib.MapleCryptoLib
{
    /// <summary>
    /// Contains all the constant values used for various functions
    /// </summary>
    public static class CryptoConstants
    {
        /// <summary>
        /// AES UserKey used by MapleStory
        /// </summary>
        private static readonly byte[] UserKey =
        {
            //16 * 8
            0x13, 0x00, 0x00, 0x00, 0x52, 0x00, 0x00, 0x00, 0x2A, 0x00, 0x00, 0x00, 0x5B, 0x00, 0x00, 0x00,
            0x08, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x60, 0x00, 0x00, 0x00,
            0x06, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x43, 0x00, 0x00, 0x00, 0x0F, 0x00, 0x00, 0x00,
            0xB4, 0x00, 0x00, 0x00, 0x4B, 0x00, 0x00, 0x00, 0x35, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00,
            0x1B, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00, 0x00, 0x5F, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00,
            0x0F, 0x00, 0x00, 0x00, 0x50, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x1B, 0x00, 0x00, 0x00,
            0x33, 0x00, 0x00, 0x00, 0x55, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00,
            0x52, 0x00, 0x00, 0x00, 0xDE, 0x00, 0x00, 0x00, 0xC7, 0x00, 0x00, 0x00, 0x1E, 0x00, 0x00, 0x00
        };

        /// <summary>
        /// IV used to create the WzKey for GMS
        /// </summary>
        private static readonly byte[] WzGmsIv = {0x4D, 0x23, 0xC7, 0x2B};

        /// <summary>
        /// IV used to create the WzKey for EMS
        /// </summary>
        private static readonly byte[] WzEmsIv = {0xB9, 0x7D, 0x63, 0xE9};

        /// <summary>
        /// Constant used in WZ offset encryption
        /// </summary>
        public const uint WzOffsetConstant = 0x581C3F6D;

        /// <summary>
        /// Trims the AES UserKey for use an AES cryptor
        /// </summary>
        private static byte[] GetTrimmedUserKey()
        {
            var key = new byte[32];
            for (var i = 0; i < 128; i += 16) key[i / 4] = UserKey[i];
            return key;
        }

        public static byte[] GetIvByMapleVersion(WzMapleVersion ver)
        {
            switch (ver)
            {
                case WzMapleVersion.Ems:
                    return WzEmsIv;
                case WzMapleVersion.Gms:
                    return WzGmsIv;
                default:
                    return new byte[4];
            }
        }

        public static uint RotateLeft(uint x, byte n) => (x << n) | (x >> (32 - n));

        public static WzMutableKey GenerateWzKey(byte[] wzIv)
        {
            return new WzMutableKey(wzIv, GetTrimmedUserKey());
        }
    }
}