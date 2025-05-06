using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RoundSystemSO", menuName = "Scriptable Objects/RoundSystemSO")]
public class RoundSystemSO : ScriptableObject
{
    private int _maxEnemies;
    private int _currentRound;
    private int _highScore;

    public int HighScore => _highScore; 

    private void OnEnable()
    {
        _currentRound = 1;
        _maxEnemies = 2;
    }

    public void IncreaseMaxEnemies()
    {
        _maxEnemies++;
    }

    public void IncreaseRound()
    {
        _currentRound++;
        _highScore = Mathf.Max(_highScore, _currentRound);
        //if (_currentRound % 3 == 0) IncreaseMaxEnemies();
    }
    
    public int GetMaxEnemies() => _maxEnemies;
    public int GetCurrentRound() => _currentRound;
}
