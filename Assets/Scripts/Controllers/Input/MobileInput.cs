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
        private bool _swipe;
        private bool _isBlock;
        private float _prevDistance;
        private float _distance;
        private bool _mouse;
        public Action<float> OnZoom { get; set; }
        public Action<Vector2> OnSwipe { get; set; }
        public Action<Vector2> OnClick { get; set; }
        
        public static Action<float> OnTest { get; set; }


        void IEnable.Init()
        {
            _control = new TouchControl();
            
            _control.Enable();
            
            _control.Touch.Click.started += Click;
            
            _control.Touch.FirstTouchContact.started  += StartSwipe;
            _control.Touch.FirstTouchContact.canceled += EndSwipe;
            
            _control.Touch.SecondTouchContact.started  += StartZoom;
            _control.Touch.SecondTouchContact.canceled += EndZoom;
            
            _control.Touch.MouseScroll.started += MouseOnStarted;
            _control.Touch.MouseScroll.canceled += MouseOnCanceled;
        }


        private void StartZoom(InputAction.CallbackContext obj)
        {
            _zoom = true;
            ((IInput) this).Zoom();
        }

        private void StartSwipe(InputAction.CallbackContext obj)
        {
            _swipe = true;
            ((IInput) this).Swipe(_control.Touch.FirstTouchPosition);
        }

        private async void MouseOnStarted(InputAction.CallbackContext obj)
        {
            _mouse = true;
            
            while (_mouse)
            {
                var mouseAction = _control.Touch.MouseScroll.ReadValue<Vector2>();
                ZoomInvoke(mouseAction.y, 0);
                await Task.Yield();
            }
            
            OnZoom?.Invoke(0);
        }

        async void IInput.Zoom()
        {
            var prevDistance = 0f;
            while (_zoom)
            {
                var firstVector = ReturnVector(_control.Touch.FirstTouchPosition);
                var secondVector = ReturnVector(_control.Touch.SecondTouchPosition);
                var distance = Vector2.Distance(firstVector, secondVector);
                ZoomInvoke(distance, prevDistance);

                prevDistance = distance;
                await Task.Yield();
            }

            OnZoom?.Invoke(0);
        }

        async void IInput.Swipe(InputAction input)
        {
            var position = ReturnVector(input);
            var prevPosition = position;

            while (_swipe && !_zoom)
            {
                position = ReturnVector(input);
                
                if (prevPosition == Vector2.zero)
                    prevPosition = position;
                
                var direction = position - prevPosition;
                if (direction.x != 0 || direction.y != 0)
                    OnSwipe?.Invoke(direction);
                else 
                    OnSwipe?.Invoke(Vector2.zero);

                prevPosition = position;
                
                await Task.Yield();
            }
            
            OnSwipe?.Invoke(Vector2.zero);
        }


        private void Click(InputAction.CallbackContext callbackContext) => OnClick?.Invoke(_control.Touch.FirstTouchPosition.ReadValue<Vector2>());

        private void MouseOnCanceled(InputAction.CallbackContext obj) => _mouse = false;

        private void EndSwipe(InputAction.CallbackContext obj) => _swipe = false;

        private void EndZoom(InputAction.CallbackContext obj) => _zoom = false;


        void IEnable.OnDisable()
        {
            _control.Disable();
            _control.Touch.Disable();
            
            _control.Touch.SecondTouchContact.started -= StartZoom; 
            _control.Touch.SecondTouchContact.canceled -= EndZoom;
            
             _control.Touch.Click.started -= Click;
        }

        void IInput.Block(bool blocked)
        {
            if (blocked)
                _control.Disable();
            else 
                _control.Enable();
        }

        private Vector2 ReturnVector(InputAction action) => action.ReadValue<Vector2>();

        private void ZoomInvoke(float distance, float prevDistance)
        {
            // var d = (distance - prevDistance) / 100;
            // if (Mathf.Abs(d) > 0.1f)
            //     Debug.LogError("D");
            // if (distance > prevDistance || distance < prevDistance)
            //     OnZoom?.Invoke(d);
            
            var d = (distance - prevDistance) / 100;
            if (Mathf.Abs(d) > 0.025f)
                OnZoom?.Invoke(d);
            else
                OnZoom?.Invoke(d);
            
            OnTest?.Invoke(d);
        }
    }
}