using System;
using Cinemachine;
using Controllers.Input;
using Planets;
using UnityEngine;

namespace Controllers
{
    public class ControllerCamera : MonoBehaviour
    {
        public Action<PlanetsName> OnClickToPlanet;

        [SerializeField]
        private Transform _center;

        [SerializeField] 
        private Transform _target;

        [SerializeField] 
        private float _radius;

        [SerializeField] 
        private float _yPosition;

        [SerializeField] 
        private float _speed;

        [SerializeField] 
        private float _speedZoom;

        [SerializeField]
        private float _angel;

        private IInput _input;
        private Camera _camera;
        private CinemachineVirtualCamera _virtualCamera;
        public void Construct(IInput input,Transform center)
        {
            Debug.LogError(center);
            _center = center;
            _input = input;
            
            Init();
            SetTarget(_center);
        }
        private void Init()
        {
            _input.Init();
            _camera = GetComponentInChildren<Camera>();
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _input.OnZoom += OnZoom;
            _input.OnSwap += OnSwap;
            _input.OnClick += OnClick;
        }

        private void SetTarget(Transform target)
        {
            _target = target;
            _virtualCamera.LookAt = target;
        }

        private void OnDisable()
        {
            _input.OnDisable();
            _input.OnZoom -= OnZoom;
            _input.OnSwap -= OnSwap;
            _input.OnClick -= OnClick;
        }
        
        private void OnSwap(int direction)
        {
            _angel +=  direction *  _speed * Time.deltaTime;
            MoveHorizontal();
        }
        
        private void OnZoom(int direction)
        {
            _radius = Mathf.Clamp( _radius += _speedZoom * direction * Time.deltaTime,10,100);
            
            MoveHorizontal();
        }
        
        private void Update()
        {
            MoveHorizontal();
        }
        
        private void OnClick(Vector2 position)
        {
            Ray ray = _camera.ScreenPointToRay(position);
            Debug.DrawRay(ray.origin, ray.direction, Color.cyan, 2);
            RaycastHit hit;
        
            if (Physics.Raycast(ray, out hit))
            {
                var onPlanet = hit.transform.GetComponent<MovingPlanet>();
                if (onPlanet)
                {
                    SetTarget(onPlanet.transform);
                    OnClickToPlanet?.Invoke(onPlanet.namePlanet);
                }
                
                Debug.LogError(hit.transform.name);
            }
        }
        
        private void MoveHorizontal()
        {
            if (!_target)
                return;
            
            var x = Mathf.Cos(_angel) * _radius;
            var z = Mathf.Sin(_angel) * _radius;
            var y = Mathf.Lerp(0, _yPosition, _radius / 100);
        
            var vector = new Vector3(_target.position.x + x, y, _target.position.z + z);
            transform.position = vector;
        }
    }
}