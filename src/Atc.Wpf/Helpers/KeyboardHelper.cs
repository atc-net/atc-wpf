namespace Atc.Wpf.Helpers;

public static class KeyboardHelper
{
    public static bool IsKeyDownAlt()
        => Keyboard.IsKeyDown(Key.LeftAlt) ||
           Keyboard.IsKeyDown(Key.RightAlt);

    public static bool IsKeyDownCtrl()
        => Keyboard.IsKeyDown(Key.LeftCtrl) ||
           Keyboard.IsKeyDown(Key.RightCtrl);

    public static bool IsKeyDownShift()
        => Keyboard.IsKeyDown(Key.LeftShift) ||
           Keyboard.IsKeyDown(Key.RightShift);

    public static bool IsKeyDownCtrlOrShift()
        => IsKeyDownCtrl() ||
           IsKeyDownShift();

    public static bool IsKeyDownCtrlAndShift()
        => IsKeyDownCtrl() &&
           IsKeyDownShift();

    public static bool IsKeyDownAltAndShiftAndCtrl()
        => IsKeyDownAlt() &&
           IsKeyDownShift() &&
           IsKeyDownCtrl();

    public static bool IsKeyDownArrow()
        => Keyboard.IsKeyDown(Key.Left) ||
           Keyboard.IsKeyDown(Key.Up) ||
           Keyboard.IsKeyDown(Key.Right) ||
           Keyboard.IsKeyDown(Key.Down);

    public static bool IsKeyDownRegularNumber()
        => Keyboard.IsKeyDown(Key.D0) ||
           Keyboard.IsKeyDown(Key.D1) ||
           Keyboard.IsKeyDown(Key.D2) ||
           Keyboard.IsKeyDown(Key.D3) ||
           Keyboard.IsKeyDown(Key.D4) ||
           Keyboard.IsKeyDown(Key.D5) ||
           Keyboard.IsKeyDown(Key.D6) ||
           Keyboard.IsKeyDown(Key.D7) ||
           Keyboard.IsKeyDown(Key.D8) ||
           Keyboard.IsKeyDown(Key.D9);

    public static bool IsKeyDownNumPadNumber()
        => Keyboard.IsKeyDown(Key.NumPad0) ||
           Keyboard.IsKeyDown(Key.NumPad1) ||
           Keyboard.IsKeyDown(Key.NumPad2) ||
           Keyboard.IsKeyDown(Key.NumPad3) ||
           Keyboard.IsKeyDown(Key.NumPad4) ||
           Keyboard.IsKeyDown(Key.NumPad5) ||
           Keyboard.IsKeyDown(Key.NumPad6) ||
           Keyboard.IsKeyDown(Key.NumPad7) ||
           Keyboard.IsKeyDown(Key.NumPad8) ||
           Keyboard.IsKeyDown(Key.NumPad9);

    public static bool IsKeyDownNumber()
        => IsKeyDownRegularNumber() ||
           IsKeyDownNumPadNumber();

    public static bool IsKeyRegularNumber(Key key)
        => key is
            Key.D0 or
            Key.D1 or
            Key.D2 or
            Key.D3 or
            Key.D4 or
            Key.D5 or
            Key.D6 or
            Key.D7 or
            Key.D8 or
            Key.D9;

    public static bool IsKeyNumPadNumber(Key key)
        => key is
            Key.NumPad0 or
            Key.NumPad1 or
            Key.NumPad2 or
            Key.NumPad3 or
            Key.NumPad4 or
            Key.NumPad5 or
            Key.NumPad6 or
            Key.NumPad7 or
            Key.NumPad8 or
            Key.NumPad9;

    public static bool IsKeyNumber(Key key)
        => IsKeyRegularNumber(key) ||
           IsKeyNumPadNumber(key);

    public static bool IsKeyArrow(Key key)
        => key is
            Key.Left or
            Key.Up or
            Key.Right or
            Key.Down;

    public static ArrowDirectionType GetArrowDirectionTypeFromKeyDown()
    {
        var direction = ArrowDirectionType.None;
        if (Keyboard.IsKeyDown(Key.Left))
        {
            direction = ArrowDirectionType.Left;
        }
        else if (Keyboard.IsKeyDown(Key.Up))
        {
            direction = ArrowDirectionType.Up;
        }
        else if (Keyboard.IsKeyDown(Key.Right))
        {
            direction = ArrowDirectionType.Right;
        }
        else if (Keyboard.IsKeyDown(Key.Down))
        {
            direction = ArrowDirectionType.Down;
        }

        return direction;
    }
}