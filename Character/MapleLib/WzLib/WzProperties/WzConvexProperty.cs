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

namespace Character.MapleLib.WzLib.WzProperties
{
    /// <summary>
    /// A property that contains several WzExtendedPropertys
    /// </summary>
    public class WzConvexProperty : WzExtended, IPropertyContainer
    {
        #region Fields

        private List<WzImageProperty> _properties = new List<WzImageProperty>();

        #endregion

        #region Inherited Members

        /// <summary>
        /// The parent of the object
        /// </summary>
        public override WzObject Parent { get; internal set; }

        /// <summary>
        /// The WzPropertyType of the property
        /// </summary>
        public override WzPropertyType PropertyType => WzPropertyType.Convex;

        /// <summary>
        /// The properties contained in the property
        /// </summary>
        public override List<WzImageProperty> WzProperties => _properties;

        /// <summary>
        /// The name of this property
        /// </summary>
        public sealed override string Name { get; set; }

        /// <summary>
        /// Gets a wz property by it's name
        /// </summary>
        /// <param name="name">The name of the property</param>
        /// <returns>The wz property with the specified name</returns>
        public override WzImageProperty this[string name]
        {
            get { return _properties.FirstOrDefault(iwp => iwp.Name.ToLower().Equals(name.ToLower())); }
        }

        /// <summary>
        /// Gets a wz property by a path name
        /// </summary>
        /// <param name="path">path to property</param>
        /// <returns>the wz property with the specified name</returns>
        public override WzImageProperty GetFromPath(string path)
        {
            var segments = path.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
            if (segments[0] == "..") return ((WzImageProperty) Parent)[path.Substring(Name.IndexOf('/') + 1)];

            WzImageProperty ret = this;
            foreach (var t in segments)
            {
                var foundChild = false;
                foreach (var iwp in ret.WzProperties.Where(iwp => iwp.Name == t))
                {
                    ret = iwp;
                    foundChild = true;
                    break;
                }

                if (!foundChild)
                {
                    return null;
                }
            }

            return ret;
        }

        public override void Dispose()
        {
            Name = null;
            foreach (var exProp in _properties) exProp.Dispose();
            _properties.Clear();
            _properties = null;
        }

        #endregion

        #region Custom Members

        /// <summary>
        /// Creates a WzConvexProperty with the specified name
        /// </summary>
        /// <param name="name">The name of the property</param>
        public WzConvexProperty(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Adds a WzExtendedProperty to the list of properties
        /// </summary>
        /// <param name="prop">The property to add</param>
        public void AddProperty(WzImageProperty prop)
        {
            if (!(prop is WzExtended))
                throw new Exception("Property is not IExtended");
            prop.Parent = this;
            _properties.Add((WzExtended) prop);
        }

        public void AddProperties(IEnumerable<WzImageProperty> properties)
        {
            foreach (var property in properties)
                AddProperty(property);
        }

        public void ClearProperties()
        {
            foreach (var prop in _properties) prop.Parent = null;
            _properties.Clear();
        }

        #endregion
    }
}