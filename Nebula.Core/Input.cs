using Nebula.Graphics;

namespace Nebula.Core;

public static unsafe class Input
{
    private static HashSet<int> _mousePressed = new HashSet<int>();
    private static HashSet<int> _mouseHeld = new HashSet<int>();
    private static HashSet<int> _mouseReleased = new HashSet<int>();

    private static HashSet<int> _pressed = new HashSet<int>();
    private static HashSet<int> _held = new HashSet<int>();
    private static HashSet<int> _released = new HashSet<int>();
    
    private static (double, double) _mouseDelta = (0.0, 0.0);

    private static void GlfwCursorPosCallback(void* window, double dx, double dy)
    {
        _mouseDelta.Item1 += dx;
        _mouseDelta.Item2 += dy;
    }

    private static void GlfwMouseButtonCallback(void* window, int button, int action, int mods)
    {
        switch (action)
        {
            case Glfw.PRESS:
                _mousePressed.Add(button);
                _mouseHeld.Add(button);
                break;
            case Glfw.RELEASE:
                _mouseReleased.Add(button);
                _mouseHeld.Remove(button);
                break;
        }
    }
    
    private static void GlfwKeyCallback(void* window, int key, int scancode, int action, int mods)
    {
        switch (action)
        {
            case Glfw.PRESS:
                _pressed.Add(key);
                _held.Add(key);
                break;
            case Glfw.RELEASE:
                _released.Add(key);
                _held.Remove(key);
                break;
        }
    }

    private static void GlfwWindowFocusCallback(void* window, int focused)
    {
        _ResetState();
        if (focused == Glfw.TRUE) LockCursor(window);
    }

    public static void LockCursor(void* window)
    {
        Glfw.SetInputMode(window, Glfw.CURSOR, Glfw.CURSOR_DISABLED);
        if (Glfw.RawMouseMotionSupported() == Glfw.TRUE)
        {
            Glfw.SetInputMode(window, Glfw.RAW_MOUSE_MOTION, Glfw.TRUE);
        }
    }

    public static void UnlockCursor(void* window)
    {
        Glfw.SetInputMode(window, Glfw.CURSOR, Glfw.CURSOR_NORMAL);
    }
    
    public static void Setup(void* window)
    {
        Glfw.SetCursorPosCallback(window, GlfwCursorPosCallback);
        Glfw.SetMouseButtonCallback(window, GlfwMouseButtonCallback);
        
        Glfw.SetKeyCallback(window, GlfwKeyCallback);

        Glfw.SetWindowFocusCallback(window, GlfwWindowFocusCallback);
    }

    public static InputState GetState()
    {
        var state = new InputState(_pressed, _held, _released, _mousePressed, _mouseHeld, _mouseReleased, _mouseDelta);
        _PrepareNextFrame();
        return state;
    }

    private static void _PrepareNextFrame()
    {
        _pressed = new HashSet<int>();
        _released = new HashSet<int>();
        _mousePressed = new HashSet<int>();
        _mouseReleased = new HashSet<int>();
        _mouseDelta = (0.0, 0.0);
    }
    
    private static void _ResetState()
    {
        _PrepareNextFrame();
        _held = new HashSet<int>();
        _mouseHeld = new HashSet<int>();
    }
}

public class InputState
{
    private readonly HashSet<int> _pressed;
    private readonly HashSet<int> _held;
    private readonly HashSet<int> _released;

    private readonly HashSet<int> _mousePressed;
    private readonly HashSet<int> _mouseHeld;
    private readonly HashSet<int> _mouseReleased;

    private readonly (double, double) _mouseDelta;

    public InputState(HashSet<int> pressed, HashSet<int> held, HashSet<int> released, HashSet<int> mousePressed, HashSet<int> mouseHeld, HashSet<int> mouseReleased, (double, double) mouseDelta)
    {
        _pressed = pressed;
        _held = held;
        _released = released;
        _mousePressed = mousePressed;
        _mouseHeld = mouseHeld;
        _mouseReleased = mouseReleased;
        _mouseDelta = mouseDelta;
    }

    public bool IsKeyPressed(int key) => _pressed.Contains(key);
    public bool IsKeyHeld(int key) => _held.Contains(key);
    public bool IsKeyReleased(int key) => _released.Contains(key);
    
    public bool IsMouseButtonPressed(int button) => _mousePressed.Contains(button);
    public bool IsMouseButtonHeld(int button) => _mouseHeld.Contains(button);
    public bool IsMouseButtonReleased(int button) => _mouseReleased.Contains(button);

    public (double x, double y) MouseDelta => _mouseDelta;
}