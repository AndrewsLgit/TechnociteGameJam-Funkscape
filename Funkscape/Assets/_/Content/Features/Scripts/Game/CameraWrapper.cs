using UnityEngine;

public class CameraWrapper : MonoBehaviour
{
    
    #region Private Variables
    
    private Vector3 _lastPosition;
    private Camera _camera;
    private Vector3 _bottomLeftBoundary;
    private Vector3 _topRightBoundary;

    private float _gameplayZoneWidth;
    private float _gameplayZoneHeight;
    [SerializeField] private float _offset = 1;
    [SerializeField] private bool isMovingRight;
    [SerializeField] private bool isMovingUp;
    
    #endregion

    #region Unity API

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = Camera.main;
        _bottomLeftBoundary = _camera.ViewportToWorldPoint(new Vector3(0, 0, -_camera.transform.position.z));
        _topRightBoundary = _camera.ViewportToWorldPoint(new Vector3(1, 1, -_camera.transform.position.z));
        _gameplayZoneWidth = _topRightBoundary.x - _bottomLeftBoundary.x;
        _gameplayZoneHeight = _topRightBoundary.y - _bottomLeftBoundary.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyWrap();
    }
    
    #endregion

    #region Main Methods
    
    private void ApplyWrap()
    {
        var currentPosition = transform.position;
        isMovingRight = currentPosition.x - _lastPosition.x > 0;
        isMovingUp = currentPosition.y - _lastPosition.y > 0;
        
        if (isMovingRight && currentPosition.x > _topRightBoundary.x-_offset) TeleportLeft();
        else if (!isMovingRight && currentPosition.x < _bottomLeftBoundary.x+_offset) TeleportRight();
        else if (isMovingUp && currentPosition.y > _topRightBoundary.y-_offset) TeleportDown();
        else if (!isMovingUp && currentPosition.y < _bottomLeftBoundary.y+_offset) TeleportUp();

        _lastPosition = currentPosition;
    }
    
    #endregion

    #region Utils

    private void TeleportUp()
    {
        transform.position = new Vector3(transform.position.x, _gameplayZoneHeight*.5f+_offset, transform.position.z);
        
    }

    private void TeleportDown()
    {
        transform.position = new Vector3(transform.position.x, -(_gameplayZoneHeight*.5f+_offset), transform.position.z);
    }

    private void TeleportRight()
    {
        transform.position = new Vector3(_gameplayZoneWidth*.5f+_offset, transform.position.y, transform.position.z);
    }

    private void TeleportLeft()
    {
        transform.position = new Vector3(-(_gameplayZoneWidth*.5f+_offset), transform.position.y, transform.position.z);
    }
    
    #endregion  
}
