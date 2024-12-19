using UnityEngine;

namespace Planets
{
    public class ClickOnPlanet : MonoBehaviour
    {

        [SerializeField]
        private MeshRenderer _meshRenderer;

        public void OnClick()
        {
            var property = _meshRenderer.material.GetPropertyNames(MaterialPropertyType.Float);

            var effect = _meshRenderer.material.GetFloat("Test");
            Debug.Log(effect.ToString());

            _meshRenderer.material.SetFloat("Test", 10);
        }
    }
}
