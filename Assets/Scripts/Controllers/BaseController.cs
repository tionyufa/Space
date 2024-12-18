using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseController : IInput
{
    [SerializeField]
    private InputAction action;
    [SerializeField]
    private InputActionMap _map;
    public void Click()
    {
        
    }

    public void Swap()
    {

    }

    public void Zoom()
    {

    }
}

public interface IInput
{
    void Click();

    void Swap();

    void Zoom();
}