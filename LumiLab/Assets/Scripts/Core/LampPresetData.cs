using System;
using UnityEngine;

namespace LumiLab.Core
{
    public enum MaterialType
    {
        BrushedMetal = 0,
        PolishedMetal = 1,
        MatteCeramic = 2,
        Wood = 3,
        FrostedGlass = 4,
        Fabric = 5
    }

    public enum LightTempMode
    {
        WarmWhite = 0,    // 2700K
        NeutralWhite = 1, // 4000K
        CoolWhite = 2,    // 6500K
        CustomRGB = 3
    }

    [Serializable]
    public class LampConfigData
    {
        public int lampTypeIndex = 0; // 0: Aura, 1: Nordic, 2: Industrial, 3: Halo

        // Dimensions
        public float heightScale = 1.0f;
        public float baseWidthScale = 1.0f;
        public float shadeWidthScale = 1.0f;
        public float shadeHeightScale = 1.0f;
        public int shadeStyle = 0;

        // Materials
        public MaterialType baseMaterial = MaterialType.BrushedMetal;
        public MaterialType standMaterial = MaterialType.PolishedMetal;
        public MaterialType shadeMaterial = MaterialType.Fabric;

        // Colors
        public Color baseColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        public Color standColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        public Color shadeColor = new Color(0.95f, 0.93f, 0.88f, 1f);

        // Lighting
        public bool isLightOn = true;
        public float brightness = 1.0f;
        public LightTempMode lightTempMode = LightTempMode.WarmWhite;
        public Color customLightColor = new Color(1.0f, 0.85f, 0.65f, 1f);

        public LampConfigData Clone()
        {
            return (LampConfigData)this.MemberwiseClone();
        }
    }

    public static class ColorPalettePresets
    {
        // Light color temperatures
        public static readonly Color WarmWhiteColor = new Color(1.0f, 0.77f, 0.56f, 1.0f);     // 2700K
        public static readonly Color NeutralWhiteColor = new Color(1.0f, 0.92f, 0.84f, 1.0f);  // 4000K
        public static readonly Color CoolWhiteColor = new Color(0.84f, 0.91f, 1.0f, 1.0f);     // 6500K

        // Curated designer palettes
        public static readonly Color[] CuratedBaseColors = new Color[]
        {
            new Color(0.15f, 0.15f, 0.16f), // Matte Charcoal Black
            new Color(0.85f, 0.70f, 0.40f), // Brushed Brass / Gold
            new Color(0.88f, 0.88f, 0.90f), // Polished Chrome / Platinum
            new Color(0.55f, 0.38f, 0.25f), // Warm Walnut Wood
            new Color(0.92f, 0.90f, 0.86f), // Scandinavian Off-White
            new Color(0.35f, 0.45f, 0.42f)  // Sage Green Ceramic
        };

        public static readonly Color[] CuratedShadeColors = new Color[]
        {
            new Color(0.96f, 0.94f, 0.88f), // Linen Ivory
            new Color(0.12f, 0.12f, 0.13f), // Midnight Matte Black
            new Color(0.82f, 0.68f, 0.45f), // Warm Champagne
            new Color(0.78f, 0.38f, 0.28f), // Terracotta Earth
            new Color(0.30f, 0.42f, 0.52f), // Nordic Slate Blue
            new Color(0.95f, 0.85f, 0.75f)  // Soft Peach Blush
        };
    }
}
