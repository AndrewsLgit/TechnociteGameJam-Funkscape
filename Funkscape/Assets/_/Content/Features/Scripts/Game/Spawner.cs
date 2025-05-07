using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

//TODO: set all spawnPoints back to false after every round
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

    private GameManager _gameManager;
    //private Player _player;
    private float _spawnTimer;
    private bool[] _hasSpawnedEnemies;
    private bool _canSpawn = true;

    #endregion
    
    #region Unity API
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // _enemyPool = gameObject.GetComponent<SpawnPool>();
        _gameManager = FindFirstObjectByType<GameManager>();
        //_player = FindFirstObjectByType<Player>();
        _spawnTimer = 0;
        _hasSpawnedEnemies = new bool[_spawnPoints.Length];

    }

    // Update is called once per frame
    void Update()
    {
        if (_canSpawn) SpawnAsteroid();
    }
    #endregion
    
    #region Main Methods

    private void SpawnAsteroid()
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
        enemyInstance.transform.position = (Vector2)spawnPoint + posOffset; //+ (Vector2)_spawnPoint.localScale/2;
        //enemyInstance.SetActive(true);
        _hasSpawnedEnemies[randomIndex] = true;
        _spawnTimer = 0f;
        m_onSpawn.Invoke();
    }
    
    #endregion

    #region Utils

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
    }

    #endregion
}
