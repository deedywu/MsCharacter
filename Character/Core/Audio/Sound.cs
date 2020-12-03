using System;
using System.Collections.Generic;
using System.IO;
using Character.Core.Util;
using Character.MapleLib.WzLib;
using Character.MapleLib.WzLib.WzProperties;
using Microsoft.Xna.Framework.Audio;

namespace Character.Core.Audio
{

    /// <summary>
    /// 声音相关类
    /// </summary>
    public class Sound  
    {
        #region Sound.Name 枚举

        /// <summary>
        /// 游戏内经常用到的音效
        /// </summary>
        public enum Name
        {
            // UI
            ButtonClick,
            ButtonOver,
            CharSelect,
            DlgNotice,
            MenuDown,
            MenuUp,
            RaceSelect,
            ScrollUp,
            SelectMap,
            Tab,
            WorldSelect,
            DragStart,
            DragEnd,
            WorldMapOpen,
            WorldMapClose,

            // Login
            GameStart,

            // Game
            Jump,
            Drop,
            Pickup,
            Portal,
            LevelUp,
            Tombstone,
            Length
        }

        #endregion

        private readonly Bgm _bgm;
        private readonly Sfx _sfx;

        /// <summary>
        /// 播放 背景音乐
        /// </summary>
        /// <param name="path">Sound.wz中路径</param>
        /// <param name="looping">是否循环</param>
        public void Play(string path, bool looping = true)
        {
            _bgm.Play(path, looping);
        }

        /// <summary>
        /// 是否播放
        /// </summary>
        /// <returns>t/f</returns>
        public bool IsPlaying() => _bgm.IsPlaying();

        /// <summary>
        /// 背景音乐时长
        /// </summary>
        /// <returns>毫秒</returns>
        public int Time() => _bgm.Time();

        /// <summary>
        /// 播放 音效
        /// </summary>
        /// <param name="name">Sfx.Name 枚举</param>
        public void Play(Name name)
        {
            _sfx.Play(name);
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="itemId">物品ID</param>
        public void Play(int itemId)
        {
            _sfx.Play(itemId);
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="src">wz节点</param>
        public void Play(WzObject src)
        {
            _sfx.Play(src);
        }

        private Sound()
        {
            _bgm = new Bgm();
            _sfx = new Sfx();
        }

        private static Sound Instance;
        
        private static object locker = new object();

        public   static Sound get()
        {
            if (Instance != null) return Instance;
            lock (locker) Instance ??= new Sound();
            return Instance;
        }

        public void Dispose()
        {
            _bgm.Dispose();
            _sfx.Dispose();
        }

        /// <summary>
        /// 播放背景音乐的类
        /// </summary>
        private class Bgm : IDisposable
        {
            private string _path;

            private SoundEffectInstance _player;

            private int _time;

            private byte _bgmVolume = 100;

            private bool _isPlaying;

            /// <summary>
            /// 是否正在播放
            /// </summary>
            /// <returns>是否</returns>
            internal bool IsPlaying() => _isPlaying;

            /// <summary>
            /// 时长
            /// </summary>
            /// <returns>毫秒</returns>
            internal int Time() => _time;

            /// <summary>
            /// 播放
            /// </summary>
            /// <param name="path">Sound.wz内的路径</param>
            /// <param name="looping">是否循环</param>
            internal void Play(string path, bool looping)
            {
                if (path.Equals(_path)) return;
                if (!path.Equals(_path))
                { 
                    if (_bgmVolume == 0) return;
                    var strings = path.Split("/");
                    var wzObj = Wz.Sound[strings[0]];
                    for (var i = 1; i < strings.Length; i++) wzObj = wzObj[strings[i]];
                    var wzSound = (WzSoundProperty) wzObj.GetByUol();
                    _time = wzSound.Time();
                    _player?.Dispose();
                    using var stream = new MemoryStream(wzSound.Wav());
                    var se = SoundEffect.FromStream(stream);
                    stream.Dispose();
                    _player = se.CreateInstance();
                    _path = path;
                    _isPlaying = false;
                }

                _player.Play();
                _isPlaying = true;
                _player.IsLooped = looping;
            }

            public void Dispose()
            {
                _player?.Dispose();
            }
        }


        /// <summary>
        /// 播放音乐特效的类
        /// </summary>
        private class Sfx : IDisposable
        {
            private readonly Dictionary<string, SoundEffectInstance> _samples =
                new Dictionary<string, SoundEffectInstance>();

            private readonly Dictionary<Name, string> _soundIds = new Dictionary<Name, string>();

            private readonly Dictionary<string, string> _itemIds = new Dictionary<string, string>();

            private byte _sfxVolume = 100;

            /// <summary>
            /// 播放 音效
            /// </summary>
            /// <param name="name">Sfx.Name 枚举</param>
            internal void Play(Name name)
            {
                var id = _soundIds[name];
                if (id != null) PlaySfx(id);
            }

            /// <summary>
            /// 播放音效
            /// </summary>
            /// <param name="itemId">物品ID</param>
            internal void Play(int itemId)
            {
                var formatId = itemId.ToString().PadLeft(8, '0');

                string id;
                if (_itemIds.ContainsKey(formatId))
                {
                    id = _itemIds[formatId];
                }
                else
                {
                    var pid = (int) (10000 * ((float) itemId / 10000));
                    var fpId = pid.ToString().PadLeft(8, '0');
                    id = _itemIds.ContainsKey(fpId) ? _itemIds[fpId] : _itemIds["02000000"];
                }

                if (id != null) PlaySfx(id);
            }

            /// <summary>
            /// 播放音效
            /// </summary>
            /// <param name="src">wz节点</param>
            internal void Play(WzObject src)
            {
                var id = AddSound(src);
                if (id != null) PlaySfx(id);
            }
 
            private void PlaySfx(string id)
            {
                if (!_samples.ContainsKey(id)) return;
                var sample = _samples[id]; 
                sample.Volume = _sfxVolume * 0.01f;
                sample.Play();
            }

            private string AddSound(WzObject src)
            {
                var ad = (WzSoundProperty) src;
                var data = ad.Wav();
                if (!(data?.Length > 0)) return null;
                var id = ad.FullPath;
                if (_samples.ContainsKey(id)) return id;
                using var stream = new MemoryStream(data);
                var se = SoundEffect.FromStream(stream);
                _samples[id] = se.CreateInstance();
                return id;
            }

            private void AddSound(Name name, WzObject src)
            {
                var id = AddSound(src);
                if (id != null) _soundIds[name] = id;
            }

            private void AddSound(string itemId, WzObject src)
            {
                var id = AddSound(src);
                if (id != null) _itemIds[itemId] = id;
            }

            internal Sfx()
            {
                var uiSrc = Wz.Sound["UI.img"];
                AddSound(Name.ButtonClick, uiSrc["BtMouseClick"]);
                AddSound(Name.ButtonOver, uiSrc["BtMouseOver"]);
                AddSound(Name.CharSelect, uiSrc["CharSelect"]);
                AddSound(Name.DlgNotice, uiSrc["DlgNotice"]);
                AddSound(Name.MenuDown, uiSrc["MenuDown"]);
                AddSound(Name.MenuUp, uiSrc["MenuUp"]);
                AddSound(Name.RaceSelect, uiSrc["RaceSelect"]);
                AddSound(Name.ScrollUp, uiSrc["ScrollUp"]);
                AddSound(Name.SelectMap, uiSrc["SelectMap"]);
                AddSound(Name.Tab, uiSrc["Tab"]);
                AddSound(Name.WorldSelect, uiSrc["WorldSelect"]);
                AddSound(Name.DragStart, uiSrc["DragStart"]);
                AddSound(Name.DragEnd, uiSrc["DragEnd"]);
                AddSound(Name.WorldMapOpen, uiSrc["WorldmapOpen"]);
                AddSound(Name.WorldMapClose, uiSrc["WorldmapClose"]);
                var gameSrc = Wz.Sound["Game.img"];
                AddSound(Name.GameStart, gameSrc["GameIn"]);
                AddSound(Name.Jump, gameSrc["Jump"]);
                AddSound(Name.Drop, gameSrc["DropItem"]);
                AddSound(Name.Pickup, gameSrc["PickUpItem"]);
                AddSound(Name.Portal, gameSrc["Portal"]);
                AddSound(Name.LevelUp, gameSrc["LevelUp"]);
                AddSound(Name.Tombstone, gameSrc["Tombstone"]);
                var itemSrc = (WzImage) Wz.Sound["Item.img"];
                foreach (var node in itemSrc.WzProperties)
                {
                    WzSoundProperty soundProperty;
                    if ((node["Use"] is WzSoundProperty s1))
                        soundProperty = s1;
                    else
                        soundProperty = node["Use"].WzValue switch
                        {
                            WzSoundProperty s2 => s2,
                            WzSubProperty s3 => (WzSoundProperty) s3["Use"],
                            _ => null
                        };
                    if (soundProperty != null) AddSound(node.Name, soundProperty);
                }
            }

            public void Dispose()
            {
                foreach (var soundEffectInstance in _samples) soundEffectInstance.Value.Dispose();
            }
        }
    }
}