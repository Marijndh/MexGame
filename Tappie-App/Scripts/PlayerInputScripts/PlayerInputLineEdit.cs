using Godot;
using System;

public partial class PlayerInputLineEdit : LineEdit
{
private string _placeholderText;

    public override void _Ready()
    {
        // Bewaar de originele placeholder text
        _placeholderText = PlaceholderText;
    }

	public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey && eventKey.Pressed && eventKey.Keycode == Key.Enter)
        {
            ReleaseFocus();
        }
    }

    // Functie om placeholder te verwijderen wanneer het veld wordt aangeklikt
    private void OnFocusEntered()
    {
        if (Text == "")
        {
            PlaceholderText = "";
        }
    }

    // Functie om placeholder terug te zetten als het veld leeg is wanneer het de focus verliest
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
            // Controleer of de klik buiten de LineEdit was
            Vector2 mousePos = mouseEvent.GlobalPosition;
            Rect2 globalRect = GetGlobalRect();

            if (!globalRect.HasPoint(mousePos))
            {
                ReleaseFocus();
            }
        }
    }
}
