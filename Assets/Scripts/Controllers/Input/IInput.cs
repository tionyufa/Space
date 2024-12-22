using System;
using Infrastructure.Interface;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controllers.Input
{
    public interface IInput : IEnable
    {
        void Block(bool blocked);
        void Swap(InputAction input);
        void Zoom();

        Action<int> OnZoom { get; set; }
        Action<Vector2> OnSwap { get; set; }
        Action<Vector2> OnClick { get; set; }
    }
}