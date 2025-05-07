using System;
using Input;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IPlayerController
{
    public Vector2 MousePosition { get; }
    public Vector2 MouseClickPosition { get; }
    public void SubscribeToAttackEvent(Action attackAction);
    public void SubscribeToParryEvent(Action parryAction);
    public void UnsubscribeFromAttackEvent(Action attackAction);
    public void UnsubscribeFromParryEvent(Action parryAction);
}

public class PlayerController : MonoBehaviour,  IPlayerController, GameInputSystem.IPlayerActions
{
    #region Private variables

    private Vector2 _mousePosition;
    private Vector2 _mouseClickPosition;
    private Action _onAttackEvent;
    private Action _onParryEvent;
    private GameInputSystem _gameInputSystem;

    #endregion
    
    #region Public variables
    
    public Vector2 MousePosition => _mousePosition;
    public Vector2 MouseClickPosition => _mouseClickPosition;

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
        if (context.performed)
        {
            _mouseClickPosition = _gameInputSystem.Player.Look.ReadValue<Vector2>();
            _onAttackEvent?.Invoke();
        }
    }

    public void OnParry(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _onParryEvent?.Invoke();
        }
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

    public void SubscribeToParryEvent(Action parryAction)
    {
        _onParryEvent += parryAction;
    }

    public void UnsubscribeFromAttackEvent(Action attackAction)
    {
        _onAttackEvent -= attackAction;
    }

    public void UnsubscribeFromParryEvent(Action parryAction)
    {
        _onParryEvent -= parryAction;
    }

    #endregion
}
