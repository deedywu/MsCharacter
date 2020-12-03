﻿using System;
using System.Collections.Generic;
using Character.MapleLib.WzLib;
using Character.MapleLib.WzLib.WzProperties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Character.Core.Common;
using Character.Core.Graphics;
using Character.Core.Util;

namespace Character.Core.Character.Look
{
    public class Hair : IDisposable
    {
        private Dictionary<Stance.Id, Dictionary<Layer, Dictionary<short, TextureD>>> _stances;

        private static readonly Dictionary<string, Layer> LayersByName = new Dictionary<string, Layer>()
        {
            ["hair"] = Layer.Default,
            ["hairBelowBody"] = Layer.BelowBody,
            ["hairOverHead"] = Layer.OverHead,
            ["hairShade"] = Layer.Shade,
            ["backHair"] = Layer.Back,
            ["backHairBelowCap"] = Layer.BelowCap
        };

        public readonly string name;

        public readonly string color;

        public void Draw(Stance.Id stance, Layer layer, short frame, DrawArgument args)
        {
            if (stance == Stance.Id.Dead)
                _stances[Stance.Id.Stand1][Layer.OverHead][frame].Draw(args+ new Vector2(0,4));
            if (_stances.ContainsKey(stance) && _stances[stance].ContainsKey(layer) &&
                _stances[stance][layer].ContainsKey(frame))
            {
                _stances[stance][layer][frame].Draw(args);
            }
        }

        public Hair(int hairId, BodyDrawInfo drawInfo)
        {
            _stances = new Dictionary<Stance.Id, Dictionary<Layer, Dictionary<short, TextureD>>>();
            var hairNode = (WzImage) Wz.Character["hair"][$"000{hairId}.img"];
            foreach (var keyValuePair in Stance.Names)
            {
                var stanceName = keyValuePair.Value;
                var stance = keyValuePair.Key;
                var stanceNode = hairNode[stanceName];
                if (stanceNode == null) continue;
                for (short frame = 0; frame < stanceNode.WzProperties.Count; frame++)
                {
                    var frameNode = stanceNode.WzProperties[frame];
                    foreach (var layerNode0 in frameNode.WzProperties)
                    {
                        var layerNode = layerNode0.GetByUol();
                        var layerName = layerNode.Name;
                        if (!LayersByName.ContainsKey(layerName)) continue;
                        var layer = (Layer) LayersByName[layerName];
                        if (layerName.Equals("hairShade"))
                            layerNode = layerNode["0"];
                        var brow = ((WzVectorProperty) layerNode["map"]["brow"]).Pos();
                        var shift = drawInfo.GetHairPosition(stance, frame) - brow;
                        if (!_stances.ContainsKey(stance))
                            _stances[stance] = new Dictionary<Layer, Dictionary<short, TextureD>>();
                        if (!_stances[stance].ContainsKey(layer))
                            _stances[stance][layer] = new Dictionary<short, TextureD>();
                        _stances[stance][layer][frame] = new TextureD(layerNode).Shift(shift);
                    }
                }
            }

            name = (string) Wz.String["Eqp.img"]["Eqp"]["Hair"][$"{hairId}"]["name"].WzValue;
            var args = new[] {"黑色", "红色", "橘黄色", "金色", "绿色", "蓝色", "紫罗兰色", "棕色"};
            color = hairId % 10 < args.Length ? args[hairId % 10] : "";
        }

        public enum Layer
        {
            None,
            Default,
            BelowBody,
            OverHead,
            Shade,
            Back,
            BelowCap,
            NumLayers
        }

        public void Dispose()
        {
            _stances = null;
        }
    }
}