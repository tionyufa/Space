using Planets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIDescription : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Image _image;


        public void Construct(PlanetData planetData)
        {
            if (_name)
                _name.text = planetData.data.ToString();
            if (_description)
                _description.text = planetData.description.ToString();
            if (_image)
                _image.sprite = planetData.image;
        }
    }
}