using System;
using UnityEngine;

namespace LumiLab.Core
{
    public static class SaveLoadManager
    {
        private const string SAVE_KEY_PREFIX = "LumiLab_LampConfig_";

        public static void SaveLampConfig(int lampIndex, LampConfigData config)
        {
            if (config == null) return;
            try
            {
                string json = JsonUtility.ToJson(config);
                PlayerPrefs.SetString(SAVE_KEY_PREFIX + lampIndex, json);
                PlayerPrefs.Save();
                Debug.Log($"[LumiLab] Saved configuration for lamp {lampIndex} successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LumiLab] Failed to save config for lamp {lampIndex}: {ex.Message}");
            }
        }

        public static LampConfigData LoadLampConfig(int lampIndex)
        {
            string key = SAVE_KEY_PREFIX + lampIndex;
            if (PlayerPrefs.HasKey(key))
            {
                try
                {
                    string json = PlayerPrefs.GetString(key);
                    LampConfigData loaded = JsonUtility.FromJson<LampConfigData>(json);
                    if (loaded != null)
                    {
                        Debug.Log($"[LumiLab] Loaded saved configuration for lamp {lampIndex}.");
                        return loaded;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[LumiLab] Corrupted save data for lamp {lampIndex}, reverting to default: {ex.Message}");
                }
            }

            return GetDefaultConfig(lampIndex);
        }

        public static void ResetToFactoryDefaults(int lampIndex)
        {
            string key = SAVE_KEY_PREFIX + lampIndex;
            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.DeleteKey(key);
                PlayerPrefs.Save();
            }
            Debug.Log($"[LumiLab] Factory defaults restored for lamp {lampIndex}.");
        }

        public static LampConfigData GetDefaultConfig(int lampIndex)
        {
            LampConfigData config = new LampConfigData { lampTypeIndex = lampIndex };
            switch (lampIndex)
            {
                case 0: // Aura Modern
                    config.heightScale = 1.0f;
                    config.baseWidthScale = 1.0f;
                    config.shadeWidthScale = 1.0f;
                    config.shadeHeightScale = 1.0f;
                    config.shadeStyle = 0; // Tapered
                    config.baseMaterial = MaterialType.BrushedMetal;
                    config.standMaterial = MaterialType.PolishedMetal;
                    config.shadeMaterial = MaterialType.Fabric;
                    config.baseColor = new Color(0.85f, 0.70f, 0.40f); // Warm Brass
                    config.standColor = new Color(0.9f, 0.75f, 0.45f);
                    config.shadeColor = new Color(0.96f, 0.94f, 0.88f); // Linen
                    config.brightness = 1.0f;
                    config.lightTempMode = LightTempMode.WarmWhite;
                    break;

                case 1: // Nordic Glow
                    config.heightScale = 0.95f;
                    config.baseWidthScale = 1.1f;
                    config.shadeWidthScale = 1.05f;
                    config.shadeHeightScale = 0.9f;
                    config.shadeStyle = 1; // Cylindrical / Dome
                    config.baseMaterial = MaterialType.Wood;
                    config.standMaterial = MaterialType.MatteCeramic;
                    config.shadeMaterial = MaterialType.Fabric;
                    config.baseColor = new Color(0.55f, 0.38f, 0.25f); // Walnut
                    config.standColor = new Color(0.92f, 0.90f, 0.86f); // Matte Cream
                    config.shadeColor = new Color(0.98f, 0.96f, 0.92f);
                    config.brightness = 0.9f;
                    config.lightTempMode = LightTempMode.WarmWhite;
                    break;

                case 2: // Industrial Edge
                    config.heightScale = 1.15f;
                    config.baseWidthScale = 1.0f;
                    config.shadeWidthScale = 0.9f;
                    config.shadeHeightScale = 1.0f;
                    config.shadeStyle = 0;
                    config.baseMaterial = MaterialType.BrushedMetal;
                    config.standMaterial = MaterialType.BrushedMetal;
                    config.shadeMaterial = MaterialType.PolishedMetal;
                    config.baseColor = new Color(0.18f, 0.18f, 0.19f); // Matte Gunmetal
                    config.standColor = new Color(0.22f, 0.22f, 0.24f);
                    config.shadeColor = new Color(0.15f, 0.15f, 0.16f);
                    config.brightness = 1.2f;
                    config.lightTempMode = LightTempMode.NeutralWhite;
                    break;

                case 3: // Halo Contemporary
                    config.heightScale = 1.0f;
                    config.baseWidthScale = 0.95f;
                    config.shadeWidthScale = 1.1f;
                    config.shadeHeightScale = 1.0f;
                    config.shadeStyle = 0;
                    config.baseMaterial = MaterialType.PolishedMetal;
                    config.standMaterial = MaterialType.PolishedMetal;
                    config.shadeMaterial = MaterialType.FrostedGlass;
                    config.baseColor = new Color(0.88f, 0.88f, 0.90f); // Polished Chrome
                    config.standColor = new Color(0.85f, 0.85f, 0.88f);
                    config.shadeColor = new Color(0.98f, 0.98f, 1.0f); // Halo Glass
                    config.brightness = 1.1f;
                    config.lightTempMode = LightTempMode.CoolWhite;
                    break;
            }
            return config;
        }
    }
}
