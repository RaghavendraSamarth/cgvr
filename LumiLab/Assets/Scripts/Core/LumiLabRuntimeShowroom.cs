using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LumiLab.Graphics;
using LumiLab.Lamps;
using LumiLab.ShowroomCamera;
using LumiLab.UI;

namespace LumiLab.Core
{
    /// <summary>
    /// Runtime bootstrapper for LumiLab.
    /// Ensures that when the scene or application starts in Play Mode or Standalone Build,
    /// the entire showroom, environments, 4 lamps, lighting, camera and UI
    /// are verified and operational even if starting from a fresh scene.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class LumiLabRuntimeShowroom : MonoBehaviour
    {
        [SerializeField] private bool _autoBuildIfMissing = true;

        private void Awake()
        {
            if (!_autoBuildIfMissing) return;

            // Check if already configured in the scene
            LumiLabManager existingManager = FindFirstObject<LumiLabManager>();
            if (existingManager == null)
            {
                Debug.Log("[LumiLab] Showroom manager not detected in scene. Auto-constructing LumiLab showroom at runtime...");
                BuildRuntimeShowroom();
            }
        }

        public static void BuildRuntimeShowroom()
        {
            // 1. Ambient Lighting
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.25f, 0.24f, 0.22f);
            RenderSettings.ambientIntensity = 0.9f;

            // 2. Managers
            GameObject managersGO = new GameObject("=== MANAGERS ===");
            MaterialManager matManager = managersGO.AddComponent<MaterialManager>();
            LumiLabManager labManager = managersGO.AddComponent<LumiLabManager>();

            // 3. Camera
            Camera mainCam = Camera.main;
            ShowroomCameraController camCtrl = null;
            if (mainCam == null)
            {
                GameObject camGO = new GameObject("Main Camera");
                camGO.tag = "MainCamera";
                mainCam = camGO.AddComponent<Camera>();
                mainCam.clearFlags = CameraClearFlags.SolidColor;
                mainCam.backgroundColor = new Color(0.12f, 0.12f, 0.14f);
                mainCam.fieldOfView = 45f;
                mainCam.nearClipPlane = 0.1f;
                mainCam.farClipPlane = 100f;
                camGO.AddComponent<AudioListener>();
                camCtrl = camGO.AddComponent<ShowroomCameraController>();
            }
            else
            {
                camCtrl = mainCam.GetComponent<ShowroomCameraController>();
                if (camCtrl == null) camCtrl = mainCam.gameObject.AddComponent<ShowroomCameraController>();
            }

            // 4. Environments
            EnvironmentController envCtrl = BuildEnvironments();

            // 5. Lamps
            LampBase[] lamps = BuildLamps();

            // 6. Case Study 2 Tech Demo
            TechDemoController techCtrl = BuildTechDemo(mainCam, camCtrl);

            // 7. Complete UI
            LumiLabUIController uiCtrl = BuildUI(mainCam);

            // 8. Wire references
            SetField(labManager, "_lamps", lamps);
            SetField(labManager, "_environmentController", envCtrl);
            SetField(labManager, "_cameraController", camCtrl);
            SetField(labManager, "_techDemoController", techCtrl);

            WireUIReferences(uiCtrl, lamps, camCtrl, techCtrl);

            Debug.Log("[LumiLab] Runtime showroom initialization completed successfully!");
        }

        private static EnvironmentController BuildEnvironments()
        {
            GameObject root = new GameObject("Showroom_Environments");
            EnvironmentController envCtrl = root.AddComponent<EnvironmentController>();

            // 1. Luxury Living Room
            GameObject livingRoot = new GameObject("Env1_LuxuryLivingRoom");
            livingRoot.transform.SetParent(root.transform);

            GameObject floor1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor1.name = "Hardwood_Floor";
            floor1.transform.SetParent(livingRoot.transform);
            floor1.transform.position = new Vector3(0, -0.05f, 0);
            floor1.transform.localScale = new Vector3(8f, 0.1f, 8f);
            SetColor(floor1, new Color(0.28f, 0.20f, 0.14f), 0.1f, 0.45f);

            GameObject backWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backWall.name = "Accent_Wall";
            backWall.transform.SetParent(livingRoot.transform);
            backWall.transform.position = new Vector3(0, 1.5f, 1.8f);
            backWall.transform.localScale = new Vector3(6f, 3.2f, 0.1f);
            SetColor(backWall, new Color(0.18f, 0.20f, 0.22f), 0.0f, 0.2f);

            GameObject table = new GameObject("Designer_SideTable");
            table.transform.SetParent(livingRoot.transform);

            GameObject tableTop = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tableTop.name = "Marble_TableTop";
            tableTop.transform.SetParent(table.transform);
            tableTop.transform.position = new Vector3(0, 0.70f, 0);
            tableTop.transform.localScale = new Vector3(0.9f, 0.025f, 0.9f);
            SetColor(tableTop, new Color(0.88f, 0.88f, 0.86f), 0.1f, 0.8f);

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

            GameObject mount1 = new GameObject("Mount_LivingRoom");
            mount1.transform.SetParent(livingRoot.transform);
            mount1.transform.position = new Vector3(-0.05f, 0.725f, -0.05f);

            GameObject key1GO = new GameObject("LivingRoom_KeyLight");
            key1GO.transform.SetParent(livingRoot.transform);
            Light key1 = key1GO.AddComponent<Light>();
            key1.type = LightType.Directional;
            key1.color = new Color(1.0f, 0.95f, 0.88f);
            key1.intensity = 0.7f;
            key1.shadows = LightShadows.Soft;
            key1GO.transform.rotation = Quaternion.Euler(45f, -30f, 0);

            // 2. Minimal Studio
            GameObject studioRoot = new GameObject("Env2_MinimalStudio");
            studioRoot.transform.SetParent(root.transform);
            studioRoot.SetActive(false);

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

            GameObject plinth = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            plinth.name = "Display_Plinth";
            plinth.transform.SetParent(studioRoot.transform);
            plinth.transform.position = new Vector3(0, 0.35f, 0);
            plinth.transform.localScale = new Vector3(0.85f, 0.35f, 0.85f);
            SetColor(plinth, new Color(0.96f, 0.96f, 0.97f), 0f, 0.3f);

            GameObject mount2 = new GameObject("Mount_Studio");
            mount2.transform.SetParent(studioRoot.transform);
            mount2.transform.position = new Vector3(0, 0.705f, 0);

            GameObject studioKeyGO = new GameObject("Studio_KeyLight");
            studioKeyGO.transform.SetParent(studioRoot.transform);
            Light studioKey = studioKeyGO.AddComponent<Light>();
            studioKey.type = LightType.Directional;
            studioKey.color = Color.white;
            studioKey.intensity = 0.9f;
            studioKey.shadows = LightShadows.Soft;
            studioKeyGO.transform.rotation = Quaternion.Euler(50f, -40f, 0);

            GameObject studioFillGO = new GameObject("Studio_FillLight");
            studioFillGO.transform.SetParent(studioRoot.transform);
            Light studioFill = studioFillGO.AddComponent<Light>();
            studioFill.type = LightType.Directional;
            studioFill.color = new Color(0.85f, 0.90f, 1.0f);
            studioFill.intensity = 0.4f;
            studioFillGO.transform.rotation = Quaternion.Euler(30f, 60f, 0);

            // 3. Cozy Bedroom
            GameObject bedRoot = new GameObject("Env3_CozyBedroom");
            bedRoot.transform.SetParent(root.transform);
            bedRoot.SetActive(false);

            GameObject bedFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bedFloor.name = "Bedroom_Floor";
            bedFloor.transform.SetParent(bedRoot.transform);
            bedFloor.transform.position = new Vector3(0, -0.05f, 0);
            bedFloor.transform.localScale = new Vector3(8f, 0.1f, 8f);
            SetColor(bedFloor, new Color(0.35f, 0.28f, 0.22f), 0.05f, 0.35f);

            GameObject bedWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bedWall.name = "Bedroom_Wall";
            bedWall.transform.SetParent(bedRoot.transform);
            bedWall.transform.position = new Vector3(0, 1.5f, 1.8f);
            bedWall.transform.localScale = new Vector3(6f, 3.2f, 0.1f);
            SetColor(bedWall, new Color(0.24f, 0.22f, 0.22f), 0f, 0.15f);

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

            SetField(envCtrl, "_livingRoomRoot", livingRoot);
            SetField(envCtrl, "_minimalStudioRoot", studioRoot);
            SetField(envCtrl, "_cozyBedroomRoot", bedRoot);
            SetField(envCtrl, "_livingRoomMount", mount1.transform);
            SetField(envCtrl, "_minimalStudioMount", mount2.transform);
            SetField(envCtrl, "_cozyBedroomMount", mount3.transform);
            SetField(envCtrl, "_livingRoomKeyLight", key1);
            SetField(envCtrl, "_studioKeyLight", studioKey);
            SetField(envCtrl, "_studioFillLight", studioFill);
            SetField(envCtrl, "_bedroomKeyLight", bedKey);

            return envCtrl;
        }

        private static LampBase[] BuildLamps()
        {
            GameObject container = new GameObject("Lamps_Container");
            List<LampBase> list = new List<LampBase>();

            // Lamp 1: Aura Modern
            GameObject l1GO = new GameObject("Lamp1_AuraModern");
            l1GO.transform.SetParent(container.transform);
            LampAuraModern lamp1 = l1GO.AddComponent<LampAuraModern>();

            GameObject b1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            b1.name = "Aura_Base";
            b1.transform.SetParent(l1GO.transform);
            b1.transform.localPosition = new Vector3(0, 0.015f, 0);
            b1.transform.localScale = new Vector3(0.26f, 0.015f, 0.26f);

            GameObject s1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            s1.name = "Aura_Stand";
            s1.transform.SetParent(l1GO.transform);
            s1.transform.localPosition = new Vector3(0, 0.24f, 0);
            s1.transform.localScale = new Vector3(0.018f, 0.22f, 0.018f);

            GameObject sh1 = new GameObject("Aura_Shade");
            sh1.transform.SetParent(l1GO.transform);
            sh1.transform.localPosition = new Vector3(0, 0.45f, 0);
            MeshFilter sf1 = sh1.AddComponent<MeshFilter>();
            MeshRenderer sr1 = sh1.AddComponent<MeshRenderer>();

            GameObject bulb1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bulb1.name = "Aura_Bulb";
            bulb1.transform.SetParent(l1GO.transform);
            bulb1.transform.localPosition = new Vector3(0, 0.42f, 0);
            bulb1.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);

            GameObject light1GO = new GameObject("Aura_Light");
            light1GO.transform.SetParent(l1GO.transform);
            light1GO.transform.localPosition = new Vector3(0, 0.42f, 0);
            Light light1 = light1GO.AddComponent<Light>();
            light1.type = LightType.Point;
            light1.range = 5f;
            light1.intensity = 2.5f;
            light1.shadows = LightShadows.Soft;

            SetField(lamp1, "_baseTransform", b1.transform);
            SetField(lamp1, "_standTransform", s1.transform);
            SetField(lamp1, "_shadeTransform", sh1.transform);
            SetField(lamp1, "_bulbTransform", bulb1.transform);
            SetField(lamp1, "_baseRenderer", b1.GetComponent<MeshRenderer>());
            SetField(lamp1, "_standRenderer", s1.GetComponent<MeshRenderer>());
            SetField(lamp1, "_shadeRenderer", sr1);
            SetField(lamp1, "_bulbRenderer", bulb1.GetComponent<MeshRenderer>());
            SetField(lamp1, "_mainLight", light1);
            SetField(lamp1, "_shadeMeshFilter", sf1);
            lamp1.Initialize();
            list.Add(lamp1);

            // Lamp 2: Nordic Glow
            GameObject l2GO = new GameObject("Lamp2_NordicGlow");
            l2GO.transform.SetParent(container.transform);
            LampNordicGlow lamp2 = l2GO.AddComponent<LampNordicGlow>();

            GameObject b2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            b2.name = "Nordic_Base";
            b2.transform.SetParent(l2GO.transform);
            b2.transform.localPosition = new Vector3(0, 0.02f, 0);
            b2.transform.localScale = new Vector3(0.28f, 0.02f, 0.28f);

            GameObject body2 = new GameObject("Nordic_CeramicBody");
            body2.transform.SetParent(l2GO.transform);
            body2.transform.localPosition = new Vector3(0, 0.04f, 0);
            MeshFilter bodyFilter2 = body2.AddComponent<MeshFilter>();
            MeshRenderer bodyRend2 = body2.AddComponent<MeshRenderer>();

            GameObject shade2 = new GameObject("Nordic_DrumShade");
            shade2.transform.SetParent(l2GO.transform);
            shade2.transform.localPosition = new Vector3(0, 0.42f, 0);
            MeshFilter shadeFilter2 = shade2.AddComponent<MeshFilter>();
            MeshRenderer shadeRend2 = shade2.AddComponent<MeshRenderer>();

            GameObject bulb2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bulb2.name = "Nordic_Bulb";
            bulb2.transform.SetParent(l2GO.transform);
            bulb2.transform.localPosition = new Vector3(0, 0.40f, 0);
            bulb2.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);

            GameObject light2GO = new GameObject("Nordic_Light");
            light2GO.transform.SetParent(l2GO.transform);
            light2GO.transform.localPosition = new Vector3(0, 0.40f, 0);
            Light light2 = light2GO.AddComponent<Light>();
            light2.type = LightType.Point;
            light2.range = 5f;
            light2.intensity = 2.2f;
            light2.shadows = LightShadows.Soft;

            SetField(lamp2, "_baseTransform", b2.transform);
            SetField(lamp2, "_standTransform", body2.transform);
            SetField(lamp2, "_shadeTransform", shade2.transform);
            SetField(lamp2, "_bulbTransform", bulb2.transform);
            SetField(lamp2, "_baseRenderer", b2.GetComponent<MeshRenderer>());
            SetField(lamp2, "_standRenderer", bodyRend2);
            SetField(lamp2, "_shadeRenderer", shadeRend2);
            SetField(lamp2, "_bulbRenderer", bulb2.GetComponent<MeshRenderer>());
            SetField(lamp2, "_mainLight", light2);
            SetField(lamp2, "_bodyMeshFilter", bodyFilter2);
            SetField(lamp2, "_shadeMeshFilter", shadeFilter2);
            lamp2.Initialize();
            list.Add(lamp2);

            // Lamp 3: Industrial Edge
            GameObject l3GO = new GameObject("Lamp3_IndustrialEdge");
            l3GO.transform.SetParent(container.transform);
            LampIndustrialEdge lamp3 = l3GO.AddComponent<LampIndustrialEdge>();

            GameObject b3 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            b3.name = "Industrial_Base";
            b3.transform.SetParent(l3GO.transform);
            b3.transform.localPosition = new Vector3(0, 0.02f, 0);
            b3.transform.localScale = new Vector3(0.24f, 0.02f, 0.24f);

            GameObject armPivot = new GameObject("Arm_BasePivot");
            armPivot.transform.SetParent(l3GO.transform);
            armPivot.transform.localPosition = new Vector3(0, 0.04f, 0);

            GameObject lowerRod = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            lowerRod.name = "Lower_ArmRod";
            lowerRod.transform.SetParent(armPivot.transform);
            lowerRod.transform.localPosition = new Vector3(0, 0.15f, 0);
            lowerRod.transform.localScale = new Vector3(0.018f, 0.15f, 0.018f);

            GameObject elbowPivot = new GameObject("Arm_ElbowPivot");
            elbowPivot.transform.SetParent(armPivot.transform);
            elbowPivot.transform.localPosition = new Vector3(0, 0.30f, 0);

            GameObject upperRod = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            upperRod.name = "Upper_ArmRod";
            upperRod.transform.SetParent(elbowPivot.transform);
            upperRod.transform.localPosition = new Vector3(0, 0.14f, 0);
            upperRod.transform.localScale = new Vector3(0.016f, 0.14f, 0.016f);

            GameObject headPivot = new GameObject("Arm_HeadPivot");
            headPivot.transform.SetParent(elbowPivot.transform);
            headPivot.transform.localPosition = new Vector3(0, 0.28f, 0);

            GameObject shade3 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            shade3.name = "Industrial_ConeShade";
            shade3.transform.SetParent(headPivot.transform);
            shade3.transform.localPosition = new Vector3(0, -0.05f, 0.05f);
            shade3.transform.localRotation = Quaternion.Euler(110f, 0, 0);
            shade3.transform.localScale = new Vector3(0.14f, 0.08f, 0.14f);

            GameObject bulb3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bulb3.name = "Industrial_Bulb";
            bulb3.transform.SetParent(shade3.transform);
            bulb3.transform.localPosition = new Vector3(0, -0.5f, 0);
            bulb3.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);

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

            SetField(lamp3, "_baseTransform", b3.transform);
            SetField(lamp3, "_standTransform", armPivot.transform);
            SetField(lamp3, "_shadeTransform", shade3.transform);
            SetField(lamp3, "_bulbTransform", bulb3.transform);
            SetField(lamp3, "_baseRenderer", b3.GetComponent<MeshRenderer>());
            SetField(lamp3, "_standRenderer", lowerRod.GetComponent<MeshRenderer>());
            SetField(lamp3, "_shadeRenderer", shade3.GetComponent<MeshRenderer>());
            SetField(lamp3, "_bulbRenderer", bulb3.GetComponent<MeshRenderer>());
            SetField(lamp3, "_mainLight", spotL);
            SetField(lamp3, "_spotLight", spotL);
            SetField(lamp3, "_armBasePivot", armPivot.transform);
            SetField(lamp3, "_elbowPivot", elbowPivot.transform);
            SetField(lamp3, "_headPivot", headPivot.transform);
            lamp3.Initialize();
            list.Add(lamp3);

            // Lamp 4: Halo Contemporary
            GameObject l4GO = new GameObject("Lamp4_HaloContemporary");
            l4GO.transform.SetParent(container.transform);
            LampHaloContemporary lamp4 = l4GO.AddComponent<LampHaloContemporary>();

            GameObject b4 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            b4.name = "Halo_Base";
            b4.transform.SetParent(l4GO.transform);
            b4.transform.localPosition = new Vector3(0, 0.015f, 0);
            b4.transform.localScale = new Vector3(0.22f, 0.015f, 0.22f);

            GameObject stand4 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stand4.name = "Halo_Stem";
            stand4.transform.SetParent(l4GO.transform);
            stand4.transform.localPosition = new Vector3(0, 0.12f, 0);
            stand4.transform.localScale = new Vector3(0.016f, 0.12f, 0.016f);

            GameObject haloRing = new GameObject("Halo_TorusRing");
            haloRing.transform.SetParent(l4GO.transform);
            haloRing.transform.localPosition = new Vector3(0, 0.44f, 0);
            haloRing.transform.localRotation = Quaternion.Euler(0, 0, 90f);
            MeshFilter haloFilter = haloRing.AddComponent<MeshFilter>();
            MeshRenderer haloRend = haloRing.AddComponent<MeshRenderer>();

            GameObject light4GO = new GameObject("Halo_CenterLight");
            light4GO.transform.SetParent(l4GO.transform);
            light4GO.transform.localPosition = new Vector3(0, 0.44f, 0);
            Light light4 = light4GO.AddComponent<Light>();
            light4.type = LightType.Point;
            light4.range = 5.5f;
            light4.intensity = 3.0f;
            light4.shadows = LightShadows.Soft;

            SetField(lamp4, "_baseTransform", b4.transform);
            SetField(lamp4, "_standTransform", stand4.transform);
            SetField(lamp4, "_shadeTransform", haloRing.transform);
            SetField(lamp4, "_bulbTransform", haloRing.transform);
            SetField(lamp4, "_baseRenderer", b4.GetComponent<MeshRenderer>());
            SetField(lamp4, "_standRenderer", stand4.GetComponent<MeshRenderer>());
            SetField(lamp4, "_shadeRenderer", haloRend);
            SetField(lamp4, "_bulbRenderer", haloRend);
            SetField(lamp4, "_mainLight", light4);
            SetField(lamp4, "_haloRingFilter", haloFilter);
            SetField(lamp4, "_haloRingRenderer", haloRend);
            lamp4.Initialize();
            list.Add(lamp4);

            return list.ToArray();
        }

        private static TechDemoController BuildTechDemo(Camera mainCam, ShowroomCameraController camCtrl)
        {
            GameObject demoGO = new GameObject("=== TECH DEMO PEDESTAL ===");
            demoGO.transform.position = new Vector3(2.5f, 0, 0);
            TechDemoController techCtrl = demoGO.AddComponent<TechDemoController>();

            GameObject meshTarget = new GameObject("TechDemo_MeshTarget");
            meshTarget.transform.SetParent(demoGO.transform);
            meshTarget.transform.localPosition = new Vector3(0, 0.8f, 0);

            MeshFilter mf = meshTarget.AddComponent<MeshFilter>();
            MeshRenderer mr = meshTarget.AddComponent<MeshRenderer>();
            mr.material = new Material(GetLitShader());
            mr.material.color = new Color(0.85f, 0.70f, 0.40f);

            SetField(techCtrl, "_demoPedestalRoot", demoGO);
            SetField(techCtrl, "_demoMeshFilter", mf);
            SetField(techCtrl, "_demoMeshRenderer", mr);
            SetField(techCtrl, "_mainCamera", mainCam);
            SetField(techCtrl, "_cameraController", camCtrl);

            demoGO.SetActive(false);
            return techCtrl;
        }

        private static LumiLabUIController BuildUI(Camera mainCam)
        {
            GameObject canvasGO = new GameObject("LumiLab_Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGO.AddComponent<GraphicRaycaster>();

            // Event System
            if (FindFirstObject<UnityEngine.EventSystems.EventSystem>() == null)
            {
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
            }

            LumiLabUIController uiCtrl = canvasGO.AddComponent<LumiLabUIController>();

            // Designer Color Tokens (Matching React Frontend)
            Color surfaceBg = new Color(0.078f, 0.086f, 0.106f, 0.94f); // #14161b
            Color cardBg = new Color(0.102f, 0.114f, 0.141f, 0.96f);    // #1a1d24
            Color borderCol = new Color(0.153f, 0.169f, 0.208f, 1f);   // #272b35
            Color accentGold = new Color(0.831f, 0.686f, 0.216f, 1f);  // #d4af37
            Color textWhite = new Color(0.95f, 0.96f, 0.98f, 1f);
            Color textMuted = new Color(0.58f, 0.64f, 0.72f, 1f);
            Color btnNormal = new Color(0.12f, 0.14f, 0.17f, 1f);

            // Procedural Sprites for Modern Rounded Aesthetics
            Sprite panelSpr = ProceduralUIUtility.GetRoundedSprite(16, surfaceBg, borderCol, 1);
            Sprite cardSpr = ProceduralUIUtility.GetRoundedSprite(12, cardBg, borderCol, 1);
            Sprite btnSpr = ProceduralUIUtility.GetRoundedSprite(8, btnNormal, borderCol, 1);
            Sprite pillSpr = ProceduralUIUtility.GetRoundedSprite(18, surfaceBg, borderCol, 1);
            Sprite goldPillSpr = ProceduralUIUtility.GetRoundedSprite(18, new Color(0.831f, 0.686f, 0.216f, 0.22f), accentGold, 1);

            // ----------------------------------------------------
            // 1. Top Navigation Bar
            // ----------------------------------------------------
            GameObject topBar = CreateUIPanel(canvasGO.transform, "TopBar", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -28), new Vector2(0, 56), surfaceBg, null);
            
            // Brand Group: [L] LUMILAB | STUDIO
            GameObject brandGroup = new GameObject("BrandGroup");
            brandGroup.transform.SetParent(topBar.transform, false);
            RectTransform bgRt = brandGroup.AddComponent<RectTransform>();
            bgRt.anchorMin = new Vector2(0, 0.5f);
            bgRt.anchorMax = new Vector2(0, 0.5f);
            bgRt.pivot = new Vector2(0, 0.5f);
            bgRt.anchoredPosition = new Vector2(24, 0);
            bgRt.sizeDelta = new Vector2(280, 40);

            GameObject logoBox = CreateUIPanel(brandGroup.transform, "LogoBox", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0), new Vector2(32, 32), new Color(0.831f, 0.686f, 0.216f, 0.2f), ProceduralUIUtility.GetRoundedSprite(6, new Color(0.831f, 0.686f, 0.216f, 0.2f), accentGold, 1));
            Text logoTxt = CreateUIText(logoBox.transform, "L", "L", 16, TextAnchor.MiddleCenter, accentGold);
            FillParent(logoTxt.GetComponent<RectTransform>());

            Text brandText = CreateUIText(brandGroup.transform, "BrandText", "<b>LUMILAB</b>  <color=#475569>|</color>  <color=#94a3b8>STUDIO</color>", 15, TextAnchor.MiddleLeft, textWhite);
            RectTransform btRt = brandText.GetComponent<RectTransform>();
            btRt.anchorMin = new Vector2(0, 0.5f);
            btRt.anchorMax = new Vector2(1, 0.5f);
            btRt.anchoredPosition = new Vector2(42, 0);
            btRt.sizeDelta = new Vector2(-42, 32);

            // Center: Active Lamp Badge Pill
            GameObject badge = CreateUIPanel(topBar.transform, "LampBadge", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(220, 34), cardBg, ProceduralUIUtility.GetRoundedSprite(17, cardBg, borderCol, 1));
            // Dot
            GameObject dot = CreateUIPanel(badge.transform, "Dot", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(16, 0), new Vector2(8, 8), accentGold, ProceduralUIUtility.GetRoundedSprite(4, accentGold, Color.clear, 0));
            Text lampTitleText = CreateUIText(badge.transform, "LampName", "Aura Modern", 13, TextAnchor.MiddleLeft, accentGold);
            RectTransform ltRt = lampTitleText.GetComponent<RectTransform>();
            ltRt.anchorMin = Vector2.zero;
            ltRt.anchorMax = Vector2.one;
            ltRt.anchoredPosition = new Vector2(32, 0);
            ltRt.sizeDelta = new Vector2(-36, 0);

            // Top Right Actions
            Dropdown envDropdown = CreateUIDropdown(topBar.transform, "EnvDropdown", new Vector2(1, 0.5f), new Vector2(-320, 0), new Vector2(170, 34), new string[] { "🛋️ Luxury Living Room", "🏛️ Minimal Design Studio", "🛏️ Cozy Bedroom" }, btnSpr);
            Button resetBtn = CreateUIButton(topBar.transform, "ResetBtn", "↺ Reset", new Vector2(1, 0.5f), new Vector2(-195, 0), new Vector2(90, 34), btnNormal, textWhite, btnSpr);
            Button techBtn = CreateUIButton(topBar.transform, "TechBtn", "Case Study 2 🔬", new Vector2(1, 0.5f), new Vector2(-60, 0), new Vector2(130, 34), new Color(0.831f, 0.686f, 0.216f, 0.2f), accentGold, ProceduralUIUtility.GetRoundedSprite(8, new Color(0.831f, 0.686f, 0.216f, 0.2f), accentGold, 1));

            // ----------------------------------------------------
            // 2. Left Collection Drawer
            // ----------------------------------------------------
            Button leftOpenBtn = CreateUIButton(canvasGO.transform, "LeftOpenBtn", "☰ Collection", new Vector2(0, 1), new Vector2(75, -80), new Vector2(115, 36), surfaceBg, accentGold, ProceduralUIUtility.GetRoundedSprite(10, surfaceBg, borderCol, 1));
            leftOpenBtn.gameObject.SetActive(false);

            GameObject leftPanel = CreateUIPanel(canvasGO.transform, "LeftPanel_Collection", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(150, -20), new Vector2(270, 520), surfaceBg, panelSpr);

            // Left Header & Close
            Text colHeader = CreateUIText(leftPanel.transform, "Header", "<b>LAMP COLLECTION</b>", 12, TextAnchor.MiddleLeft, accentGold);
            RectTransform chRt = colHeader.GetComponent<RectTransform>();
            chRt.anchorMin = new Vector2(0, 1);
            chRt.anchorMax = new Vector2(1, 1);
            chRt.pivot = new Vector2(0.5f, 1);
            chRt.anchoredPosition = new Vector2(16, -14);
            chRt.sizeDelta = new Vector2(-48, 26);

            Button leftCloseBtn = CreateUIButton(leftPanel.transform, "LeftCloseBtn", "✕", new Vector2(1, 1), new Vector2(-22, -14), new Vector2(26, 26), Color.clear, textMuted, null);

            // 4 Lamp Cards
            string[] lampNames = { "1. Aura Modern", "2. Nordic Glow", "3. Industrial Edge", "4. Halo Contemporary" };
            string[] lampSubs = {
                "Modern brass stand with tapered linen shade",
                "Scandinavian walnut base & ceramic gourd",
                "Stepped base with double articulated arms",
                "Polished base with luminous halo ring"
            };

            List<Button> lampBtns = new List<Button>();
            List<Image> highlights = new List<Image>();
            List<GameObject> activeBadges = new List<GameObject>();

            for (int i = 0; i < 4; i++)
            {
                float yPos = -55 - (i * 92);
                GameObject card = CreateUIPanel(leftPanel.transform, $"Card_{i}", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, yPos), new Vector2(242, 80), cardBg, cardSpr);
                Button b = card.AddComponent<Button>();
                lampBtns.Add(b);

                // Golden left indicator strip
                GameObject high = CreateUIPanel(card.transform, "Highlight", new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 0.5f), new Vector2(2, 0), new Vector2(4, 0), accentGold, null);
                highlights.Add(high.GetComponent<Image>());
                high.SetActive(i == 0);

                // Title
                Text cardTitle = CreateUIText(card.transform, "Title", $"<b>{lampNames[i]}</b>", 13, TextAnchor.MiddleLeft, textWhite);
                RectTransform ctr = cardTitle.GetComponent<RectTransform>();
                ctr.anchorMin = new Vector2(0, 0.5f);
                ctr.anchorMax = new Vector2(1, 1);
                ctr.anchoredPosition = new Vector2(16, -6);
                ctr.sizeDelta = new Vector2(-75, 0);

                // Active Badge
                GameObject actBadge = CreateUIPanel(card.transform, "ActiveBadge", new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-12, -10), new Vector2(48, 20), new Color(0.831f, 0.686f, 0.216f, 0.2f), ProceduralUIUtility.GetRoundedSprite(5, new Color(0.831f, 0.686f, 0.216f, 0.2f), accentGold, 1));
                Text abTxt = CreateUIText(actBadge.transform, "AbTxt", "Active", 10, TextAnchor.MiddleCenter, accentGold);
                FillParent(abTxt.GetComponent<RectTransform>());
                activeBadges.Add(actBadge);
                actBadge.SetActive(i == 0);

                // Subtitle description
                Text subText = CreateUIText(card.transform, "Sub", lampSubs[i], 11, TextAnchor.UpperLeft, textMuted);
                RectTransform subRt = subText.GetComponent<RectTransform>();
                subRt.anchorMin = new Vector2(0, 0);
                subRt.anchorMax = new Vector2(1, 0.5f);
                subRt.anchoredPosition = new Vector2(16, 6);
                subRt.sizeDelta = new Vector2(-32, 0);
            }

            // ----------------------------------------------------
            // 3. Right Customization Drawer
            // ----------------------------------------------------
            Button rightOpenBtn = CreateUIButton(canvasGO.transform, "RightOpenBtn", "Customize ⚙", new Vector2(1, 1), new Vector2(-75, -80), new Vector2(120, 36), surfaceBg, accentGold, ProceduralUIUtility.GetRoundedSprite(10, surfaceBg, borderCol, 1));
            rightOpenBtn.gameObject.SetActive(false);

            GameObject rightPanel = CreateUIPanel(canvasGO.transform, "RightPanel_Customize", new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(1, 0.5f), new Vector2(-175, -20), new Vector2(320, 640), surfaceBg, panelSpr);

            // Header & Close
            Text custHeader = CreateUIText(rightPanel.transform, "Header", "<b>CUSTOMIZE DESIGN</b>", 12, TextAnchor.MiddleLeft, accentGold);
            RectTransform cuhRt = custHeader.GetComponent<RectTransform>();
            cuhRt.anchorMin = new Vector2(0, 1);
            cuhRt.anchorMax = new Vector2(1, 1);
            cuhRt.pivot = new Vector2(0.5f, 1);
            cuhRt.anchoredPosition = new Vector2(16, -14);
            cuhRt.sizeDelta = new Vector2(-48, 26);

            Button rightCloseBtn = CreateUIButton(rightPanel.transform, "RightCloseBtn", "✕", new Vector2(1, 1), new Vector2(-22, -14), new Vector2(26, 26), Color.clear, textMuted, null);

            // 5 Tab Buttons: Scale, Finish, Color, Light, Presets
            string[] tabNames = { "Scale", "Finish", "Color", "Light", "Presets" };
            List<Button> tabButtons = new List<Button>();
            List<Text> tabTexts = new List<Text>();
            List<GameObject> tabContents = new List<GameObject>();

            for (int i = 0; i < 5; i++)
            {
                float x = -116 + (i * 58);
                Button tabBtn = CreateUIButton(rightPanel.transform, $"TabBtn_{i}", tabNames[i], new Vector2(0.5f, 1), new Vector2(x, -50), new Vector2(52, 28), (i == 0 ? new Color(0.831f, 0.686f, 0.216f, 0.2f) : cardBg), (i == 0 ? accentGold : textMuted), ProceduralUIUtility.GetRoundedSprite(6, cardBg, borderCol, 1));
                tabButtons.Add(tabBtn);
                tabTexts.Add(tabBtn.GetComponentInChildren<Text>());

                GameObject tabContent = CreateUIPanel(rightPanel.transform, $"TabContent_{i}", new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f), new Vector2(0, -42), new Vector2(-24, -140), Color.clear, null);
                tabContents.Add(tabContent);
                tabContent.SetActive(i == 0);
            }

            // Tab 0: Scale Sliders (with real-time value text labels)
            Text hVal, bwVal, swVal, shVal;
            Slider hSlider = CreateLabeledSlider(tabContents[0].transform, "Overall Height", 0.7f, 1.4f, 1.0f, -20, out hVal);
            Slider bwSlider = CreateLabeledSlider(tabContents[0].transform, "Base Width", 0.7f, 1.4f, 1.0f, -75, out bwVal);
            Slider swSlider = CreateLabeledSlider(tabContents[0].transform, "Shade Width", 0.7f, 1.4f, 1.0f, -130, out swVal);
            Slider shSlider = CreateLabeledSlider(tabContents[0].transform, "Shade Height", 0.7f, 1.4f, 1.0f, -185, out shVal);
            Dropdown styleDropdown = CreateUIDropdown(tabContents[0].transform, "StyleDropdown", new Vector2(0.5f, 1), new Vector2(0, -250), new Vector2(260, 34), new string[] { "Style 1: Tapered Frustum", "Style 2: Fluted / Drum" }, btnSpr);

            // Tab 1: Finish (Materials)
            Dropdown baseMatDrop = CreateUIDropdown(tabContents[1].transform, "BaseMatDrop", new Vector2(0.5f, 1), new Vector2(0, -30), new Vector2(260, 34), new string[] { "Base: Brushed Metal", "Base: Polished Chrome", "Base: Matte Ceramic", "Base: Walnut Wood", "Base: Frosted Glass", "Base: Woven Fabric" }, btnSpr);
            Dropdown standMatDrop = CreateUIDropdown(tabContents[1].transform, "StandMatDrop", new Vector2(0.5f, 1), new Vector2(0, -90), new Vector2(260, 34), new string[] { "Stand: Brushed Metal", "Stand: Polished Chrome", "Stand: Matte Ceramic", "Stand: Walnut Wood" }, btnSpr);
            Dropdown shadeMatDrop = CreateUIDropdown(tabContents[1].transform, "ShadeMatDrop", new Vector2(0.5f, 1), new Vector2(0, -150), new Vector2(260, 34), new string[] { "Shade: Woven Linen Fabric", "Shade: Frosted Glass", "Shade: Brushed Brass", "Shade: Matte Ceramic" }, btnSpr);

            // Tab 2: Color
            Dropdown colorTargetDrop = CreateUIDropdown(tabContents[2].transform, "ColorTarget", new Vector2(0.5f, 1), new Vector2(0, -20), new Vector2(260, 32), new string[] { "Target: Base Component", "Target: Stand Component", "Target: Lampshade Component" }, btnSpr);

            // Swatches grid (6 curated designer swatches)
            List<Button> swatches = new List<Button>();
            Color[] swatchColors = ColorPalettePresets.CuratedBaseColors;
            for (int i = 0; i < swatchColors.Length; i++)
            {
                float x = -95 + (i % 3) * 95;
                float y = -65 - (i / 3) * 36;
                Button sw = CreateUIButton(tabContents[2].transform, $"Swatch_{i}", "", new Vector2(0.5f, 1), new Vector2(x, y), new Vector2(85, 28), swatchColors[i], Color.white, ProceduralUIUtility.GetRoundedSprite(6, swatchColors[i], borderCol, 1));
                swatches.Add(sw);
            }

            Text rVal, gVal, bVal;
            Slider rSlider = CreateLabeledSlider(tabContents[2].transform, "Red", 0f, 1f, 0.85f, -150, out rVal);
            Slider gSlider = CreateLabeledSlider(tabContents[2].transform, "Green", 0f, 1f, 0.70f, -200, out gVal);
            Slider bSlider = CreateLabeledSlider(tabContents[2].transform, "Blue", 0f, 1f, 0.40f, -250, out bVal);
            GameObject previewBox = CreateUIPanel(tabContents[2].transform, "PreviewBox", new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -305), new Vector2(260, 26), new Color(0.85f, 0.70f, 0.40f), ProceduralUIUtility.GetRoundedSprite(6, new Color(0.85f, 0.70f, 0.40f), borderCol, 1));
            Image previewImg = previewBox.GetComponent<Image>();

            // Tab 3: Light
            Button powerBtn = CreateUIButton(tabContents[3].transform, "PowerBtn", "POWER ON", new Vector2(0.5f, 1), new Vector2(0, -25), new Vector2(260, 34), accentGold, Color.black, ProceduralUIUtility.GetRoundedSprite(8, accentGold, borderCol, 1));
            Text brightVal;
            Slider brightSlider = CreateLabeledSlider(tabContents[3].transform, "Illumination Brightness", 0.1f, 2.5f, 1.0f, -75, out brightVal);
            Button warmBtn = CreateUIButton(tabContents[3].transform, "WarmBtn", "Warm White  (2700 K)", new Vector2(0.5f, 1), new Vector2(0, -140), new Vector2(260, 34), new Color(0.95f, 0.75f, 0.55f, 0.2f), new Color(1f, 0.85f, 0.6f), ProceduralUIUtility.GetRoundedSprite(8, new Color(0.95f, 0.75f, 0.55f, 0.2f), new Color(1f, 0.85f, 0.6f), 1));
            Button neutralBtn = CreateUIButton(tabContents[3].transform, "NeutralBtn", "Neutral White  (4000 K)", new Vector2(0.5f, 1), new Vector2(0, -185), new Vector2(260, 34), new Color(0.95f, 0.90f, 0.82f, 0.2f), textWhite, ProceduralUIUtility.GetRoundedSprite(8, cardBg, borderCol, 1));
            Button coolBtn = CreateUIButton(tabContents[3].transform, "CoolBtn", "Cool White  (6500 K)", new Vector2(0.5f, 1), new Vector2(0, -230), new Vector2(260, 34), new Color(0.85f, 0.92f, 1.0f, 0.2f), new Color(0.8f, 0.92f, 1f), ProceduralUIUtility.GetRoundedSprite(8, cardBg, borderCol, 1));

            // Tab 4: Presets
            string[] presetNames = { "🕯️ Cozy Evening", "⬜ Modern Minimal", "👑 Luxury Gold", "⚙️ Industrial Black", "🌈 Colorful Ambient" };
            List<Button> presetButtons = new List<Button>();
            for (int i = 0; i < 5; i++)
            {
                Button pb = CreateUIButton(tabContents[4].transform, $"Preset_{i}", presetNames[i], new Vector2(0.5f, 1), new Vector2(0, -25 - (i * 48)), new Vector2(260, 38), cardBg, textWhite, cardSpr);
                presetButtons.Add(pb);
            }

            // Save & Load at Bottom of Right Drawer
            Button saveBtn = CreateUIButton(rightPanel.transform, "SaveBtn", "💾 Save Design", new Vector2(0.5f, 0), new Vector2(-70, 30), new Vector2(125, 36), new Color(0.12f, 0.42f, 0.24f), Color.white, ProceduralUIUtility.GetRoundedSprite(8, new Color(0.12f, 0.42f, 0.24f), Color.clear, 0));
            Button loadBtn = CreateUIButton(rightPanel.transform, "LoadBtn", "📂 Load Design", new Vector2(0.5f, 0), new Vector2(70, 30), new Vector2(125, 36), cardBg, textWhite, cardSpr);

            // Toast banner
            GameObject notifGO = CreateUIPanel(canvasGO.transform, "ToastBanner", new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 110), new Vector2(360, 36), surfaceBg, pillSpr);
            Text notifText = CreateUIText(notifGO.transform, "Msg", "Design Saved", 13, TextAnchor.MiddleCenter, accentGold);
            FillParent(notifText.GetComponent<RectTransform>());
            notifGO.SetActive(false);

            // ----------------------------------------------------
            // 4. Floating Bottom Bar (Modern Pill Container)
            // ----------------------------------------------------
            GameObject botBar = CreateUIPanel(canvasGO.transform, "BottomBar", new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 36), new Vector2(860, 48), surfaceBg, pillSpr);

            Text viewLbl = CreateUIText(botBar.transform, "ViewLbl", "VIEW:", 11, TextAnchor.MiddleLeft, textMuted);
            RectTransform vlRt = viewLbl.GetComponent<RectTransform>();
            vlRt.anchorMin = new Vector2(0, 0.5f);
            vlRt.anchorMax = new Vector2(0, 0.5f);
            vlRt.anchoredPosition = new Vector2(20, 0);
            vlRt.sizeDelta = new Vector2(38, 24);

            Button v34 = CreateUIButton(botBar.transform, "V34", "3/4 View", new Vector2(0, 0.5f), new Vector2(74, 0), new Vector2(68, 30), cardBg, textWhite, btnSpr);
            Button vFr = CreateUIButton(botBar.transform, "VFr", "Front", new Vector2(0, 0.5f), new Vector2(146, 0), new Vector2(58, 30), cardBg, textWhite, btnSpr);
            Button vSd = CreateUIButton(botBar.transform, "VSd", "Side", new Vector2(0, 0.5f), new Vector2(209, 0), new Vector2(54, 30), cardBg, textWhite, btnSpr);
            Button vTp = CreateUIButton(botBar.transform, "VTp", "Top", new Vector2(0, 0.5f), new Vector2(268, 0), new Vector2(52, 30), cardBg, textWhite, btnSpr);
            Button vRs = CreateUIButton(botBar.transform, "VRs", "Reset", new Vector2(0, 0.5f), new Vector2(326, 0), new Vector2(54, 30), cardBg, textWhite, btnSpr);

            Text zoomLbl = CreateUIText(botBar.transform, "ZoomLbl", "ZOOM:", 11, TextAnchor.MiddleLeft, textMuted);
            RectTransform zlRt = zoomLbl.GetComponent<RectTransform>();
            zlRt.anchorMin = new Vector2(0, 0.5f);
            zlRt.anchorMax = new Vector2(0, 0.5f);
            zlRt.anchoredPosition = new Vector2(395, 0);
            zlRt.sizeDelta = new Vector2(40, 24);

            Button vZmOut = CreateUIButton(botBar.transform, "VZmOut", "－ Zoom", new Vector2(0, 0.5f), new Vector2(456, 0), new Vector2(62, 30), cardBg, textWhite, btnSpr);
            Button vZmIn = CreateUIButton(botBar.transform, "VZmIn", "＋ Zoom", new Vector2(0, 0.5f), new Vector2(524, 0), new Vector2(62, 30), cardBg, textWhite, btnSpr);

            Toggle rotToggle = CreateUIToggle(botBar.transform, "RotToggle", "Auto-Rotate", new Vector2(0, 0.5f), new Vector2(635, 0), new Vector2(105, 30));
            Button pwrBtn = CreateUIButton(botBar.transform, "QuickPowerBtn", "💡 Power", new Vector2(1, 0.5f), new Vector2(-55, 0), new Vector2(85, 32), accentGold, Color.black, ProceduralUIUtility.GetRoundedSprite(16, accentGold, Color.clear, 0));

            // ----------------------------------------------------
            // 5. Case Study 2 Modal Dialog
            // ----------------------------------------------------
            GameObject techOverlay = CreateUIPanel(canvasGO.transform, "TechDemoOverlay", new Vector2(0, 0), new Vector2(1, 1), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0, 0, 0, 0.75f), null);
            techOverlay.SetActive(false);

            GameObject techPanel = CreateUIPanel(techOverlay.transform, "TechDemoModal", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(740, 560), surfaceBg, panelSpr);

            Text techTitle = CreateUIText(techPanel.transform, "TechTitle", "<b>CASE STUDY 2 — GRAPHICS ARCHITECTURE & B-REP SOLID</b>", 15, TextAnchor.MiddleLeft, accentGold);
            RectTransform ttRect = techTitle.GetComponent<RectTransform>();
            ttRect.anchorMin = new Vector2(0, 1);
            ttRect.anchorMax = new Vector2(1, 1);
            ttRect.anchoredPosition = new Vector2(24, -20);
            ttRect.sizeDelta = new Vector2(-60, 30);

            Button closeTechBtn = CreateUIButton(techPanel.transform, "CloseTechBtn", "✕ Close", new Vector2(1, 1), new Vector2(-45, -20), new Vector2(70, 28), new Color(0.3f, 0.15f, 0.15f), textWhite, ProceduralUIUtility.GetRoundedSprite(6, new Color(0.3f, 0.15f, 0.15f), borderCol, 1));

            // Explanation Card
            GameObject explCard = CreateUIPanel(techPanel.transform, "ExplCard", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1), new Vector2(0, -60), new Vector2(-48, 85), cardBg, cardSpr);
            Text conceptExplanation = CreateUIText(explCard.transform, "Explanation", "Explanation of mathematical modeling and projection.", 11, TextAnchor.UpperLeft, textWhite);
            FillParent(conceptExplanation.GetComponent<RectTransform>(), 10);

            Button modeLatheBtn = CreateUIButton(techPanel.transform, "ModeLathe", "A. 2D CAD Lathe", new Vector2(0, 1), new Vector2(110, -165), new Vector2(160, 32), new Color(0.831f, 0.686f, 0.216f, 0.2f), accentGold, ProceduralUIUtility.GetRoundedSprite(6, new Color(0.831f, 0.686f, 0.216f, 0.2f), accentGold, 1));
            Button modeBRepBtn = CreateUIButton(techPanel.transform, "ModeBRep", "B. B-Rep Solid", new Vector2(0, 1), new Vector2(285, -165), new Vector2(160, 32), cardBg, textWhite, cardSpr);
            Toggle wireToggle = CreateUIToggle(techPanel.transform, "WireToggle", "Wireframe Mode", new Vector2(0, 1), new Vector2(470, -165), new Vector2(140, 32));

            Text tessVal;
            Slider tessSlider = CreateLabeledSlider(techPanel.transform, "Tessellation (Radial N: 6..48)", 6f, 48f, 24f, -210, out tessVal);
            Text scVal;
            Slider scaleSlider = CreateLabeledSlider(techPanel.transform, "Matrix Scale (S_xyz)", 0.5f, 2.0f, 1.0f, -255, out scVal);
            Text roVal;
            Slider rotSlider = CreateLabeledSlider(techPanel.transform, "Matrix Rotation (θ_y)", 0f, 360f, 0f, -300, out roVal);
            Text fovVal;
            Slider fovSlider = CreateLabeledSlider(techPanel.transform, "Viewing FOV (15°..90°)", 15f, 90f, 45f, -345, out fovVal);

            // Monospace Matrix Box
            GameObject matBox = CreateUIPanel(techPanel.transform, "MatrixBox", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0), new Vector2(0, 75), new Vector2(-48, 105), new Color(0.04f, 0.05f, 0.06f, 0.95f), ProceduralUIUtility.GetRoundedSprite(8, new Color(0.04f, 0.05f, 0.06f, 0.95f), borderCol, 1));
            Text matText = CreateUIText(matBox.transform, "MatrixText", "Matrix M = T · R · S", 11, TextAnchor.UpperLeft, new Color(0.3f, 1.0f, 0.5f));
            FillParent(matText.GetComponent<RectTransform>(), 10);

            Text perfText = CreateUIText(techPanel.transform, "PerfText", "FPS: 60 | Draw calls: 14", 11, TextAnchor.MiddleRight, accentGold);
            RectTransform ptRect = perfText.GetComponent<RectTransform>();
            ptRect.anchorMin = new Vector2(0, 0);
            ptRect.anchorMax = new Vector2(1, 0);
            ptRect.anchoredPosition = new Vector2(-24, 18);
            ptRect.sizeDelta = new Vector2(-48, 24);

            // Wire UI Controller references
            SetField(uiCtrl, "_selectedLampTitleText", lampTitleText);
            SetField(uiCtrl, "_environmentDropdown", envDropdown);
            SetField(uiCtrl, "_resetDefaultsBtn", resetBtn);
            SetField(uiCtrl, "_techDemoToggleBtn", techBtn);

            SetField(uiCtrl, "_leftPanel", leftPanel);
            SetField(uiCtrl, "_leftPanelCloseBtn", leftCloseBtn);
            SetField(uiCtrl, "_leftPanelOpenBtn", leftOpenBtn);
            SetField(uiCtrl, "_lampSelectButtons", lampBtns.ToArray());
            SetField(uiCtrl, "_lampSelectHighlights", highlights.ToArray());
            SetField(uiCtrl, "_lampActiveBadges", activeBadges.ToArray());

            SetField(uiCtrl, "_rightPanel", rightPanel);
            SetField(uiCtrl, "_rightPanelCloseBtn", rightCloseBtn);
            SetField(uiCtrl, "_rightPanelOpenBtn", rightOpenBtn);
            SetField(uiCtrl, "_tabContents", tabContents.ToArray());
            SetField(uiCtrl, "_tabButtons", tabButtons.ToArray());
            SetField(uiCtrl, "_tabButtonTexts", tabTexts.ToArray());

            SetField(uiCtrl, "_heightSlider", hSlider);
            SetField(uiCtrl, "_baseWidthSlider", bwSlider);
            SetField(uiCtrl, "_shadeWidthSlider", swSlider);
            SetField(uiCtrl, "_shadeHeightSlider", shSlider);
            SetField(uiCtrl, "_shadeStyleDropdown", styleDropdown);
            SetField(uiCtrl, "_heightValText", hVal);
            SetField(uiCtrl, "_baseWidthValText", bwVal);
            SetField(uiCtrl, "_shadeWidthValText", swVal);
            SetField(uiCtrl, "_shadeHeightValText", shVal);

            SetField(uiCtrl, "_baseMatDropdown", baseMatDrop);
            SetField(uiCtrl, "_standMatDropdown", standMatDrop);
            SetField(uiCtrl, "_shadeMatDropdown", shadeMatDrop);

            SetField(uiCtrl, "_colorTargetDropdown", colorTargetDrop);
            SetField(uiCtrl, "_swatchButtons", swatches.ToArray());
            SetField(uiCtrl, "_redSlider", rSlider);
            SetField(uiCtrl, "_greenSlider", gSlider);
            SetField(uiCtrl, "_blueSlider", bSlider);
            SetField(uiCtrl, "_redValText", rVal);
            SetField(uiCtrl, "_greenValText", gVal);
            SetField(uiCtrl, "_blueValText", bVal);
            SetField(uiCtrl, "_colorPreviewImage", previewImg);

            SetField(uiCtrl, "_lightPowerBtn", powerBtn);
            SetField(uiCtrl, "_lightPowerBtnText", powerBtn.GetComponentInChildren<Text>());
            SetField(uiCtrl, "_brightnessSlider", brightSlider);
            SetField(uiCtrl, "_brightnessValText", brightVal);
            SetField(uiCtrl, "_warmWhiteBtn", warmBtn);
            SetField(uiCtrl, "_neutralWhiteBtn", neutralBtn);
            SetField(uiCtrl, "_coolWhiteBtn", coolBtn);

            SetField(uiCtrl, "_presetButtons", presetButtons.ToArray());
            SetField(uiCtrl, "_saveBtn", saveBtn);
            SetField(uiCtrl, "_loadBtn", loadBtn);
            SetField(uiCtrl, "_notificationText", notifText);

            SetField(uiCtrl, "_view34Btn", v34);
            SetField(uiCtrl, "_viewFrontBtn", vFr);
            SetField(uiCtrl, "_viewSideBtn", vSd);
            SetField(uiCtrl, "_viewTopBtn", vTp);
            SetField(uiCtrl, "_viewResetBtn", vRs);
            SetField(uiCtrl, "_zoomInBtn", vZmIn);
            SetField(uiCtrl, "_zoomOutBtn", vZmOut);
            SetField(uiCtrl, "_autoRotateToggle", rotToggle);
            SetField(uiCtrl, "_quickPowerBtn", pwrBtn);

            SetField(uiCtrl, "_techDemoPanel", techOverlay);
            SetField(uiCtrl, "_techDemoCloseBtn", closeTechBtn);

            return uiCtrl;
        }

        private static void WireUIReferences(LumiLabUIController uiCtrl, LampBase[] lamps, ShowroomCameraController camCtrl, TechDemoController techCtrl)
        {
            GameObject techOverlay = GetField<GameObject>(uiCtrl, "_techDemoPanel");
            if (techOverlay != null && techCtrl != null)
            {
                Button latheBtn = techOverlay.transform.Find("TechDemoModal/ModeLathe")?.GetComponent<Button>();
                Button brepBtn = techOverlay.transform.Find("TechDemoModal/ModeBRep")?.GetComponent<Button>();
                Toggle wireTog = techOverlay.transform.Find("TechDemoModal/WireToggle")?.GetComponent<Toggle>();

                latheBtn?.onClick.AddListener(() => techCtrl.SetDemoMode(0));
                brepBtn?.onClick.AddListener(() => techCtrl.SetDemoMode(1));
                wireTog?.onValueChanged.AddListener(w => techCtrl.ToggleWireframe(w));

                Slider tess = techOverlay.transform.Find("TechDemoModal/Tessellation (Radial N: 6..48)")?.GetComponentInChildren<Slider>();
                Slider sc = techOverlay.transform.Find("TechDemoModal/Matrix Scale (S_xyz)")?.GetComponentInChildren<Slider>();
                Slider ro = techOverlay.transform.Find("TechDemoModal/Matrix Rotation (θ_y)")?.GetComponentInChildren<Slider>();
                Slider fov = techOverlay.transform.Find("TechDemoModal/Viewing FOV (15°..90°)")?.GetComponentInChildren<Slider>();

                tess?.onValueChanged.AddListener(v => techCtrl.SetRadialTessellation(v));
                sc?.onValueChanged.AddListener(v => techCtrl.SetScaleUniform(v));
                ro?.onValueChanged.AddListener(v => techCtrl.SetRotationY(v));
                fov?.onValueChanged.AddListener(v => techCtrl.SetCameraFOV(v));

                SetField(techCtrl, "_matrixDisplayText", techOverlay.transform.Find("TechDemoModal/MatrixBox/MatrixText")?.GetComponent<Text>());
                SetField(techCtrl, "_performanceDisplayText", techOverlay.transform.Find("TechDemoModal/PerfText")?.GetComponent<Text>());
                SetField(techCtrl, "_conceptExplanationText", techOverlay.transform.Find("TechDemoModal/ExplCard/Explanation")?.GetComponent<Text>());
            }
        }

        #region Helpers

        private static GameObject CreateUIPanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 pos, Vector2 size, Color color, Sprite sprite = null)
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
            if (sprite != null)
            {
                img.sprite = sprite;
                img.type = Image.Type.Sliced;
            }
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
            if (defaultFont == null) defaultFont = Font.CreateDynamicFontFromOSFont("Segoe UI", fontSize);
            if (defaultFont == null) defaultFont = Font.CreateDynamicFontFromOSFont("Arial", fontSize);
            txt.font = defaultFont;
            txt.fontSize = fontSize;
            txt.alignment = alignment;
            txt.color = color;
            txt.supportRichText = true;
            return txt;
        }

        private static Button CreateUIButton(Transform parent, string name, string label, Vector2 anchor, Vector2 pos, Vector2 size, Color bgCol, Color txtCol, Sprite sprite = null)
        {
            GameObject go = CreateUIPanel(parent, name, anchor, anchor, anchor, pos, size, bgCol, sprite);
            Button btn = go.AddComponent<Button>();

            ColorBlock cb = btn.colors;
            cb.normalColor = bgCol;
            cb.highlightedColor = bgCol * 1.25f;
            cb.pressedColor = bgCol * 0.8f;
            btn.colors = cb;

            if (!string.IsNullOrEmpty(label))
            {
                Text t = CreateUIText(go.transform, "Text", label, 12, TextAnchor.MiddleCenter, txtCol);
                FillParent(t.GetComponent<RectTransform>());
            }

            return btn;
        }

        private static Slider CreateLabeledSlider(Transform parent, string label, float min, float max, float val, float yPos, out Text valText)
        {
            GameObject root = new GameObject(label);
            root.transform.SetParent(parent, false);
            RectTransform rootRt = root.AddComponent<RectTransform>();
            rootRt.anchorMin = new Vector2(0.5f, 1);
            rootRt.anchorMax = new Vector2(0.5f, 1);
            rootRt.pivot = new Vector2(0.5f, 1);
            rootRt.anchoredPosition = new Vector2(0, yPos);
            rootRt.sizeDelta = new Vector2(260, 42);

            // Left title label
            Text lbl = CreateUIText(root.transform, "Label", label, 11, TextAnchor.UpperLeft, new Color(0.85f, 0.85f, 0.85f));
            RectTransform lblRt = lbl.GetComponent<RectTransform>();
            lblRt.anchorMin = new Vector2(0, 1);
            lblRt.anchorMax = new Vector2(0.7f, 1);
            lblRt.anchoredPosition = new Vector2(0, -2);
            lblRt.sizeDelta = new Vector2(0, 16);

            // Right live value text (e.g. 1.00x)
            valText = CreateUIText(root.transform, "ValText", $"{val:F2}x", 11, TextAnchor.UpperRight, new Color(0.831f, 0.686f, 0.216f));
            RectTransform vtRt = valText.GetComponent<RectTransform>();
            vtRt.anchorMin = new Vector2(0.7f, 1);
            vtRt.anchorMax = new Vector2(1, 1);
            vtRt.anchoredPosition = new Vector2(0, -2);
            vtRt.sizeDelta = new Vector2(0, 16);

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

            // Track background
            CreateUIPanel(sliderGO.transform, "Background", new Vector2(0, 0.35f), new Vector2(1, 0.65f), new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.18f, 0.20f, 0.24f), ProceduralUIUtility.GetRoundedSprite(4, new Color(0.18f, 0.20f, 0.24f), Color.clear, 0));

            // Fill
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderGO.transform, false);
            RectTransform faRt = fillArea.AddComponent<RectTransform>();
            faRt.anchorMin = new Vector2(0, 0.35f);
            faRt.anchorMax = new Vector2(1, 0.65f);
            faRt.sizeDelta = Vector2.zero;

            GameObject fill = CreateUIPanel(fillArea.transform, "Fill", new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 0.5f), Vector2.zero, Vector2.zero, new Color(0.831f, 0.686f, 0.216f), ProceduralUIUtility.GetRoundedSprite(4, new Color(0.831f, 0.686f, 0.216f), Color.clear, 0));
            slider.fillRect = fill.GetComponent<RectTransform>();

            // Handle Knob
            GameObject handleArea = new GameObject("Handle Slide Area");
            handleArea.transform.SetParent(sliderGO.transform, false);
            RectTransform haRt = handleArea.AddComponent<RectTransform>();
            haRt.anchorMin = Vector2.zero;
            haRt.anchorMax = Vector2.one;
            haRt.sizeDelta = Vector2.zero;

            GameObject handle = CreateUIPanel(handleArea.transform, "Handle", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(16, 16), Color.white, ProceduralUIUtility.GetRoundedSprite(8, Color.white, new Color(0.831f, 0.686f, 0.216f), 2));
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = handle.GetComponent<Image>();

            return slider;
        }

        private static Dropdown CreateUIDropdown(Transform parent, string name, Vector2 anchor, Vector2 pos, Vector2 size, string[] options, Sprite sprite = null)
        {
            GameObject go = CreateUIPanel(parent, name, anchor, anchor, anchor, pos, size, new Color(0.12f, 0.14f, 0.17f), sprite);
            Dropdown dd = go.AddComponent<Dropdown>();

            Text caption = CreateUIText(go.transform, "Label", options != null && options.Length > 0 ? options[0] : "", 11, TextAnchor.MiddleLeft, Color.white);
            RectTransform capRt = caption.GetComponent<RectTransform>();
            capRt.anchorMin = Vector2.zero;
            capRt.anchorMax = Vector2.one;
            capRt.anchoredPosition = new Vector2(12, 0);
            capRt.sizeDelta = new Vector2(-24, 0);
            dd.captionText = caption;

            GameObject template = CreateUIPanel(go.transform, "Template", new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 1), new Vector2(0, -4), new Vector2(0, 140), new Color(0.08f, 0.09f, 0.11f, 0.98f), ProceduralUIUtility.GetRoundedSprite(8, new Color(0.08f, 0.09f, 0.11f, 0.98f), new Color(0.2f, 0.22f, 0.26f), 1));
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
            cRt.sizeDelta = new Vector2(0, 30 * (options != null ? options.Length : 1));

            GameObject item = CreateUIPanel(content.transform, "Item", new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(0, 28), Color.clear, null);
            item.AddComponent<Toggle>();
            Text itemText = CreateUIText(item.transform, "Item Text", "Option", 11, TextAnchor.MiddleLeft, Color.white);
            FillParent(itemText.GetComponent<RectTransform>(), 12);
            dd.itemText = itemText;

            dd.options.Clear();
            if (options != null)
            {
                foreach (string opt in options) dd.options.Add(new Dropdown.OptionData(opt));
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

            GameObject box = CreateUIPanel(go.transform, "Background", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0), new Vector2(18, 18), new Color(0.18f, 0.20f, 0.24f), ProceduralUIUtility.GetRoundedSprite(4, new Color(0.18f, 0.20f, 0.24f), new Color(0.3f, 0.32f, 0.38f), 1));
            GameObject checkmark = CreateUIPanel(box.transform, "Checkmark", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(10, 10), new Color(0.831f, 0.686f, 0.216f), ProceduralUIUtility.GetRoundedSprite(2, new Color(0.831f, 0.686f, 0.216f), Color.clear, 0));
            toggle.graphic = checkmark.GetComponent<Image>();
            toggle.targetGraphic = box.GetComponent<Image>();
            toggle.isOn = true;

            Text t = CreateUIText(go.transform, "Label", label, 11, TextAnchor.MiddleLeft, Color.white);
            RectTransform trt = t.GetComponent<RectTransform>();
            trt.anchorMin = new Vector2(0, 0);
            trt.anchorMax = new Vector2(1, 1);
            trt.anchoredPosition = new Vector2(26, 0);
            trt.sizeDelta = new Vector2(-26, 0);

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

        private static void SetField(object target, string fieldName, object value)
        {
            if (target == null) return;
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null) field.SetValue(target, value);
            else
            {
                var baseField = target.GetType().BaseType?.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                baseField?.SetValue(target, value);
            }
        }

        private static T GetField<T>(object target, string fieldName) where T : class
        {
            if (target == null) return null;
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null) return field.GetValue(target) as T;
            var baseField = target.GetType().BaseType?.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return baseField?.GetValue(target) as T;
        }

        private static T FindFirstObject<T>() where T : UnityEngine.Object
        {
#if UNITY_2023_1_OR_NEWER
            return UnityEngine.Object.FindAnyObjectByType<T>();
#else
            return UnityEngine.Object.FindObjectOfType<T>();
#endif
        }

        #endregion
    }
}

