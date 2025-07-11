using Godot;
using Godot.Collections;

public partial class EventManager : Node
{
    private static EventManager _instance;
    public static EventManager Instance => _instance;

    public override void _Ready()
    {
        // Ensure only one instance exists
        if (_instance != null && _instance != this)
        {
            QueueFree(); // Remove duplicate
            return;
        }
        _instance = this;
    }

    [Signal]
    public delegate void PopupClosedEventHandler();

    [Signal]
    public delegate void DieRolledEventHandler();

    [Signal]
    public delegate void DiceThrownEventHandler(int score, bool canReroll = true);

	[Signal]
    public delegate void GameStateChangedEventHandler(GameState newState, Dictionary gameStateContext);
}
