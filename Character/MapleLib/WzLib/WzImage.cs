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
using System.Linq;
using Character.MapleLib.WzLib.Util;

namespace Character.MapleLib.WzLib
{
    /// <summary>
    /// A .img contained in a wz directory
    /// </summary>
    public class WzImage : WzObject, IPropertyContainer
    {
        #region Fields

        private List<WzImageProperty> _properties = new List<WzImageProperty>();

        public bool ParseEverything { get; private set; }

        public WzBinaryReader Reader { get; private set; }

        #endregion

        #region Constructors\Destructors

        internal WzImage(string name, WzBinaryReader reader)
        {
            Name = name;
            Reader = reader;
        }

        public override void Dispose()
        {
            Name = null;
            Reader = null;
            if (_properties == null) return;
            foreach (var prop in _properties) prop.Dispose();
            _properties.Clear();
            _properties = null;
            Parsed = false;
        }

        #endregion

        #region Inherited Members

        /// <summary>
        /// The parent of the object
        /// </summary>
        public override WzObject Parent { get; internal set; }

        /// <summary>
        /// The name of the image
        /// </summary>
        public sealed override string Name { get; set; }

        public override WzFile WzFileParent => Parent?.WzFileParent;

        /// <summary>
        /// Is the object parsed
        /// </summary>
        public bool Parsed { get; private set; }

        /// <summary>
        /// Was the image changed
        /// </summary>
        private bool Changed { get; } = false;

        /// <summary>
        /// The size in the wz file of the image
        /// </summary>
        public int BlockSize { get; set; }

        /// <summary>
        /// The offset of the image
        /// </summary>
        public uint Offset { get; set; }

        /// <summary>
        /// The WzObjectType of the image
        /// </summary>
        public virtual WzObjectType ObjectType
        {
            get
            {
                if (Reader == null) return WzObjectType.Image;
                if (!Parsed) ParseImage();
                return WzObjectType.Image;
            }
        }

        /// <summary>
        /// The properties contained in the image
        /// </summary>
        public List<WzImageProperty> WzProperties
        {
            get
            {
                if (Reader != null && !Parsed)
                {
                    ParseImage();
                }

                return _properties;
            }
        }

        /// <summary>
        /// Gets a wz property by it's name
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <returns>The wz property with the specified name</returns>
        public new WzImageProperty this[string name]
        {
            get
            {
                if (Reader == null) return _properties.FirstOrDefault(iwp => iwp.Name.ToLower().Equals(name.ToLower()));
                if (!Parsed) ParseImage();
                return _properties.FirstOrDefault(iwp => iwp.Name.ToLower().Equals(name.ToLower()));
            }
            set { }
        }

        #endregion

        #region Custom Members

        /// <summary>
        /// Gets a WzImageProperty from a path
        /// </summary>
        /// <param name="path">path to object</param>
        /// <returns>the selected WzImageProperty</returns>
        public WzImageProperty GetFromPath(string path)
        {
            if (Reader != null)
                if (!Parsed)
                    ParseImage();

            var segments = path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            if (segments[0] == "..") return null;

            WzImageProperty ret = null;
            foreach (var t in segments)
            {
                var foundChild = false;
                foreach (var iwp in (ret == null ? _properties : ret.WzProperties).Where(iwp => iwp.Name == t))
                {
                    ret = iwp;
                    foundChild = true;
                    break;
                }

                if (!foundChild) return null;
            }

            return ret;
        }

        /// <summary>
        /// Adds a property to the image
        /// </summary>
        /// <param name="prop">Property to add</param>
        public void AddProperty(WzImageProperty prop)
        {
            prop.Parent = this;
            if (Reader != null && !Parsed) ParseImage();
            _properties.Add(prop);
        }

        public void AddProperties(IEnumerable<WzImageProperty> props)
        {
            foreach (var prop in props) AddProperty(prop);
        }

        public void ClearProperties()
        {
            foreach (var prop in _properties) prop.Parent = null;
            _properties.Clear();
        }

        #endregion

        #region Parsing Methods

        /// <summary>
        /// Parses the image from the wz filetod
        /// </summary>
        public void ParseImage()
        {
            if (Parsed) return;
            if (Changed)
            {
                Parsed = true;
                return;
            }

            ParseEverything = false;
            Reader.BaseStream.Position = Offset;
            var b = Reader.ReadByte();
            if (b != 0x73 || Reader.ReadString() != "Property" || Reader.ReadUInt16() != 0)
                return;
            _properties.AddRange(WzImageProperty.ParsePropertyList(Offset, Reader, this, this));
            Parsed = true;
        }

        #endregion
    }
}