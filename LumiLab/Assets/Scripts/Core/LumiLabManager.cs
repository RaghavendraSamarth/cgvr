using System;
using UnityEngine;
using LumiLab.Lamps;
using LumiLab.Graphics;
using LumiLab.ShowroomCamera;

namespace LumiLab.Core
{
    public class LumiLabManager : MonoBehaviour
    {
        private static LumiLabManager _instance;
        public static LumiLabManager Instance => _instance;

        [Header("Lamp References")]
        [SerializeField] private LampBase[] _lamps; // 0: Aura, 1: Nordic, 2: Industrial, 3: Halo
        private LampConfigData[] _lampConfigs;
        private int _currentLampIndex = 0;

        [Header("Showroom References")]
        [SerializeField] private EnvironmentController _environmentController;
        [SerializeField] private ShowroomCameraController _cameraController;
        [SerializeField] private TechDemoController _techDemoController;

        // Events for UI synchronization
        public event Action<int, LampConfigData> OnLampChanged;
        public event Action<LampConfigData> OnConfigUpdated;
        public event Action<int> OnEnvironmentChanged;

        public int CurrentLampIndex => _currentLampIndex;
        public LampConfigData CurrentConfig => _lampConfigs != null && _lampConfigs.Length > _currentLampIndex ? _lampConfigs[_currentLampIndex] : null;
        public LampBase CurrentLamp => _lamps != null && _lamps.Length > _currentLampIndex ? _lamps[_currentLampIndex] : null;

        private void Awake()
        {
            if (_instance == null) _instance = this;
            else if (_instance != this) Destroy(gameObject);

            InitializeLampConfigs();
        }

        private void Start()
        {
            // Initialize all lamps
            if (_lamps != null)
            {
                for (int i = 0; i < _lamps.Length; i++)
                {
                    if (_lamps[i] != null)
                    {
                        _lamps[i].Initialize();
                        _lamps[i].gameObject.SetActive(false);
                    }
                }
            }

            // Set default environment (Living Room)
            if (_environmentController != null)
            {
                _environmentController.SetEnvironment(0);
            }

            // Select initial lamp
            SelectLamp(0);
        }

        private void InitializeLampConfigs()
        {
            _lampConfigs = new LampConfigData[4];
            for (int i = 0; i < 4; i++)
            {
                _lampConfigs[i] = SaveLoadManager.LoadLampConfig(i);
            }
        }

        public void SelectLamp(int index)
        {
            if (_lamps == null || index < 0 || index >= _lamps.Length) return;

            _currentLampIndex = index;

            for (int i = 0; i < _lamps.Length; i++)
            {
                if (_lamps[i] != null)
                {
                    bool isActive = (i == _currentLampIndex);
                    _lamps[i].gameObject.SetActive(isActive);
                    if (isActive)
                    {
                        MountLampToCurrentEnvironment(_lamps[i]);
                        _lamps[i].ApplyConfig(_lampConfigs[_currentLampIndex]);

                        // Auto-frame camera
                        if (_cameraController != null)
                        {
                            _cameraController.SetTarget(_lamps[i].transform, new Vector3(0, 0.35f, 0));
                        }
                    }
                }
            }

            OnLampChanged?.Invoke(_currentLampIndex, _lampConfigs[_currentLampIndex]);
        }

        public void SetEnvironment(int envIndex)
        {
            if (_environmentController != null)
            {
                _environmentController.SetEnvironment(envIndex);
                if (CurrentLamp != null)
                {
                    MountLampToCurrentEnvironment(CurrentLamp);
                }
                OnEnvironmentChanged?.Invoke(envIndex);
            }
        }

        private void MountLampToCurrentEnvironment(LampBase lamp)
        {
            if (lamp == null || _environmentController == null) return;
            Transform mount = _environmentController.GetCurrentMountPoint();
            if (mount != null)
            {
                lamp.transform.position = mount.position;
                lamp.transform.rotation = mount.rotation;
            }
        }

        #region Customization Methods

        public void SetDimensions(float height, float baseWidth, float shadeWidth, float shadeHeight, int shadeStyle)
        {
            LampConfigData cfg = CurrentConfig;
            if (cfg == null) return;

            cfg.heightScale = height;
            cfg.baseWidthScale = baseWidth;
            cfg.shadeWidthScale = shadeWidth;
            cfg.shadeHeightScale = shadeHeight;
            cfg.shadeStyle = shadeStyle;

            if (CurrentLamp != null)
            {
                CurrentLamp.UpdateDimensions(height, baseWidth, shadeWidth, shadeHeight, shadeStyle);
            }

            OnConfigUpdated?.Invoke(cfg);
        }

        public void SetMaterials(MaterialType baseMat, MaterialType standMat, MaterialType shadeMat)
        {
            LampConfigData cfg = CurrentConfig;
            if (cfg == null) return;

            cfg.baseMaterial = baseMat;
            cfg.standMaterial = standMat;
            cfg.shadeMaterial = shadeMat;

            if (CurrentLamp != null)
            {
                CurrentLamp.UpdateMaterials(cfg.baseMaterial, cfg.baseColor, cfg.standMaterial, cfg.standColor, cfg.shadeMaterial, cfg.shadeColor);
            }

            OnConfigUpdated?.Invoke(cfg);
        }

        public void SetColors(Color baseCol, Color standCol, Color shadeCol)
        {
            LampConfigData cfg = CurrentConfig;
            if (cfg == null) return;

            cfg.baseColor = baseCol;
            cfg.standColor = standCol;
            cfg.shadeColor = shadeCol;

            if (CurrentLamp != null)
            {
                CurrentLamp.UpdateMaterials(cfg.baseMaterial, cfg.baseColor, cfg.standMaterial, cfg.standColor, cfg.shadeMaterial, cfg.shadeColor);
            }

            OnConfigUpdated?.Invoke(cfg);
        }

        public void SetBaseColor(Color col)
        {
            LampConfigData cfg = CurrentConfig;
            if (cfg == null) return;
            cfg.baseColor = col;
            if (CurrentLamp != null)
                CurrentLamp.UpdateMaterials(cfg.baseMaterial, cfg.baseColor, cfg.standMaterial, cfg.standColor, cfg.shadeMaterial, cfg.shadeColor);
            OnConfigUpdated?.Invoke(cfg);
        }

        public void SetStandColor(Color col)
        {
            LampConfigData cfg = CurrentConfig;
            if (cfg == null) return;
            cfg.standColor = col;
            if (CurrentLamp != null)
                CurrentLamp.UpdateMaterials(cfg.baseMaterial, cfg.baseColor, cfg.standMaterial, cfg.standColor, cfg.shadeMaterial, cfg.shadeColor);
            OnConfigUpdated?.Invoke(cfg);
        }

        public void SetShadeColor(Color col)
        {
            LampConfigData cfg = CurrentConfig;
            if (cfg == null) return;
            cfg.shadeColor = col;
            if (CurrentLamp != null)
                CurrentLamp.UpdateMaterials(cfg.baseMaterial, cfg.baseColor, cfg.standMaterial, cfg.standColor, cfg.shadeMaterial, cfg.shadeColor);
            OnConfigUpdated?.Invoke(cfg);
        }

        public void ToggleLight(bool isOn)
        {
            LampConfigData cfg = CurrentConfig;
            if (cfg == null) return;

            cfg.isLightOn = isOn;
            if (CurrentLamp != null)
            {
                CurrentLamp.ApplyConfig(cfg);
            }
            OnConfigUpdated?.Invoke(cfg);
        }

        public void SetBrightness(float brightness)
        {
            LampConfigData cfg = CurrentConfig;
            if (cfg == null) return;

            cfg.brightness = brightness;
            if (CurrentLamp != null)
            {
                CurrentLamp.ApplyConfig(cfg);
            }
            OnConfigUpdated?.Invoke(cfg);
        }

        public void SetLightTemperature(LightTempMode mode)
        {
            LampConfigData cfg = CurrentConfig;
            if (cfg == null) return;

            cfg.lightTempMode = mode;
            if (CurrentLamp != null)
            {
                CurrentLamp.ApplyConfig(cfg);
            }
            OnConfigUpdated?.Invoke(cfg);
        }

        public void SetCustomLightColor(Color col)
        {
            LampConfigData cfg = CurrentConfig;
            if (cfg == null) return;

            cfg.lightTempMode = LightTempMode.CustomRGB;
            cfg.customLightColor = col;
            if (CurrentLamp != null)
            {
                CurrentLamp.ApplyConfig(cfg);
            }
            OnConfigUpdated?.Invoke(cfg);
        }

        public void ApplyPreset(int presetIndex)
        {
            LampConfigData cfg = CurrentConfig;
            if (cfg == null) return;

            switch (presetIndex)
            {
                case 0: // Cozy Evening
                    cfg.baseMaterial = MaterialType.Wood;
                    cfg.standMaterial = MaterialType.BrushedMetal;
                    cfg.shadeMaterial = MaterialType.Fabric;
                    cfg.baseColor = new Color(0.55f, 0.38f, 0.25f);
                    cfg.standColor = new Color(0.85f, 0.70f, 0.40f); // Brass
                    cfg.shadeColor = new Color(0.96f, 0.94f, 0.88f); // Linen
                    cfg.isLightOn = true;
                    cfg.brightness = 1.0f;
                    cfg.lightTempMode = LightTempMode.WarmWhite;
                    break;

                case 1: // Modern Minimal
                    cfg.baseMaterial = MaterialType.MatteCeramic;
                    cfg.standMaterial = MaterialType.BrushedMetal;
                    cfg.shadeMaterial = MaterialType.MatteCeramic;
                    cfg.baseColor = new Color(0.15f, 0.15f, 0.16f); // Matte Charcoal
                    cfg.standColor = new Color(0.88f, 0.88f, 0.90f); // Chrome
                    cfg.shadeColor = new Color(0.95f, 0.95f, 0.95f);
                    cfg.isLightOn = true;
                    cfg.brightness = 1.1f;
                    cfg.lightTempMode = LightTempMode.NeutralWhite;
                    break;

                case 2: // Luxury Gold
                    cfg.baseMaterial = MaterialType.PolishedMetal;
                    cfg.standMaterial = MaterialType.PolishedMetal;
                    cfg.shadeMaterial = MaterialType.Fabric;
                    cfg.baseColor = new Color(0.88f, 0.72f, 0.42f); // Polished Gold
                    cfg.standColor = new Color(0.90f, 0.75f, 0.45f);
                    cfg.shadeColor = new Color(0.82f, 0.68f, 0.45f); // Champagne
                    cfg.isLightOn = true;
                    cfg.brightness = 1.3f;
                    cfg.lightTempMode = LightTempMode.WarmWhite;
                    break;

                case 3: // Industrial Black
                    cfg.baseMaterial = MaterialType.BrushedMetal;
                    cfg.standMaterial = MaterialType.BrushedMetal;
                    cfg.shadeMaterial = MaterialType.PolishedMetal;
                    cfg.baseColor = new Color(0.12f, 0.12f, 0.13f); // Gunmetal
                    cfg.standColor = new Color(0.18f, 0.18f, 0.20f);
                    cfg.shadeColor = new Color(0.15f, 0.15f, 0.16f);
                    cfg.isLightOn = true;
                    cfg.brightness = 1.25f;
                    cfg.lightTempMode = LightTempMode.CoolWhite;
                    break;

                case 4: // Colorful Ambient
                    cfg.baseMaterial = MaterialType.PolishedMetal;
                    cfg.standMaterial = MaterialType.PolishedMetal;
                    cfg.shadeMaterial = MaterialType.FrostedGlass;
                    cfg.baseColor = new Color(0.88f, 0.88f, 0.90f);
                    cfg.standColor = new Color(0.9f, 0.9f, 0.92f);
                    cfg.shadeColor = new Color(0.95f, 0.85f, 0.95f);
                    cfg.isLightOn = true;
                    cfg.brightness = 1.2f;
                    cfg.lightTempMode = LightTempMode.CustomRGB;
                    cfg.customLightColor = new Color(0.95f, 0.35f, 0.75f); // Sunset Magenta
                    break;
            }

            if (CurrentLamp != null)
            {
                CurrentLamp.ApplyConfig(cfg);
            }
            OnConfigUpdated?.Invoke(cfg);
        }

        public void SaveCurrentConfig()
        {
            SaveLoadManager.SaveLampConfig(_currentLampIndex, CurrentConfig);
        }

        public void LoadCurrentConfig()
        {
            _lampConfigs[_currentLampIndex] = SaveLoadManager.LoadLampConfig(_currentLampIndex);
            if (CurrentLamp != null)
            {
                CurrentLamp.ApplyConfig(_lampConfigs[_currentLampIndex]);
            }
            OnConfigUpdated?.Invoke(_lampConfigs[_currentLampIndex]);
        }

        public void ResetFactoryDefaults()
        {
            SaveLoadManager.ResetToFactoryDefaults(_currentLampIndex);
            _lampConfigs[_currentLampIndex] = SaveLoadManager.GetDefaultConfig(_currentLampIndex);
            if (CurrentLamp != null)
            {
                CurrentLamp.ApplyConfig(_lampConfigs[_currentLampIndex]);
            }
            OnConfigUpdated?.Invoke(_lampConfigs[_currentLampIndex]);
        }

        #endregion

        #region React & WebGL Integration Bridge

        public void WebSelectLamp(int index) => SelectLamp(index);
        public void WebSetEnvironment(int index) => SetEnvironment(index);

        public void WebSetDimensions(string jsonPayload)
        {
            try
            {
                WebDimensionsPayload d = JsonUtility.FromJson<WebDimensionsPayload>(jsonPayload);
                if (d != null)
                {
                    SetDimensions(d.height, d.baseWidth, d.shadeWidth, d.shadeHeight, d.style);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[LumiLab] WebSetDimensions error: {ex.Message}");
            }
        }

        public void WebSetMaterials(string jsonPayload)
        {
            try
            {
                WebMaterialsPayload m = JsonUtility.FromJson<WebMaterialsPayload>(jsonPayload);
                if (m != null)
                {
                    SetMaterials((MaterialType)m.baseMat, (MaterialType)m.standMat, (MaterialType)m.shadeMat);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[LumiLab] WebSetMaterials error: {ex.Message}");
            }
        }

        public void WebSetColor(string jsonPayload)
        {
            try
            {
                WebColorPayload c = JsonUtility.FromJson<WebColorPayload>(jsonPayload);
                if (c != null)
                {
                    Color col = new Color(c.r, c.g, c.b, 1.0f);
                    if (c.target == "base") SetBaseColor(col);
                    else if (c.target == "stand") SetStandColor(col);
                    else SetShadeColor(col);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[LumiLab] WebSetColor error: {ex.Message}");
            }
        }

        public void WebToggleLight(int isOn) => ToggleLight(isOn != 0);
        public void WebSetBrightness(float b) => SetBrightness(b);
        public void WebSetLightTemperature(int mode) => SetLightTemperature((LightTempMode)mode);
        public void WebApplyPreset(int p) => ApplyPreset(p);

        public void WebSetCameraView(string view)
        {
            if (_cameraController == null) return;
            switch (view.ToLower())
            {
                case "front": _cameraController.PresetFront(); break;
                case "side": _cameraController.PresetSide(); break;
                case "top": _cameraController.PresetTop(); break;
                case "34":
                case "threequarter": _cameraController.PresetThreeQuarter(); break;
                case "reset": _cameraController.ResetView(); break;
            }
        }

        public void WebToggleAutoRotate(int on)
        {
            if (_cameraController != null) _cameraController.AutoRotate = (on != 0);
        }

        public void WebSaveConfig() => SaveCurrentConfig();
        public void WebLoadConfig() => LoadCurrentConfig();
        public void WebResetDefaults() => ResetFactoryDefaults();

        #endregion
    }

    [Serializable]
    public class WebDimensionsPayload
    {
        public float height = 1.0f;
        public float baseWidth = 1.0f;
        public float shadeWidth = 1.0f;
        public float shadeHeight = 1.0f;
        public int style = 0;
    }

    [Serializable]
    public class WebMaterialsPayload
    {
        public int baseMat = 0;
        public int standMat = 1;
        public int shadeMat = 5;
    }

    [Serializable]
    public class WebColorPayload
    {
        public string target = "shade";
        public float r = 1.0f;
        public float g = 1.0f;
        public float b = 1.0f;
    }
}
