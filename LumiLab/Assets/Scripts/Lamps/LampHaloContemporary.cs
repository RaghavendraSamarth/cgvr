using UnityEngine;
using LumiLab.Core;
using LumiLab.Graphics;

namespace LumiLab.Lamps
{
    public class LampHaloContemporary : LampBase
    {
        [Header("Halo Geometry")]
        [SerializeField] private MeshFilter _haloRingFilter;
        [SerializeField] private MeshRenderer _haloRingRenderer;

        public override void Initialize()
        {
            base.Initialize();
            BuildProceduralHalo();
        }

        private void BuildProceduralHalo()
        {
            if (_haloRingFilter != null && _haloRingFilter.sharedMesh == null)
            {
                // Procedural Torus Ring
                Mesh torusMesh = ProceduralMeshUtility.CreateTorus(0.22f, 0.022f, 48, 16, "HaloContemporary_Torus");
                _haloRingFilter.sharedMesh = torusMesh;
            }
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
                _shadeTransform.localScale = new Vector3(shadeWidthScale, shadeHeightScale, shadeWidthScale);
                float standTop = GetStandTopY(heightScale);
                _shadeTransform.localPosition = new Vector3(0, standTop + 0.22f * shadeHeightScale, 0);
            }

            if (_mainLight != null && _shadeTransform != null)
            {
                _mainLight.transform.position = _shadeTransform.position;
            }
        }

        protected override float GetStandTopY(float heightScale)
        {
            return 0.20f * heightScale;
        }

        public override void UpdateLighting(bool isOn, float brightness, Color lightColor)
        {
            base.UpdateLighting(isOn, brightness, lightColor);

            if (_haloRingRenderer != null && MaterialManager.Instance != null)
            {
                if (isOn)
                {
                    _haloRingRenderer.material = MaterialManager.Instance.CreateBulbMaterial(lightColor, brightness * 1.2f);
                }
                else
                {
                    _haloRingRenderer.material = MaterialManager.Instance.CreateMaterialInstance(MaterialType.FrostedGlass, new Color(0.85f, 0.85f, 0.85f));
                }
            }
        }

        protected override float GetBaseLightIntensity()
        {
            return 3.0f;
        }
    }
}
