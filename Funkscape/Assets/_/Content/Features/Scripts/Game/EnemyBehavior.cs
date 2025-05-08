using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// TODO: Add enemy attack
// TODO: Add state machine for Movement translation and Attack
// TODO: Change enemy color when attacking
// 
public class EnemyBehavior : MonoBehaviour
{
    #region Public Variables

    public UnityEvent m_onEnemyDestoyed;
    
    #endregion
    
    #region Private Variables
    
    //private Transform _playerTransform;
    private Rigidbody2D _rigidbody;
    private Sequence _tweenSequence;
    private Player _player;
    private GameManager _gameManager;
    private int _thrust = 5;
    private bool _movedRight = false;
    private bool _movedUp = false;
    private bool _attacking = false;
    private int _idleCycleNb = 0;
    private int _spawnIndex = 0;

    [SerializeField] private Vector2 _idleMoveDistance = new Vector2(0.6f, 0.3f);
    [SerializeField] private float _idleMoveTime = 0.8f;
    [SerializeField] private Transform _weaponPoint;
    [SerializeField] private SpawnPool _projectilePool;
    [SerializeField] private RoundSystemSO _roundSystemSO;
    
    #endregion

    #region Unity API

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DOTween.Init(recycleAllByDefault: true);
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _projectilePool = FindObjectsByType<SpawnPool>(sortMode: FindObjectsSortMode.None).FirstOrDefault(x => x.CompareTag("EnemyPool"));
        _player = FindFirstObjectByType<Player>();
        _gameManager = FindFirstObjectByType<GameManager>();
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
                    //.SetEase(Ease.InCirc)
                    .OnComplete(() =>
                    {
                        _movedRight = false;
                        if (!_attacking) Attack();
                    });
                break;
            case false:
                yield return transform.DOMoveX(transform.localPosition.x + _idleMoveDistance.x, _idleMoveTime)
                    //.SetEase(Ease.InCirc)
                    .OnComplete(() => _movedRight = true);
                break;
        }
    }

    private void Attack()
    {
        GameObject projectile = _projectilePool.GetFirstAvailableProjectile();
        //projectile.GetComponent<EnemyProjectile>().m_onBlink.AddListener();
        projectile.transform.position = _weaponPoint.position;
        //projectile.transform.rotation = _weaponPoint.rotation;
        //laser.transform.rotation = transform.rotation;
        projectile.SetActive(true);
        _attacking = true;
        
        //Debug.Log($"Shot {laser.name}");
    }

    public void SetSpawnIndex(int spawnIndex)
    {
        _spawnIndex = spawnIndex;
    }

    private void Randomizer()
    {
        
    }

    public void Deactivate()
    {
        _gameManager.KillEnemy(this);
        m_onEnemyDestoyed.Invoke();
        
        m_onEnemyDestoyed.RemoveAllListeners();
        gameObject.SetActive(false);
        
        Destroy(gameObject);
    }

    #endregion
}
