using System.Collections.Generic;
using UnityEngine;
using LumiLab.Core;

namespace LumiLab.Graphics
{
    public class MaterialManager : MonoBehaviour
    {
        private static MaterialManager _instance;
        public static MaterialManager Instance => _instance;

        private Shader _litShader;
        private Dictionary<string, Material> _materialCache = new Dictionary<string, Material>();
        private Texture2D _woodTexture;

        private void Awake()
        {
            if (_instance == null) _instance = this;
            else if (_instance != this) Destroy(gameObject);

            InitializeShaders();
            GenerateProceduralTextures();
        }

        private void InitializeShaders()
        {
            _litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (_litShader == null) _litShader = Shader.Find("Universal Render Pipeline/Simple Lit");
            if (_litShader == null) _litShader = Shader.Find("Standard");
            if (_litShader == null) _litShader = Shader.Find("Diffuse");
        }

        private void GenerateProceduralTextures()
        {
            // Procedural subtle wood grain texture
            int width = 256;
            int height = 256;
            _woodTexture = new Texture2D(width, height, TextureFormat.RGBA32, true);
            _woodTexture.name = "Procedural_WoodGrain";

            Color woodBase = new Color(0.6f, 0.42f, 0.28f);
            Color woodDark = new Color(0.45f, 0.3f, 0.18f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float u = (float)x / width;
                    float v = (float)y / height;

                    // Wood ring pattern formula: sin(distance * freq + turbulence)
                    float dist = Mathf.Sqrt(Mathf.Pow(u * 2f - 1f, 2) + Mathf.Pow(v * 4f - 2f, 2));
                    float noise = Mathf.PerlinNoise(u * 8f, v * 32f) * 0.25f;
                    float ring = Mathf.Sin((dist + noise) * 24f) * 0.5f + 0.5f;

                    Color c = Color.Lerp(woodBase, woodDark, ring);
                    _woodTexture.SetPixel(x, y, c);
                }
            }
            _woodTexture.Apply();
        }

        public Material CreateMaterialInstance(MaterialType type, Color tintColor)
        {
            if (_litShader == null) InitializeShaders();

            Material mat = new Material(_litShader);
            mat.name = $"Mat_{type}_{ColorUtility.ToHtmlStringRGB(tintColor)}";

            // Configure material properties based on physical type
            switch (type)
            {
                case MaterialType.BrushedMetal:
                    mat.color = tintColor;
                    SetProperty(mat, "_BaseColor", tintColor);
                    SetProperty(mat, "_Metallic", 0.85f);
                    SetProperty(mat, "_Smoothness", 0.48f);
                    break;

                case MaterialType.PolishedMetal:
                    mat.color = tintColor;
                    SetProperty(mat, "_BaseColor", tintColor);
                    SetProperty(mat, "_Metallic", 0.98f);
                    SetProperty(mat, "_Smoothness", 0.92f);
                    break;

                case MaterialType.MatteCeramic:
                    mat.color = tintColor;
                    SetProperty(mat, "_BaseColor", tintColor);
                    SetProperty(mat, "_Metallic", 0.0f);
                    SetProperty(mat, "_Smoothness", 0.18f);
                    break;

                case MaterialType.Wood:
                    mat.color = tintColor;
                    SetProperty(mat, "_BaseColor", tintColor);
                    if (_woodTexture != null)
                    {
                        if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", _woodTexture);
                        if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", _woodTexture);
                    }
                    SetProperty(mat, "_Metallic", 0.05f);
                    SetProperty(mat, "_Smoothness", 0.35f);
                    break;

                case MaterialType.FrostedGlass:
                    Color glassColor = new Color(tintColor.r, tintColor.g, tintColor.b, 0.75f);
                    mat.color = glassColor;
                    SetProperty(mat, "_BaseColor", glassColor);
                    SetProperty(mat, "_Metallic", 0.05f);
                    SetProperty(mat, "_Smoothness", 0.75f);
                    // Subtle subsurface emission
                    if (mat.HasProperty("_EmissionColor"))
                    {
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", tintColor * 0.15f);
                    }
                    break;

                case MaterialType.Fabric:
                    mat.color = tintColor;
                    SetProperty(mat, "_BaseColor", tintColor);
                    SetProperty(mat, "_Metallic", 0.0f);
                    SetProperty(mat, "_Smoothness", 0.10f);
                    break;
            }

            return mat;
        }

        private void SetProperty(Material mat, string propName, float val)
        {
            if (mat.HasProperty(propName)) mat.SetFloat(propName, val);
        }

        private void SetProperty(Material mat, string propName, Color col)
        {
            if (mat.HasProperty(propName)) mat.SetColor(propName, col);
        }

        public Material CreateBulbMaterial(Color lightColor, float intensity)
        {
            if (_litShader == null) InitializeShaders();
            Material mat = new Material(_litShader);
            mat.name = "Mat_BulbEmission";

            Color emissiveCol = lightColor * Mathf.LinearToGammaSpace(intensity * 1.5f);
            mat.color = lightColor;
            SetProperty(mat, "_BaseColor", lightColor);
            SetProperty(mat, "_Metallic", 0.0f);
            SetProperty(mat, "_Smoothness", 0.95f);

            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", emissiveCol);
            }
            return mat;
        }
    }
}
