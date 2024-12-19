using System;
using Infrastructure.Interface;
using UnityEngine;

namespace Controllers.Input
{
    public interface IInput : IEnable
    {
        void Swap();
        void Zoom();

        Action<int> OnZoom { get; set; }
        Action<int> OnSwap { get; set; }
        Action<Vector2> OnClick { get; set; }
    }
}