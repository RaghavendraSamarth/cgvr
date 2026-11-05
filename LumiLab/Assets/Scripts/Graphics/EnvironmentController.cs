using UnityEngine;
using UnityEngine.Rendering;

namespace LumiLab.Graphics
{
    public class EnvironmentController : MonoBehaviour
    {
        [Header("Showroom Environments")]
        [SerializeField] private GameObject _livingRoomRoot;
        [SerializeField] private GameObject _minimalStudioRoot;
        [SerializeField] private GameObject _cozyBedroomRoot;

        [Header("Lamp Mount Points")]
        [SerializeField] private Transform _livingRoomMount;
        [SerializeField] private Transform _minimalStudioMount;
        [SerializeField] private Transform _cozyBedroomMount;

        [Header("Environment Lights")]
        [SerializeField] private Light _livingRoomKeyLight;
        [SerializeField] private Light _studioKeyLight;
        [SerializeField] private Light _studioFillLight;
        [SerializeField] private Light _bedroomKeyLight;

        private int _currentEnvironmentIndex = 0; // 0: Living Room, 1: Studio, 2: Bedroom

        public int CurrentEnvironmentIndex => _currentEnvironmentIndex;

        public void SetEnvironment(int envIndex)
        {
            _currentEnvironmentIndex = Mathf.Clamp(envIndex, 0, 2);

            if (_livingRoomRoot != null) _livingRoomRoot.SetActive(_currentEnvironmentIndex == 0);
            if (_minimalStudioRoot != null) _minimalStudioRoot.SetActive(_currentEnvironmentIndex == 1);
            if (_cozyBedroomRoot != null) _cozyBedroomRoot.SetActive(_currentEnvironmentIndex == 2);

            // Configure ambient lighting according to environment mood
            switch (_currentEnvironmentIndex)
            {
                case 0: // Luxury Living Room
                    RenderSettings.ambientLight = new Color(0.24f, 0.22f, 0.20f);
                    RenderSettings.ambientIntensity = 0.8f;
                    if (_livingRoomKeyLight != null) _livingRoomKeyLight.enabled = true;
                    if (_studioKeyLight != null) _studioKeyLight.enabled = false;
                    if (_studioFillLight != null) _studioFillLight.enabled = false;
                    if (_bedroomKeyLight != null) _bedroomKeyLight.enabled = false;
                    break;

                case 1: // Minimal Design Studio
                    RenderSettings.ambientLight = new Color(0.35f, 0.36f, 0.38f);
                    RenderSettings.ambientIntensity = 1.0f;
                    if (_livingRoomKeyLight != null) _livingRoomKeyLight.enabled = false;
                    if (_studioKeyLight != null) _studioKeyLight.enabled = true;
                    if (_studioFillLight != null) _studioFillLight.enabled = true;
                    if (_bedroomKeyLight != null) _bedroomKeyLight.enabled = false;
                    break;

                case 2: // Cozy Bedroom
                    RenderSettings.ambientLight = new Color(0.18f, 0.15f, 0.14f);
                    RenderSettings.ambientIntensity = 0.6f;
                    if (_livingRoomKeyLight != null) _livingRoomKeyLight.enabled = false;
                    if (_studioKeyLight != null) _studioKeyLight.enabled = false;
                    if (_studioFillLight != null) _studioFillLight.enabled = false;
                    if (_bedroomKeyLight != null) _bedroomKeyLight.enabled = true;
                    break;
            }
        }

        public Transform GetCurrentMountPoint()
        {
            switch (_currentEnvironmentIndex)
            {
                case 0: return _livingRoomMount;
                case 1: return _minimalStudioMount;
                case 2: return _cozyBedroomMount;
                default: return _livingRoomMount;
            }
        }
    }
}
