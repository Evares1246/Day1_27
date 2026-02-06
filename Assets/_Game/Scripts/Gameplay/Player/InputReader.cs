using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace GameDesign.Gameplay
{
    public class InputReader : MonoBehaviour, PlayerControls.IGameplayActions
    {
        public Vector2 LookDelta { get; private set; }

        // 修正：使用小写 on 开头以区分方法名
        public event Action onToggleView;
        public event Action onClick;

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

        private void OnDisable() => _controls?.Gameplay.Disable();

        public void OnLook(InputAction.CallbackContext context) => LookDelta = context.ReadValue<Vector2>();

        // 这里是接口实现的方法，大写开头
        public void OnToggleView(InputAction.CallbackContext context)
        {
            if (context.performed) onToggleView?.Invoke();
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (context.performed) onClick?.Invoke();
        }
    }
}