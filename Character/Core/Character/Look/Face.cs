using System;
using System.Collections.Generic;
using Character.MapleLib.WzLib;
using Character.MapleLib.WzLib.WzProperties;
using Character.Core.Common;
using Character.Core.Graphics;
using Character.Core.Util;

namespace Character.Core.Character.Look
{
    public class Face : IDisposable
    {
        private Dictionary<Expression.Id, Dictionary<short, Frame>> _expressions;

        public readonly string Name;

        public short NextFrame(Expression.Id exp, short frame)
        {
            if (_expressions.ContainsKey(exp) && _expressions[exp].ContainsKey((short) (frame + 1)))
                return (short) (frame + 1);
            return 0;
        }

        public short GetDelay(Expression.Id exp, short frame)
        {
            if (_expressions.ContainsKey(exp) && _expressions[exp].ContainsKey(frame))
                return _expressions[exp][frame].delay;
            return 0;
        }

        public void Draw(Expression.Id exp, short frame, DrawArgument args)
        {
            if (_expressions.ContainsKey(exp) && _expressions[exp].ContainsKey(frame))
                _expressions[exp][frame].Tex.Draw(args);
        }

        public Face(int faceId)
        {
            _expressions = new Dictionary<Expression.Id, Dictionary<short, Frame>>();
            var wzObject = (WzDirectory) Wz.Character["face"];
            var faceNode = (WzImage) wzObject[$"000{faceId}.img"];
            foreach (var keyValuePair in Expression.Names)
            {
                var exp = keyValuePair.Value;
                if (exp == Expression.Id.Default)
                {
                    if (!_expressions.ContainsKey(Expression.Id.Default))
                        _expressions[Expression.Id.Default] = new Dictionary<short, Frame>();
                    _expressions[Expression.Id.Default][0] = new Frame(faceNode["default"]);
                }
                else
                {
                    var expNode = faceNode[keyValuePair.Key];
                    for (short frame = 0; frame < expNode.WzProperties.Count; frame++)
                    {
                        var frameNode = expNode[$"{frame}"];
                        if (frameNode != null)
                        {
                            if (!_expressions.ContainsKey(exp))
                                _expressions[exp] = new Dictionary<short, Frame>();
                            _expressions[exp][frame] = new Frame(frameNode);
                        }
                    }
                }
            }

            Name = (string) Wz.String["Eqp.img"]["Eqp"]["Face"][$"{faceId}"]["name"].WzValue;
        }


        private class Frame
        {
            public readonly TextureD Tex;

            public readonly short delay;

            public Frame(WzObject src)
            {
                var shift = (((WzVectorProperty) src["face"]["map"]["brow"]).Pos());
                Tex = new TextureD(src["Face"]).Shift(-shift);
                delay = (short) (((WzIntProperty) src["delay"])?.Value ?? 0);
                if (delay == 0)
                    delay = 2500;
            }
        }

        public void Dispose()
        {
            _expressions = null;
        }
    }

    public static class Expression
    {
        public enum Id : short
        {
            Default, // 默认
            Blink, // 眨眼(动画)
            Hit, // 受伤 F1
            Smile, // 大笑 F2
            Troubled, // 无趣 F3 
            Cry, // 哭 F4
            Angry, // 愤怒 F5
            Bewildered, // 懵逼 F6
            Stunned, // 无语 F7
            Blaze, // 极其愤怒
            Bowing, // 打瞌睡
            Cheers, // 干杯
            Chu, // 亲亲
            Dam, // 流口水
            Despair, // 绝望
            Glitter, // 亮晶晶
            Hot, // 喷火
            Hum, // 无语
            Love, // 爱慕
            Oops, // 哦~
            Pain, // 疼哭
            Shine, // 贪财
            Vomit, // 口区
            Wink, // 眨巴眼
            Length
        }

        public static Id ByAction(int action)
        {
            action -= 98;
            if (action < (int) Id.Length)
                return (Id) action;
            return Id.Default;
        }

        internal static readonly Dictionary<string, Id> Names = new Dictionary<string, Id>();

        static Expression()
        {
            var arr = new[]
            {
                "default", "blink", "hit", "smile", "troubled", "cry",
                "angry", "bewildered", "stunned", "blaze", "bowing", "cheers",
                "chu", "dam", "despair", "glitter", "hot", "hum",
                "love", "oops", "pain", "shine", "vomit", "wink"
            };
            for (short i = 0; i < arr.Length; i++)
                Names[arr[i]] = (Id) i;
        }
    }
}