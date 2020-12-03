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
using Character.MapleLib.WzLib.Util;

namespace Character.MapleLib.WzLib.WzProperties
{
    /// <summary>
    /// A property that is stored in the wz file with a signed byte and possibly followed by an int. If the 
    /// signed byte is equal to -128, the value is is the int that follows, else the value is the byte.
    /// </summary>
    public class WzIntProperty : WzImageProperty
    {
        #region Fields

        #endregion

        #region Inherited Members

        public override object WzValue => Value;

        /// <summary>
        /// The parent of the object
        /// </summary>
        public override WzObject Parent { get; internal set; }

        /// <summary>
        /// The WzPropertyType of the property
        /// </summary>
        public override WzPropertyType PropertyType => WzPropertyType.Int;

        /// <summary>
        /// The name of the property
        /// </summary>
        public sealed override string Name { get; set; }

        /// <summary>
        /// Dispose the object
        /// </summary>
        public override void Dispose()
        {
            Name = null;
        }

        #endregion

        #region Custom Members

        /// <summary>
        /// The value of the property
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Creates a WzCompressedIntProperty with the specified name and value
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <param name="value">The value of the property</param>
        public WzIntProperty(string name, int value)
        {
            Name = name;
            Value = value;
        }

        #endregion

        #region Cast Values

        public override float GetFloat() => Value;

        public override double GetDouble() => Value;

        public override int GetInt() => Value;

        public override short GetShort() => (short) Value;

        public override long GetLong() => Value;

        public override string ToString() => Value.ToString();

        #endregion
    }
}