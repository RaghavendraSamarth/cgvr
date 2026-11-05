using UnityEngine;
using LumiLab.Core;
using LumiLab.Graphics;

namespace LumiLab.Lamps
{
    public class LampIndustrialEdge : LampBase
    {
        [Header("Industrial Articulation")]
        [SerializeField] private Transform _armBasePivot;
        [SerializeField] private Transform _elbowPivot;
        [SerializeField] private Transform _headPivot;

        [SerializeField] private Light _spotLight;

        public override void Initialize()
        {
            base.Initialize();
            // Default angled posture
            if (_armBasePivot != null) _armBasePivot.localEulerAngles = new Vector3(0, 0, 18f);
            if (_elbowPivot != null) _elbowPivot.localEulerAngles = new Vector3(0, 0, -42f);
            if (_headPivot != null) _headPivot.localEulerAngles = new Vector3(30f, 0, 10f);
        }

        public void SetArmAngles(float baseAngle, float elbowAngle, float headAngle)
        {
            if (_armBasePivot != null) _armBasePivot.localEulerAngles = new Vector3(0, 0, baseAngle);
            if (_elbowPivot != null) _elbowPivot.localEulerAngles = new Vector3(0, 0, elbowAngle);
            if (_headPivot != null) _headPivot.localEulerAngles = new Vector3(headAngle, 0, 0);
        }

        public override void UpdateDimensions(float heightScale, float baseWidthScale, float shadeWidthScale, float shadeHeightScale, int shadeStyle)
        {
            CacheScales();

            if (_baseTransform != null)
            {
                _baseTransform.localScale = new Vector3(
                    _initialBaseScale.x * baseWidthScale,
                    _initialBaseScale.y,
                    _initialBaseScale.z * baseWidthScale
                );
            }

            if (_armBasePivot != null)
            {
                _armBasePivot.localScale = new Vector3(1.0f, heightScale, 1.0f);
            }

            if (_shadeTransform != null)
            {
                _shadeTransform.localScale = new Vector3(shadeWidthScale, shadeHeightScale, shadeWidthScale);
            }
        }

        public override void UpdateLighting(bool isOn, float brightness, Color lightColor)
        {
            base.UpdateLighting(isOn, brightness, lightColor);

            if (_spotLight != null)
            {
                _spotLight.enabled = isOn;
                _spotLight.color = lightColor;
                _spotLight.intensity = isOn ? brightness * 4.0f : 0f;
            }
        }

        protected override float GetBaseLightIntensity()
        {
            return 1.8f;
        }
    }
}
