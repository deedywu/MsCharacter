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

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression; 
using Character.MapleLib.Helpers;
using Character.MapleLib.WzLib.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Character.MapleLib.WzLib.WzProperties
{
    /// <summary>
    /// A property that contains the information for a byte array
    /// </summary>
    public class WzPngProperty : WzImageProperty
    {
        #region Fields

        private readonly int _format;
        private readonly int _format2;

        private byte[] _compressedBytes;

        private byte[] _data;

        // true: argb32 false:rgb565
        private bool _formatType = true;

        private bool _listWzUsed;

        private readonly WzBinaryReader _wzReader;
        private readonly long _offs;

        #endregion

        #region Inherited Members

        public Texture2D ToTexture2D(GraphicsDevice device)
        {
            var data = GetPng(false);
            if (data == null) return null;
            var t2 = new Texture2D(device, Width, Height);
            var pixels = data;
            for (var i = 0; i < pixels.Length; i += 4)
            {
                // bgra => rgba
                var ob = pixels[i];
                var og = pixels[i + 1];
                var or = pixels[i + 2];
                var oa = pixels[i + 3];
                pixels[i] = or;
                pixels[i + 1] = og;
                pixels[i + 2] = ob;
                pixels[i + 3] = oa;
                if (pixels[i + 3] == 255) continue;
                var alpha = pixels[i + 3] / 255f;
                pixels[i] = (byte) (pixels[i] * alpha);
                pixels[i + 1] = (byte) (pixels[i + 1] * alpha);
                pixels[i + 2] = (byte) (pixels[i + 2] * alpha);
            }

            t2.SetData(pixels);
            return t2;
        }

        public override object WzValue => GetPng(false);

        /// <summary>
        /// The parent of the object
        /// </summary>
        public override WzObject Parent { get; internal set; }

        /// <summary>
        /// The name of the property
        /// </summary>
        public override string Name
        {
            get => "PNG";
            set { }
        }

        /// <summary>
        /// The WzPropertyType of the property
        /// </summary>
        public override WzPropertyType PropertyType => WzPropertyType.Png;

        /// <summary>
        /// Disposes the object
        /// </summary>
        public override void Dispose()
        {
            _compressedBytes = null;
            if (_data == null || _data.Length == 0) return;
            _data = null;
        }

        #endregion

        #region Custom Members

        /// <summary>
        /// The width of the png
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// The height of the png
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Creates a blank WzPngProperty
        /// </summary> 
        internal WzPngProperty(WzBinaryReader reader, bool parseNow)
        {
            // Read compressed bytes
            Width = reader.ReadCompressedInt();
            Height = reader.ReadCompressedInt();
            _format = reader.ReadCompressedInt();
            _format2 = reader.ReadByte();
            reader.BaseStream.Position += 4;
            _offs = reader.BaseStream.Position;
            var len = reader.ReadInt32() - 1;
            reader.BaseStream.Position += 1;

            if (len > 0)
            {
                if (parseNow)
                {
                    _compressedBytes = _wzReader.ReadBytes(len);
                    ParsePng();
                }
                else
                    reader.BaseStream.Position += len;
            }

            _wzReader = reader;
        }

        #endregion

        #region Parsing Methods

        private byte[] GetPng(bool saveInMemory)
        {
            if (_data != null) return _data;
            var pos = _wzReader.BaseStream.Position;
            _wzReader.BaseStream.Position = _offs;
            var len = _wzReader.ReadInt32() - 1;
            _wzReader.BaseStream.Position += 1;
            if (len > 0)
                _compressedBytes = _wzReader.ReadBytes(len);
            ParsePng();
            _wzReader.BaseStream.Position = pos;
            if (saveInMemory) return _data;
            var pngImage = _data;
            _data = null;
            _compressedBytes = null;
            return pngImage;
        }

        private void ParsePng()
        {
            DeflateStream zlib;
            int uncompressedSize;
            int x = 0, y = 0;
            byte[] data = null;
            var imgParent = ParentImage;
            byte[] decBuf;

            var reader = new BinaryReader(new MemoryStream(_compressedBytes));
            var header = reader.ReadUInt16();
            _listWzUsed = header != 0x9C78 && header != 0xDA78 && header != 0x0178 && header != 0x5E78;
            if (!_listWzUsed)
            {
                zlib = new DeflateStream(reader.BaseStream, CompressionMode.Decompress);
            }
            else
            {
                reader.BaseStream.Position -= 2;
                var dataStream = new MemoryStream();
                var endOfPng = _compressedBytes.Length;

                while (reader.BaseStream.Position < endOfPng)
                {
                    var blockSize = reader.ReadInt32();
                    for (var i = 0; i < blockSize; i++)
                    {
                        dataStream.WriteByte((byte) (reader.ReadByte() ^ imgParent.Reader.WzKey[i]));
                    }
                }

                dataStream.Position = 2;
                zlib = new DeflateStream(dataStream, CompressionMode.Decompress);
            }

            switch (_format + _format2)
            {
                case 1:
                    uncompressedSize = Width * Height * 2;
                    decBuf = new byte[uncompressedSize];
                    zlib.Read(decBuf, 0, uncompressedSize);
                    var argb = new byte[uncompressedSize * 2];
                    for (var i = 0; i < uncompressedSize; i++)
                    {
                        var b = decBuf[i] & 0x0F;
                        b |= (b << 4);
                        argb[i * 2] = (byte) b;
                        var g = decBuf[i] & 0xF0;
                        g |= (g >> 4);
                        argb[i * 2 + 1] = (byte) g;
                    }

                    data = argb;
                    break;
                case 2:
                    uncompressedSize = Width * Height * 4;
                    decBuf = new byte[uncompressedSize];
                    zlib.Read(decBuf, 0, uncompressedSize);
                    data = decBuf;
                    break;
                case 3: // thanks to Elem8100 
                    uncompressedSize = ((int) Math.Ceiling(Width / 4.0)) * 4 * ((int) Math.Ceiling(Height / 4.0)) * 4 /
                                       8;
                    decBuf = new byte[uncompressedSize];
                    zlib.Read(decBuf, 0, uncompressedSize);
                    var argb2 = new byte[Width * Height];
                {
                    var w = ((int) Math.Ceiling(Width / 4.0));
                    var h = ((int) Math.Ceiling(Height / 4.0));
                    for (var i = 0; i < h; i++)
                    {
                        int index2;
                        for (var j = 0; j < w; j++)
                        {
                            var index = (j + i * w) * 2;
                            index2 = j * 4 + i * Width * 4;
                            var p = (decBuf[index] & 0x0F) | ((decBuf[index] & 0x0F) << 4);
                            p |= ((decBuf[index] & 0xF0) | ((decBuf[index] & 0xF0) >> 4)) << 8;
                            p |= ((decBuf[index + 1] & 0x0F) | ((decBuf[index + 1] & 0x0F) << 4)) << 16;
                            p |= ((decBuf[index + 1] & 0xF0) | ((decBuf[index] & 0xF0) >> 4)) << 24;

                            for (var k = 0; k < 4; k++)
                            {
                                if (x * 4 + k < Width)
                                {
                                    argb2[index2 + k] = (byte) p;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }

                        index2 = y * Width * 4;
                        for (var m = 1; m < 4; m++)
                        {
                            if (y * 4 + m < Height)
                            {
                                Array.Copy(argb2, index2, argb2, index2 + m * Width, Width);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                    data = argb2;
                    break;

                case 513:
                    uncompressedSize = Width * Height * 2;
                    decBuf = new byte[uncompressedSize];
                    zlib.Read(decBuf, 0, uncompressedSize);
                    _formatType = true;
                    data = decBuf;
                    break;

                case 517:
                    uncompressedSize = Width * Height / 128;
                    decBuf = new byte[uncompressedSize];
                    zlib.Read(decBuf, 0, uncompressedSize);
                    data = new byte[Width * Height * 4];
                    var id = 0;
                    for (var i = 0; i < uncompressedSize; i++)
                    {
                        for (byte j = 0; j < 8; j++)
                        {
                            var iB = Convert.ToByte(((decBuf[i] & (0x01 << (7 - j))) >> (7 - j)) * 0xFF);
                            for (var k = 0; k < 16; k++)
                            {
                                data[id * 4] = 0Xff;
                                data[id * 4 + 1] = iB;
                                data[id * 4 + 2] = iB;
                                data[id * 4 + 3] = iB;
                                id++;
                            }
                        }
                    }

                    break;

                case 1026:
                    uncompressedSize = Width * Height;
                    decBuf = new byte[uncompressedSize];
                    zlib.Read(decBuf, 0, uncompressedSize);
                    decBuf = GetPixelDataDxt3(decBuf, Width, Height);
                    data = decBuf;
                    break;

                case 2050: // thanks to Elem8100
                    uncompressedSize = Width * Height;
                    decBuf = new byte[uncompressedSize];
                    zlib.Read(decBuf, 0, uncompressedSize);
                    decBuf = GetPixelDataDxt5(decBuf, Width, Height);
                    data = decBuf;
                    break;

                default:
                    ErrorLogger.Log(ErrorLevel.MissingFeature,
                        $"Unknown PNG format {_format} {_format2}");
                    break;
            }

            _data = data;
        }

        #endregion

        #region DXT Format Parser

        private static byte[] GetPixelDataDxt3(byte[] rawData, int width, int height)
        {
            var pixel = new byte[width * height * 4];

            var colorTable = new Color[4];
            var colorIdxTable = new int[16];
            var alphaTable = new byte[16];
            for (var y = 0; y < height; y += 4)
            {
                for (var x = 0; x < width; x += 4)
                {
                    var off = x * 4 + y * width;
                    ExpandAlphaTable(alphaTable, rawData, off);
                    var u0 = BitConverter.ToUInt16(rawData, off + 8);
                    var u1 = BitConverter.ToUInt16(rawData, off + 10);
                    ExpandColorTable(colorTable, u0, u1);
                    ExpandColorIndexTable(colorIdxTable, rawData, off + 12);

                    for (var j = 0; j < 4; j++)
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            SetPixel(pixel,
                                x + i,
                                y + j,
                                width,
                                colorTable[colorIdxTable[j * 4 + i]],
                                alphaTable[j * 4 + i]);
                        }
                    }
                }
            }

            return pixel;
        }

        private static byte[] GetPixelDataDxt5(byte[] rawData, int width, int height)
        {
            var pixel = new byte[width * height * 4];

            var colorTable = new Color[4];
            var colorIdxTable = new int[16];
            var alphaTable = new byte[8];
            var alphaIdxTable = new int[16];
            for (var y = 0; y < height; y += 4)
            {
                for (var x = 0; x < width; x += 4)
                {
                    var off = x * 4 + y * width;
                    ExpandAlphaTableDxt5(alphaTable, rawData[off + 0], rawData[off + 1]);
                    ExpandAlphaIndexTableDxt5(alphaIdxTable, rawData, off + 2);
                    var u0 = BitConverter.ToUInt16(rawData, off + 8);
                    var u1 = BitConverter.ToUInt16(rawData, off + 10);
                    ExpandColorTable(colorTable, u0, u1);
                    ExpandColorIndexTable(colorIdxTable, rawData, off + 12);

                    for (var j = 0; j < 4; j++)
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            SetPixel(pixel,
                                x + i,
                                y + j,
                                width,
                                colorTable[colorIdxTable[j * 4 + i]],
                                alphaTable[alphaIdxTable[j * 4 + i]]);
                        }
                    }
                }
            }

            return pixel;
        }

        private static void ExpandAlphaTableDxt5(IList<byte> alpha, byte a0, byte a1)
        {
            alpha[0] = a0;
            alpha[1] = a1;
            if (a0 > a1)
            {
                for (var i = 2; i < 8; i++)
                {
                    alpha[i] = (byte) (((8 - i) * a0 + (i - 1) * a1 + 3) / 7);
                }
            }
            else
            {
                for (var i = 2; i < 6; i++)
                {
                    alpha[i] = (byte) (((6 - i) * a0 + (i - 1) * a1 + 2) / 5);
                }

                alpha[6] = 0;
                alpha[7] = 255;
            }
        }

        private static void ExpandAlphaIndexTableDxt5(IList<int> alphaIndex, IReadOnlyList<byte> rawData, int offset)
        {
            for (var i = 0; i < 16; i += 8, offset += 3)
            {
                var flags = rawData[offset]
                            | (rawData[offset + 1] << 8)
                            | (rawData[offset + 2] << 16);
                for (var j = 0; j < 8; j++)
                {
                    var mask = 0x07 << (3 * j);
                    alphaIndex[i + j] = (flags & mask) >> (3 * j);
                }
            }
        }

        private static void SetPixel(IList<byte> pixelData, int x, int y, int width, Color color, byte alpha)
        {
            var offset = (y * width + x) * 4;
            pixelData[offset + 0] = color.B;
            pixelData[offset + 1] = color.G;
            pixelData[offset + 2] = color.R;
            pixelData[offset + 3] = alpha;
        }

        private static void ExpandColorTable(IList<Color> color, ushort u0, ushort u1)
        {
            color[0] = Rgb565ToColor(u0);
            color[1] = Rgb565ToColor(u1);
            color[2] = new Color
            {
                A = 0xff,
                R = (byte) ((color[0].R * 2 + color[1].R + 1) / 3),
                G = (byte) ((color[0].G * 2 + color[1].G + 1) / 3),
                B = (byte) ((color[0].B * 2 + color[1].B + 1) / 3)
            };
            color[3] = new Color
            {
                A = 0xff,
                R = (byte) ((color[0].R + color[1].R * 2 + 1) / 3),
                G = (byte) ((color[0].G + color[1].G * 2 + 1) / 3),
                B = (byte) ((color[0].B + color[1].B * 2 + 1) / 3)
            };
        }

        private static void ExpandColorIndexTable(IList<int> colorIndex, IReadOnlyList<byte> rawData, int offset)
        {
            for (var i = 0; i < 16; i += 4, offset++)
            {
                colorIndex[i + 0] = (rawData[offset] & 0x03);
                colorIndex[i + 1] = (rawData[offset] & 0x0c) >> 2;
                colorIndex[i + 2] = (rawData[offset] & 0x30) >> 4;
                colorIndex[i + 3] = (rawData[offset] & 0xc0) >> 6;
            }
        }

        private static void ExpandAlphaTable(IList<byte> alpha, IReadOnlyList<byte> rawData, int offset)
        {
            for (var i = 0; i < 16; i += 2, offset++)
            {
                alpha[i + 0] = (byte) (rawData[offset] & 0x0f);
                alpha[i + 1] = (byte) ((rawData[offset] & 0xf0) >> 4);
            }

            for (var i = 0; i < 16; i++)
            {
                alpha[i] = (byte) (alpha[i] | (alpha[i] << 4));
            }
        }

        private static Color Rgb565ToColor(ushort val)
        {
            const int rgb565MaskR = 0xf800;
            const int rgb565MaskG = 0x07e0;
            const int rgb565MaskB = 0x001f;
            var r = (val & rgb565MaskR) >> 11;
            var g = (val & rgb565MaskG) >> 5;
            var b = (val & rgb565MaskB);
            var c = new Color
            {
                A = 0xff,
                R = (byte) ((r << 3) | (r >> 2)),
                G = (byte) ((g << 2) | (g >> 4)),
                B = (byte) ((b << 3) | (b >> 2))
            };
            return c;
        }

        #endregion
    }
}