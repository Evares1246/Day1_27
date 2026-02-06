using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace GameDesign.Gameplay
{
    public class InputReader : MonoBehaviour, PlayerControls.IGameplayActions
    {
        public Vector2 LookDelta { get; private set; }

        public event Action onToggleView;
        public event Action onClick;
        public event Action onUnlockCursor;

        private PlayerControls _controls;

        public void EnableInput()
        {
            if (_controls == null)
            {
                _controls = new PlayerControls();
                _controls.Gameplay.SetCallbacks(this);
            }
            _controls.Gameplay.Enable();
        }

        private void OnDisable()
        {
            _controls?.Gameplay.Disable();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookDelta = context.ReadValue<Vector2>();
        }

        public void OnToggleView(InputAction.CallbackContext context)
        {
            if (context.performed) onToggleView?.Invoke();
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (context.performed) onClick?.Invoke();
        }

        public void OnUnlockCursor(InputAction.CallbackContext context)
        {
            if (context.performed) onUnlockCursor?.Invoke();
        }
    }
}