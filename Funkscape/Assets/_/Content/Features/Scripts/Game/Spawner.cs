using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

//TODO: make captain point to all random spawns before actually spawning enemies
public class Spawner : MonoBehaviour
{
    #region Public Variables

    public UnityEvent m_onSpawn;

    #endregion
    
    #region Private Variables

    [SerializeField] private Transform[] _spawnPoints;
    //[SerializeField] private int _maxEnemies;
    [SerializeField] private float _spawnInterval = 1.5f;
    [SerializeField] private EnemyBehavior _enemyPrefab;
    [SerializeField] private float _spawnRadius = .5f;
    [SerializeField] private GameObject _captainArm;

    private GameManager _gameManager;
    private RoundSystemSO _roundSystem;
    //private Player _player;
    private float _spawnTimer;
    private bool[] _hasSpawnedEnemies;
    private bool _canSpawn = true;
    private int _maxEnemies;
    private int _currentEnemies;
    private int[] _enemySpawnIndices;

    #endregion
    
    #region Unity API
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // _enemyPool = gameObject.GetComponent<SpawnPool>();
        _gameManager = FindFirstObjectByType<GameManager>();
        //_player = FindFirstObjectByType<Player>();
        _spawnTimer = 0;
        _currentEnemies = 0;
//        _maxEnemies = _roundSystem.GetMaxEnemies();
        _hasSpawnedEnemies = new bool[_spawnPoints.Length];

    }

    // Update is called once per frame
    void Update()
    {
        if (_canSpawn)
        {
            //_maxEnemies = _roundSystem.GetMaxEnemies();
            SpawnEnemy();
        }
    }
    #endregion
    
    #region Main Methods

    private void SpawnEnemy()
    {
        _spawnTimer += Time.deltaTime;
        if (!(_spawnTimer >= _spawnInterval)) return;
        
        var posOffset = Random.insideUnitCircle * _spawnRadius;
        var randomIndex = Random.Range(0, _spawnPoints.Length);
        var spawnPoint = _spawnPoints[randomIndex].position;
        
        if (_hasSpawnedEnemies[randomIndex]) return;
        
        //GameObject enemyInstance = _enemyPool.GetFirstAvailableProjectile();
        var enemyInstance = Instantiate(_enemyPrefab, transform);
        //todo use enemy behaviour
        enemyInstance.m_onEnemyDestoyed.AddListener(_gameManager.IncreaseEnemiesKilled);
        enemyInstance.SetSpawnIndex(randomIndex);
        
        enemyInstance.transform.position = (Vector2)spawnPoint + posOffset; //+ (Vector2)_spawnPoint.localScale/2;
        //enemyInstance.SetActive(true);
        _hasSpawnedEnemies[randomIndex] = true;
        _spawnTimer = 0f;
        m_onSpawn.Invoke();
    }

    // private void SpawnAllEnemies()
    // {
    //     _spawnTimer += Time.deltaTime;
    //     if (!(_spawnTimer >= _spawnInterval)) return;
    //     var posOffset = Random.insideUnitCircle * _spawnRadius;
    //
    //     if ()
    //     {
    //         
    //     }
    // }
    
    #endregion

    #region Utils

    private int[] GetSpawnIndices()
    {
        int[] indices = new int[_maxEnemies];
        for (int i = 0; i < _maxEnemies; i++)
        {
            var randomIndex = Random.Range(0, _spawnPoints.Length);
            indices[i] = randomIndex;
        }
        return indices;
    }

    public void SetSpawnAvailable(int index)
    {
        _hasSpawnedEnemies[index] = false;
    }

    public void SetCanSpawn(bool canSpawn)
    {
        _canSpawn = canSpawn;
    }

    public void ResetSpawnPoints()
    {
        for (int i = 0; i < _hasSpawnedEnemies.Length; i++)
        {
            _hasSpawnedEnemies[i] = false;
        }

        _enemySpawnIndices = GetSpawnIndices();
    }

    #endregion
}
