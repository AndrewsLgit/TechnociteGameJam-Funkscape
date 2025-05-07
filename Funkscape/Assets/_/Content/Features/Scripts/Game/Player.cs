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
    
    [SerializeField] private Transform _weaponPoint;
    [SerializeField] private SpawnPool _projectilePool;
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private int _maxBlinks = 4;
    [SerializeField] private float _blinkInterval = 1f;
    
    private IPlayerController _playerController;
    private Camera _camera;
    private Repeater _repeater;
    private int _blinkNb;
    private int _currentPlayerHealth;

    #endregion
    
    #region Unity API
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerController = GetComponent<IPlayerController>();
        _playerController.SubscribeToAttackEvent(LaserAttack);

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
        _playerController.UnsubscribeFromAttackEvent(LaserAttack);
        Debug.Log($"Player death | Game object active: {gameObject.activeSelf}");
    }

    #endregion
    
    #region Main Methods

    private void LookAtPosition(Vector2 targetPos)
    {
        Vector2 direction = targetPos - (Vector2)transform.position;
        var playerRotation = Quaternion.LookRotation(Vector3.forward, direction);
        //transform.rotation = playerRotation;
        _weaponPoint.rotation = playerRotation;
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

    private void Parry()
    {
        
    }

    private void OnDeath()
    {
        gameObject.SetActive(false);
    }

    private void OnHit()
    {
        _currentPlayerHealth--;
        _blinkNb = 0;
        //_repeater.StopRepeater();
        Debug.Log($"Lost health! Current health: {_currentPlayerHealth}");
        if (_currentPlayerHealth <= 0) OnDeath();
    }

    private void OnBlink()
    {
        _blinkNb++;
        Debug.Log($"Blink Nb: {_blinkNb}");
        if (_blinkNb >= _maxBlinks) OnHit();
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
        _repeater.m_repeatCount = _maxBlinks-1;
        
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