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
        private bool _isBlock;
        private float _prevDistance;
        private float _distance;
        private bool _mouse;
        public Action<int> OnZoom { get; set; }
        public Action<Vector2> OnSwap { get; set; }
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
            
            _control.Touch.MouseScroll.started += MouseOnStarted;
            _control.Touch.MouseScroll.canceled += MouseOnCanceled;
        }


        private async void MouseOnStarted(InputAction.CallbackContext obj)
        {
            _mouse = true;
            
            while (_mouse)
            {
                var mouseAction = _control.Touch.MouseScroll.ReadValue<Vector2>();
                if (mouseAction.y > 0)
                    OnZoom?.Invoke(-1);
                else if (mouseAction.y < 0)
                    OnZoom?.Invoke(1);
                await Task.Yield();
            }
            OnZoom?.Invoke(0);
        }

        private void StartSwap(InputAction.CallbackContext obj)
        {
            _swap = true;
            ((IInput) this).Swap(_control.Touch.FirstTouchPosition);
        }

        private void StartZoom(InputAction.CallbackContext obj)
        {
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
            
            
            OnZoom?.Invoke(0);
        }

        void IInput.Block(bool blocked)
        {
            if (blocked)
                _control.Disable();
            else 
                _control.Enable();
        }

        async void IInput.Swap(InputAction input)
        {
            var position = ReturnVector(input);
            var prevPosition = position;

            while (_swap && !_zoom)
            {
                position = ReturnVector(input);
                
                if (prevPosition == Vector2.zero)
                    prevPosition = position;
                
                var direction = position - prevPosition;
                if (direction.x != 0 || direction.y != 0)
                    OnSwap?.Invoke(direction);
                else 
                    OnSwap?.Invoke(Vector2.zero);

                prevPosition = position;
                
                await Task.Yield();
            }
            
            OnSwap?.Invoke(Vector2.zero);
        }


        private void Click(InputAction.CallbackContext callbackContext) => OnClick?.Invoke(_control.Touch.FirstTouchPosition.ReadValue<Vector2>());

        private void MouseOnCanceled(InputAction.CallbackContext obj) => _mouse = false;
        private void EndSwap(InputAction.CallbackContext obj) => _swap = false;

        private void EndZoom(InputAction.CallbackContext obj) => _zoom = false;


        void IEnable.OnDisable()
        {
            _control.Disable();
            
            _control.Touch.SecondTouchContact.started -= StartZoom; 
            _control.Touch.SecondTouchContact.canceled -= EndZoom;
            
             _control.Touch.Click.started -= Click;
        }

        private Vector2 ReturnVector(InputAction action) => action.ReadValue<Vector2>();
    }
}