using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LumiLab.Core;
using LumiLab.Graphics;
using LumiLab.ShowroomCamera;

namespace LumiLab.UI
{
    public class LumiLabUIController : MonoBehaviour
    {
        [Header("Top Bar")]
        [SerializeField] private Text _selectedLampTitleText;
        [SerializeField] private Dropdown _environmentDropdown;
        [SerializeField] private Button _resetDefaultsBtn;
        [SerializeField] private Button _techDemoToggleBtn;

        [Header("Left Panel - Collection Drawer")]
        [SerializeField] private GameObject _leftPanel;
        [SerializeField] private Button _leftPanelCloseBtn;
        [SerializeField] private Button _leftPanelOpenBtn;
        [SerializeField] private Button[] _lampSelectButtons; // 0: Aura, 1: Nordic, 2: Industrial, 3: Halo
        [SerializeField] private Image[] _lampSelectHighlights;
        [SerializeField] private GameObject[] _lampActiveBadges;

        [Header("Right Panel - Customization Drawer")]
        [SerializeField] private GameObject _rightPanel;
        [SerializeField] private Button _rightPanelCloseBtn;
        [SerializeField] private Button _rightPanelOpenBtn;

        [Header("Tabs in Right Panel")]
        [SerializeField] private GameObject[] _tabContents; // 0: Scale, 1: Finish, 2: Color, 3: Light, 4: Presets
        [SerializeField] private Button[] _tabButtons;
        [SerializeField] private Text[] _tabButtonTexts;

        [Header("Dimensions Controls")]
        [SerializeField] private Slider _heightSlider;
        [SerializeField] private Slider _baseWidthSlider;
        [SerializeField] private Slider _shadeWidthSlider;
        [SerializeField] private Slider _shadeHeightSlider;
        [SerializeField] private Dropdown _shadeStyleDropdown;
        [SerializeField] private Text _heightValText;
        [SerializeField] private Text _baseWidthValText;
        [SerializeField] private Text _shadeWidthValText;
        [SerializeField] private Text _shadeHeightValText;

        [Header("Materials Controls")]
        [SerializeField] private Dropdown _baseMatDropdown;
        [SerializeField] private Dropdown _standMatDropdown;
        [SerializeField] private Dropdown _shadeMatDropdown;

        [Header("Colors Controls")]
        [SerializeField] private Dropdown _colorTargetDropdown; // 0: Base, 1: Stand, 2: Shade
        [SerializeField] private Button[] _swatchButtons;
        [SerializeField] private Slider _redSlider;
        [SerializeField] private Slider _greenSlider;
        [SerializeField] private Slider _blueSlider;
        [SerializeField] private Text _redValText;
        [SerializeField] private Text _greenValText;
        [SerializeField] private Text _blueValText;
        [SerializeField] private Image _colorPreviewImage;

        [Header("Lighting Controls")]
        [SerializeField] private Button _lightPowerBtn;
        [SerializeField] private Text _lightPowerBtnText;
        [SerializeField] private Slider _brightnessSlider;
        [SerializeField] private Text _brightnessValText;
        [SerializeField] private Button _warmWhiteBtn;
        [SerializeField] private Button _neutralWhiteBtn;
        [SerializeField] private Button _coolWhiteBtn;

        [Header("Presets Controls")]
        [SerializeField] private Button[] _presetButtons; // 5 presets

        [Header("Save & Load")]
        [SerializeField] private Button _saveBtn;
        [SerializeField] private Button _loadBtn;
        [SerializeField] private Text _notificationText;

        [Header("Bottom Bar Controls")]
        [SerializeField] private Button _view34Btn;
        [SerializeField] private Button _viewFrontBtn;
        [SerializeField] private Button _viewSideBtn;
        [SerializeField] private Button _viewTopBtn;
        [SerializeField] private Button _viewResetBtn;
        [SerializeField] private Button _zoomInBtn;
        [SerializeField] private Button _zoomOutBtn;
        [SerializeField] private Toggle _autoRotateToggle;
        [SerializeField] private Button _quickPowerBtn;
        [SerializeField] private Image _quickPowerIcon;

        [Header("Tech Demo Panel (Case Study 2)")]
        [SerializeField] private GameObject _techDemoPanel;
        [SerializeField] private Button _techDemoCloseBtn;

        private float _notifyTimer = 0f;
        private int _currentTabIndex = 0;

        private void Start()
        {
            BindEventListeners();
            SubscribeToManagerEvents();

            if (LumiLabManager.Instance != null)
            {
                SyncUIFromConfig(LumiLabManager.Instance.CurrentConfig);
            }

            SelectTab(0);
        }

        private void Update()
        {
            if (_notifyTimer > 0f)
            {
                _notifyTimer -= Time.deltaTime;
                if (_notifyTimer <= 0f && _notificationText != null)
                {
                    _notificationText.transform.parent.gameObject.SetActive(false);
                }
            }
        }

        private void BindEventListeners()
        {
            // Drawer Open & Close toggles
            _leftPanelCloseBtn?.onClick.AddListener(() =>
            {
                _leftPanel?.SetActive(false);
                _leftPanelOpenBtn?.gameObject.SetActive(true);
            });
            _leftPanelOpenBtn?.onClick.AddListener(() =>
            {
                _leftPanel?.SetActive(true);
                _leftPanelOpenBtn?.gameObject.SetActive(false);
            });

            _rightPanelCloseBtn?.onClick.AddListener(() =>
            {
                _rightPanel?.SetActive(false);
                _rightPanelOpenBtn?.gameObject.SetActive(true);
            });
            _rightPanelOpenBtn?.onClick.AddListener(() =>
            {
                _rightPanel?.SetActive(true);
                _rightPanelOpenBtn?.gameObject.SetActive(false);
            });

            // Top Bar
            _environmentDropdown?.onValueChanged.AddListener(index =>
            {
                LumiLabManager.Instance?.SetEnvironment(index);
            });

            _resetDefaultsBtn?.onClick.AddListener(() =>
            {
                LumiLabManager.Instance?.ResetFactoryDefaults();
                ShowNotification("Factory defaults restored");
            });

            if (_techDemoToggleBtn != null && _techDemoPanel != null)
            {
                _techDemoToggleBtn.onClick.AddListener(() =>
                {
                    bool active = !_techDemoPanel.activeSelf;
                    _techDemoPanel.SetActive(active);
#if UNITY_2023_1_OR_NEWER
                    FindAnyObjectByType<TechDemoController>()?.SetPanelActive(active);
#else
                    FindObjectOfType<TechDemoController>()?.SetPanelActive(active);
#endif
                });
            }

            if (_techDemoCloseBtn != null && _techDemoPanel != null)
            {
                _techDemoCloseBtn.onClick.AddListener(() =>
                {
                    _techDemoPanel.SetActive(false);
#if UNITY_2023_1_OR_NEWER
                    FindAnyObjectByType<TechDemoController>()?.SetPanelActive(false);
#else
                    FindObjectOfType<TechDemoController>()?.SetPanelActive(false);
#endif
                });
            }

            // Lamp Selection Buttons
            if (_lampSelectButtons != null)
            {
                for (int i = 0; i < _lampSelectButtons.Length; i++)
                {
                    int index = i;
                    _lampSelectButtons[i]?.onClick.AddListener(() =>
                    {
                        LumiLabManager.Instance?.SelectLamp(index);
                    });
                }
            }

            // Tab Switching in Right Panel
            if (_tabButtons != null)
            {
                for (int i = 0; i < _tabButtons.Length; i++)
                {
                    int tabIndex = i;
                    _tabButtons[i]?.onClick.AddListener(() => SelectTab(tabIndex));
                }
            }

            // Dimensions Sliders
            _heightSlider?.onValueChanged.AddListener(v => {
                if (_heightValText) _heightValText.text = $"{v:F2}x";
                UpdateDimensionsFromUI();
            });
            _baseWidthSlider?.onValueChanged.AddListener(v => {
                if (_baseWidthValText) _baseWidthValText.text = $"{v:F2}x";
                UpdateDimensionsFromUI();
            });
            _shadeWidthSlider?.onValueChanged.AddListener(v => {
                if (_shadeWidthValText) _shadeWidthValText.text = $"{v:F2}x";
                UpdateDimensionsFromUI();
            });
            _shadeHeightSlider?.onValueChanged.AddListener(v => {
                if (_shadeHeightValText) _shadeHeightValText.text = $"{v:F2}x";
                UpdateDimensionsFromUI();
            });
            _shadeStyleDropdown?.onValueChanged.AddListener(v => UpdateDimensionsFromUI());

            // Materials Dropdowns
            _baseMatDropdown?.onValueChanged.AddListener(v => UpdateMaterialsFromUI());
            _standMatDropdown?.onValueChanged.AddListener(v => UpdateMaterialsFromUI());
            _shadeMatDropdown?.onValueChanged.AddListener(v => UpdateMaterialsFromUI());

            // Colors Sliders
            _redSlider?.onValueChanged.AddListener(v => {
                if (_redValText) _redValText.text = Mathf.RoundToInt(v * 255f).ToString();
                OnColorSliderChanged();
            });
            _greenSlider?.onValueChanged.AddListener(v => {
                if (_greenValText) _greenValText.text = Mathf.RoundToInt(v * 255f).ToString();
                OnColorSliderChanged();
            });
            _blueSlider?.onValueChanged.AddListener(v => {
                if (_blueValText) _blueValText.text = Mathf.RoundToInt(v * 255f).ToString();
                OnColorSliderChanged();
            });

            _colorTargetDropdown?.onValueChanged.AddListener(v => {
                LampConfigData cfg = LumiLabManager.Instance?.CurrentConfig;
                if (cfg != null)
                {
                    Color targetCol = (v == 0) ? cfg.baseColor : (v == 1 ? cfg.standColor : cfg.shadeColor);
                    _redSlider?.SetValueWithoutNotify(targetCol.r);
                    _greenSlider?.SetValueWithoutNotify(targetCol.g);
                    _blueSlider?.SetValueWithoutNotify(targetCol.b);
                    if (_redValText) _redValText.text = Mathf.RoundToInt(targetCol.r * 255f).ToString();
                    if (_greenValText) _greenValText.text = Mathf.RoundToInt(targetCol.g * 255f).ToString();
                    if (_blueValText) _blueValText.text = Mathf.RoundToInt(targetCol.b * 255f).ToString();
                    if (_colorPreviewImage) _colorPreviewImage.color = targetCol;
                }
            });

            // Swatch Buttons
            if (_swatchButtons != null)
            {
                for (int i = 0; i < _swatchButtons.Length; i++)
                {
                    int swatchIdx = i;
                    _swatchButtons[i]?.onClick.AddListener(() =>
                    {
                        if (swatchIdx < ColorPalettePresets.CuratedBaseColors.Length)
                        {
                            ApplySwatchColor(ColorPalettePresets.CuratedBaseColors[swatchIdx]);
                        }
                    });
                }
            }

            // Lighting Controls
            _lightPowerBtn?.onClick.AddListener(() =>
            {
                LampConfigData cfg = LumiLabManager.Instance?.CurrentConfig;
                if (cfg != null)
                {
                    bool next = !cfg.isLightOn;
                    LumiLabManager.Instance.ToggleLight(next);
                    UpdatePowerButtonUI(next);
                }
            });

            _brightnessSlider?.onValueChanged.AddListener(b =>
            {
                if (_brightnessValText) _brightnessValText.text = $"{b:F1}x";
                LumiLabManager.Instance?.SetBrightness(b);
            });

            _warmWhiteBtn?.onClick.AddListener(() => LumiLabManager.Instance?.SetLightTemperature(LightTempMode.WarmWhite));
            _neutralWhiteBtn?.onClick.AddListener(() => LumiLabManager.Instance?.SetLightTemperature(LightTempMode.NeutralWhite));
            _coolWhiteBtn?.onClick.AddListener(() => LumiLabManager.Instance?.SetLightTemperature(LightTempMode.CoolWhite));

            // Presets Buttons
            if (_presetButtons != null)
            {
                for (int i = 0; i < _presetButtons.Length; i++)
                {
                    int pIndex = i;
                    _presetButtons[i]?.onClick.AddListener(() =>
                    {
                        LumiLabManager.Instance?.ApplyPreset(pIndex);
                        ShowNotification($"Applied Preset: {GetPresetName(pIndex)}");
                    });
                }
            }

            // Save & Load
            _saveBtn?.onClick.AddListener(() =>
            {
                LumiLabManager.Instance?.SaveCurrentConfig();
                ShowNotification("Design saved to Unity PlayerPrefs!");
            });

            _loadBtn?.onClick.AddListener(() =>
            {
                LumiLabManager.Instance?.LoadCurrentConfig();
                ShowNotification("Design restored from Unity!");
            });

            // Bottom Bar Camera Controls
#if UNITY_2023_1_OR_NEWER
            ShowroomCameraController cam = FindAnyObjectByType<ShowroomCameraController>();
#else
            ShowroomCameraController cam = FindObjectOfType<ShowroomCameraController>();
#endif
            _view34Btn?.onClick.AddListener(() => cam?.PresetThreeQuarter());
            _viewFrontBtn?.onClick.AddListener(() => cam?.PresetFront());
            _viewSideBtn?.onClick.AddListener(() => cam?.PresetSide());
            _viewTopBtn?.onClick.AddListener(() => cam?.PresetTop());
            _viewResetBtn?.onClick.AddListener(() => cam?.ResetView());
            _zoomInBtn?.onClick.AddListener(() => cam?.ZoomIn());
            _zoomOutBtn?.onClick.AddListener(() => cam?.ZoomOut());

            _autoRotateToggle?.onValueChanged.AddListener(on =>
            {
                if (cam != null) cam.AutoRotate = on;
            });

            _quickPowerBtn?.onClick.AddListener(() =>
            {
                LampConfigData cfg = LumiLabManager.Instance?.CurrentConfig;
                if (cfg != null)
                {
                    bool next = !cfg.isLightOn;
                    LumiLabManager.Instance.ToggleLight(next);
                    UpdatePowerButtonUI(next);
                }
            });
        }

        private void SubscribeToManagerEvents()
        {
            if (LumiLabManager.Instance != null)
            {
                LumiLabManager.Instance.OnLampChanged += OnLampChanged;
                LumiLabManager.Instance.OnConfigUpdated += SyncUIFromConfig;
            }
        }

        private void OnLampChanged(int lampIndex, LampConfigData config)
        {
            string[] lampNames = { "Aura Modern", "Nordic Glow", "Industrial Edge", "Halo Contemporary" };
            if (_selectedLampTitleText != null && lampIndex < lampNames.Length)
            {
                _selectedLampTitleText.text = lampNames[lampIndex];
            }

            // Update collection highlight and active badges
            if (_lampSelectHighlights != null)
            {
                for (int i = 0; i < _lampSelectHighlights.Length; i++)
                {
                    if (_lampSelectHighlights[i] != null)
                    {
                        _lampSelectHighlights[i].gameObject.SetActive(i == lampIndex);
                    }
                }
            }

            if (_lampActiveBadges != null)
            {
                for (int i = 0; i < _lampActiveBadges.Length; i++)
                {
                    if (_lampActiveBadges[i] != null)
                    {
                        _lampActiveBadges[i].SetActive(i == lampIndex);
                    }
                }
            }

            SyncUIFromConfig(config);
        }

        public void SelectTab(int index)
        {
            _currentTabIndex = index;
            if (_tabContents != null)
            {
                for (int i = 0; i < _tabContents.Length; i++)
                {
                    if (_tabContents[i] != null)
                    {
                        _tabContents[i].SetActive(i == index);
                    }
                }
            }

            // Update tab button highlights
            if (_tabButtons != null)
            {
                Color activeBg = new Color(0.85f, 0.70f, 0.38f, 0.22f);
                Color inactiveBg = new Color(0.14f, 0.16f, 0.20f, 0.6f);
                Color activeTxt = new Color(0.95f, 0.85f, 0.45f, 1f);
                Color inactiveTxt = new Color(0.6f, 0.65f, 0.75f, 1f);

                for (int i = 0; i < _tabButtons.Length; i++)
                {
                    if (_tabButtons[i] != null)
                    {
                        bool isAct = (i == index);
                        Image img = _tabButtons[i].GetComponent<Image>();
                        if (img != null) img.color = isAct ? activeBg : inactiveBg;
                        Text t = _tabButtons[i].GetComponentInChildren<Text>();
                        if (t != null) t.color = isAct ? activeTxt : inactiveTxt;
                    }
                }
            }
        }

        private void UpdateDimensionsFromUI()
        {
            float h = _heightSlider ? _heightSlider.value : 1f;
            float bw = _baseWidthSlider ? _baseWidthSlider.value : 1f;
            float sw = _shadeWidthSlider ? _shadeWidthSlider.value : 1f;
            float sh = _shadeHeightSlider ? _shadeHeightSlider.value : 1f;
            int style = _shadeStyleDropdown ? _shadeStyleDropdown.value : 0;

            LumiLabManager.Instance?.SetDimensions(h, bw, sw, sh, style);
        }

        private void UpdateMaterialsFromUI()
        {
            MaterialType baseMat = _baseMatDropdown ? (MaterialType)_baseMatDropdown.value : MaterialType.BrushedMetal;
            MaterialType standMat = _standMatDropdown ? (MaterialType)_standMatDropdown.value : MaterialType.PolishedMetal;
            MaterialType shadeMat = _shadeMatDropdown ? (MaterialType)_shadeMatDropdown.value : MaterialType.Fabric;

            LumiLabManager.Instance?.SetMaterials(baseMat, standMat, shadeMat);
        }

        private void OnColorSliderChanged()
        {
            Color c = new Color(
                _redSlider ? _redSlider.value : 1f,
                _greenSlider ? _greenSlider.value : 1f,
                _blueSlider ? _blueSlider.value : 1f
            );

            if (_colorPreviewImage != null) _colorPreviewImage.color = c;

            int target = _colorTargetDropdown ? _colorTargetDropdown.value : 0;
            if (target == 0) LumiLabManager.Instance?.SetBaseColor(c);
            else if (target == 1) LumiLabManager.Instance?.SetStandColor(c);
            else LumiLabManager.Instance?.SetShadeColor(c);
        }

        private void ApplySwatchColor(Color c)
        {
            if (_redSlider != null) _redSlider.value = c.r;
            if (_greenSlider != null) _greenSlider.value = c.g;
            if (_blueSlider != null) _blueSlider.value = c.b;
            if (_redValText != null) _redValText.text = Mathf.RoundToInt(c.r * 255f).ToString();
            if (_greenValText != null) _greenValText.text = Mathf.RoundToInt(c.g * 255f).ToString();
            if (_blueValText != null) _blueValText.text = Mathf.RoundToInt(c.b * 255f).ToString();
            if (_colorPreviewImage != null) _colorPreviewImage.color = c;

            int target = _colorTargetDropdown ? _colorTargetDropdown.value : 0;
            if (target == 0) LumiLabManager.Instance?.SetBaseColor(c);
            else if (target == 1) LumiLabManager.Instance?.SetStandColor(c);
            else LumiLabManager.Instance?.SetShadeColor(c);
        }

        public void SyncUIFromConfig(LampConfigData cfg)
        {
            if (cfg == null) return;

            // Dimensions
            if (_heightSlider != null)
            {
                _heightSlider.SetValueWithoutNotify(cfg.heightScale);
                if (_heightValText) _heightValText.text = $"{cfg.heightScale:F2}x";
            }
            if (_baseWidthSlider != null)
            {
                _baseWidthSlider.SetValueWithoutNotify(cfg.baseWidthScale);
                if (_baseWidthValText) _baseWidthValText.text = $"{cfg.baseWidthScale:F2}x";
            }
            if (_shadeWidthSlider != null)
            {
                _shadeWidthSlider.SetValueWithoutNotify(cfg.shadeWidthScale);
                if (_shadeWidthValText) _shadeWidthValText.text = $"{cfg.shadeWidthScale:F2}x";
            }
            if (_shadeHeightSlider != null)
            {
                _shadeHeightSlider.SetValueWithoutNotify(cfg.shadeHeightScale);
                if (_shadeHeightValText) _shadeHeightValText.text = $"{cfg.shadeHeightScale:F2}x";
            }
            if (_shadeStyleDropdown != null) _shadeStyleDropdown.SetValueWithoutNotify(cfg.shadeStyle);

            // Materials
            if (_baseMatDropdown != null) _baseMatDropdown.SetValueWithoutNotify((int)cfg.baseMaterial);
            if (_standMatDropdown != null) _standMatDropdown.SetValueWithoutNotify((int)cfg.standMaterial);
            if (_shadeMatDropdown != null) _shadeMatDropdown.SetValueWithoutNotify((int)cfg.shadeMaterial);

            // Color
            int target = _colorTargetDropdown ? _colorTargetDropdown.value : 0;
            Color curCol = (target == 0) ? cfg.baseColor : (target == 1 ? cfg.standColor : cfg.shadeColor);
            if (_redSlider != null) _redSlider.SetValueWithoutNotify(curCol.r);
            if (_greenSlider != null) _greenSlider.SetValueWithoutNotify(curCol.g);
            if (_blueSlider != null) _blueSlider.SetValueWithoutNotify(curCol.b);
            if (_redValText) _redValText.text = Mathf.RoundToInt(curCol.r * 255f).ToString();
            if (_greenValText) _greenValText.text = Mathf.RoundToInt(curCol.g * 255f).ToString();
            if (_blueValText) _blueValText.text = Mathf.RoundToInt(curCol.b * 255f).ToString();
            if (_colorPreviewImage) _colorPreviewImage.color = curCol;

            // Lighting
            UpdatePowerButtonUI(cfg.isLightOn);
            if (_brightnessSlider != null)
            {
                _brightnessSlider.SetValueWithoutNotify(cfg.brightness);
                if (_brightnessValText) _brightnessValText.text = $"{cfg.brightness:F1}x";
            }
        }

        private void UpdatePowerButtonUI(bool isOn)
        {
            if (_lightPowerBtn != null)
            {
                Image img = _lightPowerBtn.GetComponent<Image>();
                if (img != null)
                {
                    img.color = isOn ? new Color(0.85f, 0.70f, 0.38f, 1f) : new Color(0.25f, 0.28f, 0.32f, 1f);
                }
            }
            if (_lightPowerBtnText != null)
            {
                _lightPowerBtnText.text = isOn ? "POWER ON" : "POWER OFF";
                _lightPowerBtnText.color = isOn ? Color.black : Color.white;
            }
            if (_quickPowerIcon != null)
            {
                _quickPowerIcon.color = isOn ? new Color(1f, 0.85f, 0.4f) : new Color(0.4f, 0.4f, 0.4f);
            }
        }

        public void ShowNotification(string msg)
        {
            if (_notificationText != null)
            {
                _notificationText.text = msg;
                _notificationText.transform.parent.gameObject.SetActive(true);
                _notifyTimer = 2.5f;
            }
        }

        private string GetPresetName(int p)
        {
            string[] names = { "Cozy Evening", "Modern Minimal", "Luxury Gold", "Industrial Black", "Colorful Ambient" };
            return (p >= 0 && p < names.Length) ? names[p] : "Preset";
        }
    }
}
