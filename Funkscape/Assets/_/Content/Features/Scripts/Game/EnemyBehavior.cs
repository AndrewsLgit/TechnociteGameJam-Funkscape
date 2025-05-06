using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// TODO: Add enemy attack
// TODO: Add state machine for Movement translation and Attack
public class EnemyBehavior : MonoBehaviour
{
    #region Public Variables

    public UnityEvent m_onEnemyDestoyed;
    
    #endregion
    
    #region Private Variables
    
    //private Transform _playerTransform;
    private Rigidbody2D _rigidbody;
    private Sequence _tweenSequence;
    private int _thrust = 5;
    private bool _movedRight = false;
    private bool _movedUp = false;
    private int _idleCycleNb = 0;

    [SerializeField] private Vector2 _idleMoveDistance = new Vector2(0.6f, 0.3f);
    [SerializeField] private float _idleMoveTime = 0.8f;
    [SerializeField] private Transform _weaponPoint;
    [SerializeField] private SpawnPool _projectilePool;
    
    #endregion

    #region Unity API

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DOTween.Init(recycleAllByDefault: true);
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _projectilePool = FindFirstObjectByType<SpawnPool>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(IdleMovement());
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Laser"))
        {
            Debug.Log($"Hit by laser: {collision.gameObject.name}");
            Deactivate();
        }
    }

    private void OnEnable()
    {
        //LookAtPosition(_playerInput.GetPlayerPosition());
    }

    private void OnDisable()
    {
        DOTween.Clear();
    }

    #endregion

    #region Main Methods

    private IEnumerator IdleMovement()
    {
        // _tweenSequence  = DOTween.Sequence()
        //     .SetLoops(-1, LoopType.Yoyo)
        //     .SetEase(Ease.Linear);
        
        // _tweenSequence.Append(
        //     transform.DOMoveX(transform.position.x + _idleMoveDistance.x, _idleMoveTime))
        //     .SetEase(Ease.Linear);
        // _tweenSequence.Append(
        //     transform.DOMoveY(transform.position.y + _idleMoveDistance.y, _idleMoveTime))
        //     .SetEase(Ease.Linear);
        // _tweenSequence.Append(
        //     transform.DOMove(newPos, _idleMoveTime));
        // _tweenSequence.Play();
        
        switch (_movedRight)
        {
            case true:
                yield return transform.DOMoveX(transform.localPosition.x - _idleMoveDistance.x, _idleMoveTime)
                    .OnComplete(() => _movedRight = false);
                break;
            case false:
                transform.DOMoveX(transform.localPosition.x + _idleMoveDistance.x, _idleMoveTime)
                    .OnComplete(() => _movedRight = true);
                break;
        }
    }

    private void Attack()
    {
        GameObject projectile = _projectilePool.GetFirstAvailableProjectile();
        projectile.transform.position = _weaponPoint.position;
        projectile.transform.rotation = _weaponPoint.rotation;
        //laser.transform.rotation = transform.rotation;
        projectile.SetActive(true);
        //Debug.Log($"Shot {laser.name}");
    }

    private void Deactivate()
    {
        m_onEnemyDestoyed.Invoke();
        m_onEnemyDestoyed.RemoveAllListeners();
        gameObject.SetActive(false);
    }

    #endregion
}
