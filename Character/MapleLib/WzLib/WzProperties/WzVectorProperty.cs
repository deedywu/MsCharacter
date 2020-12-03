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

using Microsoft.Xna.Framework;

namespace Character.MapleLib.WzLib.WzProperties
{
    /// <summary>
    /// A property that contains an x and a y value
    /// </summary>
    public class WzVectorProperty : WzExtended
    {
        #region Fields

        #endregion

        #region Inherited Members

        public override object WzValue => new Vector2(X.Value, Y.Value);

        /// <summary>
        /// The parent of the object
        /// </summary>
        public override WzObject Parent { get; internal set; }

        /// <summary>
        /// The name of the property
        /// </summary>
        public sealed override string Name { get; set; }

        /// <summary>
        /// The WzPropertyType of the property
        /// </summary>
        public override WzPropertyType PropertyType => WzPropertyType.Vector;

        /// <summary>
        /// Disposes the object
        /// </summary>
        public override void Dispose()
        {
            Name = null;
            X.Dispose();
            X = null;
            Y.Dispose();
            Y = null;
        }

        #endregion

        #region Custom Members

        /// <summary>
        /// The X value of the Vector2D
        /// </summary>
        public WzIntProperty X { get; set; }

        /// <summary>
        /// The Y value of the Vector2D
        /// </summary>
        public WzIntProperty Y { get; set; }

        /// <summary>
        /// Creates a WzVectorProperty with the specified name
        /// </summary>
        /// <param name="name">The name of the property</param>
        public WzVectorProperty(string name)
        {
            Name = name;
        }

        #endregion

        #region Cast Values

        /// <summary>
        /// The Vector2 of the Vector2D created from the X and Y
        /// </summary>
        public override Vector2 Pos() => new Vector2(X.Value, Y.Value);

        public override string ToString() => "X: " + X.Value + ", Y: " + Y.Value;

        #endregion
    }
}