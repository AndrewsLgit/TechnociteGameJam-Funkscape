using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// todo: add repeater for captain's dance timing

// after 5 rounds decrease action interval
public class GameManager : MonoBehaviour
{

    #region Public Variables

    public UnityEvent m_disableEnemySpawn;
    public UnityEvent m_enableEnemySpawn;
    public UnityEvent m_resetSpawnPoints;

    #endregion
    
    #region Private Variables
    
    [SerializeField] private RoundSystemSO _roundSystemSO;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    private Repeater _repeater;
    private int _enemiesAlive;
    private int _enemiesKilled;
    private int _maxEnemies;
    private float _counterInterval = .4f;
    private List<EnemyBehavior> _spawnedEnemies = new List<EnemyBehavior>();
    
    private bool _hasAnyoneShot;

    #endregion
    
    #region Unity API
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _enemiesAlive = 0;
        _enemiesKilled = 0;
        
        _repeater = GetComponent<Repeater>();
        _repeater.m_loopForever = true;
        _repeater.m_OnRepeat.AddListener(Counter);
        _repeater.m_repeatTime = _counterInterval;
        _repeater.StartRepeater();
    }

    // Update is called once per frame
    void Update()
    {
        _maxEnemies = _roundSystemSO.GetMaxEnemies();
        _hasAnyoneShot = _roundSystemSO.m_hasAnyoneShot;
    }
    
    #endregion

    #region Main Methods

    public void RestartGame()
    {
        
    }

    public void StartGame()
    {
        
    }

    public void PauseGame()
    {
        
    }

    public void IncreaseEnemiesAlive()
    {
        _enemiesAlive++;
        if (_enemiesAlive == _roundSystemSO.GetMaxEnemies()) m_disableEnemySpawn.Invoke();
    }
    
    public void OnGUI()
    {
        GUILayout.Button(" " + _roundSystemSO.GetMaxEnemies());
    }

    public void IncreaseEnemiesKilled()
    {
        _enemiesKilled++;
        if (_enemiesKilled != _roundSystemSO.GetMaxEnemies()) return;
        _roundSystemSO.IncreaseRound();
        m_resetSpawnPoints.Invoke();
        _spawnedEnemies.Clear();
        
        if (_roundSystemSO.GetCurrentRound() % 3 == 0)
        {
            //_roundSystemSO.IncreaseMaxEnemies();
            Debug.Log("Increased max enemies: " + _roundSystemSO.GetMaxEnemies());
        }
        Debug.Log($"Passed to round {_roundSystemSO.GetCurrentRound()}");
        
        m_enableEnemySpawn.Invoke();
        _enemiesKilled = 0;
        _enemiesAlive = 0;
    }
    
    #endregion

    #region Utils

    private void Counter()
    {
        Debug.Log($"Counter triggered with interval: {_counterInterval}");
    }

    public void AddSpawnedEnemy(EnemyBehavior enemy)
    {
        _spawnedEnemies.Add(enemy);
    }

    public void KillEnemy(EnemyBehavior enemy)
    {
        var index = _spawnedEnemies.IndexOf(enemy);
        
        Debug.Log($"Enemy {_spawnedEnemies[index].name} was killed");
        _spawnedEnemies.RemoveAt(index);
    }

    #endregion
}
