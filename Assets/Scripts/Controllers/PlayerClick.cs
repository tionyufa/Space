using System;
using System.Collections;
using Controllers.Input;
using Planets;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Controller
{
    public class PlayerClick : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        private IInput _input = new MobileInput();

        private void Awake()
        {
            _input.Init();
            
            _input.OnClick += OnClick;
        }

        private void OnDisable()
        {
            _input.OnClick -= OnClick;
        }

        private void OnClick(Vector2 position)
        {
            Ray ray = _camera.ScreenPointToRay(position);
            Debug.LogError(position);
            Debug.DrawRay(ray.origin,ray.direction,Color.cyan,2);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                var onPlanet = hit.transform.GetComponent<ClickOnPlanet>();
                if (onPlanet != null)
                    onPlanet.OnClick();

                Debug.Log(hit.transform.name);
            }
        }
        
        

        // private void Update()
        // {
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;
        //
        //     var raycasts = Physics.RaycastAll(ray); //Physics.Raycast(ray, out hit);
        //     if (raycasts.Length > 0)
        //         for (int i = 0; i < raycasts.Length; i++)
        //             Debug.LogError(raycasts[i].transform.name);
        //
        //     if (Input.GetMouseButtonDown(0))
        //     {
        //         
        //     }
        // }
    }
}