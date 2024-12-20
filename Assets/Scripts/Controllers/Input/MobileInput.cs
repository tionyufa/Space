using System;
using System.Threading.Tasks;
using Infrastructure.Interface;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers.Input
{
    public class MobileInput : IInput
    {
        [SerializeField] private InputControl _input;
        private TouchControl _control;
        private bool _zoom;
        private bool _swap;
        private float _prevDistance;
        private float _distance;
        public Action<int> OnZoom { get; set; }
        public Action<int> OnSwap { get; set; }
        public Action<Vector2> OnClick { get; set; }

        void IEnable.Init()
        {
            _control = new TouchControl();
            
            _control.Enable();
            
            _control.Touch.Click.started += Click;
            
            _control.Touch.FirstTouchContact.started  += StartSwap;
            _control.Touch.FirstTouchContact.canceled += EndSwap;
            
            _control.Touch.SecondTouchContact.started  += StartZoom;
            _control.Touch.SecondTouchContact.canceled += EndZoom;

        }

        private void StartSwap(InputAction.CallbackContext obj)
        {
            _swap = true;
            ((IInput) this).Swap();
        }

        private void StartZoom(InputAction.CallbackContext obj)
        {
            Debug.LogError("StartZoom");
            _zoom = true;
            ((IInput) this).Zoom();
        }

        async void IInput.Zoom()
        {
            var prevDistance = 0f;
            while (_zoom)
            {
                var firstVector = ReturnVector(_control.Touch.FirstTouchPosition);
                var secondVector = ReturnVector(_control.Touch.SecondTouchPosition);
                var distance = Vector2.Distance(firstVector, secondVector);
                if (distance > prevDistance)
                    OnZoom?.Invoke(-1);
                else if (distance < prevDistance)
                    OnZoom?.Invoke(1);

                prevDistance = distance;
                await Task.Yield();
            }
        }


        async void IInput.Swap()
        {
            Debug.LogError("swap");
            var position = ReturnVector(_control.Touch.FirstTouchPosition);
            var prevPosition = position;
            
           
            while (_swap && !_zoom)
            {
                position = ReturnVector(_control.Touch.FirstTouchPosition);
                
                if (prevPosition == Vector2.zero)
                    prevPosition = position;
                
                var direction = position - prevPosition;
                if (direction.x > 0)
                    OnSwap?.Invoke(1);
                else if (direction.x < 0)
                    OnSwap?.Invoke(-1);
                else 
                    OnSwap?.Invoke(0);

                prevPosition = position;
                
                await Task.Yield();
            }
        }

        private void Click(InputAction.CallbackContext callbackContext) => OnClick?.Invoke(_control.Touch.FirstTouchPosition.ReadValue<Vector2>());

        private void EndSwap(InputAction.CallbackContext obj) => _swap = false;

        private void EndZoom(InputAction.CallbackContext obj) => _zoom = false;

        void IEnable.OnDisable()
        {
            _control.Disable();
            
            _control.Touch.SecondTouchContact.started -= StartZoom; 
            _control.Touch.SecondTouchContact.canceled -= EndZoom;
            
            // _control.Touch.Click.started -= Click;
        }

        private Vector2 ReturnVector(InputAction action) => action.ReadValue<Vector2>();
    }
}