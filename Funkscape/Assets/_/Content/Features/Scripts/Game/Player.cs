using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

//TODO: Player IS NOT the character that dances, they are 2 different things
// player will only shoot and take damage
// the character on the ship will point out spawn points in sequence before enemies spawn

// todo: add player Iframe when damage taken
public class Player : MonoBehaviour
{
    
    #region Private Variables

    [SerializeField] private GameObject _crosshair;
    [SerializeField] private GameObject _crosshairFlash;
    [SerializeField] private Transform _weaponPoint;
    [SerializeField] private SpawnPool _projectilePool;
    
    [SerializeField] private GameObject _windowHp3;
    [SerializeField] private GameObject _windowHp2;
    [SerializeField] private GameObject _windowHp1;

    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private int _maxBlinks = 4;
    [SerializeField] private float _blinkInterval = 1f;
    
    private IPlayerController _playerController;
    private Camera _camera;
    private Repeater _repeater;
    private GameManager _gameManager;
    private int _blinkNb;
    private int _currentPlayerHealth;
    private Vector2 _ray2D;
    private bool _hasParried;

    #endregion
    
    #region Unity API
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();

        _playerController = GetComponent<IPlayerController>();
        _playerController.SubscribeToAttackEvent(Shoot);
        _playerController.SubscribeToEscapeEvent(_gameManager.PauseGame);
        _playerController.SubscribeToParryEvent(Parry);
        _windowHp3.SetActive(true);
        _windowHp2.SetActive(false);
        _windowHp1.SetActive(false);

        _currentPlayerHealth = _maxHealth;
        _blinkNb = 0;
        _camera = Camera.main;
        
        _repeater = GetComponent<Repeater>();
        _repeater.m_loopForever = false;
        _repeater.m_repeatCount = _maxBlinks;
    }

    // Update is called once per frame
    void Update()
    {
        LookAtPosition(GetMouseWorldPoint());
    }

    private void OnDisable()
    {
        _playerController.UnsubscribeFromAttackEvent(Shoot);
        _playerController.UnsubscribeFromEscapeEvent(_gameManager.PauseGame);
        _playerController.UnsubscribeFromParryEvent(Parry);
        Debug.Log($"Player death | Game object active: {gameObject.activeSelf}");
    }

    #endregion
    
    #region Main Methods

    private void LookAtPosition(Vector2 targetPos)
    {
        _crosshair.transform.position = targetPos;
        _crosshairFlash.transform.position = targetPos;
        //Vector2 direction = targetPos - (Vector2)transform.position;
        //var playerRotation = Quaternion.LookRotation(Vector3.forward, direction);
        //transform.rotation = playerRotation;
        //_weaponPoint.rotation = playerRotation;
    }

    private void LaserAttack()
    {
        var laser = _projectilePool.GetFirstAvailableProjectile();
        var laserBehaviour = laser.GetComponent<Laser>();
        var targetPos = GetMouseClickPosition();
        Vector2 direction = targetPos - (Vector2)transform.position;

        var laserRotation = Quaternion.LookRotation(Vector3.forward, direction);
        
        laser.transform.position = _weaponPoint.position;
        laser.transform.rotation = laserRotation;
        //laser.transform.rotation = transform.rotation;
        //laser.BehaviourComponent.MoveToClickLocation(GetMouseWorldPoint());
        laser.SetActive(true);
        //Debug.Log($"Shot {laser.name}");
    }

    private void Shoot()
    {
        var hit = Physics2D.Raycast(GetMouseClickPosition(), Vector2.zero);
        //Debug.Log($"Ray info: {_ray2D}");
        if (hit.collider == null) return;
        if (!hit.transform.CompareTag("Enemy")) return;
        
        hit.collider.gameObject.GetComponent<EnemyBehavior>().Deactivate();
        Debug.Log($"Enemy hit {hit.transform.name}");
    }

    private void Parry()
    {
        if (_blinkNb is <= 3 and > 0) _hasParried = true;
    }

    private void OnDeath()
    {
        _gameManager.GameOver();
        gameObject.SetActive(false);
    }

    private void OnHit()
    {
        switch (_hasParried)
        {
            case true:
                _hasParried = false;
                Debug.Log($"Player Parry!");
                return;
            case false:
                _currentPlayerHealth--;
                _blinkNb = 0;
                _repeater.StopRepeater();
                break;
        }


        switch (_currentPlayerHealth)
        {
            case 3:
                _windowHp3.SetActive(true);
                _windowHp2.SetActive(false);
                _windowHp1.SetActive(false);
                break;
            case 2:
                _windowHp2.SetActive(true);
                break;
            case 1:
                _windowHp1.SetActive(true);
                break;
            // case 0:
            //     OnDeath();
            //     break;
            default:
                //throw new ArgumentOutOfRangeException();
            OnDeath();
                break;
        }
        //_repeater.StopRepeater();
        Debug.Log($"Lost health! Current health: {_currentPlayerHealth}");
        if (_currentPlayerHealth <= 0) OnDeath();
    }

    private void OnBlink()
    {
        _crosshairFlash.SetActive(!_crosshairFlash.activeSelf);
        _crosshair.SetActive(!_crosshair.activeSelf);
        _blinkNb++;
        
        Debug.Log($"Blink Nb: {_blinkNb}");
        if (_blinkNb >= _maxBlinks)
        {
            _crosshairFlash.SetActive(false);
            _crosshair.SetActive(true);
            OnHit();
        }
    }

    private IEnumerator AlarmBlink(float time)
    {
        while (_blinkNb < _maxBlinks) {
            _blinkNb++;
            Debug.Log($"Alarm has blinked {_blinkNb} times");
            //if (_blinkNb >= _maxBlinks) OnHit();
        }
        yield return new WaitForSeconds(time);
        OnHit();
    }
    
    #endregion
    
    #region Utils

    public void StartRepeater(float blinkInterval)
    { 
        blinkInterval /= _maxBlinks;
        _repeater.m_repeatTime = blinkInterval;
        _repeater.m_repeatCount = _maxBlinks;
        
        _repeater.m_OnRepeat.AddListener(OnBlink);
        //_repeater.m_OnStartupEnd.AddListener(OnHit);
        _repeater.StartRepeater();
        Debug.Log($"Started blinker with {blinkInterval} seconds");
    }

    private Vector2 GetMousePosition2D() => _playerController.MousePosition;
    private Vector2 GetMouseWorldPoint() => _camera.ScreenToWorldPoint(GetMousePosition2D());
    
    private Vector2 GetMouseClickPosition() => _camera.ScreenToWorldPoint(_playerController.MouseClickPosition);

    #endregion
}