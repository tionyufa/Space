using System;
using System.Collections;
using System.Threading.Tasks;
using Controllers.Input;
using Infrastructure.Setting;
using Planets;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class CinemachineCameraController : MonoBehaviour
    {
        public enum State
        {
            Default,
            MoveToPlanet,
            LookAt
        }

        public event Action<PlanetsName> OnClickToPlanet;

        [SerializeField] 
        private AudioSource _audio;
        [SerializeField] 
        private Transform _target;

        [SerializeField] 
        private float distance = 5f;
        
        [SerializeField] 
        private Vector2 pitchLimits = new Vector2(-30, 60);

        [SerializeField] 
        private float minDistance = 2f;

        [SerializeField] 
        private float maxDistance = 10f;

        [SerializeField]
        private float transitionDuration = 1.5f;
        [SerializeField]
        private float transitionDurationToDefault = 0.5f;

        private State _currentState = State.Default;
        private MovingPlanet _currentPlanet;
        private SettingsDisplay _settingsDisplay;
        private InterfaceAnimManager _interfaceAnimManager;
        private Transform _defaultTarget;
        private Transform newTarget;
        private IInput _input;
        private Camera _camera;
        private Vector2 lookInput;
        private bool _block;
        private bool _default;
        private float zoomInput;
        private float currentPitch = 0f;
        private float currentYaw = 0f;
        private float _casheDistance;
        private float _distanceToDefault = 30;
        [SerializeField] private Vector2 _vector2;
        private ScrollRect _scroll;
        public void Construct(IInput input, Transform sun, SettingsDisplay settingsDisplay,
            InterfaceAnimManager interfaceAnimManager)
        {
            _input = input;
            _target = sun;
            _settingsDisplay = settingsDisplay;
            _interfaceAnimManager = interfaceAnimManager;

            _scroll = interfaceAnimManager.GetComponentInChildren<ScrollRect>();
            Init();
        }

        private void Init()
        {
            _input.Init();
            _camera = GetComponentInChildren<Camera>();

            _input.OnZoom += OnZoom;
            _input.OnSwipe += OnSwipe;
            _input.OnClick += OnClick;

            _casheDistance = minDistance;
            _defaultTarget = _target;
            _interfaceAnimManager.OnEndAppear += ReturnPosition;
            
            currentPitch = 16.3f;
            currentYaw = 26.5f;
        }

        private void OnDisable()
        {
            _input.OnDisable();
        }

        private void OnClick(Vector2 position)
        {
            Ray ray = _camera.ScreenPointToRay(position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var onPlanet = hit.transform.GetComponent<MovingPlanet>();
                if (onPlanet)
                {
                    CheckCurrent(onPlanet);
                }
            }
        }

        private void CheckCurrent(MovingPlanet onPlanet)
        {
            if (_currentPlanet != null)
            {
                if (onPlanet == _currentPlanet)
                    return;
                if (onPlanet != _currentPlanet)
                    _currentPlanet.ResetSpeed();
            }

            _currentPlanet = onPlanet;
            _currentPlanet.StopSpeed();
            
            SetTarget(_currentPlanet.transform);

            OnClickToPlanet?.Invoke(_currentPlanet.namePlanet);
            StartCoroutine(nameof(MoveToPlanet));
        }

        private void LateUpdate()
        {
            if (_target == null || _block)
                return;

            currentYaw += lookInput.x * _settingsDisplay.rotationSpeed * Time.deltaTime;
            currentPitch -= lookInput.y * _settingsDisplay.rotationSpeed * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, pitchLimits.x, pitchLimits.y);

            distance -= zoomInput * _settingsDisplay.zoomSpeed * Time.deltaTime;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
            
            Vector3 direction = new Vector3(0, 0, -distance);
            Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
            transform.position = _target.position + rotation * direction;
            transform.LookAt(_target);
            
            if (_currentState == State.LookAt && distance > _distanceToDefault)
                DefaultPosition();
        }

        private void SetTarget(Transform target) => newTarget = target;

        private void OnSwipe(Vector2 obj) => lookInput = obj;

        private void OnZoom(float obj) => zoomInput = obj;

        private void DefaultPosition()
        {
            Vector3 startPosition = transform.position;
            Vector3 endPosition = _defaultTarget.position;
            newTarget = _defaultTarget;
            
            minDistance =  distance = Vector3.Distance(startPosition, endPosition);
            _block = true;
            
            StartCoroutine(nameof(MoveToDefault));
        }

        private IEnumerator MoveToDefault()
        {
            _currentState = State.MoveToPlanet;
            StartPosition(out var startRotation, out var startTargetPosition, out var endTargetPosition);
            float elapsedTime = 0f;

            OpenUI(false);
            while (elapsedTime < transitionDurationToDefault)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / transitionDurationToDefault;
                t = Mathf.SmoothStep(0f, 1f, t);

                transform.rotation = Quaternion.Slerp(startRotation, Quaternion.LookRotation(endTargetPosition - transform.position), t);
                
                yield return null;
            }
            SetPosition();
            
            DefaultSetting();
        }

        private IEnumerator MoveToPlanet()
        {
            _audio.Play();
            
            _block = true;
            _currentState = State.MoveToPlanet;
            
            var startPosition = StartPosition(out var startRotation, out var startTargetPosition, out var endTargetPosition);
            var elapsedTime = 0f;
            distance = Vector3.Distance(startPosition, endTargetPosition);

            OpenUI(true);
            while (elapsedTime < transitionDuration && distance > minDistance * 2)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / transitionDuration;
                t = Mathf.SmoothStep(0f, 1f, t);

                var interpolatedTargetPosition = Vector3.Lerp(startTargetPosition, endTargetPosition, t);
                transform.position = Vector3.Lerp(startPosition, interpolatedTargetPosition, t);

                transform.rotation = Quaternion.Slerp(startRotation, Quaternion.LookRotation(endTargetPosition - transform.position), t);
                distance = Vector3.Distance(transform.position, newTarget.position);

                if (distance <= minDistance * 2)
                {
                    SetPosition();
                    yield break;
                }
                yield return null;
            }

            SetPosition();
        }

        private void ReturnPosition(InterfaceAnimManager _iam)
        {
            if (_scroll)
                _scroll.content.anchoredPosition = Vector2.zero;
        }

        private void OpenUI(bool active)
        {
            if (_interfaceAnimManager)
                if (active)
                    _interfaceAnimManager.StartAppear();
            
                else
                    _interfaceAnimManager.StartDisappear();
            
            ReturnPosition(null);
        }

        private void SetPosition()
        {
            _target = newTarget;
            _block = false;
            
            var rotation = transform.rotation;

            currentYaw = rotation.eulerAngles.y;
            currentPitch = rotation.eulerAngles.x;

            _currentState = State.LookAt;
        }

        private Vector3 StartPosition(out Quaternion startRotation, out Vector3 startTargetPosition,
            out Vector3 endTargetPosition)
        {
            Vector3 startPosition = transform.position;
            startRotation = transform.rotation;
            startTargetPosition = _target.position;

            endTargetPosition = newTarget.position;
            return startPosition;
        }

        private void DefaultSetting()
        {
            minDistance = _casheDistance;
            _currentState = State.Default;
            _currentPlanet = null;
        }
    }
}
