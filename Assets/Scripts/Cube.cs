using System.Collections;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Renderer), typeof(Renderer))]
public class Cube : MonoBehaviour
{
    [SerializeField] private float _minLifeTime = 2f;
    [SerializeField] private float _maxLifeTime = 5f;

    private Renderer _renderer;
    private Rigidbody _rigidbody;
    private Color _defaultColor;
    private bool _isTouched = false;
    private float _lifeTime;

    public event Action<Cube> Touched;

    private void Awake()
    {
        _isTouched = false;
        _renderer = GetComponent<Renderer>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _defaultColor = _renderer.material.color;
        _lifeTime = Random.Range(_minLifeTime, _maxLifeTime);
    }

    private void OnEnable()
    {
        ResetVelocity();
        _renderer.material.color = _defaultColor;
        _isTouched = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isTouched)
        {
            return;
        }

        if (collision.transform.TryGetComponent<Platform>(out Platform platform))
        {
            _isTouched = true;
            _renderer.material.color = new Color(Random.value, Random.value, Random.value);
            StartCoroutine(WaitForSeconds(_lifeTime));
        }
    }

    private void ResetVelocity()
    {
        if (_rigidbody.velocity != Vector3.zero)
        {
            _rigidbody.velocity = Vector3.zero;
        }
    }

    private IEnumerator WaitForSeconds(float lifeTime)
    {
        var wait = new WaitForSeconds(lifeTime);

        yield return wait;
        Touched?.Invoke(this);
    }
}
