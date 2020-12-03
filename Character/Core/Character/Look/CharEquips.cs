﻿using System;
using System.Collections.Generic;
using Character.Core.Graphics;
using Character.Core.Util;

namespace Character.Core.Character.Look
{
    public class CharEquips : IDisposable
    {
        private readonly Dictionary<EquipSlot.Id, Clothing> _clothes = new Dictionary<EquipSlot.Id, Clothing>();

        public void Draw(EquipSlot.Id slot, Stance.Id stance, Clothing.Layer layer, short frame, DrawArgument args)
        {
            if (!_clothes.ContainsKey(slot)) return;
            _clothes[slot].Draw(stance, layer, frame, args);
        }

        public void AddEquip(int itemId, BodyDrawInfo drawInfo)
        {
            if (itemId <= 0) return;
            var cloth = GameUtil.GetCloth(itemId, drawInfo);
            _clothes[cloth.EqSlot] = cloth;
        }

        public void RemoveEquip(EquipSlot.Id slot)
        {
            _clothes.Remove(slot);
        }

        public bool IsVisible(EquipSlot.Id slot)
        {
            if (!_clothes.ContainsKey(slot)) return false;
            return _clothes[slot].TransParent == false;
        }

        public bool CompareLayer(EquipSlot.Id slot, Stance.Id stance, Clothing.Layer layer)
        {
            return _clothes.ContainsKey(slot) && _clothes[slot].ContainsLayer(stance, layer);
        }

        public bool HasOverAll => GetEquip(EquipSlot.Id.Top) / 10000 == 105;

        public int GetEquip(EquipSlot.Id slot)
        {
            return !_clothes.ContainsKey(slot) ? 0 : _clothes[slot].ItemId;
        }

        public bool HasWeapon => GetWeapon() != 0;

        public bool IsTwoHanded()
        {
            return _clothes.ContainsKey(EquipSlot.Id.Weapon) && _clothes[EquipSlot.Id.Weapon].TwoHanded;
        }

        public CapType GetCapType()
        {
            if (!_clothes.ContainsKey(EquipSlot.Id.Hat)) return CapType.None;
            var hat = _clothes[EquipSlot.Id.Hat];
            var vsLot = hat.VsLot;
            if (vsLot.Equals("CpH1H5"))
                return CapType.HalfCover;
            if (vsLot.Equals("CpH1H5AyAs"))
                return CapType.FullCover;
            return vsLot.Equals("CpH5") ? CapType.Headband : CapType.None;
        }

        public Stance.Id AdjustStance(Stance.Id stance)
        {
            if (!_clothes.ContainsKey(EquipSlot.Id.Weapon)) return stance;
            var weapon = _clothes[EquipSlot.Id.Weapon];
            switch (stance)
            {
                case Stance.Id.Stand1:
                case Stance.Id.Stand2:
                    return weapon.Stand;
                case Stance.Id.Walk1:
                case Stance.Id.Walk2:
                    return weapon.Walk;
            }

            return stance;
        }

        public int GetWeapon() => GetEquip(EquipSlot.Id.Weapon);

        public CharEquips()
        {
            foreach (var keyValuePair in _clothes)
                _clothes.Remove(keyValuePair.Key);
        }

        public enum CapType
        {
            None,
            Headband,
            Hairpin,
            HalfCover,
            FullCover
        }

        public void Dispose()
        {
            foreach (var keyValuePair in _clothes)
                _clothes.Remove(keyValuePair.Key);
        }
    }
}