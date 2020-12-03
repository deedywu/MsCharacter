﻿﻿using System;
using System.Collections.Generic;
using Character.MapleLib.WzLib;
using Character.MapleLib.WzLib.WzProperties;
using Microsoft.Xna.Framework;
using Character.Core.Common;
using Character.Core.Graphics;
using Character.Core.Util;

namespace Character.Core.Character.Look
{
    public class Body : IDisposable
    {
        private string _name;

        public string Name => _name;

        private Dictionary<Stance.Id, Dictionary<Layer, Dictionary<short, TextureD>>> _stances;

        private static readonly Dictionary<string, Layer> LayersByName = new Dictionary<string, Layer>()
        {
            ["body"] = Layer.Body,
            ["body"] = Layer.Body,
            ["backBody"] = Layer.Body,
            ["arm"] = Layer.Arm,
            ["armBelowHead"] = Layer.ArmBelowHead,
            ["armBelowHeadOverMailChest"] = Layer.ArmBelowHeadOverMail,
            ["armOverHair"] = Layer.ArmOverHair,
            ["armOverHairBelowWeapon"] = Layer.ArmOverHairBelowWeapon,
            ["handBelowWeapon"] = Layer.HandBelowWeapon,
            ["handOverHair"] = Layer.HandOverHair,
            ["handOverWeapon"] = Layer.HandOverWeapon,
            ["head"] = Layer.Head
        };

        public void Draw(Stance.Id stance, Layer layer, short frame, DrawArgument args)
        {
            if (stance == Stance.Id.Dead)
            {
                if (layer == Layer.Head)
                    _stances[Stance.Id.Stand1][layer][frame].Draw(args + new Vector2(0, 4));
                else
                {
                    if (!_stances.ContainsKey(stance) || !_stances[stance].ContainsKey(layer) ||
                        !_stances[stance][layer].ContainsKey(frame)) return;
                    _stances[stance][layer][frame].Draw(args + new Vector2(args.XScale.Equals(-1) ? 3 : -3, 0));
                }

                return;
            }

            if (!_stances.ContainsKey(stance) || !_stances[stance].ContainsKey(layer) ||
                !_stances[stance][layer].ContainsKey(frame)) return;
            _stances[stance][layer][frame].Draw(args);
        }

        public Body(int skin, BodyDrawInfo drawInfo)
        {
            _stances = new Dictionary<Stance.Id, Dictionary<Layer, Dictionary<short, TextureD>>>();
            var strId = skin.ToString().PadLeft(2, '0');
            var bodyNode = (WzImage) Wz.Character[$"000020{strId}.img"];
            var headNode = (WzImage) Wz.Character[$"000120{strId}.img"];
            foreach (var keyValuePair in Stance.Names)
            {
                var stanceName = keyValuePair.Value;
                var stance = keyValuePair.Key;
                var stanceNode = bodyNode[stanceName];
                if (stanceNode == null) continue;
                for (short frame = 0; frame < stanceNode.WzProperties.Count; frame++)
                {
                    var frameNode = stanceNode.WzProperties[frame];

                    foreach (var partNode0 in frameNode.WzProperties)
                    {
                        var partNode = partNode0.GetByUol();
                        if (!(partNode is WzCanvasProperty)) continue;
                        var z = ((WzStringProperty) partNode["z"]).Value;
                        var layer = LayerByName(z);
                        if (layer == Layer.None)
                            continue;
                        Vector2 shift;
                        var wzMap = partNode["map"];
                        switch (layer)
                        {
                            case Layer.HandBelowWeapon:
                                shift = drawInfo.GetHandPosition(stance, frame);
                                shift -= ((WzVectorProperty) wzMap["handMove"])?.Pos() ?? new Vector2();
                                break;
                            default:
                                shift = drawInfo.GetBodyPosition(stance, frame);
                                shift -= ((WzVectorProperty) wzMap["navel"])?.Pos() ?? new Vector2();
                                break;
                        }

                        if (!_stances.ContainsKey(stance))
                            _stances[stance] = new Dictionary<Layer, Dictionary<short, TextureD>>();
                        if (!_stances[stance].ContainsKey(layer))
                            _stances[stance][layer] = new Dictionary<short, TextureD>();
                        _stances[stance][layer][frame] = new TextureD(partNode).Shift(shift);
                    }

                    WzObject headSfNode0 = headNode[stanceName]?[frame.ToString()]["head"];
                    if (headSfNode0 == null) continue;
                    var headSfNode = headSfNode0.GetByUol();
                    var shift2 = drawInfo.GetHeadPosition(stance, frame);
                    if (!_stances.ContainsKey(stance))
                        _stances[stance] = new Dictionary<Layer, Dictionary<short, TextureD>>();
                    if (!_stances[stance].ContainsKey(Layer.Head))
                        _stances[stance][Layer.Head] = new Dictionary<short, TextureD>();
                    _stances[stance][Layer.Head][frame] = new TextureD(headSfNode).Shift(shift2);
                }
            }

            var args = new[] {"Light", "Tan", "Dark", "Pale", "Blue", "Green", "", "", "", "Grey", "Pink", "Red"};
            _name = skin < args.Length ? args[skin] : "";
        }

        public enum Layer
        {
            None,
            Body,
            Arm,
            ArmBelowHead,
            ArmBelowHeadOverMail,
            ArmOverHair,
            ArmOverHairBelowWeapon,
            HandBelowWeapon,
            HandOverHair,
            HandOverWeapon,
            Head,
            NumLayers
        }

        public static Layer LayerByName(string str)
        {
            return LayersByName.ContainsKey(str) ? LayersByName[str] : Layer.None;
        }

        public void Dispose()
        {
            _stances = null;
        }
    }
}