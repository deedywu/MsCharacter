﻿﻿﻿/*  MapleLib - A general-purpose MapleStory library
 * Copyright (C) 2015 haha01haha01 and contributors
   
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
using System.Security.Cryptography;

namespace Character.MapleLib.WzLib.Util
{
    public class WzMutableKey
    {
        private const int BatchSize = 4096;
        private readonly byte[] _iv;
        private readonly byte[] _aesKey;
        private byte[] _keys;

        public WzMutableKey(byte[] wzIv, byte[] aesKey)
        {
            _iv = wzIv;
            _aesKey = aesKey;
        }

        public byte this[int index]
        {
            get
            {
                if (_keys == null || _keys.Length <= index)
                {
                    EnsureKeySize(index + 1);
                }

                return _keys[index];
            }
        }

        private void EnsureKeySize(int size)
        {
            if (_keys != null && _keys.Length >= size)
            {
                return;
            }

            size = (int) Math.Ceiling(1.0 * size / BatchSize) * BatchSize;
            var newKeys = new byte[size];

            if (BitConverter.ToInt32(_iv, 0) == 0)
            {
                _keys = newKeys;
                return;
            }

            var startIndex = 0;

            if (_keys != null)
            {
                Buffer.BlockCopy(_keys, 0, newKeys, 0, _keys.Length);
                startIndex = _keys.Length;
            }

            var aes = Rijndael.Create();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Key = _aesKey;
            aes.Mode = CipherMode.ECB;
            var ms = new MemoryStream(newKeys, startIndex, newKeys.Length - startIndex, true);
            var s = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);

            for (var i = startIndex; i < size; i += 16)
            {
                if (i == 0)
                {
                    var block = new byte[16];
                    for (var j = 0; j < block.Length; j++)
                    {
                        block[j] = _iv[j % 4];
                    }

                    s.Write(block, 0, block.Length);
                }
                else
                {
                    s.Write(newKeys, i - 16, 16);
                }
            }

            s.Flush();
            ms.Close();
            _keys = newKeys;
        }
    }
}