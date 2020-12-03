﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Character.Core.Character.Look;
using Character.Core.Data;

namespace Character.Core.Util
{
    public static class GameUtil
    {
        #region 装扮

        private static readonly Dictionary<int, Clothing> ClothCache = new Dictionary<int, Clothing>();

        public static Clothing GetCloth(int clothId, BodyDrawInfo drawInfo)
        {
            if (!ClothCache.ContainsKey(clothId))
                ClothCache[clothId] = new Clothing(clothId, drawInfo);
            return ClothCache[clothId];
        }

        #endregion

        #region 身体

        private static readonly Dictionary<int, Body> BodyCache = new Dictionary<int, Body>();

        public static Body GetBody(int bodyId, BodyDrawInfo drawInfo)
        {
            if (!BodyCache.ContainsKey(bodyId))
                BodyCache[bodyId] = new Body(bodyId, drawInfo);
            return BodyCache[bodyId];
        }

        #endregion

        #region 头发

        private static readonly Dictionary<int, Hair> HairCache = new Dictionary<int, Hair>();

        public static Hair GetHair(int hairId, BodyDrawInfo drawInfo)
        {
            if (!HairCache.ContainsKey(hairId))
                HairCache[hairId] = new Hair(hairId, drawInfo);
            return HairCache[hairId];
        }

        #endregion

        #region 脸型

        private static readonly Dictionary<int, Face> FaceCache = new Dictionary<int, Face>();

        public static Face GetFace(int faceId)
        {
            if (!FaceCache.ContainsKey(faceId))
                FaceCache[faceId] = new Face(faceId);
            return FaceCache[faceId];
        }

        #endregion

        #region 武器

        private static readonly Dictionary<int, WeaponData> WeaponDataDictionary = new Dictionary<int, WeaponData>();

        public static WeaponData GetWeaponData(int weaponId)
        {
            if (!WeaponDataDictionary.ContainsKey(weaponId))
                WeaponDataDictionary[weaponId] = new WeaponData(weaponId);
            return WeaponDataDictionary[weaponId];
        }

        #endregion

        #region 物品

        private static readonly Dictionary<int, EquipData> EquipDataDictionary = new Dictionary<int, EquipData>();

        public static EquipData GetEquipData(int itemId)
        {
            if (!EquipDataDictionary.ContainsKey(itemId))
                EquipDataDictionary[itemId] = new EquipData(itemId);
            return EquipDataDictionary[itemId];
        }

        #endregion

        #region 技能数据

        private static readonly Dictionary<int, SkillData> SkillDataDictionary = new Dictionary<int, SkillData>();

        public static SkillData GetSkillData(int skillId)
        {
            if (!SkillDataDictionary.ContainsKey(skillId))
                SkillDataDictionary[skillId] = new SkillData(skillId);
            return SkillDataDictionary[skillId];
        }

        #endregion

        /**
         * 全局精灵处理
         */
        public static SpriteBatch SpriteBatch;

        /**
         * 游戏每步ms
         */
        public const short TimeStep = 8;

        /*
         * 图形设备管理器
         */
        public static GraphicsDeviceManager Graphics;

        /*
         * {
			{ 0.00f, 0.00f, 0.00f }, // Black
			{ 1.00f, 1.00f, 1.00f }, // White
			{ 1.00f, 1.00f, 0.00f }, // Yellow
			{ 0.00f, 0.00f, 1.00f }, // Blue
			{ 1.00f, 0.00f, 0.00f }, // Red
			{ 0.80f, 0.30f, 0.30f }, // DarkRed
			{ 0.50f, 0.25f, 0.00f }, // Brown
			{ 0.34f, 0.20f, 0.07f }, // Jambalaya
			{ 0.50f, 0.50f, 0.50f }, // Lightgrey
			{ 0.25f, 0.25f, 0.25f }, // Darkgrey
			{ 1.00f, 0.50f, 0.00f }, // Orange
			{ 0.00f, 0.75f, 1.00f }, // Mediumblue
			{ 0.50f, 0.00f, 0.50f }, // Violet
			{ 0.47f, 0.40f, 0.27f }, // Tobacco Brown
			{ 0.74f, 0.74f, 0.67f }, // Eagle
			{ 0.60f, 0.60f, 0.54f }, // Lemon Grass
			{ 0.20f, 0.20f, 0.27f }, // Tuna
			{ 0.94f, 0.94f, 0.94f }, // Gallery
			{ 0.60f, 0.60f, 0.60f }, // Dusty Gray
			{ 0.34f, 0.34f, 0.34f }, // Emperor
			{ 0.20f, 0.20f, 0.20f }, // Mine Shaft
			{ 1.00f, 1.00f, 0.87f }, // Half and Half
			{ 0.00f, 0.40f, 0.67f }, // Endeavour
			{ 0.30f, 0.20f, 0.10f }, // Brown Derby
			{ 0.94f, 0.95f, 0.95f }, // Porcelain
			{ 0.34f, 0.27f, 0.14f }, // Irish Coffee
			{ 0.47f, 0.47f, 0.47f }, // Boulder
			{ 0.00f, 0.75f, 0.00f }, // Green (Mob HP Bar)
			{ 0.00f, 1.00f, 0.00f }, // Light Green (Mob HP Bar)
			{ 0.00f, 0.50f, 0.00f }, // Japanese Laurel (Mob HP Bar)
			{ 0.67f, 0.67f, 0.60f }, // Gray Olive
			{ 0.80f, 1.00f, 0.00f }, // Electric Lime
			{ 1.00f, 0.80f, 0.00f }, // Supernova
			{ 0.47f, 1.00f, 0.00f }, // Chartreuse
			{ 0.47f, 0.80f, 1.00f }, // Malibu
			{ 0.67f, 0.67f, 0.67f }, // Silver Chalice
			{ 0.54f, 0.54f, 0.54f }, // Gray
			{ 0.94f, 0.00f, 0.20f }  // Torch Red
		};
         */
    }
}