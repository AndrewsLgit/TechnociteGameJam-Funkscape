using System;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IPlayerController
{
    public Vector2 MousePosition { get; }
    public void SubscribeToAttackEvent(Action attackAction);
    public void UnsubscribeFromAttackEvent(Action attackAction);
}

public class PlayerController : MonoBehaviour,  IPlayerController, GameInputSystem.IPlayerActions
{
    #region Private variables

    private Vector2 _mousePosition;
    private Action _onAttackEvent;
    private GameInputSystem _gameInputSystem;

    #endregion
    
    #region Public variables
    
    public Vector2 MousePosition => _mousePosition;

    #endregion
    
    #region Unity API
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameInputSystem = new GameInputSystem();
        _gameInputSystem.Enable();
        _gameInputSystem.Player.SetCallbacks(this);
    }

    // Update is called once per frame
    void Update()
    {
        _mousePosition = _gameInputSystem.Player.Look.ReadValue<Vector2>();
    }

    private void OnDisable()
    {
        _gameInputSystem.Disable();
    }
    
    #endregion
    
    #region Main Methods

    public void OnLook(InputAction.CallbackContext context)
    {
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed) _onAttackEvent?.Invoke();
    }

    public void OnMenu(InputAction.CallbackContext context)
    {
    }
    
    #endregion

    #region Utils

    public void SubscribeToAttackEvent(Action attackAction)
    {
        _onAttackEvent += attackAction;
    }

    public void UnsubscribeFromAttackEvent(Action attackAction)
    {
        _onAttackEvent -= attackAction;
    }

    #endregion
}
