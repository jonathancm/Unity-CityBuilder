using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CityBuilder
{
    public class InputSystem : MonoBehaviour
    {
        public enum UserActionType
        {
            None,
            World,
            UI
        }

        public enum MouseButtons
        {
            LeftClick = 0,
            RightClick = 1,
            MiddleClick = 2
        }

        public enum InputButtonState
        {
            isHeldUp,
            wasJustPressed,
            isHeldDown,
            wasJustReleased
        }

        [System.Serializable]
        public struct MouseInput
        {
            public InputButtonState leftClick;
            public InputButtonState middleClick;
            public InputButtonState rightClick;
            public Vector2 currentScreenPosition;
            public Vector2 deltaScroll;
            public Vector2? dragStartPosition;
        }

        //
        // Properties
        //
        public UserActionType ActionType { get; private set; } = UserActionType.None;
        public MouseInput Mouse{ get { return _mouse; } }
        private MouseInput _mouse;

        //
        // Cached References
        //
        private CanvasMonitor canvasMonitor = null;

        private void Start()
        {
            canvasMonitor = FindObjectOfType<CanvasMonitor>();
        }

        public void UpdateInput()
        {
            //
            // General Input
            //
            _mouse.currentScreenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            _mouse.deltaScroll = IsMouseOverGameWindow() ? Input.mouseScrollDelta : Vector2.zero;
            _mouse.leftClick = GetMouseButtonState(MouseButtons.LeftClick);
            _mouse.rightClick = GetMouseButtonState(MouseButtons.RightClick);
            _mouse.middleClick = GetMouseButtonState(MouseButtons.MiddleClick);

            //
            // Drag detection
            //
            if (_mouse.rightClick == InputButtonState.wasJustPressed)
                _mouse.dragStartPosition = _mouse.currentScreenPosition;
            else if (_mouse.rightClick == InputButtonState.wasJustReleased)
                _mouse.dragStartPosition = null;

            //
            // Action Type
            //
            if (_mouse.leftClick == InputButtonState.wasJustPressed
                || _mouse.rightClick == InputButtonState.wasJustPressed
                || _mouse.middleClick == InputButtonState.wasJustPressed)
            {
                ActionType = canvasMonitor.isPointerOverGUI ? UserActionType.UI : UserActionType.World;
            }
            else if (_mouse.leftClick == InputButtonState.isHeldUp
                && _mouse.rightClick == InputButtonState.isHeldUp
                && _mouse.middleClick == InputButtonState.isHeldUp)
            {
                ActionType = UserActionType.None;
            }
        }

        private InputButtonState GetMouseButtonState(MouseButtons button)
        {
            if (Input.GetMouseButtonDown((int)button) && IsMouseOverGameWindow())
                return InputButtonState.wasJustPressed;
            else if (Input.GetMouseButtonUp((int)button))
                return InputButtonState.wasJustReleased;
            else if (Input.GetMouseButton((int)button) == true)
                return InputButtonState.isHeldDown;
            else
                return InputButtonState.isHeldUp;
        }

        public static bool IsMouseOverGameWindow()
        {
            return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y);
        }
    }
}
