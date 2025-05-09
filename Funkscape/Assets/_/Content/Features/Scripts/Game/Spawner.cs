using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    [SerializeField] private float _spawnInterval = 1.5f;
    [SerializeField] private EnemyBehavior _enemyPrefab;
    [SerializeField] private float _spawnRadius = .5f;
    [SerializeField] private GameObject _captainArm; 
    [SerializeField] private RoundSystemSO _roundSystem;


    private GameManager _gameManager;
    private EnemyBehavior[] _spawnedEnemies;
    private float _spawnTimer;
    private bool[] _hasSpawnedEnemies;
    private bool _canSpawn = true;
    private int _maxEnemies;
    private int _currentEnemyIndex;
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
        _currentEnemyIndex = 0;

        _maxEnemies = _roundSystem.GetMaxEnemies();
        _hasSpawnedEnemies = new bool[_spawnPoints.Length];
        _enemySpawnIndices = GetSpawnIndices();
        ResetSpawnPoints();

    }

    // Update is called once per frame
    void Update()
    {
        _maxEnemies = _roundSystem.GetMaxEnemies();
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
        if (_currentEnemyIndex >= _maxEnemies) return;
        
        var posOffset = Random.insideUnitCircle * _spawnRadius;
        Debug.Log($"SpawnEnemy() | _currentEnemyIndex: {_currentEnemyIndex}");
        Debug.Log($"SpawnEnemy() | _enemySpawnIndices[_currentEnemyIndex]: {_enemySpawnIndices[_currentEnemyIndex]}");
        var index = _enemySpawnIndices[_currentEnemyIndex];
        Debug.Log($"SpawnEnemy() | Random index from array: {index}");
        //var randomIndex = Random.Range(0, _spawnPoints.Length);
        var spawnPoint = _spawnPoints[index].position;
        
        if (_hasSpawnedEnemies[index]) return;
        
        //GameObject enemyInstance = _enemyPool.GetFirstAvailableProjectile();
        var enemyInstance = Instantiate(_enemyPrefab, transform);

        //_spawnedEnemies[_currentEnemyIndex] = enemyInstance;
        _gameManager.AddSpawnedEnemy(enemyInstance);
        enemyInstance.m_onEnemyDestoyed.AddListener(_gameManager.IncreaseEnemiesKilled);
        enemyInstance.SetSpawnIndex(index);
        
        enemyInstance.transform.position = (Vector2)spawnPoint + posOffset; //+ (Vector2)_spawnPoint.localScale/2;
        //enemyInstance.SetActive(true);
        _hasSpawnedEnemies[index] = true;
        _spawnTimer = 0f;
        _currentEnemyIndex++;
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
        _maxEnemies = _roundSystem.GetMaxEnemies();
        List<int> spawnIndices = new List<int>(){ 0, 1, 2, 3, 4 };
        
        int[] indices = new int[_maxEnemies];
        for (int i = 0; i < _maxEnemies; i++)
        {
            var randomIndex = Random.Range(0, spawnIndices.Count);
            //var randomIndex = Random.Range(0, _spawnPoints.Length);
            // foreach (var index in indices)
            // {
            //     if(index == randomIndex) randomIndex = Random.Range(0, _spawnPoints.Length);
            // }
            indices[i] = spawnIndices[randomIndex];
            spawnIndices.RemoveAt(randomIndex);
        }
        Debug.Log($"GetSpawnIndices() | Random indices: {indices}");
        return indices;
    }

    private void CaptainPointAtSpawns()
    {
        // for (int i = 0; i < _enemySpawnIndices.Length; i++)
        // {
        //     Vector2 direction = _spawnPoints[_enemySpawnIndices[i]].transform.position - _captainArm.transform.position;
        //     var armRotation = Quaternion.LookRotation(Vector3.forward, direction);
        //     _captainArm.transform.rotation = armRotation;
        //     //new WaitForSeconds(_spawnInterval);
        //     //_weaponPoint.rotation = playerRotation;
        // }

        // for (int i = 0; i < _enemySpawnIndices.Length; i++)
        // {
        //     Vector2 direction = _spawnPoints[i].transform.position - _captainArm.transform.position;
        //     var armRotation = Quaternion.LookRotation(Vector3.forward, direction);
        //
        //     var tween = _captainArm.transform.DORotate(direction, _spawnInterval);
        //     // tween.OnUpdate(() => _isBlinking = tween.position >= _70percentTime);
        //     // tween.OnUpdate(() =>
        //     // {
        //     //     _currentTweenTime = tween.position;
        //     //     if (tween.position >= _timeToMaxSize * 0.5f && _canBlink)
        //     //     {
        //     //         //Blinker();
        //     //         Debug.Log($"Starting Blinker with {_timeToMaxSize - tween.position}");
        //     //
        //     //         m_onBlink?.Invoke(_timeToMaxSize - tween.position);
        //     //         _canBlink = false;
        //     //     }
        //     // });
        // }
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
        _currentEnemyIndex = 0;
        _spawnedEnemies = new EnemyBehavior[_maxEnemies];
        //CaptainPointAtSpawns();
    }

    #endregion
}
