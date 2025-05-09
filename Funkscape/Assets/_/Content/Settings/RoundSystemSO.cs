using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundSystemSO", menuName = "Scriptable Objects/RoundSystemSO")]
public class RoundSystemSO : ScriptableObject
{
    private int _maxEnemies;
    private int _currentRound;
    private int _highScore;
    private float _beatInterval;
    private bool _hasAnyoneShot;

    public int m_highScore => _highScore; 
    public bool m_hasAnyoneShot => _hasAnyoneShot;

    private void OnEnable()
    {
        _currentRound = 1;
        _maxEnemies = 2;
        _beatInterval = .5f;
        _hasAnyoneShot = false;
    }

    public void IncreaseMaxEnemies()
    {
        _maxEnemies++;
    }

    public void DecreaseBeatInterval()
    {
        _beatInterval -= .1f;
    }

    public void IncreaseRound()
    {
        _currentRound++;
        _highScore = Mathf.Max(_highScore, _currentRound);
        if (_currentRound % 3 != 0) return;
        if(_maxEnemies < 5) IncreaseMaxEnemies();
        if(_currentRound >= 5) DecreaseBeatInterval();
    }
    
    public int GetMaxEnemies() => _maxEnemies;
    
    public int GetCurrentRound() => _currentRound;
    
    public float GetBeatInterval() => _beatInterval;
    
    public void SetHasAnyoneShot(bool hasAnyoneShot) => _hasAnyoneShot =  hasAnyoneShot;
}
