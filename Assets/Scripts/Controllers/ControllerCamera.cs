using System;
using System.Collections;
using System.Threading.Tasks;
using Cinemachine;
using Controllers.Input;
using Planets;
using UnityEngine;

namespace Controllers
{
    public class ControllerCamera : MonoBehaviour
    {
        public Action<PlanetsName> OnClickToPlanet;

        [SerializeField]
        private Transform _defaultTarget;

        [SerializeField] 
        private Transform _target;

        [SerializeField] 
        private float _radius;

        [SerializeField] 
        private float _yPosition;

        [SerializeField] 
        private float _speed;

        [SerializeField] 
        private float _speedZoom;

        [SerializeField]
        private float _angel;
        
        [SerializeField] 
        private float _speedToPlanet;

        [SerializeField] 
        private float _distance;

        private IInput _input;
        private Camera _camera;
        private CinemachineVirtualCamera _virtualCamera;
        public void Construct(IInput input,Transform center)
        {
            Debug.LogError(center);
            _defaultTarget = center;
            _input = input;
            
            Init();
            SetTarget(_defaultTarget);
        }
        private void Init()
        {
            _input.Init();
            _camera = GetComponentInChildren<Camera>();
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            
            _input.OnZoom += HandleZoom;
            _input.OnSwap += HandlePan;
            _input.OnClick += OnClick;
        }

        // private void SetTarget(Transform target)
        // {
        //     _target = target;
        //     _virtualCamera.LookAt = target;
        // }
        //
        // private void OnDisable()
        // {
        //     _input.OnDisable();
        //     _input.OnZoom -= OnZoom;
        //     _input.OnSwap -= OnSwap;
        //     _input.OnClick -= OnClick;
        // }
        //
        // private void OnSwap(int direction)
        // {
        //     _angel +=  direction *  _speed * Time.deltaTime;
        //     Move();
        // }
        //
        // private void OnZoom(int direction)
        // {
        //     _radius = Mathf.Clamp( _radius += _speedZoom * direction * Time.deltaTime,10,100);
        //     
        //     Move();
        // }
        private void OnClick(Vector2 position)
        {
            Ray ray = _camera.ScreenPointToRay(position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var onPlanet = hit.transform.GetComponent<MovingPlanet>();
                if (onPlanet)
                {
                    onPlanet.StopSpeed();
                    SetTarget(onPlanet.transform);
                    OnClickToPlanet?.Invoke(onPlanet.namePlanet);
                    
                    //StartCoroutine(nameof(MoveToPlanet));
                }
            }
        }
        //
        // private async void Move()
        // {
        //     if (_target)
        //     {
        //         var x = Mathf.Cos(_angel) * _radius;
        //         var z = Mathf.Sin(_angel) * _radius;
        //         var y = Mathf.Lerp(0, _yPosition, _radius / 100);
        //
        //         var target = new Vector3(_target.position.x + x, y, _target.position.z + z);
        //
        //         while (Vector3.Distance(transform.position, target) > 0.5)
        //         {
        //             transform.position = target;// Vector3.MoveTowards(transform.position, target, 0.2f);
        //             await Task.Yield();
        //         }
        //     }
        // }
        //
        // private IEnumerator MoveToPlanet()
        // {
        //     while (_radius > _distance)
        //     {
        //         _radius -= Time.deltaTime * _speedToPlanet;
        //         Move();
        //         yield return null;
        //     }
        // }
        
        public Transform target; // Текущий таргет (цель камеры)
        public float panSpeed = 10f; // Скорость панорамирования (движения по горизонтали/вертикали)
    public float zoomSpeed = 10f; // Скорость изменения зума
    public float minZoom = 5f; // Минимальный зум
    public float maxZoom = 50f; // Максимальный зум

    [SerializeField]
    private Vector3 offset; // Смещение камеры относительно таргета
    private float currentZoom = 20f; // Текущий зум
    private Vector3 currentPan; // Текущее смещение по горизонтали и вертикали

    void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.position; // Рассчитать начальное смещение камеры
        }
    }

    void Update()
    {
        if (target == null) return;
        
        UpdateCameraPosition();
    }

    // Обработка зума
    void HandleZoom(int zoom)
    {
        //float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= zoom * zoomSpeed * Time.deltaTime;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    // Обработка панорамирования (движения по горизонтали и вертикали)
    void HandlePan(Vector2 vector2)
    {
        // if (Input.GetMouseButton(1)) // Правая кнопка мыши
        // {
        
        float horizontal = vector2.y * panSpeed * Time.deltaTime;
        float vertical = vector2.x * panSpeed * Time.deltaTime;

        Debug.LogError(vector2);
        currentPan += new Vector3(-vertical, horizontal, 0);
        //}
    }

    // Обновление позиции камеры
    void UpdateCameraPosition()
    {
        // Рассчитать новую позицию камеры
        Vector3 desiredPosition = target.position + offset;
        desiredPosition += currentPan;

        // Установить позицию камеры с учетом зума
        transform.position = desiredPosition - transform.forward * currentZoom;
        transform.LookAt(target.position);
    }

    // Переключение таргета
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        offset = transform.position - target.position;
        currentPan = Vector3.zero; // Сброс панорамирования
    }
        
    
    }
}