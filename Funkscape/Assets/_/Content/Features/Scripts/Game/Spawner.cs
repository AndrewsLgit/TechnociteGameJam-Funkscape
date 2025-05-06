using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

//TODO: fix issue where enemies stop spawning after a certain amount of time
public class Spawner : MonoBehaviour
{
    #region Public Variables

    public UnityEvent m_onSpawn;

    #endregion
    
    #region Private Variables

    //TODO: modify spawner so that there are around 5 fixed spawn points with a spawn radius around them
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private int _maxEnemies;
    [SerializeField] private float _spawnInterval = 1.5f;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _spawnRadius = .5f;

    private float _spawnTimer;
    //private GameManager _gameManager;
    private Player _player;

    #endregion
    
    #region Unity API
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // _enemyPool = gameObject.GetComponent<SpawnPool>();
        //_gameManager = FindFirstObjectByType<GameManager>();
        _player = FindFirstObjectByType<Player>();
        _spawnTimer = 0;

    }

    // Update is called once per frame
    void Update()
    {
        SpawnAsteroid();
    }
    #endregion
    
    #region Main Methods

    private void SpawnAsteroid()
    {
        _spawnTimer += Time.deltaTime;
        if (!(_spawnTimer >= _spawnInterval)) return;
        //GameObject enemyInstance = _enemyPool.GetFirstAvailableProjectile();
        var enemyInstance = Instantiate(_enemyPrefab, transform);
        enemyInstance.SetActive(false);
            
        // var enemyBehavior = enemyInstance.GetComponent<EnemyBehavior>();
        // enemyBehavior.m_onEnemyDestroyed.AddListener(_gameManager.OnScore);
        // enemyBehavior.m_onPlayerHit.AddListener(_player.OnDeath);
            
        //var randomPos = new Vector2(Random.Range(-_spawnArea.localScale.x, _spawnArea.localScale.x), Random.Range(-_spawnArea.localScale.y, _spawnArea.localScale.y));
            
        var posOffset = Random.insideUnitCircle * _spawnRadius;
        var spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)].position;
        enemyInstance.transform.position = (Vector2)spawnPoint + posOffset; //+ (Vector2)_spawnPoint.localScale/2;
        enemyInstance.SetActive(true);
        _spawnTimer = 0f;
    }
    
    #endregion
}
