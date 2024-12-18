using Assets.Scripts.Planets;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Controller
{
    public class PlayerClick : MonoBehaviour

    {        
        private void Update()

        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            var raycasts = Physics.RaycastAll(ray); //Physics.Raycast(ray, out hit);
           // if (raycasts.Length > 0)
                //for (int i = 0; i < raycasts.Length; i++)
                    //Debug.LogError(raycasts[i].transform.name);

            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out hit))
                {
                    var onPlanet = hit.transform.GetComponent<ClickOnPlanet>();
                    if (onPlanet != null)
                        onPlanet.OnClick();

                    Debug.Log(hit.transform.name);
                }
            }
        }
    }
}