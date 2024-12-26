using UnityEngine;

namespace Planets
{
    public class MovingPlanet : MonoBehaviour
    {
        [SerializeField] 
        private PlanetsName _name;
        public PlanetsName namePlanet => _name;

        [SerializeField]
        private Transform _center;

        [SerializeField]
        private LineRenderer _line;

        private PlanetData _data;
        private float _speed = 1;
        private float _speedCashe;
        private float _radius = 5f;
        private float _angel;
        private float _speedRotation = -5f;
        private int _segments = 100;

        public void Construct(PlanetData data, LineRenderer line,Transform center)
        {
            _data = data;
            _center = center;
            _line = line;

            _name = _data.name;
            Init();
        }
        private void Init()
        {
            SetSetting();
            DrawLine();
            _line.loop = true;
            _speedRotation = _data.speedRotationY;
            _angel = Random.Range(0, 180);
        }

        private void SetSetting()
        {
            _line.startColor = Color.white;
            _line.endColor = Color.white;
            
            _speed = _data.speed;
            _speedCashe = _speed;
            
            _radius = _data.radius;
        }

        private void DrawLine()
        {
            if (!_line)
                return;

            _line.positionCount = _segments + 1;
            _line.useWorldSpace = false;

            var angleStep = 360f / _segments;

            for (int i = 0; i <= _segments; i++)
            {
                var angle = Mathf.Deg2Rad * angleStep * i;
                var x = Mathf.Cos(angle) * _radius;
                var z = Mathf.Sin(angle) * _radius;
                var vector = new Vector3( x, _center.position.y,  z);
                _line.SetPosition(i, vector);

            }
        }

        private void Update()
        {
            if (!_line || _center == null)
                return;
        
            _angel += _speed * Time.deltaTime;

            var x = Mathf.Cos(_angel) * _radius;
            var z = Mathf.Sin(_angel) * _radius;

            var vector = new Vector3(_center.position.x + x, _center.position.y, _center.position.z + z);
            transform.position = vector;

            var euler = transform.rotation.eulerAngles;
            euler.y += Time.deltaTime * _speedRotation;
            transform.rotation = Quaternion.Euler(euler);

            DrawLine();
        }

        public void StopSpeed() => _speed = 0;

        public void ResetSpeed() => _speed = _speedCashe;
    }
}
