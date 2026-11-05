using UnityEngine;
using UnityEngine.EventSystems;

namespace LumiLab.ShowroomCamera
{
    public class ShowroomCameraController : MonoBehaviour
    {
        [Header("Target & Distances")]
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _targetOffset = new Vector3(0, 0.35f, 0);

        [SerializeField] private float _distance = 2.4f;
        [SerializeField] private float _minDistance = 0.4f;
        [SerializeField] private float _maxDistance = 5.5f;

        [Header("Orbit Angles")]
        [SerializeField] private float _yaw = 45.0f;       // Horizontal
        [SerializeField] private float _pitch = 22.0f;     // Vertical
        [SerializeField] private float _minPitch = 5.0f;
        [SerializeField] private float _maxPitch = 85.0f;

        [Header("Sensitivity & Smoothing")]
        [SerializeField] private float _orbitSpeed = 3.5f;
        [SerializeField] private float _zoomSpeed = 2.0f;
        [SerializeField] private float _smoothTime = 0.12f;

        [Header("Turntable Auto-Rotate")]
        [SerializeField] private bool _autoRotate = false;
        [SerializeField] private float _autoRotateSpeed = 12.0f;

        private float _targetDistance;
        private float _targetYaw;
        private float _targetPitch;

        private float _distanceVelocity;
        private float _yawVelocity;
        private float _pitchVelocity;

        private Vector2 _lastMousePos;
        private bool _isDragging = false;

        public bool AutoRotate
        {
            get => _autoRotate;
            set => _autoRotate = value;
        }

        private void Start()
        {
            _targetDistance = _distance;
            _targetYaw = _yaw;
            _targetPitch = _pitch;
        }

        private void Update()
        {
            HandleInput();

            if (_autoRotate && !_isDragging)
            {
                _targetYaw += _autoRotateSpeed * Time.deltaTime;
                _targetYaw %= 360f;
            }

            // Smooth damping
            _distance = Mathf.SmoothDamp(_distance, _targetDistance, ref _distanceVelocity, _smoothTime);
            _yaw = Mathf.SmoothDampAngle(_yaw, _targetYaw, ref _yawVelocity, _smoothTime);
            _pitch = Mathf.SmoothDamp(_pitch, _targetPitch, ref _pitchVelocity, _smoothTime);
        }

        private void LateUpdate()
        {
            Vector3 focusPoint = _target != null ? _target.position + _targetOffset : _targetOffset;
            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0);
            Vector3 position = focusPoint - (rotation * Vector3.forward * _distance);

            transform.rotation = rotation;
            transform.position = position;
        }

        private void HandleInput()
        {
            bool pointerOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

            // Touch Input
            if (Input.touchCount == 1)
            {
                Touch t = Input.GetTouch(0);
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(t.fingerId))
                    return;

                if (t.phase == TouchPhase.Moved)
                {
                    _targetYaw += t.deltaPosition.x * _orbitSpeed * 0.15f;
                    _targetPitch -= t.deltaPosition.y * _orbitSpeed * 0.15f;
                    _targetPitch = Mathf.Clamp(_targetPitch, _minPitch, _maxPitch);
                }
                return;
            }
            else if (Input.touchCount == 2)
            {
                Touch t0 = Input.GetTouch(0);
                Touch t1 = Input.GetTouch(1);

                Vector2 prevPos0 = t0.position - t0.deltaPosition;
                Vector2 prevPos1 = t1.position - t1.deltaPosition;
                float prevDist = (prevPos0 - prevPos1).magnitude;
                float currDist = (t0.position - t1.position).magnitude;
                float pinchDelta = (currDist - prevDist) * 0.005f;

                _targetDistance = Mathf.Clamp(_targetDistance - pinchDelta * _zoomSpeed, _minDistance, _maxDistance);
                return;
            }

            // Mouse Input
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                if (!pointerOverUI)
                {
                    _isDragging = true;
                    _lastMousePos = Input.mousePosition;
                }
            }

            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                _isDragging = false;
            }

            if (_isDragging)
            {
                Vector2 mouseDelta = (Vector2)Input.mousePosition - _lastMousePos;
                _lastMousePos = Input.mousePosition;

                _targetYaw += mouseDelta.x * _orbitSpeed * 0.1f;
                _targetPitch -= mouseDelta.y * _orbitSpeed * 0.1f;
                _targetPitch = Mathf.Clamp(_targetPitch, _minPitch, _maxPitch);
            }

            // Zoom via Mouse Scroll Wheel / Delta
            if (!pointerOverUI)
            {
                float scroll = Input.mouseScrollDelta.y;
                if (Mathf.Abs(scroll) < 0.001f) scroll = Input.GetAxis("Mouse ScrollWheel") * 10f;
                if (Mathf.Abs(scroll) > 0.001f)
                {
                    _targetDistance = Mathf.Clamp(_targetDistance - scroll * 0.20f * _zoomSpeed, _minDistance, _maxDistance);
                }
            }

            // Keyboard navigation shortcuts
            if (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.KeypadPlus) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                ZoomIn(1.8f * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                ZoomOut(1.8f * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                _targetYaw -= 50f * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                _targetYaw += 50f * Time.deltaTime;
            }
        }

        public void SetTarget(Transform target, Vector3 offset)
        {
            _target = target;
            _targetOffset = offset;
        }

        public void PresetThreeQuarter()
        {
            _targetYaw = 45f;
            _targetPitch = 22f;
            _targetDistance = 2.4f;
        }

        public void PresetFront()
        {
            _targetYaw = 0f;
            _targetPitch = 15f;
            _targetDistance = 2.2f;
        }

        public void PresetSide()
        {
            _targetYaw = 90f;
            _targetPitch = 15f;
            _targetDistance = 2.2f;
        }

        public void PresetTop()
        {
            _targetYaw = 0f;
            _targetPitch = 82f;
            _targetDistance = 2.8f;
        }

        public void ResetView()
        {
            PresetThreeQuarter();
        }

        public void Zoom(float delta)
        {
            _targetDistance = Mathf.Clamp(_targetDistance + delta, _minDistance, _maxDistance);
        }

        public void ZoomIn(float step = 0.35f)
        {
            _targetDistance = Mathf.Clamp(_targetDistance - step, _minDistance, _maxDistance);
        }

        public void ZoomOut(float step = 0.35f)
        {
            _targetDistance = Mathf.Clamp(_targetDistance + step, _minDistance, _maxDistance);
        }

        public float Distance => _distance;
        public float TargetDistance
        {
            get => _targetDistance;
            set => _targetDistance = Mathf.Clamp(value, _minDistance, _maxDistance);
        }
    }
}
