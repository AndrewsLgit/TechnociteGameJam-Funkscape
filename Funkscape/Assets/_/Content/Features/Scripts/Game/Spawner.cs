using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Spawner : MonoBehaviour
{
    #region Private Variables

    //TODO: modify spawner so that there are around 5 fixed spawn points with a spawn radius around them
    [SerializeField] private Transform _spawnArea;
    [SerializeField] private int _maxEnemies;
    [SerializeField] private bool _isSpawnerCoroutine = false;
    [SerializeField] private float _spawnInterval = 1.5f;

    private SpawnPool _enemyPool;
    private float _spawnTimer;
    //private GameManager _gameManager;
    private Player _player;

    #endregion
    
    #region Unity API
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // _enemyPool = gameObject.GetComponent<SpawnPool>();
        _enemyPool = gameObject.GetComponentInChildren<SpawnPool>();
        //_gameManager = FindFirstObjectByType<GameManager>();
        _player = FindFirstObjectByType<Player>();
        _spawnTimer = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (_isSpawnerCoroutine) StartCoroutine(SpawnAsteroidCoroutine());
        else SpawnAsteroid();
    }
    #endregion
    
    #region Main Methods

    private void SpawnAsteroid()
    {
        _spawnTimer += Time.deltaTime;
        int activeEnemies = _enemyPool.ActiveProjectileCount;
        if (_spawnTimer >= _spawnInterval && activeEnemies < _maxEnemies)
        {
            GameObject enemyInstance = _enemyPool.GetFirstAvailableProjectile();
            
            // var enemyBehavior = enemyInstance.GetComponent<EnemyBehavior>();
            // enemyBehavior.m_onEnemyDestroyed.AddListener(_gameManager.OnScore);
            // enemyBehavior.m_onPlayerHit.AddListener(_player.OnDeath);
            
            var randomPos = new Vector2(Random.Range(-_spawnArea.localScale.x, _spawnArea.localScale.x), Random.Range(-_spawnArea.localScale.y, _spawnArea.localScale.y));
            enemyInstance.transform.position = randomPos; //+ (Vector2)_spawnPoint.localScale/2;
            enemyInstance.SetActive(true);
            _spawnTimer = 0f;
        }
    }

    private IEnumerator SpawnAsteroidCoroutine()
    {
        yield return new WaitForSeconds(_spawnInterval);

        if (_enemyPool.ActiveProjectileCount >= _maxEnemies) yield break;
        
        GameObject enemyInstance = _enemyPool.GetFirstAvailableProjectile();
        
        // var enemyBehavior = enemyInstance.GetComponent<EnemyBehavior>();
        // enemyBehavior.m_onAsteroidDestroyed.AddListener(_gameManager.OnScore);
        // enemyBehavior.m_onPlayerHit.AddListener(_player.OnDeath);
        
        var randomPos = new Vector2(Random.Range(-_spawnArea.localScale.x, _spawnArea.localScale.x),
            Random.Range(-_spawnArea.localScale.y, _spawnArea.localScale.y));
        enemyInstance.transform.position = randomPos; //+ (Vector2)_spawnPoint.localScale/2;
        enemyInstance.SetActive(true);
    }
    
    #endregion
}
