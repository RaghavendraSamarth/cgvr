using UnityEngine;
using LumiLab.Core;
using LumiLab.Graphics;

namespace LumiLab.Lamps
{
    public abstract class LampBase : MonoBehaviour
    {
        [Header("Lamp Components")]
        [SerializeField] protected Transform _baseTransform;
        [SerializeField] protected Transform _standTransform;
        [SerializeField] protected Transform _shadeTransform;
        [SerializeField] protected Transform _bulbTransform;

        [SerializeField] protected MeshRenderer _baseRenderer;
        [SerializeField] protected MeshRenderer _standRenderer;
        [SerializeField] protected MeshRenderer _shadeRenderer;
        [SerializeField] protected MeshRenderer _bulbRenderer;

        [Header("Lighting")]
        [SerializeField] protected Light _mainLight;

        protected LampConfigData _currentConfig;

        protected Vector3 _initialBaseScale = new Vector3(0.28f, 0.02f, 0.28f);
        protected Vector3 _initialStandScale = new Vector3(0.04f, 0.35f, 0.04f);
        protected Vector3 _initialShadeScale = Vector3.one;
        protected bool _scalesCached = false;

        public virtual void Initialize()
        {
            CacheScales();
        }

        protected void CacheScales()
        {
            if (_scalesCached) return;
            if (_baseTransform != null && _baseTransform.localScale != Vector3.one)
                _initialBaseScale = _baseTransform.localScale;
            if (_standTransform != null && _standTransform.localScale != Vector3.one)
                _initialStandScale = _standTransform.localScale;
            if (_shadeTransform != null && _shadeTransform.localScale != Vector3.one)
                _initialShadeScale = _shadeTransform.localScale;
            _scalesCached = true;
        }

        public virtual void ApplyConfig(LampConfigData config)
        {
            _currentConfig = config;
            UpdateDimensions(config.heightScale, config.baseWidthScale, config.shadeWidthScale, config.shadeHeightScale, config.shadeStyle);
            UpdateMaterials(config.baseMaterial, config.baseColor, config.standMaterial, config.standColor, config.shadeMaterial, config.shadeColor);
            UpdateLighting(config.isLightOn, config.brightness, GetEffectiveLightColor(config));
        }

        public virtual void UpdateDimensions(float heightScale, float baseWidthScale, float shadeWidthScale, float shadeHeightScale, int shadeStyle)
        {
            CacheScales();

            // Proportional layout scaling relative to initial designed dimensions
            if (_baseTransform != null)
            {
                _baseTransform.localScale = new Vector3(
                    _initialBaseScale.x * baseWidthScale,
                    _initialBaseScale.y,
                    _initialBaseScale.z * baseWidthScale
                );
            }

            if (_standTransform != null)
            {
                _standTransform.localScale = new Vector3(
                    _initialStandScale.x * baseWidthScale,
                    _initialStandScale.y * heightScale,
                    _initialStandScale.z * baseWidthScale
                );
            }

            if (_shadeTransform != null)
            {
                _shadeTransform.localScale = new Vector3(
                    _initialShadeScale.x * shadeWidthScale,
                    _initialShadeScale.y * shadeHeightScale,
                    _initialShadeScale.z * shadeWidthScale
                );
                // Adjust shade Y position smoothly based on stand height
                float standTop = GetStandTopY(heightScale);
                _shadeTransform.localPosition = new Vector3(_shadeTransform.localPosition.x, standTop, _shadeTransform.localPosition.z);
            }

            if (_bulbTransform != null && _shadeTransform != null)
            {
                _bulbTransform.localPosition = _shadeTransform.localPosition + new Vector3(0, -0.05f * shadeHeightScale, 0);
            }

            if (_mainLight != null && _bulbTransform != null)
            {
                _mainLight.transform.position = _bulbTransform.position;
            }
        }

        protected virtual float GetStandTopY(float heightScale)
        {
            return 0.5f * heightScale;
        }

        public virtual void UpdateMaterials(MaterialType baseMat, Color baseCol, MaterialType standMat, Color standCol, MaterialType shadeMat, Color shadeCol)
        {
            if (MaterialManager.Instance == null) return;

            if (_baseRenderer != null)
            {
                _baseRenderer.material = MaterialManager.Instance.CreateMaterialInstance(baseMat, baseCol);
            }

            if (_standRenderer != null)
            {
                _standRenderer.material = MaterialManager.Instance.CreateMaterialInstance(standMat, standCol);
            }

            if (_shadeRenderer != null)
            {
                _shadeRenderer.material = MaterialManager.Instance.CreateMaterialInstance(shadeMat, shadeCol);
            }
        }

        public virtual void UpdateLighting(bool isOn, float brightness, Color lightColor)
        {
            if (_mainLight != null)
            {
                _mainLight.enabled = isOn;
                _mainLight.color = lightColor;
                _mainLight.intensity = isOn ? brightness * GetBaseLightIntensity() : 0f;
            }

            if (_bulbRenderer != null && MaterialManager.Instance != null)
            {
                if (isOn)
                {
                    _bulbRenderer.material = MaterialManager.Instance.CreateBulbMaterial(lightColor, brightness);
                }
                else
                {
                    _bulbRenderer.material = MaterialManager.Instance.CreateMaterialInstance(MaterialType.MatteCeramic, new Color(0.2f, 0.2f, 0.2f));
                }
            }
        }

        protected virtual float GetBaseLightIntensity()
        {
            return 2.5f;
        }

        protected Color GetEffectiveLightColor(LampConfigData config)
        {
            switch (config.lightTempMode)
            {
                case LightTempMode.WarmWhite:
                    return ColorPalettePresets.WarmWhiteColor;
                case LightTempMode.NeutralWhite:
                    return ColorPalettePresets.NeutralWhiteColor;
                case LightTempMode.CoolWhite:
                    return ColorPalettePresets.CoolWhiteColor;
                case LightTempMode.CustomRGB:
                    return config.customLightColor;
                default:
                    return ColorPalettePresets.WarmWhiteColor;
            }
        }

        public Light GetMainLight() => _mainLight;
        public Transform GetCenterTransform() => _shadeTransform != null ? _shadeTransform : transform;
    }
}
