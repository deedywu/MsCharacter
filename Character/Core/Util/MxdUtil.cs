﻿using System.Collections.Generic;
using System.Security.Cryptography;
using Character.MapleLib.WzLib;
using Character.MapleLib.WzLib.WzProperties;
using Microsoft.Xna.Framework;
using Character.Core.Character.Look;

namespace Character.Core.Util
{
    public static class MxdUtil
    {
        public static Vector2 GetVector2(this IReadOnlyDictionary<Body.Layer, Dictionary<string, Vector2>> bodyShiftMap,
            Body.Layer layer, string name)
        {
            if (bodyShiftMap.ContainsKey(layer) && bodyShiftMap[layer].ContainsKey(name))
                return bodyShiftMap[layer][name];
            return new Vector2();
        }

        public static WzObject GetByUol(this WzObject wzObject)
        {
            return wzObject is WzUolProperty
                ? (WzObject) (wzObject.WzValue is WzUolProperty
                    ? ((WzUolProperty) wzObject.WzValue).WzValue
                    : wzObject.WzValue)
                : wzObject;
        }

        public static string GetString(this WzObject wzObject, string index)
        {
            return ((WzStringProperty) wzObject[index]).Value;
        }

        public static float Lerp(float first, float second, float alpha)
        {
            return alpha <= 0.0f ? first
                : alpha >= 1.0f ? second
                : first.Equals(second) ? first
                : ((1.0f - alpha) * first + alpha * second);
        }

        public static string Md5Encrypt(string strText)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var result = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(strText));
            return System.Text.Encoding.Default.GetString(result);
        }
    }
}