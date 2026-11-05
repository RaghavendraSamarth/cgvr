using UnityEngine;
using LumiLab.Core;
using LumiLab.Graphics;

namespace LumiLab.Lamps
{
    public class LampNordicGlow : LampBase
    {
        [SerializeField] private MeshFilter _bodyMeshFilter;
        [SerializeField] private MeshFilter _shadeMeshFilter;

        public override void Initialize()
        {
            base.Initialize();
            BuildProceduralParts();
        }

        private void BuildProceduralParts()
        {
            // 1. Ceramic rounded body (revolved organic contour)
            if (_bodyMeshFilter != null && _bodyMeshFilter.sharedMesh == null)
            {
                Vector2[] bodyProfile = new Vector2[]
                {
                    new Vector2(0.02f, 0.02f),
                    new Vector2(0.12f, 0.04f),
                    new Vector2(0.17f, 0.12f), // Belly of the ceramic vase
                    new Vector2(0.14f, 0.22f),
                    new Vector2(0.06f, 0.28f), // Neck
                    new Vector2(0.05f, 0.32f),
                    new Vector2(0.02f, 0.33f)
                };
                Mesh bodyMesh = ProceduralMeshUtility.CreateRevolvedMesh(bodyProfile, 32, "NordicGlow_CeramicBody");
                _bodyMeshFilter.sharedMesh = bodyMesh;
            }

            // 2. Fabric drum shade
            if (_shadeMeshFilter != null && _shadeMeshFilter.sharedMesh == null)
            {
                Mesh shadeMesh = ProceduralMeshUtility.CreateFrustumShade(0.20f, 0.18f, 0.26f, 32, "NordicGlow_DrumShade");
                _shadeMeshFilter.sharedMesh = shadeMesh;
            }
        }

        protected override float GetStandTopY(float heightScale)
        {
            return 0.35f * heightScale;
        }

        public override void UpdateDimensions(float heightScale, float baseWidthScale, float shadeWidthScale, float shadeHeightScale, int shadeStyle)
        {
            base.UpdateDimensions(heightScale, baseWidthScale, shadeWidthScale, shadeHeightScale, shadeStyle);

            if (_shadeMeshFilter != null)
            {
                float topR = (shadeStyle == 1) ? 0.20f : 0.16f; // Pure drum vs slightly tapered
                _shadeMeshFilter.sharedMesh = ProceduralMeshUtility.CreateFrustumShade(
                    0.20f * shadeWidthScale,
                    topR * shadeWidthScale,
                    0.26f * shadeHeightScale,
                    32,
                    "NordicGlow_DynamicShade"
                );
            }
        }
    }
}
