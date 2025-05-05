using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EnemyBehavior : MonoBehaviour
{
    #region Public Variables

    public UnityEvent m_onPlayerHit;
    public UnityEvent m_onEnemyDestoyed;
    
    #endregion
    
    #region Private Variables
    
    //private Transform _playerTransform;
    private Rigidbody2D _rigidbody;
    private int _thrust = 5;

    [SerializeField] private Vector2 _idleMoveDistance = new Vector2(3,3);
    [SerializeField] private float _idleMoveTime = 0.2f;
    [SerializeField] private Transform _weaponPoint;
    [SerializeField] private SpawnPool _projectilePool;
    
    #endregion

    #region Unity API

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _projectilePool = FindFirstObjectByType<SpawnPool>();
        //_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        // LookAtPosition(_playerTransform.transform.position);
        //Move();
        IdleMovement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Laser"))
        {
            Debug.Log($"Hit by laser: {collision.gameObject.name}");
            //Destroy(this.gameObject);
            Deactivate();
        }

        // if (collision.gameObject.CompareTag("Player"))
        // {
        //     Debug.Log($"Collision with player: {collision.gameObject.name}");
        //     m_onPlayerHit.Invoke();
        // }
    }

    private void OnEnable()
    {
        //LookAtPosition(_playerInput.GetPlayerPosition());
    }

    #endregion

    #region Main Methods

    private void IdleMovement()
    {
        StartCoroutine(MoveDown());
        StartCoroutine(MoveLeft());
        StartCoroutine(MoveUp());
        StartCoroutine(MoveRight());
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
    
    /*private void LookAtPosition(Vector2 targetPos)
    {
        Vector2 direction = (targetPos) - (Vector2)gameObject.transform.position;
        gameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }*/

    private void Deactivate()
    {
        m_onEnemyDestoyed.Invoke();
        m_onEnemyDestoyed.RemoveAllListeners();
        m_onPlayerHit.RemoveAllListeners();
        gameObject.SetActive(false);
    }

    #endregion
    
    #region Utils

    private IEnumerator MoveUp()
    {
        yield return new WaitForSeconds(_idleMoveTime);
        transform.Translate(gameObject.transform.up * (Time.deltaTime * _idleMoveDistance.y));
    }

    private IEnumerator MoveDown()
    {
        yield return new WaitForSeconds(_idleMoveTime);
        transform.Translate(-gameObject.transform.up * (Time.deltaTime * _idleMoveDistance.y));
    }

    private IEnumerator MoveLeft()
    {
        yield return new WaitForSeconds(_idleMoveTime);
        transform.Translate(-gameObject.transform.right * (Time.deltaTime * _idleMoveDistance.x));
    }

    private IEnumerator MoveRight()
    {
        yield return new WaitForSeconds(_idleMoveTime);
        transform.Translate(gameObject.transform.right * (Time.deltaTime * _idleMoveDistance.x));
    }
    #endregion
}
