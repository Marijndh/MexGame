using Godot;
using System;

public partial class CustomLineEdit : LineEdit
{
private string _placeholderText;

    public override void _Ready()
    {
        _placeholderText = PlaceholderText;
    }

	public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey && eventKey.Pressed && eventKey.Keycode == Key.Enter)
        {
            ReleaseFocus();
        }
    }

    private void OnFocusEntered()
    {
        if (Text == "")
        {
            PlaceholderText = "";
        }
    }

    private void OnFocusExited()
    {
        if (Text == "")
        {
            PlaceholderText = _placeholderText;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            Vector2 mousePos = mouseEvent.GlobalPosition;
            Rect2 globalRect = GetGlobalRect();

            if (!globalRect.HasPoint(mousePos))
            {
                ReleaseFocus();
            }
        }
    }
}
