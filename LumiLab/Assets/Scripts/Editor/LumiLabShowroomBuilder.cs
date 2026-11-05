using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using LumiLab.Core;
using LumiLab.Graphics;
using LumiLab.Lamps;
using LumiLab.ShowroomCamera;
using LumiLab.UI;

namespace LumiLab.Editor
{
    public static class LumiLabShowroomBuilder
    {
        private const string SCENE_PATH = "Assets/Scenes/LumiLab_Showroom.unity";

        [MenuItem("LumiLab/Build Complete Showroom", false, 1)]
        public static void BuildCompleteShowroom()
        {
            Debug.Log("[LumiLab] Starting automated showroom generation...");

            // 1. Create and setup new empty scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 2. Build complete showroom and modern UI hierarchy
            LumiLabRuntimeShowroom.BuildRuntimeShowroom();

            // 3. Save Scene & Update Build Settings
            EditorSceneManager.SaveScene(scene, SCENE_PATH);
            AddSceneToBuildSettings(SCENE_PATH);

            Debug.Log($"<color=green>[LumiLab] Showroom generation complete! Scene saved to: {SCENE_PATH}</color>");
        }

        public static void BuildCompleteShowroomBatch()
        {
            try
            {
                BuildCompleteShowroom();
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LumiLab] Batch build exception: {ex}");
                EditorApplication.Exit(1);
            }
        }

        [MenuItem("LumiLab/Build Standalone Player", false, 2)]
        public static void BuildStandalonePlayer()
        {
            Debug.Log("[LumiLab] Preparing scene for standalone build...");
            BuildCompleteShowroom();

            string buildDir = "Builds/Windows";
            if (!System.IO.Directory.Exists(buildDir))
            {
                System.IO.Directory.CreateDirectory(buildDir);
            }

            string exePath = System.IO.Path.Combine(buildDir, "LumiLab.exe");
            Debug.Log($"[LumiLab] Building Standalone Windows64 player: {exePath}...");

            BuildPlayerOptions opt = new BuildPlayerOptions();
            opt.scenes = new[] { SCENE_PATH };
            opt.locationPathName = exePath;
            opt.target = BuildTarget.StandaloneWindows64;
            opt.options = BuildOptions.None;

            var report = BuildPipeline.BuildPlayer(opt);
            var summary = report.summary;

            if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"<color=green>[LumiLab] Player build SUCCEEDED! Output: {exePath} ({summary.totalSize / (1024 * 1024)} MB)</color>");
            }
            else
            {
                Debug.LogError($"[LumiLab] Player build FAILED with status: {summary.result}");
            }
        }

        public static void BuildStandalonePlayerBatch()
        {
            try
            {
                BuildStandalonePlayer();
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LumiLab] Standalone batch build exception: {ex}");
                EditorApplication.Exit(1);
            }
        }

        [MenuItem("LumiLab/Build WebGL", false, 3)]
        public static void BuildWebGL()
        {
            Debug.Log("[LumiLab] Preparing showroom scene for WebGL build...");
            BuildCompleteShowroom();

            string buildDir = "Builds/WebGL";
            if (!System.IO.Directory.Exists(buildDir))
            {
                System.IO.Directory.CreateDirectory(buildDir);
            }

            Debug.Log("[LumiLab] Configuring WebGL PlayerSettings...");
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
            PlayerSettings.WebGL.decompressionFallback = true;
            PlayerSettings.WebGL.emscriptenArgs = "--bind";

            Debug.Log($"[LumiLab] Building WebGL Player to {buildDir}...");

            BuildPlayerOptions opt = new BuildPlayerOptions();
            opt.scenes = new[] { SCENE_PATH };
            opt.locationPathName = buildDir;
            opt.target = BuildTarget.WebGL;
            opt.options = BuildOptions.None;

            var report = BuildPipeline.BuildPlayer(opt);
            var summary = report.summary;

            if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"<color=green>[LumiLab] WebGL build SUCCEEDED! Total size: {summary.totalSize / (1024 * 1024)} MB in {summary.totalTime.TotalSeconds:F1}s</color>");
            }
            else
            {
                Debug.LogError($"[LumiLab] WebGL build FAILED with status: {summary.result}");
            }
        }

        public static void BuildWebGLBatch()
        {
            try
            {
                BuildWebGL();
                EditorApplication.Exit(0);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LumiLab] WebGL batch build exception: {ex}");
                EditorApplication.Exit(1);
            }
        }

        private static void SetupRenderSettings()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.25f, 0.24f, 0.22f);
            RenderSettings.ambientIntensity = 0.9f;
        }

        private static EnvironmentController BuildEnvironments()
        {
            GameObject root = new GameObject("Showroom_Environments");
            EnvironmentController envCtrl = root.AddComponent<EnvironmentController>();

            Shader litShader = GetLitShader();

            // ----------------------------------------------------
            // Environment 1: Luxury Living Room
            // ----------------------------------------------------
            GameObject livingRoot = new GameObject("Env1_LuxuryLivingRoom");
            livingRoot.transform.SetParent(root.transform);

            // Floor
            GameObject floor1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor1.name = "Hardwood_Floor";
            floor1.transform.SetParent(livingRoot.transform);
            floor1.transform.position = new Vector3(0, -0.05f, 0);
            floor1.transform.localScale = new Vector3(8f, 0.1f, 8f);
            SetColor(floor1, new Color(0.28f, 0.20f, 0.14f), 0.1f, 0.45f);

            // Feature Accent Wall (with vertical slat panelling)
            GameObject backWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backWall.name = "Accent_Wall";
            backWall.transform.SetParent(livingRoot.transform);
            backWall.transform.position = new Vector3(0, 1.5f, 1.8f);
            backWall.transform.localScale = new Vector3(6f, 3.2f, 0.1f);
            SetColor(backWall, new Color(0.18f, 0.20f, 0.22f), 0.0f, 0.2f);

            // Side Table (where lamp sits)
            GameObject table = new GameObject("Designer_SideTable");
            table.transform.SetParent(livingRoot.transform);
            table.transform.position = new Vector3(0, 0, 0);

            // Table top
            GameObject tableTop = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tableTop.name = "Marble_TableTop";
            tableTop.transform.SetParent(table.transform);
            tableTop.transform.position = new Vector3(0, 0.70f, 0);
            tableTop.transform.localScale = new Vector3(0.9f, 0.025f, 0.9f);
            SetColor(tableTop, new Color(0.88f, 0.88f, 0.86f), 0.1f, 0.8f);

            // Table Legs / Pedestal
            GameObject tableLeg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tableLeg.name = "Brass_Leg";
            tableLeg.transform.SetParent(table.transform);
            tableLeg.transform.position = new Vector3(0, 0.35f, 0);
            tableLeg.transform.localScale = new Vector3(0.08f, 0.35f, 0.08f);
            SetColor(tableLeg, new Color(0.85f, 0.70f, 0.40f), 0.9f, 0.7f);

            GameObject tableBase = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tableBase.name = "Brass_Foot";
            tableBase.transform.SetParent(table.transform);
            tableBase.transform.position = new Vector3(0, 0.015f, 0);
            tableBase.transform.localScale = new Vector3(0.6f, 0.015f, 0.6f);
            SetColor(tableBase, new Color(0.85f, 0.70f, 0.40f), 0.9f, 0.7f);

            // Decor: Minimalist Book Stack
            GameObject book1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            book1.name = "Art_Book";
            book1.transform.SetParent(table.transform);
            book1.transform.position = new Vector3(0.24f, 0.73f, 0.12f);
            book1.transform.rotation = Quaternion.Euler(0, 15f, 0);
            book1.transform.localScale = new Vector3(0.18f, 0.025f, 0.24f);
            SetColor(book1, new Color(0.15f, 0.15f, 0.16f), 0f, 0.2f);

            // Mount point
            GameObject mount1 = new GameObject("Mount_LivingRoom");
            mount1.transform.SetParent(livingRoot.transform);
            mount1.transform.position = new Vector3(-0.05f, 0.725f, -0.05f);

            // Warm ambient key light
            GameObject keyLight1GO = new GameObject("LivingRoom_KeyLight");
            keyLight1GO.transform.SetParent(livingRoot.transform);
            Light keyLight1 = keyLight1GO.AddComponent<Light>();
            keyLight1.type = LightType.Directional;
            keyLight1.color = new Color(1.0f, 0.95f, 0.88f);
            keyLight1.intensity = 0.7f;
            keyLight1.shadows = LightShadows.Soft;
            keyLight1GO.transform.rotation = Quaternion.Euler(45f, -30f, 0);

            // ----------------------------------------------------
            // Environment 2: Minimal Design Studio
            // ----------------------------------------------------
            GameObject studioRoot = new GameObject("Env2_MinimalStudio");
            studioRoot.transform.SetParent(root.transform);
            studioRoot.SetActive(false);

            // Studio cyclorama floor & backdrop
            GameObject studioFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            studioFloor.name = "Cyclorama_Floor";
            studioFloor.transform.SetParent(studioRoot.transform);
            studioFloor.transform.position = new Vector3(0, -0.05f, 0);
            studioFloor.transform.localScale = new Vector3(10f, 0.1f, 10f);
            SetColor(studioFloor, new Color(0.90f, 0.90f, 0.92f), 0f, 0.15f);

            GameObject studioCyc = GameObject.CreatePrimitive(PrimitiveType.Cube);
            studioCyc.name = "Cyclorama_Backdrop";
            studioCyc.transform.SetParent(studioRoot.transform);
            studioCyc.transform.position = new Vector3(0, 2f, 2.5f);
            studioCyc.transform.localScale = new Vector3(10f, 4.5f, 0.1f);
            SetColor(studioCyc, new Color(0.92f, 0.92f, 0.94f), 0f, 0.15f);

            // Cylindrical display plinth
            GameObject plinth = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            plinth.name = "Display_Plinth";
            plinth.transform.SetParent(studioRoot.transform);
            plinth.transform.position = new Vector3(0, 0.35f, 0);
            plinth.transform.localScale = new Vector3(0.85f, 0.35f, 0.85f);
            SetColor(plinth, new Color(0.96f, 0.96f, 0.97f), 0f, 0.3f);

            GameObject mount2 = new GameObject("Mount_Studio");
            mount2.transform.SetParent(studioRoot.transform);
            mount2.transform.position = new Vector3(0, 0.705f, 0);

            // Studio 3-point lights
            GameObject studioKeyGO = new GameObject("Studio_KeyLight");
            studioKeyGO.transform.SetParent(studioRoot.transform);
            Light studioKey = studioKeyGO.AddComponent<Light>();
            studioKey.type = LightType.Directional;
            studioKey.color = new Color(1.0f, 1.0f, 1.0f);
            studioKey.intensity = 0.9f;
            studioKey.shadows = LightShadows.Soft;
            studioKeyGO.transform.rotation = Quaternion.Euler(50f, -40f, 0);

            GameObject studioFillGO = new GameObject("Studio_FillLight");
            studioFillGO.transform.SetParent(studioRoot.transform);
            Light studioFill = studioFillGO.AddComponent<Light>();
            studioFill.type = LightType.Directional;
            studioFill.color = new Color(0.85f, 0.90f, 1.0f);
            studioFill.intensity = 0.4f;
            studioFill.shadows = LightShadows.None;
            studioFillGO.transform.rotation = Quaternion.Euler(30f, 60f, 0);

            // ----------------------------------------------------
            // Environment 3: Cozy Bedroom
            // ----------------------------------------------------
            GameObject bedRoot = new GameObject("Env3_CozyBedroom");
            bedRoot.transform.SetParent(root.transform);
            bedRoot.SetActive(false);

            // Bedroom Floor
            GameObject bedFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bedFloor.name = "Bedroom_Floor";
            bedFloor.transform.SetParent(bedRoot.transform);
            bedFloor.transform.position = new Vector3(0, -0.05f, 0);
            bedFloor.transform.localScale = new Vector3(8f, 0.1f, 8f);
            SetColor(bedFloor, new Color(0.35f, 0.28f, 0.22f), 0.05f, 0.35f);

            // Back Wall with soft warm paint
            GameObject bedWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bedWall.name = "Bedroom_Wall";
            bedWall.transform.SetParent(bedRoot.transform);
            bedWall.transform.position = new Vector3(0, 1.5f, 1.8f);
            bedWall.transform.localScale = new Vector3(6f, 3.2f, 0.1f);
            SetColor(bedWall, new Color(0.24f, 0.22f, 0.22f), 0f, 0.15f);

            // Bed Headboard & Mattress
            GameObject headboard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            headboard.name = "Bed_Headboard";
            headboard.transform.SetParent(bedRoot.transform);
            headboard.transform.position = new Vector3(0.9f, 0.65f, 1.6f);
            headboard.transform.localScale = new Vector3(1.4f, 1.3f, 0.15f);
            SetColor(headboard, new Color(0.18f, 0.18f, 0.20f), 0f, 0.1f);

            GameObject mattress = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mattress.name = "Bed_Mattress";
            mattress.transform.SetParent(bedRoot.transform);
            mattress.transform.position = new Vector3(0.9f, 0.32f, 0.8f);
            mattress.transform.localScale = new Vector3(1.3f, 0.45f, 1.6f);
            SetColor(mattress, new Color(0.85f, 0.82f, 0.78f), 0f, 0.1f);

            // Nightstand Table
            GameObject nightstand = GameObject.CreatePrimitive(PrimitiveType.Cube);
            nightstand.name = "Bedside_Nightstand";
            nightstand.transform.SetParent(bedRoot.transform);
            nightstand.transform.position = new Vector3(-0.25f, 0.33f, 0.4f);
            nightstand.transform.localScale = new Vector3(0.55f, 0.66f, 0.55f);
            SetColor(nightstand, new Color(0.22f, 0.17f, 0.13f), 0.05f, 0.3f);

            GameObject mount3 = new GameObject("Mount_Bedroom");
            mount3.transform.SetParent(bedRoot.transform);
            mount3.transform.position = new Vector3(-0.25f, 0.67f, 0.4f);

            GameObject bedKeyGO = new GameObject("Bedroom_KeyLight");
            bedKeyGO.transform.SetParent(bedRoot.transform);
            Light bedKey = bedKeyGO.AddComponent<Light>();
            bedKey.type = LightType.Directional;
            bedKey.color = new Color(1.0f, 0.88f, 0.75f);
            bedKey.intensity = 0.5f;
            bedKey.shadows = LightShadows.Soft;
            bedKeyGO.transform.rotation = Quaternion.Euler(40f, -20f, 0);

            // Assign fields in EnvironmentController
            SetPrivateField(envCtrl, "_livingRoomRoot", livingRoot);
            SetPrivateField(envCtrl, "_minimalStudioRoot", studioRoot);
            SetPrivateField(envCtrl, "_cozyBedroomRoot", bedRoot);
            SetPrivateField(envCtrl, "_livingRoomMount", mount1.transform);
            SetPrivateField(envCtrl, "_minimalStudioMount", mount2.transform);
            SetPrivateField(envCtrl, "_cozyBedroomMount", mount3.transform);
            SetPrivateField(envCtrl, "_livingRoomKeyLight", keyLight1);
            SetPrivateField(envCtrl, "_studioKeyLight", studioKey);
            SetPrivateField(envCtrl, "_studioFillLight", studioFill);
            SetPrivateField(envCtrl, "_bedroomKeyLight", bedKey);

            return envCtrl;
        }

        private static LampBase[] BuildLamps()
        {
            GameObject container = new GameObject("Lamps_Container");
            List<LampBase> list = new List<LampBase>();

            // ----------------------------------------------------
            // 1. Lamp Aura Modern
            // ----------------------------------------------------
            GameObject lamp1GO = new GameObject("Lamp1_AuraModern");
            lamp1GO.transform.SetParent(container.transform);
            LampAuraModern lamp1 = lamp1GO.AddComponent<LampAuraModern>();

            // Base
            GameObject base1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            base1.name = "Aura_Base";
            base1.transform.SetParent(lamp1GO.transform);
            base1.transform.localPosition = new Vector3(0, 0.015f, 0);
            base1.transform.localScale = new Vector3(0.26f, 0.015f, 0.26f);

            // Stand
            GameObject stand1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stand1.name = "Aura_Stand";
            stand1.transform.SetParent(lamp1GO.transform);
            stand1.transform.localPosition = new Vector3(0, 0.24f, 0);
            stand1.transform.localScale = new Vector3(0.018f, 0.22f, 0.018f);

            // Shade
            GameObject shade1 = new GameObject("Aura_Shade");
            shade1.transform.SetParent(lamp1GO.transform);
            shade1.transform.localPosition = new Vector3(0, 0.45f, 0);
            MeshFilter shadeFilter1 = shade1.AddComponent<MeshFilter>();
            MeshRenderer shadeRend1 = shade1.AddComponent<MeshRenderer>();

            // Bulb
            GameObject bulb1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bulb1.name = "Aura_Bulb";
            bulb1.transform.SetParent(lamp1GO.transform);
            bulb1.transform.localPosition = new Vector3(0, 0.42f, 0);
            bulb1.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);

            // Main Light
            GameObject light1GO = new GameObject("Aura_Light");
            light1GO.transform.SetParent(lamp1GO.transform);
            light1GO.transform.localPosition = new Vector3(0, 0.42f, 0);
            Light l1 = light1GO.AddComponent<Light>();
            l1.type = LightType.Point;
            l1.range = 5f;
            l1.intensity = 2.5f;
            l1.shadows = LightShadows.Soft;

            SetPrivateField(lamp1, "_baseTransform", base1.transform);
            SetPrivateField(lamp1, "_standTransform", stand1.transform);
            SetPrivateField(lamp1, "_shadeTransform", shade1.transform);
            SetPrivateField(lamp1, "_bulbTransform", bulb1.transform);
            SetPrivateField(lamp1, "_baseRenderer", base1.GetComponent<MeshRenderer>());
            SetPrivateField(lamp1, "_standRenderer", stand1.GetComponent<MeshRenderer>());
            SetPrivateField(lamp1, "_shadeRenderer", shadeRend1);
            SetPrivateField(lamp1, "_bulbRenderer", bulb1.GetComponent<MeshRenderer>());
            SetPrivateField(lamp1, "_mainLight", l1);
            SetPrivateField(lamp1, "_shadeMeshFilter", shadeFilter1);

            lamp1.Initialize();
            list.Add(lamp1);

            // ----------------------------------------------------
            // 2. Lamp Nordic Glow
            // ----------------------------------------------------
            GameObject lamp2GO = new GameObject("Lamp2_NordicGlow");
            lamp2GO.transform.SetParent(container.transform);
            LampNordicGlow lamp2 = lamp2GO.AddComponent<LampNordicGlow>();

            // Wooden base
            GameObject base2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            base2.name = "Nordic_Base";
            base2.transform.SetParent(lamp2GO.transform);
            base2.transform.localPosition = new Vector3(0, 0.02f, 0);
            base2.transform.localScale = new Vector3(0.28f, 0.02f, 0.28f);

            // Rounded Ceramic Body (Procedural Lathe)
            GameObject body2 = new GameObject("Nordic_CeramicBody");
            body2.transform.SetParent(lamp2GO.transform);
            body2.transform.localPosition = new Vector3(0, 0.04f, 0);
            MeshFilter bodyFilter2 = body2.AddComponent<MeshFilter>();
            MeshRenderer bodyRend2 = body2.AddComponent<MeshRenderer>();

            // Fabric Drum Shade
            GameObject shade2 = new GameObject("Nordic_DrumShade");
            shade2.transform.SetParent(lamp2GO.transform);
            shade2.transform.localPosition = new Vector3(0, 0.42f, 0);
            MeshFilter shadeFilter2 = shade2.AddComponent<MeshFilter>();
            MeshRenderer shadeRend2 = shade2.AddComponent<MeshRenderer>();

            // Bulb
            GameObject bulb2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bulb2.name = "Nordic_Bulb";
            bulb2.transform.SetParent(lamp2GO.transform);
            bulb2.transform.localPosition = new Vector3(0, 0.40f, 0);
            bulb2.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);

            GameObject light2GO = new GameObject("Nordic_Light");
            light2GO.transform.SetParent(lamp2GO.transform);
            light2GO.transform.localPosition = new Vector3(0, 0.40f, 0);
            Light l2 = light2GO.AddComponent<Light>();
            l2.type = LightType.Point;
            l2.range = 5f;
            l2.intensity = 2.2f;
            l2.shadows = LightShadows.Soft;

            SetPrivateField(lamp2, "_baseTransform", base2.transform);
            SetPrivateField(lamp2, "_standTransform", body2.transform);
            SetPrivateField(lamp2, "_shadeTransform", shade2.transform);
            SetPrivateField(lamp2, "_bulbTransform", bulb2.transform);
            SetPrivateField(lamp2, "_baseRenderer", base2.GetComponent<MeshRenderer>());
            SetPrivateField(lamp2, "_standRenderer", bodyRend2);
            SetPrivateField(lamp2, "_shadeRenderer", shadeRend2);
            SetPrivateField(lamp2, "_bulbRenderer", bulb2.GetComponent<MeshRenderer>());
            SetPrivateField(lamp2, "_mainLight", l2);
            SetPrivateField(lamp2, "_bodyMeshFilter", bodyFilter2);
            SetPrivateField(lamp2, "_shadeMeshFilter", shadeFilter2);

            lamp2.Initialize();
            list.Add(lamp2);

            // ----------------------------------------------------
            // 3. Lamp Industrial Edge
            // ----------------------------------------------------
            GameObject lamp3GO = new GameObject("Lamp3_IndustrialEdge");
            lamp3GO.transform.SetParent(container.transform);
            LampIndustrialEdge lamp3 = lamp3GO.AddComponent<LampIndustrialEdge>();

            // Stepped Base
            GameObject base3 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            base3.name = "Industrial_Base";
            base3.transform.SetParent(lamp3GO.transform);
            base3.transform.localPosition = new Vector3(0, 0.02f, 0);
            base3.transform.localScale = new Vector3(0.24f, 0.02f, 0.24f);

            // Arm Base Pivot
            GameObject armBasePivot = new GameObject("Arm_BasePivot");
            armBasePivot.transform.SetParent(lamp3GO.transform);
            armBasePivot.transform.localPosition = new Vector3(0, 0.04f, 0);

            // Lower Arm Rod
            GameObject lowerRod = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lowerRod.name = "Lower_ArmRod";
            lowerRod.transform.SetParent(armBasePivot.transform);
            lowerRod.transform.localPosition = new Vector3(0, 0.15f, 0);
            lowerRod.transform.localScale = new Vector3(0.018f, 0.15f, 0.018f);

            // Elbow Joint
            GameObject elbowPivot = new GameObject("Arm_ElbowPivot");
            elbowPivot.transform.SetParent(armBasePivot.transform);
            elbowPivot.transform.localPosition = new Vector3(0, 0.30f, 0);

            // Upper Arm Rod
            GameObject upperRod = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            upperRod.name = "Upper_ArmRod";
            upperRod.transform.SetParent(elbowPivot.transform);
            upperRod.transform.localPosition = new Vector3(0, 0.14f, 0);
            upperRod.transform.localScale = new Vector3(0.016f, 0.14f, 0.016f);

            // Head Pivot
            GameObject headPivot = new GameObject("Arm_HeadPivot");
            headPivot.transform.SetParent(elbowPivot.transform);
            headPivot.transform.localPosition = new Vector3(0, 0.28f, 0);

            // Spotlight Cone Shade
            GameObject shade3 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shade3.name = "Industrial_ConeShade";
            shade3.transform.SetParent(headPivot.transform);
            shade3.transform.localPosition = new Vector3(0, -0.05f, 0.05f);
            shade3.transform.localRotation = Quaternion.Euler(110f, 0, 0);
            shade3.transform.localScale = new Vector3(0.14f, 0.08f, 0.14f);

            // Bulb inside cone
            GameObject bulb3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bulb3.name = "Industrial_Bulb";
            bulb3.transform.SetParent(shade3.transform);
            bulb3.transform.localPosition = new Vector3(0, -0.5f, 0);
            bulb3.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);

            // Spot Light
            GameObject spotGO = new GameObject("Industrial_SpotLight");
            spotGO.transform.SetParent(shade3.transform);
            spotGO.transform.localPosition = new Vector3(0, -0.6f, 0);
            spotGO.transform.localRotation = Quaternion.Euler(90f, 0, 0);
            Light spotL = spotGO.AddComponent<Light>();
            spotL.type = LightType.Spot;
            spotL.spotAngle = 55f;
            spotL.range = 6f;
            spotL.intensity = 3.5f;
            spotL.shadows = LightShadows.Soft;

            SetPrivateField(lamp3, "_baseTransform", base3.transform);
            SetPrivateField(lamp3, "_standTransform", armBasePivot.transform);
            SetPrivateField(lamp3, "_shadeTransform", shade3.transform);
            SetPrivateField(lamp3, "_bulbTransform", bulb3.transform);
            SetPrivateField(lamp3, "_baseRenderer", base3.GetComponent<MeshRenderer>());
            SetPrivateField(lamp3, "_standRenderer", lowerRod.GetComponent<MeshRenderer>());
            SetPrivateField(lamp3, "_shadeRenderer", shade3.GetComponent<MeshRenderer>());
            SetPrivateField(lamp3, "_bulbRenderer", bulb3.GetComponent<MeshRenderer>());
            SetPrivateField(lamp3, "_mainLight", spotL);
            SetPrivateField(lamp3, "_spotLight", spotL);
            SetPrivateField(lamp3, "_armBasePivot", armBasePivot.transform);
            SetPrivateField(lamp3, "_elbowPivot", elbowPivot.transform);
            SetPrivateField(lamp3, "_headPivot", headPivot.transform);

            lamp3.Initialize();
            list.Add(lamp3);

            // ----------------------------------------------------
            // 4. Lamp Halo Contemporary
            // ----------------------------------------------------
            GameObject lamp4GO = new GameObject("Lamp4_HaloContemporary");
            lamp4GO.transform.SetParent(container.transform);
            LampHaloContemporary lamp4 = lamp4GO.AddComponent<LampHaloContemporary>();

            // Minimalist Base
            GameObject base4 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            base4.name = "Halo_Base";
            base4.transform.SetParent(lamp4GO.transform);
            base4.transform.localPosition = new Vector3(0, 0.015f, 0);
            base4.transform.localScale = new Vector3(0.22f, 0.015f, 0.22f);

            // Vertical Support Rod
            GameObject stand4 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stand4.name = "Halo_Stem";
            stand4.transform.SetParent(lamp4GO.transform);
            stand4.transform.localPosition = new Vector3(0, 0.12f, 0);
            stand4.transform.localScale = new Vector3(0.016f, 0.12f, 0.016f);

            // Torus Halo Ring
            GameObject haloRing = new GameObject("Halo_TorusRing");
            haloRing.transform.SetParent(lamp4GO.transform);
            haloRing.transform.localPosition = new Vector3(0, 0.44f, 0);
            haloRing.transform.localRotation = Quaternion.Euler(0, 0, 90f); // Vertical standing ring
            MeshFilter haloFilter = haloRing.AddComponent<MeshFilter>();
            MeshRenderer haloRend = haloRing.AddComponent<MeshRenderer>();

            // Center Point Light
            GameObject light4GO = new GameObject("Halo_CenterLight");
            light4GO.transform.SetParent(lamp4GO.transform);
            light4GO.transform.localPosition = new Vector3(0, 0.44f, 0);
            Light l4 = light4GO.AddComponent<Light>();
            l4.type = LightType.Point;
            l4.range = 5.5f;
            l4.intensity = 3.0f;
            l4.shadows = LightShadows.Soft;

            SetPrivateField(lamp4, "_baseTransform", base4.transform);
            SetPrivateField(lamp4, "_standTransform", stand4.transform);
            SetPrivateField(lamp4, "_shadeTransform", haloRing.transform);
            SetPrivateField(lamp4, "_bulbTransform", haloRing.transform);
            SetPrivateField(lamp4, "_baseRenderer", base4.GetComponent<MeshRenderer>());
            SetPrivateField(lamp4, "_standRenderer", stand4.GetComponent<MeshRenderer>());
            SetPrivateField(lamp4, "_shadeRenderer", haloRend);
            SetPrivateField(lamp4, "_bulbRenderer", haloRend);
            SetPrivateField(lamp4, "_mainLight", l4);
            SetPrivateField(lamp4, "_haloRingFilter", haloFilter);
            SetPrivateField(lamp4, "_haloRingRenderer", haloRend);

            lamp4.Initialize();
            list.Add(lamp4);

            return list.ToArray();
        }

        private static TechDemoController BuildTechDemo(Camera mainCam, ShowroomCameraController camCtrl)
        {
            GameObject demoGO = new GameObject("=== TECH DEMO PEDESTAL ===");
            demoGO.transform.position = new Vector3(2.5f, 0, 0); // Positioned slightly to the side
            TechDemoController techCtrl = demoGO.AddComponent<TechDemoController>();

            GameObject meshTarget = new GameObject("TechDemo_MeshTarget");
            meshTarget.transform.SetParent(demoGO.transform);
            meshTarget.transform.localPosition = new Vector3(0, 0.8f, 0);

            MeshFilter mf = meshTarget.AddComponent<MeshFilter>();
            MeshRenderer mr = meshTarget.AddComponent<MeshRenderer>();
            mr.material = new Material(GetLitShader());
            mr.material.color = new Color(0.85f, 0.70f, 0.40f);

            SetPrivateField(techCtrl, "_demoPedestalRoot", demoGO);
            SetPrivateField(techCtrl, "_demoMeshFilter", mf);
            SetPrivateField(techCtrl, "_demoMeshRenderer", mr);
            SetPrivateField(techCtrl, "_mainCamera", mainCam);
            SetPrivateField(techCtrl, "_cameraController", camCtrl);

            demoGO.SetActive(false); // Hidden until opened via UI
            return techCtrl;
        }

        private static LumiLabUIController BuildUI(Camera mainCam)
        {
            // 1. Create Canvas
            GameObject canvasGO = new GameObject("LumiLab_Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGO.AddComponent<GraphicRaycaster>();

            // 2. EventSystem
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            var inputModuleType = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputModuleType != null)
            {
                esGO.AddComponent(inputModuleType);
            }
            else
            {
                esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            // 3. UI Controller
            LumiLabUIController uiCtrl = canvasGO.AddComponent<LumiLabUIController>();

            // Colors
            Color panelBg = new Color(0.10f, 0.11f, 0.13f, 0.92f);
            Color headerBg = new Color(0.14f, 0.15f, 0.18f, 0.95f);
            Color accentGold = new Color(0.85f, 0.70f, 0.38f, 1f);
            Color btnNormal = new Color(0.18f, 0.20f, 0.24f, 1f);

            // ----------------------------------------------------
            // Top Navigation Bar
            // ----------------------------------------------------
            GameObject topBar = CreateUIPanel(canvasGO.transform, "TopBar", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -32), new Vector2(0, 64), headerBg);

            // Logo & Title
            Text title = CreateUIText(topBar.transform, "Title", "<b>LUMILAB</b>  <color=#888888>|</color>  STUDIO", 20, TextAnchor.MiddleLeft, Color.white);
            RectTransform titleRect = title.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0);
            titleRect.anchorMax = new Vector2(0, 1);
            titleRect.pivot = new Vector2(0, 0.5f);
            titleRect.anchoredPosition = new Vector2(24, 0);
            titleRect.sizeDelta = new Vector2(260, 0);

            // Selected Lamp Name Badge
            GameObject badge = CreateUIPanel(topBar.transform, "LampBadge", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(240, 36), new Color(0.20f, 0.22f, 0.26f, 0.9f));
            Text lampTitleText = CreateUIText(badge.transform, "LampName", "Aura Modern", 16, TextAnchor.MiddleCenter, accentGold);
            FillParent(lampTitleText.GetComponent<RectTransform>());

            // Top Right Controls (Environment Dropdown, Reset, Tech Demo)
            Dropdown envDropdown = CreateUIDropdown(topBar.transform, "EnvDropdown", new Vector2(1, 0.5f), new Vector2(-280, 0), new Vector2(160, 36), new string[] { "Living Room", "Design Studio", "Cozy Bedroom" });
            Button resetBtn = CreateUIButton(topBar.transform, "ResetBtn", "Reset Defaults", new Vector2(1, 0.5f), new Vector2(-155, 0), new Vector2(120, 36), btnNormal, Color.white);
            Button techBtn = CreateUIButton(topBar.transform, "TechBtn", "Case Study 2", new Vector2(1, 0.5f), new Vector2(-45, 0), new Vector2(110, 36), accentGold, Color.black);

            // ----------------------------------------------------
            // Left Panel — Lamp Collection Drawer
            // ----------------------------------------------------
            Button leftToggle = CreateUIButton(canvasGO.transform, "LeftToggleBtn", "☰ Collection", new Vector2(0, 1), new Vector2(70, -84), new Vector2(110, 36), btnNormal, Color.white);
            GameObject leftPanel = CreateUIPanel(canvasGO.transform, "LeftPanel_Collection", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(130, -32), new Vector2(240, 520), panelBg);

            Text colHeader = CreateUIText(leftPanel.transform, "Header", "<b>LAMP COLLECTION</b>", 14, TextAnchor.MiddleCenter, accentGold);
            RectTransform colHeaderRect = colHeader.GetComponent<RectTransform>();
            colHeaderRect.anchorMin = new Vector2(0, 1);
            colHeaderRect.anchorMax = new Vector2(1, 1);
            colHeaderRect.pivot = new Vector2(0.5f, 1);
            colHeaderRect.anchoredPosition = new Vector2(0, -16);
            colHeaderRect.sizeDelta = new Vector2(0, 30);

            string[] lampNames = { "1. Aura Modern", "2. Nordic Glow", "3. Industrial Edge", "4. Halo Contemporary" };
            List<Button> lampBtns = new List<Button>();
            List<Image> highlights = new List<Image>();

            for (int i = 0; i < 4; i++)
            {
                float yPos = -70 - (i * 90);
                GameObject card = CreateUIPanel(leftPanel.transform, $"Card_{i}", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, yPos), new Vector2(210, 75), new Color(0.15f, 0.16f, 0.20f, 0.95f));
                Button b = card.AddComponent<Button>();
                lampBtns.Add(b);

                Text cardTitle = CreateUIText(card.transform, "Title", $"<b>{lampNames[i]}</b>", 14, TextAnchor.MiddleLeft, Color.white);
                RectTransform ctr = cardTitle.GetComponent<RectTransform>();
                ctr.anchorMin = new Vector2(0, 0.5f);
                ctr.anchorMax = new Vector2(1, 1);
                ctr.anchoredPosition = new Vector2(16, -6);
                ctr.sizeDelta = new Vector2(-32, 0);

                // Highlight strip on left
                GameObject high = CreateUIPanel(card.transform, "Highlight", new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 0.5f), new Vector2(3, 0), new Vector2(6, 0), accentGold);
                highlights.Add(high.GetComponent<Image>());
                high.SetActive(i == 0);
            }

            // ----------------------------------------------------
            // Right Panel — Customization Drawer
            // ----------------------------------------------------
            Button rightToggle = CreateUIButton(canvasGO.transform, "RightToggleBtn", "Customize ⚙", new Vector2(1, 1), new Vector2(-75, -84), new Vector2(120, 36), btnNormal, Color.white);
            GameObject rightPanel = CreateUIPanel(canvasGO.transform, "RightPanel_Customize", new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-160, -32), new Vector2(300, 640), panelBg);

            Text custHeader = CreateUIText(rightPanel.transform, "Header", "<b>CUSTOMIZE DESIGN</b>", 14, TextAnchor.MiddleCenter, accentGold);
            RectTransform custHeaderRect = custHeader.GetComponent<RectTransform>();
            custHeaderRect.anchorMin = new Vector2(0, 1);
            custHeaderRect.anchorMax = new Vector2(1, 1);
            custHeaderRect.pivot = new Vector2(0.5f, 1);
            custHeaderRect.anchoredPosition = new Vector2(0, -12);
            custHeaderRect.sizeDelta = new Vector2(0, 26);

            // Tab Buttons
            string[] tabNames = { "Scale", "Material", "Color", "Light", "Presets" };
            List<Button> tabButtons = new List<Button>();
            List<GameObject> tabContents = new List<GameObject>();

            for (int i = 0; i < 5; i++)
            {
                float x = -116 + (i * 58);
                Button tabBtn = CreateUIButton(rightPanel.transform, $"TabBtn_{i}", tabNames[i], new Vector2(0.5f, 1), new Vector2(x, -50), new Vector2(54, 26), btnNormal, Color.white);
                tabButtons.Add(tabBtn);

                GameObject tabContent = CreateUIPanel(rightPanel.transform, $"TabContent_{i}", new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f), new Vector2(0, -45), new Vector2(-20, -150), Color.clear);
                tabContents.Add(tabContent);
                tabContent.SetActive(i == 0);
            }

            // Tab 0: Dimensions
            Slider hSlider = CreateLabeledSlider(tabContents[0].transform, "Height", 0.7f, 1.5f, 1.0f, -20);
            Slider bwSlider = CreateLabeledSlider(tabContents[0].transform, "Base Width", 0.7f, 1.5f, 1.0f, -70);
            Slider swSlider = CreateLabeledSlider(tabContents[0].transform, "Shade Width", 0.7f, 1.5f, 1.0f, -120);
            Slider shSlider = CreateLabeledSlider(tabContents[0].transform, "Shade Height", 0.7f, 1.5f, 1.0f, -170);
            Dropdown styleDropdown = CreateUIDropdown(tabContents[0].transform, "StyleDropdown", new Vector2(0.5f, 1), new Vector2(0, -230), new Vector2(240, 32), new string[] { "Style: Tapered / Modern", "Style: Drum / Fluted" });

            // Tab 1: Materials
            Dropdown baseMatDrop = CreateUIDropdown(tabContents[1].transform, "BaseMatDrop", new Vector2(0.5f, 1), new Vector2(0, -30), new Vector2(240, 32), new string[] { "Base: Brushed Metal", "Base: Polished Metal", "Base: Matte Ceramic", "Base: Walnut Wood", "Base: Frosted Glass", "Base: Fabric" });
            Dropdown standMatDrop = CreateUIDropdown(tabContents[1].transform, "StandMatDrop", new Vector2(0.5f, 1), new Vector2(0, -90), new Vector2(240, 32), new string[] { "Stand: Brushed Metal", "Stand: Polished Metal", "Stand: Matte Ceramic", "Stand: Walnut Wood", "Stand: Frosted Glass", "Stand: Fabric" });
            Dropdown shadeMatDrop = CreateUIDropdown(tabContents[1].transform, "ShadeMatDrop", new Vector2(0.5f, 1), new Vector2(0, -150), new Vector2(240, 32), new string[] { "Shade: Brushed Metal", "Shade: Polished Metal", "Shade: Matte Ceramic", "Shade: Walnut Wood", "Shade: Frosted Glass", "Shade: Fabric" });

            // Tab 2: Colors
            Dropdown colorTargetDrop = CreateUIDropdown(tabContents[2].transform, "ColorTarget", new Vector2(0.5f, 1), new Vector2(0, -20), new Vector2(240, 30), new string[] { "Target: Base", "Target: Stand", "Target: Shade" });

            // Swatches grid
            List<Button> swatches = new List<Button>();
            Color[] swatchColors = ColorPalettePresets.CuratedBaseColors;
            for (int i = 0; i < swatchColors.Length; i++)
            {
                float x = -90 + (i % 3) * 90;
                float y = -65 - (i / 3) * 36;
                Button sw = CreateUIButton(tabContents[2].transform, $"Swatch_{i}", "", new Vector2(0.5f, 1), new Vector2(x, y), new Vector2(80, 28), swatchColors[i], Color.white);
                swatches.Add(sw);
            }

            Slider rSlider = CreateLabeledSlider(tabContents[2].transform, "Red", 0f, 1f, 0.85f, -150);
            Slider gSlider = CreateLabeledSlider(tabContents[2].transform, "Green", 0f, 1f, 0.70f, -195);
            Slider bSlider = CreateLabeledSlider(tabContents[2].transform, "Blue", 0f, 1f, 0.40f, -240);
            GameObject previewBox = CreateUIPanel(tabContents[2].transform, "PreviewBox", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -290), new Vector2(240, 24), new Color(0.85f, 0.70f, 0.40f));
            Image previewImg = previewBox.GetComponent<Image>();

            // Tab 3: Lighting
            Toggle lightToggle = CreateUIToggle(tabContents[3].transform, "LightPowerToggle", "Lamp Power On", new Vector2(0.5f, 1), new Vector2(0, -25), new Vector2(220, 30));
            Slider brightSlider = CreateLabeledSlider(tabContents[3].transform, "Brightness", 0.1f, 2.5f, 1.0f, -75);
            Button warmBtn = CreateUIButton(tabContents[3].transform, "WarmBtn", "Warm White (2700K)", new Vector2(0.5f, 1), new Vector2(0, -140), new Vector2(240, 30), new Color(0.95f, 0.75f, 0.55f), Color.black);
            Button neutralBtn = CreateUIButton(tabContents[3].transform, "NeutralBtn", "Neutral White (4000K)", new Vector2(0.5f, 1), new Vector2(0, -180), new Vector2(240, 30), new Color(0.95f, 0.90f, 0.82f), Color.black);
            Button coolBtn = CreateUIButton(tabContents[3].transform, "CoolBtn", "Cool White (6500K)", new Vector2(0.5f, 1), new Vector2(0, -220), new Vector2(240, 30), new Color(0.85f, 0.92f, 1.0f), Color.black);

            // Tab 4: Presets
            string[] presetNames = { "Cozy Evening", "Modern Minimal", "Luxury Gold", "Industrial Black", "Colorful Ambient" };
            List<Button> presetButtons = new List<Button>();
            for (int i = 0; i < 5; i++)
            {
                Button pb = CreateUIButton(tabContents[4].transform, $"Preset_{i}", presetNames[i], new Vector2(0.5f, 1), new Vector2(0, -30 - (i * 45)), new Vector2(240, 36), btnNormal, Color.white);
                presetButtons.Add(pb);
            }

            // Save & Load at bottom of right panel
            Button saveBtn = CreateUIButton(rightPanel.transform, "SaveBtn", "💾 Save Design", new Vector2(0.5f, 0), new Vector2(-65, 30), new Vector2(115, 34), new Color(0.2f, 0.35f, 0.25f), Color.white);
            Button loadBtn = CreateUIButton(rightPanel.transform, "LoadBtn", "📂 Load Design", new Vector2(0.5f, 0), new Vector2(65, 30), new Vector2(115, 34), btnNormal, Color.white);

            // Toast notification banner
            GameObject notifGO = CreateUIPanel(canvasGO.transform, "ToastBanner", new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 110), new Vector2(360, 36), new Color(0.1f, 0.1f, 0.12f, 0.9f));
            Text notifText = CreateUIText(notifGO.transform, "Msg", "Design Saved", 14, TextAnchor.MiddleCenter, accentGold);
            FillParent(notifText.GetComponent<RectTransform>());
            notifGO.SetActive(false);

            // ----------------------------------------------------
            // Bottom Bar Controls
            // ----------------------------------------------------
            GameObject botBar = CreateUIPanel(canvasGO.transform, "BottomBar", new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 38), new Vector2(680, 52), headerBg);

            Button v34 = CreateUIButton(botBar.transform, "V34", "3/4 View", new Vector2(0, 0.5f), new Vector2(50, 0), new Vector2(80, 34), btnNormal, Color.white);
            Button vFr = CreateUIButton(botBar.transform, "VFr", "Front", new Vector2(0, 0.5f), new Vector2(135, 0), new Vector2(70, 34), btnNormal, Color.white);
            Button vSd = CreateUIButton(botBar.transform, "VSd", "Side", new Vector2(0, 0.5f), new Vector2(210, 0), new Vector2(65, 34), btnNormal, Color.white);
            Button vTp = CreateUIButton(botBar.transform, "VTp", "Top", new Vector2(0, 0.5f), new Vector2(280, 0), new Vector2(65, 34), btnNormal, Color.white);
            Button vRs = CreateUIButton(botBar.transform, "VRs", "Reset", new Vector2(0, 0.5f), new Vector2(350, 0), new Vector2(65, 34), btnNormal, Color.white);

            Toggle rotToggle = CreateUIToggle(botBar.transform, "RotToggle", "Auto-Rotate", new Vector2(0, 0.5f), new Vector2(460, 0), new Vector2(120, 34));
            Button pwrBtn = CreateUIButton(botBar.transform, "QuickPowerBtn", "💡 Power", new Vector2(1, 0.5f), new Vector2(-50, 0), new Vector2(85, 34), new Color(0.85f, 0.70f, 0.38f), Color.black);

            // ----------------------------------------------------
            // Technical Demonstration Modal Panel (Case Study 2)
            // ----------------------------------------------------
            GameObject techPanel = CreateUIPanel(canvasGO.transform, "TechDemoPanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(720, 540), new Color(0.08f, 0.09f, 0.11f, 0.98f));
            techPanel.SetActive(false);

            Text techTitle = CreateUIText(techPanel.transform, "TechTitle", "<b>CASE STUDY 2 — GRAPHICS ARCHITECTURE & B-REP DEMO</b>", 16, TextAnchor.MiddleLeft, accentGold);
            RectTransform ttRect = techTitle.GetComponent<RectTransform>();
            ttRect.anchorMin = new Vector2(0, 1);
            ttRect.anchorMax = new Vector2(1, 1);
            ttRect.anchoredPosition = new Vector2(24, -20);
            ttRect.sizeDelta = new Vector2(-48, 30);

            Button closeTechBtn = CreateUIButton(techPanel.transform, "CloseTechBtn", "✕ Close", new Vector2(1, 1), new Vector2(-45, -20), new Vector2(70, 28), new Color(0.3f, 0.15f, 0.15f), Color.white);

            Text conceptExplanation = CreateUIText(techPanel.transform, "Explanation", "Explanation of mathematical modeling and projection.", 12, TextAnchor.UpperLeft, new Color(0.85f, 0.85f, 0.85f));
            RectTransform ceRect = conceptExplanation.GetComponent<RectTransform>();
            ceRect.anchorMin = new Vector2(0, 1);
            ceRect.anchorMax = new Vector2(1, 1);
            ceRect.anchoredPosition = new Vector2(24, -75);
            ceRect.sizeDelta = new Vector2(-48, 70);

            // Mode buttons (A: Lathe, B: B-Rep Hollow)
            Button modeLatheBtn = CreateUIButton(techPanel.transform, "ModeLathe", "A. 2D CAD Lathe", new Vector2(0, 1), new Vector2(110, -135), new Vector2(160, 30), btnNormal, Color.white);
            Button modeBRepBtn = CreateUIButton(techPanel.transform, "ModeBRep", "B. B-Rep Solid", new Vector2(0, 1), new Vector2(280, -135), new Vector2(160, 30), btnNormal, Color.white);
            Toggle wireToggle = CreateUIToggle(techPanel.transform, "WireToggle", "Wireframe Mode", new Vector2(0, 1), new Vector2(460, -135), new Vector2(140, 30));

            // Sliders for Tech Demo
            Slider tessSlider = CreateLabeledSlider(techPanel.transform, "Tessellation (Radial N: 6..48)", 6f, 48f, 24f, -180);
            Slider scaleSlider = CreateLabeledSlider(techPanel.transform, "Matrix Scale (S_xyz)", 0.5f, 2.0f, 1.0f, -225);
            Slider rotSlider = CreateLabeledSlider(techPanel.transform, "Matrix Rotation (θ_y)", 0f, 360f, 0f, -270);
            Slider fovSlider = CreateLabeledSlider(techPanel.transform, "Viewing FOV (15°..90°)", 15f, 90f, 45f, -315);

            // Live 4x4 Matrix Display Box
            GameObject matBox = CreateUIPanel(techPanel.transform, "MatrixBox", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), new Vector2(0, 85), new Vector2(-48, 110), new Color(0.04f, 0.05f, 0.06f, 0.95f));
            Text matText = CreateUIText(matBox.transform, "MatrixText", "Matrix M = T · R · S", 11, TextAnchor.UpperLeft, Color.green);
            FillParent(matText.GetComponent<RectTransform>(), 10);

            // Performance HUD Text
            Text perfText = CreateUIText(techPanel.transform, "PerfText", "FPS: 60 | Draw calls: 14", 11, TextAnchor.MiddleRight, accentGold);
            RectTransform ptRect = perfText.GetComponent<RectTransform>();
            ptRect.anchorMin = new Vector2(0, 0);
            ptRect.anchorMax = new Vector2(1, 0);
            ptRect.anchoredPosition = new Vector2(-24, 20);
            ptRect.sizeDelta = new Vector2(-48, 24);

            // Wire UI Controller fields
            SetPrivateField(uiCtrl, "_selectedLampTitleText", lampTitleText);
            SetPrivateField(uiCtrl, "_environmentDropdown", envDropdown);
            SetPrivateField(uiCtrl, "_resetDefaultsBtn", resetBtn);
            SetPrivateField(uiCtrl, "_techDemoToggleBtn", techBtn);

            SetPrivateField(uiCtrl, "_leftPanel", leftPanel);
            SetPrivateField(uiCtrl, "_leftPanelToggleBtn", leftToggle);
            SetPrivateField(uiCtrl, "_lampSelectButtons", lampBtns.ToArray());
            SetPrivateField(uiCtrl, "_lampSelectHighlights", highlights.ToArray());

            SetPrivateField(uiCtrl, "_rightPanel", rightPanel);
            SetPrivateField(uiCtrl, "_rightPanelToggleBtn", rightToggle);
            SetPrivateField(uiCtrl, "_tabContents", tabContents.ToArray());
            SetPrivateField(uiCtrl, "_tabButtons", tabButtons.ToArray());

            SetPrivateField(uiCtrl, "_heightSlider", hSlider);
            SetPrivateField(uiCtrl, "_baseWidthSlider", bwSlider);
            SetPrivateField(uiCtrl, "_shadeWidthSlider", swSlider);
            SetPrivateField(uiCtrl, "_shadeHeightSlider", shSlider);
            SetPrivateField(uiCtrl, "_shadeStyleDropdown", styleDropdown);

            SetPrivateField(uiCtrl, "_baseMatDropdown", baseMatDrop);
            SetPrivateField(uiCtrl, "_standMatDropdown", standMatDrop);
            SetPrivateField(uiCtrl, "_shadeMatDropdown", shadeMatDrop);

            SetPrivateField(uiCtrl, "_colorTargetDropdown", colorTargetDrop);
            SetPrivateField(uiCtrl, "_swatchButtons", swatches.ToArray());
            SetPrivateField(uiCtrl, "_redSlider", rSlider);
            SetPrivateField(uiCtrl, "_greenSlider", gSlider);
            SetPrivateField(uiCtrl, "_blueSlider", bSlider);
            SetPrivateField(uiCtrl, "_colorPreviewImage", previewImg);

            SetPrivateField(uiCtrl, "_lightPowerToggle", lightToggle);
            SetPrivateField(uiCtrl, "_brightnessSlider", brightSlider);
            SetPrivateField(uiCtrl, "_warmWhiteBtn", warmBtn);
            SetPrivateField(uiCtrl, "_neutralWhiteBtn", neutralBtn);
            SetPrivateField(uiCtrl, "_coolWhiteBtn", coolBtn);

            SetPrivateField(uiCtrl, "_presetButtons", presetButtons.ToArray());
            SetPrivateField(uiCtrl, "_saveBtn", saveBtn);
            SetPrivateField(uiCtrl, "_loadBtn", loadBtn);
            SetPrivateField(uiCtrl, "_notificationText", notifText);

            SetPrivateField(uiCtrl, "_view34Btn", v34);
            SetPrivateField(uiCtrl, "_viewFrontBtn", vFr);
            SetPrivateField(uiCtrl, "_viewSideBtn", vSd);
            SetPrivateField(uiCtrl, "_viewTopBtn", vTp);
            SetPrivateField(uiCtrl, "_viewResetBtn", vRs);
            SetPrivateField(uiCtrl, "_autoRotateToggle", rotToggle);
            SetPrivateField(uiCtrl, "_quickPowerBtn", pwrBtn);

            SetPrivateField(uiCtrl, "_techDemoPanel", techPanel);
            SetPrivateField(uiCtrl, "_techDemoCloseBtn", closeTechBtn);

            return uiCtrl;
        }

        private static void WireManagerReferences(LumiLabManager labManager, LampBase[] lamps, EnvironmentController envCtrl, ShowroomCameraController camCtrl, TechDemoController techCtrl)
        {
            SetPrivateField(labManager, "_lamps", lamps);
            SetPrivateField(labManager, "_environmentController", envCtrl);
            SetPrivateField(labManager, "_cameraController", camCtrl);
            SetPrivateField(labManager, "_techDemoController", techCtrl);
        }

        private static void WireUIReferences(LumiLabUIController uiCtrl, LampBase[] lamps, ShowroomCameraController camCtrl, TechDemoController techCtrl)
        {
            // Tech demo callbacks
            GameObject techPanel = GetPrivateField<GameObject>(uiCtrl, "_techDemoPanel");
            if (techPanel != null && techCtrl != null)
            {
                Button latheBtn = techPanel.transform.Find("ModeLathe")?.GetComponent<Button>();
                Button brepBtn = techPanel.transform.Find("ModeBRep")?.GetComponent<Button>();
                Toggle wireTog = techPanel.transform.Find("WireToggle")?.GetComponent<Toggle>();

                latheBtn?.onClick.AddListener(() => techCtrl.SetDemoMode(0));
                brepBtn?.onClick.AddListener(() => techCtrl.SetDemoMode(1));
                wireTog?.onValueChanged.AddListener(w => techCtrl.ToggleWireframe(w));

                Slider tess = techPanel.transform.Find("Tessellation (Radial N: 6..48)")?.GetComponentInChildren<Slider>();
                Slider sc = techPanel.transform.Find("Matrix Scale (S_xyz)")?.GetComponentInChildren<Slider>();
                Slider ro = techPanel.transform.Find("Matrix Rotation (θ_y)")?.GetComponentInChildren<Slider>();
                Slider fov = techPanel.transform.Find("Viewing FOV (15°..90°)")?.GetComponentInChildren<Slider>();

                tess?.onValueChanged.AddListener(v => techCtrl.SetRadialTessellation(v));
                sc?.onValueChanged.AddListener(v => techCtrl.SetScaleUniform(v));
                ro?.onValueChanged.AddListener(v => techCtrl.SetRotationY(v));
                fov?.onValueChanged.AddListener(v => techCtrl.SetCameraFOV(v));

                SetPrivateField(techCtrl, "_matrixDisplayText", techPanel.transform.Find("MatrixBox/MatrixText")?.GetComponent<Text>());
                SetPrivateField(techCtrl, "_performanceDisplayText", techPanel.transform.Find("PerfText")?.GetComponent<Text>());
                SetPrivateField(techCtrl, "_conceptExplanationText", techPanel.transform.Find("Explanation")?.GetComponent<Text>());
            }
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            int existingIndex = buildScenes.FindIndex(s => s.path == scenePath);
            if (existingIndex >= 0)
            {
                buildScenes[existingIndex].enabled = true;
            }
            else
            {
                buildScenes.Insert(0, new EditorBuildSettingsScene(scenePath, true));
            }
            EditorBuildSettings.scenes = buildScenes.ToArray();
        }

        #region UI Construction Helpers

        private static GameObject CreateUIPanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 size, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            RectTransform rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Image img = go.AddComponent<Image>();
            img.color = color;
            return go;
        }

        private static Text CreateUIText(Transform parent, string name, string content, int fontSize, TextAnchor alignment, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            RectTransform rt = go.AddComponent<RectTransform>();

            Text txt = go.AddComponent<Text>();
            txt.text = content;
            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (defaultFont == null) defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            if (defaultFont == null) defaultFont = Font.CreateDynamicFontFromOSFont("Arial", fontSize);
            if (defaultFont == null) defaultFont = Font.CreateDynamicFontFromOSFont("Segoe UI", fontSize);
            txt.font = defaultFont;
            txt.fontSize = fontSize;
            txt.alignment = alignment;
            txt.color = color;
            txt.supportRichText = true;
            return txt;
        }

        private static Button CreateUIButton(Transform parent, string name, string label, Vector2 anchor, Vector2 pos, Vector2 size, Color bgCol, Color txtCol)
        {
            GameObject go = CreateUIPanel(parent, name, anchor, anchor, anchor, pos, size, bgCol);
            Button btn = go.AddComponent<Button>();

            ColorBlock cb = btn.colors;
            cb.normalColor = bgCol;
            cb.highlightedColor = bgCol * 1.25f;
            cb.pressedColor = bgCol * 0.8f;
            btn.colors = cb;

            if (!string.IsNullOrEmpty(label))
            {
                Text t = CreateUIText(go.transform, "Text", label, 13, TextAnchor.MiddleCenter, txtCol);
                FillParent(t.GetComponent<RectTransform>());
            }

            return btn;
        }

        private static Slider CreateLabeledSlider(Transform parent, string label, float min, float max, float val, float yPos)
        {
            GameObject root = new GameObject(label);
            root.transform.SetParent(parent, false);
            RectTransform rootRt = root.AddComponent<RectTransform>();
            rootRt.anchorMin = new Vector2(0.5f, 1);
            rootRt.anchorMax = new Vector2(0.5f, 1);
            rootRt.pivot = new Vector2(0.5f, 1);
            rootRt.anchoredPosition = new Vector2(0, yPos);
            rootRt.sizeDelta = new Vector2(250, 40);

            Text lbl = CreateUIText(root.transform, "Label", label, 12, TextAnchor.UpperLeft, new Color(0.85f, 0.85f, 0.85f));
            RectTransform lblRt = lbl.GetComponent<RectTransform>();
            lblRt.anchorMin = new Vector2(0, 1);
            lblRt.anchorMax = new Vector2(1, 1);
            lblRt.anchoredPosition = new Vector2(0, -2);
            lblRt.sizeDelta = new Vector2(0, 16);

            // Slider track
            GameObject sliderGO = new GameObject("Slider");
            sliderGO.transform.SetParent(root.transform, false);
            RectTransform sliderRt = sliderGO.AddComponent<RectTransform>();
            sliderRt.anchorMin = new Vector2(0, 0);
            sliderRt.anchorMax = new Vector2(1, 0);
            sliderRt.anchoredPosition = new Vector2(0, 10);
            sliderRt.sizeDelta = new Vector2(0, 16);

            Slider slider = sliderGO.AddComponent<Slider>();
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = val;

            // Background
            GameObject bg = CreateUIPanel(sliderGO.transform, "Background", new Vector2(0, 0.25f), new Vector2(1, 0.75f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.2f, 0.22f, 0.25f));
            // Fill
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderGO.transform, false);
            RectTransform faRt = fillArea.AddComponent<RectTransform>();
            faRt.anchorMin = new Vector2(0, 0.25f);
            faRt.anchorMax = new Vector2(1, 0.75f);
            faRt.sizeDelta = Vector2.zero;

            GameObject fill = CreateUIPanel(fillArea.transform, "Fill", new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 0.5f), Vector2.zero, Vector2.zero, new Color(0.85f, 0.70f, 0.38f));
            slider.fillRect = fill.GetComponent<RectTransform>();

            // Handle
            GameObject handleArea = new GameObject("Handle Slide Area");
            handleArea.transform.SetParent(sliderGO.transform, false);
            RectTransform haRt = handleArea.AddComponent<RectTransform>();
            haRt.anchorMin = Vector2.zero;
            haRt.anchorMax = Vector2.one;
            haRt.sizeDelta = Vector2.zero;

            GameObject handle = CreateUIPanel(handleArea.transform, "Handle", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(16, 20), Color.white);
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = handle.GetComponent<Image>();

            return slider;
        }

        private static Dropdown CreateUIDropdown(Transform parent, string name, Vector2 anchor, Vector2 pos, Vector2 size, string[] options)
        {
            GameObject go = CreateUIPanel(parent, name, anchor, anchor, anchor, pos, size, new Color(0.18f, 0.20f, 0.24f));
            Dropdown dd = go.AddComponent<Dropdown>();

            Text caption = CreateUIText(go.transform, "Label", options != null && options.Length > 0 ? options[0] : "", 12, TextAnchor.MiddleLeft, Color.white);
            RectTransform capRt = caption.GetComponent<RectTransform>();
            capRt.anchorMin = Vector2.zero;
            capRt.anchorMax = Vector2.one;
            capRt.anchoredPosition = new Vector2(10, 0);
            capRt.sizeDelta = new Vector2(-20, 0);
            dd.captionText = caption;

            // Template
            GameObject template = CreateUIPanel(go.transform, "Template", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 1), new Vector2(0, -2), new Vector2(0, 120), new Color(0.14f, 0.15f, 0.18f));
            template.AddComponent<ScrollRect>();
            template.SetActive(false);
            dd.template = template.GetComponent<RectTransform>();

            GameObject viewport = new GameObject("Viewport");
            viewport.transform.SetParent(template.transform, false);
            RectTransform vpRt = viewport.AddComponent<RectTransform>();
            vpRt.anchorMin = Vector2.zero;
            vpRt.anchorMax = Vector2.one;
            vpRt.sizeDelta = Vector2.zero;
            viewport.AddComponent<Mask>();
            viewport.AddComponent<Image>().color = Color.clear;

            GameObject content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            RectTransform cRt = content.AddComponent<RectTransform>();
            cRt.anchorMin = new Vector2(0, 1);
            cRt.anchorMax = new Vector2(1, 1);
            cRt.pivot = new Vector2(0.5f, 1);
            cRt.sizeDelta = new Vector2(0, 28 * (options != null ? options.Length : 1));

            GameObject item = CreateUIPanel(content.transform, "Item", new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(0, 26), Color.clear);
            Toggle itemToggle = item.AddComponent<Toggle>();
            Text itemText = CreateUIText(item.transform, "Item Text", "Option", 12, TextAnchor.MiddleLeft, Color.white);
            FillParent(itemText.GetComponent<RectTransform>(), 10);
            dd.itemText = itemText;

            dd.options.Clear();
            if (options != null)
            {
                foreach (string opt in options)
                {
                    dd.options.Add(new Dropdown.OptionData(opt));
                }
            }

            return dd;
        }

        private static Toggle CreateUIToggle(Transform parent, string name, string label, Vector2 anchor, Vector2 pos, Vector2 size)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            RectTransform rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = anchor;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Toggle toggle = go.AddComponent<Toggle>();

            // Box
            GameObject box = CreateUIPanel(go.transform, "Background", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0), new Vector2(20, 20), new Color(0.2f, 0.22f, 0.25f));
            GameObject checkmark = CreateUIPanel(box.transform, "Checkmark", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(12, 12), new Color(0.85f, 0.70f, 0.38f));
            toggle.graphic = checkmark.GetComponent<Image>();
            toggle.targetGraphic = box.GetComponent<Image>();
            toggle.isOn = true;

            // Label
            Text t = CreateUIText(go.transform, "Label", label, 12, TextAnchor.MiddleLeft, Color.white);
            RectTransform trt = t.GetComponent<RectTransform>();
            trt.anchorMin = new Vector2(0, 0);
            trt.anchorMax = new Vector2(1, 1);
            trt.anchoredPosition = new Vector2(28, 0);
            trt.sizeDelta = new Vector2(-28, 0);

            return toggle;
        }

        private static void FillParent(RectTransform rt, float margin = 0f)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(-margin * 2, -margin * 2);
        }

        private static void SetColor(GameObject go, Color col, float metallic, float smoothness)
        {
            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                Material mat = new Material(GetLitShader());
                mat.color = col;
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", col);
                if (mat.HasProperty("_Metallic")) mat.SetFloat("_Metallic", metallic);
                if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
                mr.material = mat;
            }
        }

        private static Shader GetLitShader()
        {
            Shader s = Shader.Find("Universal Render Pipeline/Lit");
            if (s == null) s = Shader.Find("Universal Render Pipeline/Simple Lit");
            if (s == null) s = Shader.Find("Standard");
            if (s == null) s = Shader.Find("Diffuse");
            return s;
        }

        private static void SetPrivateField(object target, string fieldName, object value)
        {
            if (target == null) return;
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(target, value);
            }
            else
            {
                // Check base class
                var baseField = target.GetType().BaseType?.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                baseField?.SetValue(target, value);
            }
        }

        private static T GetPrivateField<T>(object target, string fieldName) where T : class
        {
            if (target == null) return null;
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                return field.GetValue(target) as T;
            }
            var baseField = target.GetType().BaseType?.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return baseField?.GetValue(target) as T;
        }

        #endregion
    }
}
