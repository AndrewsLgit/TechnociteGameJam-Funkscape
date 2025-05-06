using UnityEngine;

//TODO: Player IS NOT the character that dances, they are 2 different things
// player will only shoot and take damage
// the character on the ship will point out spawn points
public class Player : MonoBehaviour
{
    
    #region Private Variables
    
    [SerializeField] private Transform _weaponPoint;
    [SerializeField] private SpawnPool _projectilePool;

    
    private IPlayerController _playerController;
    private Camera _camera;

    #endregion
    
    #region Unity API
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerController = GetComponent<IPlayerController>();
        
        _camera = Camera.main;
        
        _playerController.SubscribeToAttackEvent(LaserAttack);
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
        GameObject laser = _projectilePool.GetFirstAvailableProjectile();
        laser.transform.position = _weaponPoint.position;
        laser.transform.rotation = _weaponPoint.rotation;
        //laser.transform.rotation = transform.rotation;
        laser.SetActive(true);
        //Debug.Log($"Shot {laser.name}");
    }

    public void OnDeath()
    {
        gameObject.SetActive(false);
    }
    
    #endregion
    
    #region Utils

    private Vector2 GetMousePosition2D() => _playerController.MousePosition;
    private Vector2 GetMouseWorldPoint() => _camera.ScreenToWorldPoint(GetMousePosition2D());

    #endregion
}
