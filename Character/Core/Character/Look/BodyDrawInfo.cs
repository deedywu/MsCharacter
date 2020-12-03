﻿using System;
using System.Collections.Generic;
using Character.MapleLib.WzLib;
using Character.MapleLib.WzLib.WzProperties;
using Microsoft.Xna.Framework;
using Character.Core.Util;
using Character.Core.Character.Look;

namespace Character.Core.Character.Look
{
    public class BodyAction
    {
        private readonly Stance.Id _stance;

        public readonly short Frame;

        public readonly short Delay;

        private readonly Vector2 _move;

        public readonly bool AttackFrame;

        public BodyAction(WzSubProperty src)
        {
            _stance = Look.Stance.ByString(((WzStringProperty) src["action"]).Value);
            if (src["frame"] != null)
                Frame = (short) src["frame"];
            WzObject moveProperty = src["move"];
            if (moveProperty != null && moveProperty is WzVectorProperty)
                _move = (Vector2) moveProperty.WzValue;
            short sgnDelay = 0;
            if (src["delay"] != null)
                sgnDelay = (short) src["delay"];
            if (sgnDelay == 0)
                sgnDelay = 100;
            if (sgnDelay > 0)
            {
                Delay = sgnDelay;
                AttackFrame = true;
            }
            else if (sgnDelay < 0)
            {
                Delay = (short) -sgnDelay;
                AttackFrame = false;
            }
        }

        public BodyAction()
        {
        }

        public bool IsAttackFrame()
        {
            return AttackFrame;
        }

        public short GetFrame()
        {
            return Frame;
        }

        public Vector2 Move()
        {
            return _move;
        }

        public Stance.Id Stance()
        {
            return _stance;
        }
    }

    public class BodyDrawInfo
    {
        private readonly Dictionary<Stance.Id, Dictionary<short, Vector2>> _bodyPositions;
        private readonly Dictionary<Stance.Id, Dictionary<short, Vector2>> _armPositions;
        private readonly Dictionary<Stance.Id, Dictionary<short, Vector2>> _handPositions;
        private readonly Dictionary<Stance.Id, Dictionary<short, Vector2>> _headPositions;
        private readonly Dictionary<Stance.Id, Dictionary<short, Vector2>> _hairPositions;
        private readonly Dictionary<Stance.Id, Dictionary<short, Vector2>> _facePositions;
        private readonly Dictionary<Stance.Id, Dictionary<short, short>> _stanceDelays;
        private readonly Dictionary<string, Dictionary<short, BodyAction>> _bodyActions;
        private readonly Dictionary<string, List<short>> _attackDelays;

        public BodyDrawInfo()
        {
            var bodyNode = (WzImage) Wz.Character["00002000.img"];
            var headNode = (WzImage) Wz.Character["00012000.img"];
            _bodyPositions = new Dictionary<Stance.Id, Dictionary<short, Vector2>>();
            _armPositions = new Dictionary<Stance.Id, Dictionary<short, Vector2>>();
            _handPositions = new Dictionary<Stance.Id, Dictionary<short, Vector2>>();
            _headPositions = new Dictionary<Stance.Id, Dictionary<short, Vector2>>();
            _hairPositions = new Dictionary<Stance.Id, Dictionary<short, Vector2>>();
            _facePositions = new Dictionary<Stance.Id, Dictionary<short, Vector2>>();
            _stanceDelays = new Dictionary<Stance.Id, Dictionary<short, short>>();
            _bodyActions = new Dictionary<string, Dictionary<short, BodyAction>>();
            _attackDelays = new Dictionary<string, List<short>>();
            foreach (var stanceNode in bodyNode.WzProperties)
                if (stanceNode is WzSubProperty)
                {
                    var stStr = stanceNode.Name;
                    if (stStr.Equals("dead"))
                    {
                        Console.WriteLine();
                    }

                    short attackDelay = 0;
                    for (short frame = 0; frame < stanceNode.WzProperties.Count; frame++)
                    {
                        WzObject frameNode = stanceNode.WzProperties[frame];
                        if (frameNode is WzSubProperty)
                            if (frameNode["action"] is WzStringProperty)
                            {
                                var action = new BodyAction((WzSubProperty) frameNode);
                                if (!_bodyActions.ContainsKey(stStr))
                                    _bodyActions[stStr] = new Dictionary<short, BodyAction>();
                                _bodyActions[stStr][frame] = action;
                                if (action.IsAttackFrame())
                                {
                                    if (!_attackDelays.ContainsKey(stStr))
                                        _attackDelays[stStr] = new List<short>();
                                    _attackDelays[stStr].Add(attackDelay);
                                }

                                attackDelay += action.Delay;
                            }
                            else
                            {
                                var stance = Stance.ByString(stStr);
                                if (stance == Stance.Id.None)
                                    continue;
                                short delay = 0;
                                if (frameNode["delay"] != null)
                                    delay = (short) frameNode["delay"];
                                if (delay <= 0)
                                    delay = 100;
                                if (!_stanceDelays.ContainsKey(stance))
                                    _stanceDelays[stance] = new Dictionary<short, short>();
                                _stanceDelays[stance][frame] = delay;
                                var bodyShiftMap = new Dictionary<Body.Layer, Dictionary<string, Vector2>>();
                                foreach (var partNode0 in ((WzSubProperty) frameNode).WzProperties)
                                {
                                    var partNode = partNode0.GetByUol();
                                    if (partNode is WzCanvasProperty)
                                    {
                                        var zStr = ((WzStringProperty) partNode["z"]).Value;
                                        var z = Body.LayerByName(zStr);
                                        var map = (WzSubProperty) partNode["map"];
                                        foreach (var innerMap in map.WzProperties)
                                            if (innerMap is WzVectorProperty)
                                            {
                                                if (!bodyShiftMap.ContainsKey(z))
                                                    bodyShiftMap[z] = new Dictionary<string, Vector2>();
                                                bodyShiftMap[z][innerMap.Name] = (Vector2) innerMap.WzValue;
                                            }

                                        var headMap = headNode[stStr]?[frame.ToString()]["head"]["map"]
                                            .WzProperties;
                                        if (headMap != null)
                                            foreach (var mapNode in headMap)
                                                if (mapNode is WzVectorProperty)
                                                {
                                                    if (!bodyShiftMap.ContainsKey(Body.Layer.Head))
                                                        bodyShiftMap[Body.Layer.Head] =
                                                            new Dictionary<string, Vector2>();
                                                    bodyShiftMap[Body.Layer.Head][mapNode.Name] =
                                                        (Vector2) mapNode.WzValue;
                                                }

                                        if (!_bodyPositions.ContainsKey(stance))
                                            _bodyPositions[stance] = new Dictionary<short, Vector2>();
                                        _bodyPositions[stance][frame] =
                                            bodyShiftMap.GetVector2(Body.Layer.Body, "navel");
                                        if (!_armPositions.ContainsKey(stance))
                                            _armPositions[stance] = new Dictionary<short, Vector2>();
                                        _armPositions[stance][frame] =
                                            bodyShiftMap.ContainsKey(Body.Layer.Arm)
                                                ? bodyShiftMap.GetVector2(Body.Layer.Arm, "hand") -
                                                  bodyShiftMap.GetVector2(Body.Layer.Arm, "navel") +
                                                  bodyShiftMap.GetVector2(Body.Layer.Body, "navel")
                                                : bodyShiftMap.GetVector2(Body.Layer.ArmOverHair, "hand") -
                                                  bodyShiftMap.GetVector2(Body.Layer.ArmOverHair, "navel") +
                                                  bodyShiftMap.GetVector2(Body.Layer.Body, "navel");
                                        if (!_handPositions.ContainsKey(stance))
                                            _handPositions[stance] = new Dictionary<short, Vector2>();
                                        _handPositions[stance][frame] =
                                            bodyShiftMap.GetVector2(Body.Layer.HandBelowWeapon, "handMove");
                                        if (!_headPositions.ContainsKey(stance))
                                            _headPositions[stance] = new Dictionary<short, Vector2>();
                                        _headPositions[stance][frame] =
                                            bodyShiftMap.GetVector2(Body.Layer.Body, "neck") -
                                            bodyShiftMap.GetVector2(Body.Layer.Head, "neck");
                                        if (!_facePositions.ContainsKey(stance))
                                            _facePositions[stance] = new Dictionary<short, Vector2>();
                                        _facePositions[stance][frame] =
                                            bodyShiftMap.GetVector2(Body.Layer.Body, "neck") -
                                            bodyShiftMap.GetVector2(Body.Layer.Head, "neck") +
                                            bodyShiftMap.GetVector2(Body.Layer.Head, "brow");
                                        if (!_hairPositions.ContainsKey(stance))
                                            _hairPositions[stance] = new Dictionary<short, Vector2>();
                                        _hairPositions[stance][frame] =
                                            bodyShiftMap.GetVector2(Body.Layer.Head, "brow") -
                                            bodyShiftMap.GetVector2(Body.Layer.Head, "neck") +
                                            bodyShiftMap.GetVector2(Body.Layer.Body, "neck");
                                    }
                                }
                            }
                    }
                }
        }

        public Vector2 GetBodyPosition(Stance.Id stance, short frame)
        {
            if (_bodyPositions.ContainsKey(stance) && _bodyPositions[stance].ContainsKey(frame))
                return _bodyPositions[stance][frame];
            return new Vector2();
        }

        public Vector2 GetArmPosition(Stance.Id stance, short frame)
        {
            if (_armPositions.ContainsKey(stance) && _armPositions[stance].ContainsKey(frame))
                return _armPositions[stance][frame];
            return new Vector2();
        }

        public Vector2 GetHandPosition(Stance.Id stance, short frame)
        {
            if (_handPositions.ContainsKey(stance) && _handPositions[stance].ContainsKey(frame))
                return _handPositions[stance][frame];
            return new Vector2();
        }

        public Vector2 GetHeadPosition(Stance.Id stance, short frame)
        {
            if (_headPositions.ContainsKey(stance) && _headPositions[stance].ContainsKey(frame))
                return _headPositions[stance][frame];
            return new Vector2();
        }

        public Vector2 GetHairPosition(Stance.Id stance, short frame)
        {
            if (_hairPositions.ContainsKey(stance) && _hairPositions[stance].ContainsKey(frame))
                return _hairPositions[stance][frame];
            return new Vector2();
        }

        public Vector2 GetFacePosition(Stance.Id stance, short frame)
        {
            if (stance == Stance.Id.Dead)
                return _facePositions[Stance.Id.Stand1][frame] + new Vector2(0, 4);
            if (_facePositions.ContainsKey(stance) && _facePositions[stance].ContainsKey(frame))
                return _facePositions[stance][frame];
            return new Vector2();
        }

        public short NextFrame(Stance.Id stance, short frame)
        {
            if (_stanceDelays.ContainsKey(stance) && _stanceDelays[stance].ContainsKey((short) (frame + 1)))
                return (short) (frame + 1);
            return 0;
        }

        public short GetDelay(Stance.Id stance, short frame)
        {
            if (_stanceDelays.ContainsKey(stance) && _stanceDelays[stance].ContainsKey(frame))
                return _stanceDelays[stance][frame];
            return 100;
        }


        public short GetAttackDelay(string action, int no)
        {
            if (_attackDelays.ContainsKey(action) && no < _attackDelays[action].Count)
                return _attackDelays[action][no];
            return 0;
        }

        public short NextActionFrame(string action, short frame)
        {
            if (_bodyActions.ContainsKey(action) && _bodyActions[action].ContainsKey((short) (frame + 1)))
                return (short) (frame + 1);
            return 0;
        }

        public BodyAction GetAction(string action, short frame)
        {
            if (_bodyActions.ContainsKey(action) && _bodyActions[action].ContainsKey(frame))
                return _bodyActions[action][frame];
            return null;
        }
    }
}