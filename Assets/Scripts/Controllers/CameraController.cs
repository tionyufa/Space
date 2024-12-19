using System;
using Cinemachine;
using Controllers.Input;
using UnityEngine;

namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private CinemachineVirtualCamera _camera;

        [SerializeReference]
        private IInput _input = new MobileInput();

        [SerializeField]
        private Transform _center;
        [SerializeField] 
        private float _radius;
        [SerializeField] 
        private float _yPosition;
        [SerializeField]
        private float _angel;
        [SerializeField] 
        private float _speed;
        [SerializeField] 
        private float _speedZoom;
        
        public CameraController(IInput input)
        {
            
        }
        private void Awake()
        {
            _input.Init();
            _input.OnZoom += OnZoom;
            _input.OnSwap += OnSwap;
        }

        private void OnDisable()
        {
            _input.OnDisable();
            _input.OnZoom -= OnZoom;
        }

        private void OnSwap(int direction)
        {
            _angel +=  direction *  _speed * Time.deltaTime;
            MoveHorizontal();
        }

        private void MoveHorizontal()
        {
            var x = Mathf.Cos(_angel) * _radius;
            var z = Mathf.Sin(_angel) * _radius;
            var y = Mathf.Lerp(0, _yPosition, _radius / 100);

            var vector = new Vector3(_center.position.x + x, y, _center.position.z + z);
            transform.position = vector;
        }

        private void OnZoom(int direction)
        {
            _radius = Mathf.Clamp( _radius += _speedZoom * direction * Time.deltaTime,10,100);
            
            MoveHorizontal();
        }
    }
}