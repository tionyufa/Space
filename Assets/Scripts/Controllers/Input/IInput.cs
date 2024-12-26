using System;
using Infrastructure.Interface;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers.Input
{
    public interface IInput : IEnable
    {
        void Block(bool blocked);
        void Swipe(InputAction input);
        void Zoom();

        Action<float> OnZoom { get; set; }
        Action<Vector2> OnSwipe { get; set; }
        Action<Vector2> OnClick { get; set; }
    }
}