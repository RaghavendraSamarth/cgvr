using UnityEngine;
using LumiLab.Core;
using LumiLab.Graphics;

namespace LumiLab.Lamps
{
    public class LampAuraModern : LampBase
    {
        [SerializeField] private MeshFilter _shadeMeshFilter;

        public override void Initialize()
        {
            base.Initialize();
            BuildProceduralParts();
        }

        private void BuildProceduralParts()
        {
            if (_shadeMeshFilter != null && _shadeMeshFilter.sharedMesh == null)
            {
                // Revolved tapered modern lampshade profile
                Vector2[] profile = new Vector2[]
                {
                    new Vector2(0.24f, 0.0f),   // Bottom opening
                    new Vector2(0.20f, 0.12f),
                    new Vector2(0.14f, 0.28f),  // Top rim
                    new Vector2(0.04f, 0.29f)   // Inner cap
                };
                Mesh shadeMesh = ProceduralMeshUtility.CreateRevolvedMesh(profile, 36, "AuraModern_Shade");
                _shadeMeshFilter.sharedMesh = shadeMesh;
            }
        }

        protected override float GetStandTopY(float heightScale)
        {
            // Base is around 0.03, stand height scales from 0.03 up to 0.45 * heightScale
            return 0.45f * heightScale;
        }

        public override void UpdateDimensions(float heightScale, float baseWidthScale, float shadeWidthScale, float shadeHeightScale, int shadeStyle)
        {
            base.UpdateDimensions(heightScale, baseWidthScale, shadeWidthScale, shadeHeightScale, shadeStyle);

            if (_shadeMeshFilter != null)
            {
                // Switch between two distinct shade profiles if requested
                Vector2[] profile;
                if (shadeStyle == 1) // Fluted modern
                {
                    profile = new Vector2[]
                    {
                        new Vector2(0.25f, 0.0f),
                        new Vector2(0.16f, 0.10f),
                        new Vector2(0.20f, 0.22f),
                        new Vector2(0.12f, 0.30f)
                    };
                }
                else // Classic Tapered
                {
                    profile = new Vector2[]
                    {
                        new Vector2(0.24f, 0.0f),
                        new Vector2(0.20f, 0.12f),
                        new Vector2(0.14f, 0.28f),
                        new Vector2(0.04f, 0.29f)
                    };
                }
                _shadeMeshFilter.sharedMesh = ProceduralMeshUtility.CreateRevolvedMesh(profile, 36, "AuraModern_DynamicShade");
            }
        }
    }
}
