using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
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
    [SerializeField] private TextMeshProUGUI _breakTimeText;
    [SerializeField] private TextMeshProUGUI _breakTimeNb;
    
    [SerializeField] private GameObject _pauseMenu = null;
    [SerializeField] private GameObject _commandsMenu = null;
    [SerializeField] private GameObject _gameOverMenu = null;
    
    private Repeater _repeater;
    private bool _isPaused;
    private bool _isFirstRound;
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
        _isPaused = true;
        CommandScreen();
        SetScoreTexts();
        
        _repeater = GetComponent<Repeater>();
        _repeater.m_loopForever = true;
        _repeater.m_OnRepeat.AddListener(RandomizeEnemyActions);
        _repeater.m_repeatTime = _counterInterval;
        _repeater.StartRepeater();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPaused || _isFirstRound) return;
        _maxEnemies = _roundSystemSO.GetMaxEnemies();
        _hasAnyoneShot = _roundSystemSO.m_hasAnyoneShot;

        //RandomizeEnemyActions();
    }
    
    #endregion

    #region Main Methods
    
    public void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1;
        _pauseMenu.SetActive(_isPaused );
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        _gameOverMenu.SetActive(true);
    }

    private void CommandScreen()
    {
        _isFirstRound = true;
        _commandsMenu.SetActive(_isFirstRound);
        Time.timeScale = 0;
    }

    private void RandomizeEnemyActions()
    {
        if (_roundSystemSO.m_hasAnyoneShot) return;
        if(_spawnedEnemies == null) return;
        var rnd = Random.Range(0, 1f);
        var rndIndex = Random.Range(0, _spawnedEnemies.Count);
        Debug.Log($"RandomizeEnemyActions random index {rndIndex}");
        if (_spawnedEnemies == null) return;
        switch (rnd)
        {
            case > .6f:
                _spawnedEnemies[rndIndex].Attack();
                break;
            case > .4f:
                    StartCoroutine(_spawnedEnemies[rndIndex].IdleMovement(_roundSystemSO.GetBeatInterval()));
                break;
            
        }
    }
    
    public void PauseGame()
    {
        if (_isFirstRound)
        {
            _isFirstRound = false;
            _isPaused = _isFirstRound;
            Time.timeScale = 1;
            _commandsMenu.SetActive(false);
            return;
        }
        _isPaused = !_isPaused;
        Time.timeScale = _isPaused ? 0 : 1;
        _pauseMenu.SetActive(_isPaused);
    }

    public void IncreaseEnemiesAlive()
    {
        _enemiesAlive++;
        if (_enemiesAlive == _roundSystemSO.GetMaxEnemies()) m_disableEnemySpawn.Invoke();
    }
    
    // public void OnGUI()
    // {
    //     GUILayout.Button(" " + _roundSystemSO.GetMaxEnemies());
    // }

    public void IncreaseEnemiesKilled()
    {
        _enemiesKilled++;
        if (_enemiesKilled != _roundSystemSO.GetMaxEnemies()) return;
        StartCoroutine(PauseGame(3f));
        _roundSystemSO.IncreaseRound();
        SetScoreTexts();
        _repeater.m_repeatTime = _roundSystemSO.GetBeatInterval();
        _repeater.StopRepeater();
        _repeater.StartRepeater();
        m_resetSpawnPoints.Invoke();
        _spawnedEnemies.Clear();
        
        // if (_roundSystemSO.GetCurrentRound() % 3 == 0)
        // {
        //     //_roundSystemSO.IncreaseMaxEnemies();
        //     Debug.Log("Increased max enemies: " + _roundSystemSO.GetMaxEnemies());
        // }
        Debug.Log($"Passed to round {_roundSystemSO.GetCurrentRound()}");
        
        m_enableEnemySpawn.Invoke();
        _enemiesKilled = 0;
        _enemiesAlive = 0;
    }
    
    #endregion

    #region Utils

    private void SetScoreTexts()
    {
        _scoreText.text = $"Round {_roundSystemSO.GetCurrentRound()}";
        _highScoreText.text = $"HighScore: {_roundSystemSO.m_highScore}";
    }

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
    
    private IEnumerator PauseGame(float pauseTime){
        SetBreakTimerText(true);
        Debug.Log ("Inside PauseGame()");
        Time.timeScale = 0f;
        var timer = 4f;
        float pauseEndTime = Time.realtimeSinceStartup + pauseTime;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            _breakTimeNb.text = Mathf.FloorToInt(timer).ToString();
            timer -= Time.unscaledDeltaTime;
            yield return 0;
        }
        Time.timeScale = 1f;
        Debug.Log("Done with my pause");
        SetBreakTimerText(false);
    }

    private void SetBreakTimerText(bool active)
    {
        
        _breakTimeText.gameObject.SetActive(active);
        _breakTimeNb.gameObject.SetActive(active);
        
    }

    #endregion
}
