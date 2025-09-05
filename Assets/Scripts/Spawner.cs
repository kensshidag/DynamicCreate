using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Cube _cube;
    [SerializeField] private Collider _platformCollider;
    [SerializeField] private int _poolDefaultSize = 5;
    [SerializeField] private int _poolMaxSize = 5;
    [SerializeField] private float _spawnRate = 1f;

    private ObjectPool<Cube> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Cube>(
            createFunc: () => Instantiate(_cube),
            actionOnGet: (cube) => SetParameters(cube),
            actionOnRelease: (cube) => cube.gameObject.SetActive(false),
            actionOnDestroy: (cube) => DestroyCube(cube),
            collectionCheck: true,
            defaultCapacity: _poolDefaultSize,
            maxSize: _poolMaxSize);
    }

    private void Start()
    {
        StartCoroutine(SpawnTimer());
    }

    private void SetParameters(Cube cube)
    {
        cube.gameObject.SetActive(true);
        cube.transform.position = GetRandomSpawnPoint();
        cube.Touched += ReturnCubeToPool;
    }

    private void DestroyCube(Cube cube)
    {
        Destroy(cube.gameObject);
    }

    private Vector3 GetRandomSpawnPoint()
    {
        Vector3 position;
        Vector3 platformSize = _platformCollider.bounds.size;
        float decreaser = 0.8f;
        float spawnHeight = 26f;
        float platformWidthApothem = platformSize.x / 2;
        float platformLengthApothem = platformSize.z / 2;

        position = _platformCollider.transform.position;
        position.x = Random.Range(-platformWidthApothem, platformWidthApothem) * decreaser;
        position.z = Random.Range(-platformLengthApothem, platformLengthApothem) * decreaser;
        position.y = spawnHeight;

        return position;
    }

    private void ReturnCubeToPool(Cube cube)
    {
        cube.Touched -= ReturnCubeToPool;
        _pool.Release(cube);
    }

    private void SpawnCube()
    {
        _pool.Get();
    }

    private IEnumerator SpawnTimer()
    {
        var delay = new WaitForSeconds(_spawnRate);

        while (true)
        {
            yield return delay;
            SpawnCube();
        }
    }
}
