using UnityEngine;

public class LaserBehavior : MonoBehaviour
{
    #region Private Variables
    
    [SerializeField] private int _thrust = 15;
    [SerializeField] private float _lifeTime = 3;
    
    #endregion

    #region Unity API

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void OnEnable()
    {
        Invoke(nameof(Deactivate), _lifeTime);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Deactivate));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log($"Laser hit: {collision.gameObject.name}");
            Deactivate();
        }
    }

    #endregion

    #region Main Methods

    private void Move()
    {
        transform.Translate(Vector3.up * (Time.deltaTime * _thrust));
    }
    
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
