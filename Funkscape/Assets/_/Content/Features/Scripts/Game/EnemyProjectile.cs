using System;
using DG.Tweening;
using UnityEngine;

// Projectile causes indicator to blink 4 times - timer (to decide) -> from 1st-3rd: vulnerable to damage when parrying, at 4th blink -> damage to player -> projectile disappears
// Right after spawn, trigger behaviour
// Projectile triggers indicator at ~70% max size
public class EnemyProjectile : MonoBehaviour
{

    #region Private Variables

    [SerializeField] private float _maxSize = 8f;
    [SerializeField] private float _timeToMaxSize = 1f;
    [SerializeField] private float _timeToDestroy = 3f;

    private bool _isBlinking = false;
    private int _maxBlinkNb = 4;
    
    #endregion
    
    #region Unity API
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DOTween.Init(recycleAllByDefault: true);
        IncreaseSize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        Invoke(nameof(Deactivate), _timeToDestroy);
        DOTween.Init(recycleAllByDefault: true);
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
        transform.DOScale(_maxSize, _timeToMaxSize).SetEase(Ease.OutBack);
    }
    
    #endregion

    #region Utils

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
