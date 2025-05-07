using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Repeater : MonoBehaviour
{
    #region Public Variables
    
    public UnityEvent m_OnStartupEnd = new();
    public UnityEvent m_OnRepeat = new();

    public float m_startupTime = 0;
    public float m_repeatTime = 1;
    public int m_repeatCount = 0;
    public bool m_loopForever = false; 
    
    #endregion
    
    #region Private Variables
    
    private float _repeatDelta = 0;
    private float _startupDelta = 0;
    private int _repeatCount = 0;
    private bool _startRepeater = false;
    
    #endregion

    #region Unity API

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnEnable()
    {
        _repeatCount = m_repeatCount;
        _repeatDelta = m_repeatTime;
        _startupDelta = m_startupTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (_startRepeater)
        {
            PlayRepeater();
        }
    }
    
    #endregion
    

    #region Main Methods
    
    private void PlayRepeater()
    {
        if (_startupDelta < 0)
        {
            if (_repeatDelta < 0)
            {
                _repeatDelta = m_repeatTime;
                m_OnRepeat.Invoke();
                if (!m_loopForever)
                {
                    _repeatCount--;
                    if (_repeatCount < 0)
                    {
                        this.enabled = false;
                    }
                }
            }
            else
            {
                _repeatDelta -= Time.deltaTime;
            }
        }
        else
        {
            _startupDelta -= Time.deltaTime;
            m_OnStartupEnd.Invoke();
        }
    }
    
    #endregion

    #region Utils

    public void StartRepeater()
    {
        _startRepeater = true;
        _repeatDelta = m_repeatTime;
        _startupDelta = m_startupTime;
        _repeatCount = m_repeatCount;
    }

    public void StopRepeater()
    {
        _startRepeater = false;
    }
    
    public int GetCurrentRepeatCount() =>  _repeatCount;

    #endregion
}
