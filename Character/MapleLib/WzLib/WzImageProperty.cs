﻿﻿﻿﻿﻿/*  MapleLib - A general-purpose MapleStory library
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
using Character.MapleLib.WzLib.Util;
using Character.MapleLib.WzLib.WzProperties;

namespace Character.MapleLib.WzLib
{
    /// <summary>
    /// An interface for wz img properties
    /// </summary>
    public abstract class WzImageProperty : WzObject
    {
        #region Virtual\Abstrcat Members

        public virtual List<WzImageProperty> WzProperties => null;

        public new virtual WzImageProperty this[string name]
        {
            get => null;
            set => throw new NotImplementedException();
        }

        public virtual WzImageProperty GetFromPath(string path)
        {
            return null;
        }

        public abstract WzPropertyType PropertyType { get; }

        /// <summary>
        /// The image that this property is contained in
        /// </summary>
        protected WzImage ParentImage
        {
            get
            {
                var parent = Parent;
                while (parent != null)
                {
                    if (parent is WzImage image) return image;
                    parent = parent.Parent;
                }

                return null;
            }
        }

        public virtual WzObjectType ObjectType => WzObjectType.Property;

        public override WzFile WzFileParent => ParentImage.WzFileParent;

        #endregion

        #region Extended Properties Parsing

        internal static IEnumerable<WzImageProperty> ParsePropertyList(uint offset, WzBinaryReader reader,
            WzObject parent,
            WzImage parentImg)
        {
            var entryCount = reader.ReadCompressedInt();
            var properties = new List<WzImageProperty>(entryCount);
            for (var i = 0; i < entryCount; i++)
            {
                var name = reader.ReadStringBlock(offset);
                var pType = reader.ReadByte();
                switch (pType)
                {
                    case 0:
                        properties.Add(new WzNullProperty(name) {Parent = parent});
                        break;
                    case 11:
                    case 2:
                        properties.Add(new WzShortProperty(name, reader.ReadInt16()) {Parent = parent});
                        break;
                    case 3:
                    case 19:
                        properties.Add(new WzIntProperty(name, reader.ReadCompressedInt()) {Parent = parent});
                        break;
                    case 20:
                        properties.Add(new WzLongProperty(name, reader.ReadLong()) {Parent = parent});
                        break;
                    case 4:
                        var type = reader.ReadByte();
                        if (type == 0x80)
                            properties.Add(new WzFloatProperty(name, reader.ReadSingle()) {Parent = parent});
                        else if (type == 0)
                            properties.Add(new WzFloatProperty(name, 0f) {Parent = parent});
                        break;
                    case 5:
                        properties.Add(new WzDoubleProperty(name, reader.ReadDouble()) {Parent = parent});
                        break;
                    case 8:
                        properties.Add(new WzStringProperty(name, reader.ReadStringBlock(offset)) {Parent = parent});
                        break;
                    case 9:
                        var eob = (int) (reader.ReadUInt32() + reader.BaseStream.Position);
                        WzImageProperty exProp = ParseExtendedProp(reader, offset, name, parent, parentImg);
                        properties.Add(exProp);
                        if (reader.BaseStream.Position != eob) reader.BaseStream.Position = eob;
                        break;
                    default:
                        throw new Exception("Unknown property type at ParsePropertyList");
                }
            }

            return properties;
        }

        private static WzExtended ParseExtendedProp(WzBinaryReader reader, uint offset, string name,
            WzObject parent, WzImage imgParent)
        {
            switch (reader.ReadByte())
            {
                case 0x01:
                case 0x1B:
                    return ExtractMore(reader, offset, name,
                        reader.ReadStringAtOffset(offset + reader.ReadInt32()), parent, imgParent);
                case 0x00:
                case 0x73:
                    return ExtractMore(reader, offset, name, "", parent, imgParent);
                default:
                    throw new Exception("Invalid byte read at ParseExtendedProp");
            }
        }

        private static WzExtended ExtractMore(WzBinaryReader reader, uint offset, string name, string iName,
            WzObject parent, WzImage imgParent)
        {
            if (iName == "")
                iName = reader.ReadString();
            switch (iName)
            {
                case "Property":
                    var subProp = new WzSubProperty(name) {Parent = parent};
                    reader.BaseStream.Position += 2; // Reserved?
                    subProp.AddProperties(ParsePropertyList(offset, reader, subProp, imgParent));
                    return subProp;
                case "Canvas":
                    var canvasProp = new WzCanvasProperty(name) {Parent = parent};
                    reader.BaseStream.Position++;
                    if (reader.ReadByte() == 1)
                    {
                        reader.BaseStream.Position += 2;
                        canvasProp.AddProperties(
                            ParsePropertyList(offset, reader, canvasProp, imgParent));
                    }

                    canvasProp.PngProperty = new WzPngProperty(reader, imgParent.ParseEverything) {Parent = canvasProp};
                    return canvasProp;
                case "Shape2D#Vector2D":
                    var vecProp = new WzVectorProperty(name) {Parent = parent};
                    vecProp.X = new WzIntProperty("X", reader.ReadCompressedInt()) {Parent = vecProp};
                    vecProp.Y = new WzIntProperty("Y", reader.ReadCompressedInt()) {Parent = vecProp};
                    return vecProp;
                case "Shape2D#Convex2D":
                    var convexProp = new WzConvexProperty(name) {Parent = parent};
                    var convexEntryCount = reader.ReadCompressedInt();
                    convexProp.WzProperties.Capacity = convexEntryCount;
                    for (var i = 0; i < convexEntryCount; i++)
                        convexProp.AddProperty(ParseExtendedProp(reader, offset, name, convexProp, imgParent));
                    return convexProp;
                case "Sound_DX8":
                    var soundProp = new WzSoundProperty(name, reader, imgParent.ParseEverything)
                        {Parent = parent};
                    return soundProp;
                case "UOL":
                    reader.BaseStream.Position++;
                    switch (reader.ReadByte())
                    {
                        case 0:
                            return new WzUolProperty(name, reader.ReadString()) {Parent = parent};
                        case 1:
                            return new WzUolProperty(name, reader.ReadStringAtOffset(offset + reader.ReadInt32()))
                                {Parent = parent};
                    }

                    throw new Exception("Unsupported UOL type");
                default:
                    throw new Exception("Unknown iName: " + iName);
            }
        }

        #endregion
    }
}