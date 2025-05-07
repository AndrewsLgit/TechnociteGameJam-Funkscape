using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

// Projectile causes indicator to blink 4 times - timer (to decide) -> from 1st-3rd: vulnerable to damage when parrying, at 4th blink -> damage to player -> projectile disappears
// Right after spawn, trigger behaviour
// Projectile triggers indicator at ~70% max size
public class EnemyProjectile : MonoBehaviour
{
    #region Public Variables

    public UnityEvent<float> m_onBlink;

    #endregion

    #region Private Variables

    [SerializeField] private float _maxSize = 8f;
    [SerializeField] private float _timeToMaxSize = 2f;
    [SerializeField] private float _timeToDestroy = 3f;

    private float _70percentTime;
    private float _10percentTime;
    private float _currentTweenTime = 0f;
    private Vector2 _startScale;
    private bool _canBlink;
    private int _maxBlinkNb = 4;
    private Player _player;
    
    #endregion
    
    #region Unity API
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _70percentTime= _timeToMaxSize * 0.7f;
        _10percentTime = _timeToMaxSize * 0.1f;
        _startScale = transform.localScale;
        _canBlink = true;
        _player = FindFirstObjectByType<Player>();
        m_onBlink.AddListener(_player.StartRepeater);
        DOTween.Init(recycleAllByDefault: true);
        IncreaseSize();
    }

    // Update is called once per frame
    void Update()
    {
        //if (_isBlinking) Blinker();
    }

    private void Blinker()
    {
        // yield return new WaitForSeconds(_timeToMaxSize * 0.1f);
        // if (_canBlink)
        // {
        //     Debug.Log($"Starting Blinker");
        //
        //     m_onBlink?.Invoke(_timeToMaxSize - _currentTweenTime);
        //     _canBlink = false;
        // }
    }

    private void OnEnable()
    {
        Invoke(nameof(Deactivate), _timeToDestroy);
        _70percentTime= _timeToMaxSize * 0.7f;
        _10percentTime = _timeToMaxSize * 0.1f;
        _canBlink = true;
        _player = FindFirstObjectByType<Player>();
        m_onBlink.AddListener(_player.StartRepeater);
        DOTween.Init(recycleAllByDefault: true);
        IncreaseSize();
    
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Deactivate));
        DOTween.Clear();
    }

    #endregion
    
    #region Main Methods

    private void IncreaseSize()
    {
        var tween = transform.DOScale(_maxSize, _timeToMaxSize)
            .SetEase(Ease.OutCirc);
        
        // tween.OnUpdate(() => _isBlinking = tween.position >= _70percentTime);
        tween.OnUpdate(() =>
        {
            _currentTweenTime = tween.position;
            if (tween.position >= _timeToMaxSize * 0.5f && _canBlink)
            {
                //Blinker();
                Debug.Log($"Starting Blinker with {_timeToMaxSize - tween.position}");

                m_onBlink?.Invoke(_timeToMaxSize - tween.position);
                _canBlink = false;
            }
        });
    }
    
    #endregion

    #region Utils

    private void Deactivate()
    {
        gameObject.SetActive(false);
        transform.localScale = _startScale;
    }

    #endregion
}
