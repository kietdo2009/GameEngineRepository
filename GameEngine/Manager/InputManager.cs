using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI311.GameEngine
{
    public static class InputManager
    {
        static KeyboardState PreviousKeyboardState { get; set; }
        static KeyboardState CurrentKeyboardState { get; set; }
        static MouseState PreviousMouseState { get; set; }
        static MouseState CurrentMouseState { get; set; }

        public static void Initialize()
        {
            PreviousKeyboardState = CurrentKeyboardState =
            Keyboard.GetState();
            PreviousMouseState = CurrentMouseState =
            Mouse.GetState();
        }
        public static void Update()
        {
            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }
        public static bool IsKeyDown(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key);
        }
        public static bool IsKeyUp(Keys key)
        {
            return CurrentKeyboardState.IsKeyUp(key);
        }
        public static bool IsKeyPressed(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key) &&
            PreviousKeyboardState.IsKeyUp(key);
        }

        public static bool IsKeyReleased(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key) &&
            PreviousKeyboardState.IsKeyUp(key);
        }
        public static Vector2 GetMousePosition()
        {
            return new Vector2(CurrentMouseState.X, CurrentMouseState.Y);
        }
        public static bool IsMouseDown(int button)
        {
            switch (button)
            {
                case 0:
                    return CurrentMouseState.LeftButton ==
                    ButtonState.Pressed;
                case 1:
                    return CurrentMouseState.RightButton ==
                    ButtonState.Pressed;
                case 2:
                    return CurrentMouseState.MiddleButton ==
                    ButtonState.Pressed;
                default:
                    return false;
            }
        }
        public static bool IsMouseUp(int button)
        {
            switch (button)
            {
                case 0:
                    return CurrentMouseState.LeftButton ==
                    ButtonState.Released;
                case 1:
                    return CurrentMouseState.RightButton ==
                    ButtonState.Released;
                case 2:
                    return CurrentMouseState.MiddleButton ==
                    ButtonState.Released;
                default:
                    return false;
            }
        }
        // Fix for CS0161: Ensure all code paths return a value.
        // Fix for IDE0060: Remove unused parameter 'key' since it is not used and not part of a shipped public API.
        public static bool IsMouseReleased(int button)
        {
            switch (button)
            {
                case 0:
                    return CurrentMouseState.LeftButton == ButtonState.Released &&
                           PreviousMouseState.LeftButton == ButtonState.Pressed;
                case 1:
                    return CurrentMouseState.RightButton == ButtonState.Released &&
                           PreviousMouseState.RightButton == ButtonState.Pressed;
                case 2:
                    return CurrentMouseState.MiddleButton == ButtonState.Released &&
                           PreviousMouseState.MiddleButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }
        public static bool IsMousePressed(int button)
        {
            switch (button)
            {
                case 0:
                    return CurrentMouseState.LeftButton == ButtonState.Pressed &&
                           PreviousMouseState.LeftButton == ButtonState.Released;
                case 1:
                    return CurrentMouseState.RightButton == ButtonState.Pressed &&
                           PreviousMouseState.RightButton == ButtonState.Released;
                case 2:
                    return CurrentMouseState.MiddleButton == ButtonState.Pressed &&
                           PreviousMouseState.MiddleButton == ButtonState.Released;
                default:
                    return false;
            }
        }

    }
}

