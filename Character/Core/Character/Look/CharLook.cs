﻿﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Character.Core.Data;
using Character.Core.Graphics;
using Character.Core.Net;
using Character.Core.Template;
using Character.Core.Util;

namespace Character.Core.Character.Look
{
    public class CharLook : IDisposable
    {
        private readonly CharEquips _equips = new CharEquips();

        private readonly Nominal<Stance.Id> _stance = new Nominal<Stance.Id>();

        private readonly Nominal<Expression.Id> _expression = new Nominal<Expression.Id>();

        private readonly Nominal<short> _expFrame = new Nominal<short>();

        private readonly Nominal<short> _stFrame = new Nominal<short>();

        private readonly TimedBool _alerted = new TimedBool();

        private readonly BodyDrawInfo _drawInfo = new BodyDrawInfo();

        private readonly List<List<Stance.Id>> _deGenStances = new List<List<Stance.Id>>();

        private readonly List<List<Stance.Id>> _attackStances = new List<List<Stance.Id>>();

        public Hair Hair { get; private set; }

        public Face Face { get; private set; }

        public Body Body { get; private set; }

        public BodyAction Action { get; set; }

        public CharEquips Equips => _equips;

        private short _steLapSed;

        private short _expELapSed;

        private bool _flip;

        private string _actionStr;

        private short _actFrame;

        public CharLook()
        {
            Reset();
            Body = null;
            Hair = null;
            Face = null;
            _equips = null;
        }

        private void Init()
        {
            _deGenStances.Add(new List<Stance.Id>() {(Stance.Id.None)});
            _deGenStances.Add(new List<Stance.Id>() {Stance.Id.None});
            _deGenStances.Add(new List<Stance.Id>() {Stance.Id.None});
            _deGenStances.Add(new List<Stance.Id>() {Stance.Id.SwingT1, Stance.Id.SwingT3});
            _deGenStances.Add(new List<Stance.Id>() {Stance.Id.SwingT1, Stance.Id.StabT1});
            _deGenStances.Add(new List<Stance.Id>() {Stance.Id.None});
            _deGenStances.Add(new List<Stance.Id>() {Stance.Id.None});
            _deGenStances.Add(new List<Stance.Id>() {Stance.Id.SwingT1, Stance.Id.StabT1});
            _deGenStances.Add(new List<Stance.Id>() {Stance.Id.None});
            _deGenStances.Add(new List<Stance.Id>() {Stance.Id.SwingP1, Stance.Id.StabT2});

            _attackStances.Add(new List<Stance.Id>() {Stance.Id.None});
            _attackStances.Add(new List<Stance.Id>()
                {Stance.Id.StabO1, Stance.Id.StabO2, Stance.Id.SwingO1, Stance.Id.SwingO2, Stance.Id.SwingO3});
            _attackStances.Add(new List<Stance.Id>() {Stance.Id.StabT1, Stance.Id.SwingP1});
            _attackStances.Add(new List<Stance.Id>() {Stance.Id.Shoot1});
            _attackStances.Add(new List<Stance.Id>() {Stance.Id.Shoot2});
            _attackStances.Add(new List<Stance.Id>()
                {Stance.Id.StabO1, Stance.Id.StabO2, Stance.Id.SwingT1, Stance.Id.SwingT2, Stance.Id.SwingT3});
            _attackStances.Add(new List<Stance.Id>() {Stance.Id.SwingO1, Stance.Id.SwingO2});
            _attackStances.Add(new List<Stance.Id>() {Stance.Id.SwingO1, Stance.Id.SwingO2});
            _attackStances.Add(new List<Stance.Id>() {Stance.Id.None});
            _attackStances.Add(new List<Stance.Id>() {Stance.Id.Shot});
        }

        public CharLook(LookEntry entry)
        {
            Init();
            Reset();
            Body = new Body(entry.Skin, _drawInfo);
            Hair = new Hair(entry.Hair, _drawInfo);
            Face = new Face(entry.Face);
            foreach (var keyValuePair in entry.Equips)
                AddEquip(keyValuePair.Value);
        }

        public void Reset()
        {
            _flip = true;
            Action = null;
            _actionStr = "";
            _actFrame = 0;
            SetStance(Stance.Id.Stand1);
            _stFrame.Set(0);
            _steLapSed = 0;
            SetExpression(Expression.Id.Default);
            _expFrame.Set(0);
            _expELapSed = 0;
        }

        #region 绘制

        private void Draw(Stance.Id inStance, Expression.Id inExpression, short inFrame, short inExpFrame,
            DrawArgument args)
        {
            if (Stance.IsClimbing(inStance)) // 判断是否为 爬绳
            {
                Body.Draw(inStance, Body.Layer.Body, inFrame, args);
                _equips.Draw(EquipSlot.Id.Gloves, inStance, Clothing.Layer.Glove, inFrame, args);
                _equips.Draw(EquipSlot.Id.Shoes, inStance, Clothing.Layer.Shoes, inFrame, args);
                _equips.Draw(EquipSlot.Id.Bottom, inStance, Clothing.Layer.Pants, inFrame, args);
                _equips.Draw(EquipSlot.Id.Top, inStance, Clothing.Layer.Top, inFrame, args);
                _equips.Draw(EquipSlot.Id.Top, inStance, Clothing.Layer.Mail, inFrame, args);
                _equips.Draw(EquipSlot.Id.Cape, inStance, Clothing.Layer.Cape, inFrame, args);
                Body.Draw(inStance, Body.Layer.Head, inFrame, args);
                _equips.Draw(EquipSlot.Id.EarAcc, inStance, Clothing.Layer.Earrings, inFrame, args);
                switch (_equips.GetCapType())
                {
                    case CharEquips.CapType.None:
                        Hair.Draw(inStance, Hair.Layer.Back, inFrame, args);
                        break;
                    case CharEquips.CapType.Headband:
                        _equips.Draw(EquipSlot.Id.Hat, inStance, Clothing.Layer.Cap, inFrame, args);
                        Hair.Draw(inStance, Hair.Layer.Back, inFrame, args);
                        break;
                    case CharEquips.CapType.HalfCover:
                        Hair.Draw(inStance, Hair.Layer.BelowCap, inFrame, args);
                        _equips.Draw(EquipSlot.Id.Hat, inStance, Clothing.Layer.Cap, inFrame, args);
                        break;
                    case CharEquips.CapType.FullCover:
                        _equips.Draw(EquipSlot.Id.Hat, inStance, Clothing.Layer.Cap, inFrame, args);
                        break;
                }

                _equips.Draw(EquipSlot.Id.Shield, inStance, Clothing.Layer.BackShield, inFrame, args);
                _equips.Draw(EquipSlot.Id.Weapon, inStance, Clothing.Layer.BackWeapon, inFrame, args);
            }
            else
            {
                var faceShift = _drawInfo.GetFacePosition(inStance, inFrame);
                var faceArgs =
                    args + (faceShift - (args.XScale.Equals(-1f) ? new Vector2(faceShift.X * 2, 0) : new Vector2()));
                Hair.Draw(inStance, Hair.Layer.BelowBody, inFrame, args);
                _equips.Draw(EquipSlot.Id.Cape, inStance, Clothing.Layer.Cape, inFrame, args);
                _equips.Draw(EquipSlot.Id.Shield, inStance, Clothing.Layer.ShieldBelowBody, inFrame, args);
                _equips.Draw(EquipSlot.Id.Weapon, inStance, Clothing.Layer.WeaponBelowBody, inFrame, args);
                _equips.Draw(EquipSlot.Id.Hat, inStance, Clothing.Layer.CapBelowBody, inFrame, args);
                Body.Draw(inStance, Body.Layer.Body, inFrame, args);
                _equips.Draw(EquipSlot.Id.Gloves, inStance, Clothing.Layer.WristOverBody, inFrame, args);
                _equips.Draw(EquipSlot.Id.Gloves, inStance, Clothing.Layer.GloveOverBody, inFrame, args);
                _equips.Draw(EquipSlot.Id.Shoes, inStance, Clothing.Layer.Shoes, inFrame, args);
                Body.Draw(inStance, Body.Layer.ArmBelowHead, inFrame, args);

                if (_equips.HasOverAll)
                {
                    _equips.Draw(EquipSlot.Id.Top, inStance, Clothing.Layer.Mail, inFrame, args);
                }
                else
                {
                    _equips.Draw(EquipSlot.Id.Bottom, inStance, Clothing.Layer.Pants, inFrame, args);
                    _equips.Draw(EquipSlot.Id.Top, inStance, Clothing.Layer.Top, inFrame, args);
                }

                Body.Draw(inStance, Body.Layer.ArmBelowHeadOverMail, inFrame, args);
                Hair.Draw(inStance, Hair.Layer.Default, inFrame, args);
                _equips.Draw(EquipSlot.Id.Shield, inStance, Clothing.Layer.ShieldOverHair, inFrame, args);
                _equips.Draw(EquipSlot.Id.EarAcc, inStance, Clothing.Layer.Earrings, inFrame, args);
                Body.Draw(inStance, Body.Layer.Head, inFrame, args);
                Hair.Draw(inStance, Hair.Layer.Shade, inFrame, args);
                Face.Draw(inExpression, inExpFrame, faceArgs);
                _equips.Draw(EquipSlot.Id.Face, inStance, Clothing.Layer.FaceAcc, 0, faceArgs);
                _equips.Draw(EquipSlot.Id.EyeAcc, inStance, Clothing.Layer.EyeAcc, inFrame, args);
                _equips.Draw(EquipSlot.Id.Shield, inStance, Clothing.Layer.Shield, inFrame, args);
                switch (_equips.GetCapType())
                {
                    case CharEquips.CapType.None:
                        Hair.Draw(inStance, Hair.Layer.OverHead, inFrame, args);
                        break;
                    case CharEquips.CapType.Headband:
                        _equips.Draw(EquipSlot.Id.Hat, inStance, Clothing.Layer.Cap, inFrame, args);
                        Hair.Draw(inStance, Hair.Layer.Default, inFrame, args);
                        Hair.Draw(inStance, Hair.Layer.OverHead, inFrame, args);
                        _equips.Draw(EquipSlot.Id.Hat, inStance, Clothing.Layer.CapOverHair, inFrame, args);
                        break;
                    case CharEquips.CapType.HalfCover:
                        Hair.Draw(inStance, Hair.Layer.Default, inFrame, args);
                        _equips.Draw(EquipSlot.Id.Hat, inStance, Clothing.Layer.Cap, inFrame, args);
                        break;
                    case CharEquips.CapType.FullCover:
                        _equips.Draw(EquipSlot.Id.Hat, inStance, Clothing.Layer.Cap, inFrame, args);
                        break;
                }

                _equips.Draw(EquipSlot.Id.Weapon, inStance, Clothing.Layer.WeaponBelowArm, inFrame, args);

                if (inStance == Stance.Id.StabO1)
                    Console.WriteLine();
                if (IsTwoHanded(inStance))
                {
                    _equips.Draw(EquipSlot.Id.Top, inStance, Clothing.Layer.MailArm, inFrame, args);
                    Body.Draw(inStance, Body.Layer.Arm, inFrame, args);
                    Body.Draw(inStance, Body.Layer.ArmOverHair, inFrame, args);
                    Body.Draw(inStance, Body.Layer.ArmOverHairBelowWeapon, inFrame, args);
                    _equips.Draw(EquipSlot.Id.Weapon, inStance, Clothing.Layer.Weapon, inFrame, args);
                }
                else
                {
                    _equips.Draw(EquipSlot.Id.Weapon, inStance, Clothing.Layer.Weapon, inFrame, args);
                    Body.Draw(inStance, Body.Layer.Arm, inFrame, args);
                    Body.Draw(inStance, Body.Layer.ArmOverHair, inFrame, args);
                    Body.Draw(inStance, Body.Layer.ArmOverHairBelowWeapon, inFrame, args);
                    _equips.Draw(EquipSlot.Id.Top, inStance, Clothing.Layer.MailArm, inFrame, args);
                }

                _equips.Draw(EquipSlot.Id.Gloves, inStance, Clothing.Layer.Wrist, inFrame, args);
                _equips.Draw(EquipSlot.Id.Gloves, inStance, Clothing.Layer.Glove, inFrame, args);
                _equips.Draw(EquipSlot.Id.Weapon, inStance, Clothing.Layer.WeaponOverGlove, inFrame, args);

                Body.Draw(inStance, Body.Layer.HandBelowWeapon, inFrame, args);

                _equips.Draw(EquipSlot.Id.Weapon, inStance, Clothing.Layer.WeaponOverHand, inFrame, args);
                _equips.Draw(EquipSlot.Id.Weapon, inStance, Clothing.Layer.WeaponOverBody, inFrame, args);
                Body.Draw(inStance, Body.Layer.HandOverHair, inFrame, args);
                Body.Draw(inStance, Body.Layer.HandOverWeapon, inFrame, args);

                _equips.Draw(EquipSlot.Id.Gloves, inStance, Clothing.Layer.WristOverHair, inFrame, args);
                _equips.Draw(EquipSlot.Id.Gloves, inStance, Clothing.Layer.GloveOverHair, inFrame, args);
            }
        }

        public void Draw(DrawArgument args, float alpha)
        {
            if (Body == null || Hair == null || Face == null) return;
            var acMove = new Vector2();
            if (Action != null)
                acMove = Action.Move();
            var relArgs = new DrawArgument(acMove, _flip);

            var inStance = _stance.Get(alpha);
            var inExpression = _expression.Get(alpha);
            var inFrame = _stFrame.Get(alpha);
            var inExpFrame = _expFrame.Get(alpha);
            switch (inStance)
            {
                case Stance.Id.Stand1:
                case Stance.Id.Stand2:
                    if (_alerted.Bool)
                        inStance = Stance.Id.Alert;
                    break;
            }

            var args2 = (relArgs + args);
            Draw(inStance, inExpression, inFrame, inExpFrame, args2);
        }

        public void Draw(Vector2 position, bool flipped, Stance.Id inStance, Expression.Id inExpression)
        {
            inStance = _equips.AdjustStance(inStance);
            Draw(inStance, inExpression, 0, 0, new DrawArgument(position, flipped));
        }

        #endregion

        public bool Update(short timeStep)
        {
            // todo
            if (timeStep == 0)
            {
                _stance.Normalize();
                _stFrame.Normalize();
                _expression.Normalize();
                _expFrame.Normalize();
            }

            _alerted.Update();
            var aniEnd = false;
            if (Action == null)
            {
                var delay = GetDelay(_stance.Get(), _stFrame.Get());
                var delta = delay - _steLapSed;
                if (timeStep >= delta)
                {
                    _steLapSed = (short) (timeStep - delta);
                    var nextFrame = GetNextFrame(_stance.Get(), _stFrame.Get());
                    var threshold = (float) delta / timeStep;
                    _stFrame.Next(nextFrame, threshold);
                    if (_stFrame.Get() == 0)
                        aniEnd = true;
                }
                else
                {
                    _stance.Normalize();
                    _stFrame.Normalize();
                    _steLapSed += timeStep;
                }
            }
            else
            {
                var delay = Action.Delay;
                var delta = (short) (delay - _steLapSed);
                if (timeStep >= delta)
                {
                    _steLapSed = (short) (timeStep - delta);
                    _actFrame = _drawInfo.NextActionFrame(_actionStr, _actFrame);
                    if (_actFrame > 0)
                    {
                        Action = _drawInfo.GetAction(_actionStr, _actFrame);
                        var threshold = (float) delta / timeStep;
                        _stance.Next(Action.Stance(), threshold);
                        _stFrame.Next(Action.GetFrame(), threshold);
                    }
                    else
                    {
                        aniEnd = true;
                        Action = null;
                        _actionStr = "";
                        SetStance(Stance.Id.Stand1);
                    }
                }
                else
                {
                    _stance.Normalize();
                    _stFrame.Normalize();
                    _steLapSed += timeStep;
                }
            }

            var expDelay = Face.GetDelay(_expression.Get(), _expFrame.Get());
            var expDelta = expDelay - _expELapSed;
            if (timeStep >= expDelta)
            {
                _expELapSed = (short) (timeStep - expDelta);
                var nextExpFrame = Face.NextFrame(_expression.Get(), _expFrame.Get());
                var fcThreshold = (float) expDelta / timeStep;
                _expFrame.Next(nextExpFrame, fcThreshold);
                if (_expFrame.Get() == 0)
                {
                    _expression.Next(
                        _expression.Get() == Expression.Id.Default ? Expression.Id.Blink : Expression.Id.Default,
                        fcThreshold);
                }
            }
            else
            {
                _expression.Normalize();
                _expFrame.Normalize();
                _expELapSed += timeStep;
            }

            return aniEnd;
        }

        public void SetBody(int skinId)
        {
            Body = GameUtil.GetBody(skinId, _drawInfo);
        }

        public void SetHair(int hairId)
        {
            Hair = GameUtil.GetHair(hairId, _drawInfo);
        }

        public void SetFace(int faceId)
        {
            Face = GameUtil.GetFace(faceId);
        }

        public void UpdateTwoHanded() => SetStance(Stance.BaseOf(_stance.Get()));

        public void AddEquip(int itemId)
        {
            _equips.AddEquip(itemId, _drawInfo);
            UpdateTwoHanded();
        }

        public void RemoveEquip(EquipSlot.Id slot)
        {
            _equips.RemoveEquip(slot);
            if (slot == EquipSlot.Id.Weapon) UpdateTwoHanded();
        }

        public void Attack(bool degenerate)
        {
            var weaponId = _equips.GetWeapon();
            if (weaponId <= 0) return;
            var weapon = GameUtil.GetWeaponData(weaponId);
            var attackType = weapon.Attack;
            if (attackType == 9 && !degenerate)
            {
                _stance.Set(Stance.Id.Shot);
                SetAction("handgun");
            }
            else
            {
                var attackStance = GetAttackStance(attackType, degenerate);
                _stance.Set(attackStance);
                _stFrame.Set(0);
                _steLapSed = 0;
            }

            weapon.Play(degenerate);
        }

        public void Attack(Stance.Id newStance)
        {
            if (Action != null && newStance == Stance.Id.None)
                return;
            switch (newStance)
            {
                case Stance.Id.Shot:
                    SetAction("handgun");
                    break;
                default:
                    SetStance(newStance);
                    break;
            }
        }

        public void SetStance(Stance.Id newStance)
        {
            if (Action != null || newStance == Stance.Id.None) return;
            var adjStance = _equips.AdjustStance(newStance);
            if (_stance.Get() == adjStance) return;
            _stance.Set(adjStance);
            _stFrame.Set(0);
            _steLapSed = 0;
        }

        public Stance.Id GetAttackStance(short attack, bool degenerate)
        {
            if (_stance.Get() == Stance.Id.Prone) return Stance.Id.ProneStab;
            if (attack <= (short) EnumAttack.None || attack >= (short) EnumAttack.NumAttacks) return Stance.Id.Stand1;
            var stances = degenerate ? _deGenStances[attack] : _attackStances[attack];
            return stances.Count <= 0 ? Stance.Id.Stand1 : stances[RandomZer.NextInt(stances.Count)];
        }

        public short GetDelay(Stance.Id st, short fr)
        {
            return _drawInfo.GetDelay(st, fr);
        }

        public short GetNextFrame(Stance.Id st, short fr)
        {
            return _drawInfo.NextFrame(st, fr);
        }

        public void SetExpression(Expression.Id newExpression)
        {
            if (_expression.Get() == newExpression) return;
            _expression.Set(newExpression);
            _expFrame.Set(0);
            _expELapSed = 0;
        }

        public void SetAction(string acStr)
        {
            if (acStr.Equals(_actionStr) || acStr.Equals("")) return;
            var acStance = Stance.ByString(acStr);
            if (acStance != Stance.Id.None)
                SetStance(acStance);
            else
            {
                Action = _drawInfo.GetAction(acStr, 0);
                if (Action == null) return;
                _actFrame = 0;
                _steLapSed = 0;
                _actionStr = acStr;
                _stance.Set(Action.Stance());
                _stFrame.Set(Action.GetFrame());
            }
        }

        public void SetDirection(bool f)
        {
            _flip = f;
        }

        public void SetAlerted(long millis)
        {
            _alerted.SetFor(millis);
        }

        public bool IsAlerted => _alerted.Bool;

        public bool IsTwoHanded(Stance.Id stance)
        {
            switch (stance)
            {
                case Stance.Id.Stand1:
                case Stance.Id.Walk1:
                    return false;
                case Stance.Id.Stand2:
                case Stance.Id.Walk2:
                    return true;
            }

            return _equips.IsTwoHanded();
        }

        public short GetAttackDelay(int no, short ff)
        {
            if (Action != null)
                return _drawInfo.GetAttackDelay(_actionStr, no);
            else
            {
                short delay = 0;
                for (short frame = 0; frame < ff; frame++)
                    delay += GetDelay(_stance.Get(), frame);
                return delay;
            }
        }

        public short GetFrame() => _stFrame.Get();

        public Stance.Id GetStance() => _stance.Get();

        private enum EnumAttack
        {
            None = 0,
            S1A1M1D = 1,
            Spear = 2,
            Bow = 3,
            Crossbow = 4,
            S2A2M2 = 5,
            Wand = 6,
            Claw = 7,
            Gun = 9,
            NumAttacks
        }

        public void Dispose()
        {
        }
    }
}