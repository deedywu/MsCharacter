using System;
using System.Collections.Generic;
using System.Linq;
using Character.MapleLib.WzLib;
using Character.MapleLib.WzLib.WzProperties;
using Microsoft.Xna.Framework;
using Character.Core.Common;
using Character.Core.Data;
using Character.Core.Graphics;
using Character.Core.Util;

namespace Character.Core.Character.Look
{
    public class Clothing : IDisposable
    {
        private static readonly Dictionary<string, Layer> SubLayerNames = new Dictionary<string, Layer>()
        {
            // WEAPON
            ["weaponOverHand"] = Layer.WeaponOverHand,
            ["weaponOverGlove"] = Layer.WeaponOverGlove,
            ["weaponOverBody"] = Layer.WeaponOverBody,
            ["weaponBelowArm"] = Layer.WeaponBelowArm,
            ["weaponBelowBody"] = Layer.WeaponBelowBody,
            ["backWeaponOverShield"] = Layer.BackWeapon,
            // SHIELD
            ["shieldOverHair"] = Layer.ShieldOverHair,
            ["shieldBelowBody"] = Layer.ShieldBelowBody,
            ["backShield"] = Layer.BackShield,
            // GLOVE
            ["gloveWrist"] = Layer.Wrist,
            ["gloveOverHair"] = Layer.GloveOverHair,
            ["gloveOverBody"] = Layer.GloveOverBody,
            ["gloveWristOverHair"] = Layer.WristOverHair,
            ["gloveWristOverBody"] = Layer.WristOverBody,
            // CAP
            ["capOverHair"] = Layer.CapOverHair,
            ["capBelowBody"] = Layer.CapBelowBody
        };

        private Dictionary<Stance.Id, Dictionary<Layer, Dictionary<Short, TextureD>>> _stances;

        public readonly int ItemId;

        public readonly EquipSlot.Id EqSlot;

        public readonly bool TwoHanded;

        public readonly Stance.Id Walk;

        public readonly Stance.Id Stand;

        public readonly string VsLot;

        public readonly bool TransParent;

        public void Draw(Stance.Id stance, Layer layer, short frame, DrawArgument args)
        {
            if (!_stances.ContainsKey(stance) || !_stances[stance].ContainsKey(layer)) return;
            foreach (var keyValuePair in _stances[stance][layer].Where(keyValuePair => keyValuePair.Key.S == frame))
                keyValuePair.Value.Draw(args);
        }

        public bool ContainsLayer(Stance.Id stance, Layer layer)
        {
            return _stances.ContainsKey(stance) && _stances[stance].ContainsKey(layer);
        }

        public Clothing(int id, BodyDrawInfo drawInfo)
        {
            _stances = new Dictionary<Stance.Id, Dictionary<Layer, Dictionary<Short, TextureD>>>();
            ItemId = id;
            var equipData = GameUtil.GetEquipData(ItemId);
            EqSlot = equipData.EqSlot;
            TwoHanded = EqSlot == EquipSlot.Id.Weapon && new WeaponData(ItemId).TwoHanded;
            const int noneWeaponTypes = 15;
            const int weaponOffset = noneWeaponTypes + 15;
            const int weaponTypes = 20;
            var index = (ItemId / 10000) - 100;
            Layer chLayer;
            if (index < noneWeaponTypes)
                chLayer = _layers[index];
            else if (index >= weaponOffset && index < weaponOffset + weaponTypes)
                chLayer = Layer.Weapon;
            else
                chLayer = Layer.Cape;
            var strId = $"0{ItemId}";
            var category = equipData.ItemData.category;
            var src = (WzImage) Wz.Character[category][$"{strId}.img"];
            var info = (WzSubProperty) src["info"];
            VsLot = info.GetString("vslot");
            switch ((int)info["stand"])
            {
                case 1:
                    Stand = Stance.Id.Stand1;
                    break;
                case 2:
                    Stand = Stance.Id.Stand2;
                    break;
                default:
                    Stand = TwoHanded ? Stance.Id.Stand2 : Stance.Id.Stand1;
                    break;
            }

            switch ((int)info["walk"])
            {
                case 1:
                    Walk = Stance.Id.Walk1;
                    break;
                case 2:
                    Walk = Stance.Id.Walk2;
                    break;
                default:
                    Walk = TwoHanded ? Stance.Id.Walk2 : Stance.Id.Walk1;
                    break;
            }

            foreach (var iter in Stance.Names)
            {
                var stanceName = iter.Value;
                var stance = iter.Key;
                var stanceNode = src[stanceName];
                if (stanceNode == null) continue;
                for (short frame = 0; frame < stanceNode.WzProperties.Count; frame++)
                {
                    var frameNode = (WzSubProperty) stanceNode[$"{frame}"].GetByUol();
                    foreach (var partNode0 in frameNode.WzProperties)
                    {
                        if (partNode0.GetByUol() is WzCanvasProperty)
                        {
                            var partNode = (WzCanvasProperty) partNode0.GetByUol();
                            var part = partNode.Name;
                            var z = chLayer;
                            var zs = partNode.GetString("z");
                            if (part.Equals("mailArm"))
                                z = Layer.MailArm;
                            else
                            {
                                if (SubLayerNames.ContainsKey(zs))
                                    z = SubLayerNames[zs];
                            }

                            var parent = "";
                            var parentPos = new Vector2();
                            foreach (var mapNode0 in partNode["map"].WzProperties)
                            {
                                var mapNode = mapNode0.GetByUol();
                                if (mapNode is WzVectorProperty)
                                {
                                    parent = mapNode.Name;
                                    parentPos = ((WzVectorProperty) mapNode).Pos();
                                }
                            }

                            var shift = new Vector2();
                            switch (EqSlot)
                            {
                                case EquipSlot.Id.Face:
                                    shift -= parentPos;
                                    break;
                                case EquipSlot.Id.Shoes:
                                case EquipSlot.Id.Gloves:
                                case EquipSlot.Id.Top:
                                case EquipSlot.Id.Bottom:
                                case EquipSlot.Id.Cape:
                                    shift = drawInfo.GetBodyPosition(stance, frame) - parentPos;
                                    break;
                                case EquipSlot.Id.Hat:
                                case EquipSlot.Id.EarAcc:
                                case EquipSlot.Id.EyeAcc:
                                    shift = drawInfo.GetFacePosition(stance, frame) - parentPos;
                                    break;
                                case EquipSlot.Id.Shield:
                                case EquipSlot.Id.Weapon:
                                    if (parent.Equals("handMove"))
                                        shift += drawInfo.GetHeadPosition(stance, frame);
                                    else if (parent.Equals("hand"))
                                        shift += drawInfo.GetArmPosition(stance, frame);
                                    else if (parent.Equals("navel"))
                                        shift += drawInfo.GetBodyPosition(stance, frame);
                                    shift -= parentPos;
                                    break;
                            }

                            if (!_stances.ContainsKey(stance))
                                _stances[stance] = new Dictionary<Layer, Dictionary<Short, TextureD>>();
                            if (!_stances[stance].ContainsKey(z))
                                _stances[stance][z] = new Dictionary<Short, TextureD>();
                            _stances[stance][z][new Short(frame)] = new TextureD(partNode).Shift(shift);
                        }
                    }
                }
            }

            var transParents = new HashSet<int>()
            {
                1002186
            };
            TransParent = transParents.Contains(ItemId);
        }

        private readonly Layer[] _layers = new[]
        {
            Layer.Cap,
            Layer.FaceAcc,
            Layer.EyeAcc,
            Layer.Earrings,
            Layer.Top,
            Layer.Mail,
            Layer.Pants,
            Layer.Shoes,
            Layer.Glove,
            Layer.Shield,
            Layer.Cape,
            Layer.Ring,
            Layer.Pendant,
            Layer.Belt,
            Layer.Medal
        };

        public enum Layer
        {
            Cape,
            Shoes,
            Pants,
            Top,
            Mail,
            MailArm,
            Earrings,
            FaceAcc,
            EyeAcc,
            Pendant,
            Belt,
            Medal,
            Ring,
            Cap,
            CapBelowBody,
            CapOverHair,
            Glove,
            Wrist,
            GloveOverHair,
            WristOverHair,
            GloveOverBody,
            WristOverBody,
            Shield,
            BackShield,
            ShieldBelowBody,
            ShieldOverHair,
            Weapon,
            BackWeapon,
            WeaponBelowArm,
            WeaponBelowBody,
            WeaponOverHand,
            WeaponOverBody,
            WeaponOverGlove,
            NumLayers
        }

        public void Dispose()
        {
            _stances = null;
        }

        /// <summary>
        /// 允许出现重复key使用的
        /// </summary>
        private class Short
        {
            public short S { get; }

            public Short(short s)
            {
                this.S = s;
            }
        }
    }
}