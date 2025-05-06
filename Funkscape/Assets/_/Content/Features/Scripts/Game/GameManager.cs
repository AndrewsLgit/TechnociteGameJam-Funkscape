using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{

    #region Public Variables

    public UnityEvent m_disableEnemySpawn;
    public UnityEvent m_enableEnemySpawn;
    public UnityEvent m_resetSpawnPoints;

    #endregion
    
    #region Private Variables
    
    [SerializeField] private RoundSystemSO _roundSystemSO;
    private int _enemiesAlive;
    private int _enemiesKilled;
    private int _maxEnemies;
    
    #endregion
    
    #region Unity API
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _enemiesAlive = 0;
        _enemiesKilled = 0;
    }

    // Update is called once per frame
    void Update()
    {
        _maxEnemies = _roundSystemSO.GetMaxEnemies();
    }
    
    #endregion

    #region Main Methods

    public void IncreaseEnemiesAlive()
    {
        _enemiesAlive++;
        if (_enemiesAlive == _maxEnemies) m_disableEnemySpawn.Invoke();
    }

    public void IncreaseEnemiesKilled()
    {
        _enemiesKilled++;
        if (_enemiesKilled != _maxEnemies) return;
        _roundSystemSO.IncreaseRound();
        m_resetSpawnPoints.Invoke();
        
        if (_roundSystemSO.GetCurrentRound() % 3 == 0)
        {
            _roundSystemSO.IncreaseMaxEnemies();
            Debug.Log("Increased max enemies: " + _maxEnemies);
        }
        Debug.Log($"Passed to round {_roundSystemSO.GetCurrentRound()}");
        
        m_enableEnemySpawn.Invoke();
        _enemiesKilled = 0;
        _enemiesAlive = 0;
    }
    
    

    #endregion
}
