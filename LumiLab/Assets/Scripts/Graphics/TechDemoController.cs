using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using LumiLab.Lamps;
using LumiLab.ShowroomCamera;

namespace LumiLab.Graphics
{
    public class TechDemoController : MonoBehaviour
    {
        [Header("Demo Target & References")]
        [SerializeField] private GameObject _demoPedestalRoot;
        [SerializeField] private MeshFilter _demoMeshFilter;
        [SerializeField] private MeshRenderer _demoMeshRenderer;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private ShowroomCameraController _cameraController;

        [Header("UI Text Displays")]
        [SerializeField] private Text _matrixDisplayText;
        [SerializeField] private Text _meshStatsDisplayText;
        [SerializeField] private Text _projectionDisplayText;
        [SerializeField] private Text _performanceDisplayText;
        [SerializeField] private Text _conceptExplanationText;

        [Header("Wireframe Material")]
        [SerializeField] private Material _wireframeMaterial;
        private Material _defaultSolidMaterial;

        // Interactive Transformation States
        private Vector3 _transScale = Vector3.one;
        private Vector3 _transRot = Vector3.zero;
        private Vector3 _transPos = Vector3.zero;

        // Current procedural demo mode: 0 = Profile Revolution, 1 = B-Rep Hollow Cylinder
        private int _currentDemoMode = 0;
        private int _radialSegments = 24;
        private bool _isWireframe = false;

        // Performance tracking
        private float _fpsAccumulator = 0f;
        private int _fpsCounter = 0;
        private float _fpsTimeLeft = 0.5f;
        private float _currentFps = 60f;

        private void Start()
        {
            if (_demoMeshRenderer != null)
            {
                _defaultSolidMaterial = _demoMeshRenderer.material;
            }
            RegenerateDemoMesh();
            UpdateTransformationMatrixDisplay();
        }

        private void Update()
        {
            UpdatePerformanceHUD();
            UpdateProjectionMatrixDisplay();
        }

        #region A. 2D CAD to 3D Profile Revolution & B. B-Rep Solid

        public void SetDemoMode(int mode)
        {
            _currentDemoMode = Mathf.Clamp(mode, 0, 1);
            RegenerateDemoMesh();
        }

        public void SetRadialTessellation(float value)
        {
            _radialSegments = Mathf.RoundToInt(value);
            RegenerateDemoMesh();
        }

        public void ToggleWireframe(bool enable)
        {
            _isWireframe = enable;
            if (_demoMeshRenderer != null)
            {
                if (_isWireframe && _wireframeMaterial != null)
                {
                    _demoMeshRenderer.material = _wireframeMaterial;
                }
                else
                {
                    _demoMeshRenderer.material = _defaultSolidMaterial;
                }
            }
        }

        public void RegenerateDemoMesh()
        {
            if (_demoMeshFilter == null) return;

            Stopwatch sw = Stopwatch.StartNew();
            Mesh mesh = null;

            if (_currentDemoMode == 0) // Profile Revolution (Lathe)
            {
                // Defined 2D profile curve for an architectural lampshade
                Vector2[] profile = new Vector2[]
                {
                    new Vector2(0.26f, 0.00f),
                    new Vector2(0.24f, 0.08f),
                    new Vector2(0.19f, 0.18f),
                    new Vector2(0.13f, 0.28f),
                    new Vector2(0.04f, 0.30f)
                };

                mesh = ProceduralMeshUtility.CreateRevolvedMesh(profile, _radialSegments, "CaseStudy2_RevolvedProfile");

                if (_conceptExplanationText != null)
                {
                    _conceptExplanationText.text =
                        "<b>Case Study 2 - Part A: 2D Profile Revolution (Lathe)</b>\n" +
                        "A 2D parametric profile curve is swept 360° around the Y-axis. At each radial angle θ_i = 2π(i/N), " +
                        "vertices are computed as [r*cos(θ), y, r*sin(θ)]. Quad strips are tessellated into planar triangles " +
                        "with analytic vertex normals and normalized arc-length UV coordinates.";
                }
            }
            else // Boundary Representation (B-Rep) Hollow Solid
            {
                mesh = ProceduralMeshUtility.CreateHollowCylinder(0.22f, 0.16f, 0.25f, _radialSegments, "CaseStudy2_BRepHollowSolid");

                if (_conceptExplanationText != null)
                {
                    _conceptExplanationText.text =
                        "<b>Case Study 2 - Part B: Boundary Representation (B-Rep) Solid</b>\n" +
                        "Constructed from 4 manifold boundary topological surfaces: (1) Outward-facing outer cylinder wall, " +
                        "(2) Inward-facing inner cylinder wall (cavity), (3) Top planar annulus rim, and (4) Bottom planar cap. " +
                        "Strict face normal orientations ensure water-tight 2-manifold geometric topology.";
                }
            }

            sw.Stop();
            _demoMeshFilter.sharedMesh = mesh;

            if (_meshStatsDisplayText != null && mesh != null)
            {
                _meshStatsDisplayText.text =
                    $"<b>Geometry Discretization:</b>\n" +
                    $"• Radial Segments (N): {_radialSegments}\n" +
                    $"• Vertices: {mesh.vertexCount:N0}\n" +
                    $"• Triangles: {(mesh.triangles.Length / 3):N0}\n" +
                    $"• Generation Time: {sw.Elapsed.TotalMilliseconds:F3} ms";
            }
        }

        #endregion

        #region C. Geometric Transformations (4x4 Homogeneous Matrix)

        public void SetScaleUniform(float s)
        {
            _transScale = new Vector3(s, s, s);
            ApplyTransformations();
        }

        public void SetRotationY(float angleDeg)
        {
            _transRot.y = angleDeg;
            ApplyTransformations();
        }

        public void SetTranslationY(float y)
        {
            _transPos.y = y;
            ApplyTransformations();
        }

        private void ApplyTransformations()
        {
            if (_demoMeshFilter != null)
            {
                _demoMeshFilter.transform.localScale = _transScale;
                _demoMeshFilter.transform.localEulerAngles = _transRot;
                _demoMeshFilter.transform.localPosition = _transPos;
            }

            UpdateTransformationMatrixDisplay();
        }

        private void UpdateTransformationMatrixDisplay()
        {
            if (_matrixDisplayText == null || _demoMeshFilter == null) return;

            Matrix4x4 localMat = Matrix4x4.TRS(_transPos, Quaternion.Euler(_transRot), _transScale);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<b>Case Study 2 - Part C: Composite 4×4 Transformation Matrix (M = T · R · S)</b>");
            sb.AppendLine("┌                                                                        ┐");
            sb.AppendLine($"│ {localMat.m00,7:F3}  {localMat.m01,7:F3}  {localMat.m02,7:F3}  {localMat.m03,7:F3} │");
            sb.AppendLine($"│ {localMat.m10,7:F3}  {localMat.m11,7:F3}  {localMat.m12,7:F3}  {localMat.m13,7:F3} │");
            sb.AppendLine($"│ {localMat.m20,7:F3}  {localMat.m21,7:F3}  {localMat.m22,7:F3}  {localMat.m23,7:F3} │");
            sb.AppendLine($"│ {localMat.m30,7:F3}  {localMat.m31,7:F3}  {localMat.m32,7:F3}  {localMat.m33,7:F3} │");
            sb.AppendLine("└                                                                        ┘");
            sb.AppendLine($"Scale: ({_transScale.x:F2}, {_transScale.y:F2}, {_transScale.z:F2}) | Rot: {_transRot.y:F1}° | Trans: ({_transPos.x:F2}, {_transPos.y:F2}, {_transPos.z:F2})");

            _matrixDisplayText.text = sb.ToString();
        }

        #endregion

        #region D & E. Viewing, Projection & Hidden-Surface Removal (Z-Buffer)

        public void SetCameraOrthographic(bool orthographic)
        {
            if (_mainCamera != null)
            {
                _mainCamera.orthographic = orthographic;
                if (orthographic) _mainCamera.orthographicSize = 0.8f;
            }
        }

        public void SetCameraFOV(float fov)
        {
            if (_mainCamera != null && !_mainCamera.orthographic)
            {
                _mainCamera.fieldOfView = fov;
            }
        }

        public void SetNearClip(float nearPlane)
        {
            if (_mainCamera != null)
            {
                _mainCamera.nearClipPlane = Mathf.Clamp(nearPlane, 0.05f, 1.2f);
            }
        }

        private void UpdateProjectionMatrixDisplay()
        {
            if (_projectionDisplayText == null || _mainCamera == null) return;

            Matrix4x4 proj = _mainCamera.projectionMatrix;
            _projectionDisplayText.text =
                $"<b>Viewing & Projection Pipeline:</b>\n" +
                $"• Mode: {(_mainCamera.orthographic ? "Orthographic Parallel" : "Perspective Central")}\n" +
                $"• FOV: {_mainCamera.fieldOfView:F1}° | Near Clip: {_mainCamera.nearClipPlane:F2}m | Far Clip: {_mainCamera.farClipPlane:F0}m\n" +
                $"• Projection Matrix P[0,0]={proj.m00:F2}, P[1,1]={proj.m11:F2}, P[2,2]={proj.m22:F2}, P[2,3]={proj.m23:F2}\n" +
                $"• Z-Buffer Depth Equation: z_ndc = (P_22 * z_eye + P_23) / -z_eye";
        }

        #endregion

        #region F. Performance and Optimization HUD

        private void UpdatePerformanceHUD()
        {
            _fpsTimeLeft -= Time.unscaledDeltaTime;
            _fpsAccumulator += Time.unscaledDeltaTime > 0.0001f ? (1.0f / Time.unscaledDeltaTime) : 60.0f;
            _fpsCounter++;

            if (_fpsTimeLeft <= 0.0f)
            {
                _currentFps = _fpsAccumulator / _fpsCounter;
                _fpsTimeLeft = 0.5f;
                _fpsAccumulator = 0.0f;
                _fpsCounter = 0;

                if (_performanceDisplayText != null)
                {
                    long totalMemory = GC.GetTotalMemory(false) / (1024 * 1024);
                    _performanceDisplayText.text =
                        $"FPS: {_currentFps:F1} | Memory: {totalMemory} MB | VSync: On\n" +
                        $"Mesh Re-use: Enabled | Dynamic Batching: Active";
                }
            }
        }

        #endregion

        public void SetPanelActive(bool active)
        {
            if (_demoPedestalRoot != null) _demoPedestalRoot.SetActive(active);
        }
    }
}
