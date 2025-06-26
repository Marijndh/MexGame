using Godot;
using Godot.Collections;


public partial class EventManager : Node
{
	[Signal]
	public delegate void PopupClosedEventHandler();

	[Signal] 
	public delegate void PopupOpenedEventHandler();

	[Signal]
	public delegate void RollFinishedEventHandler();

	[Signal]
	public delegate void NewKnightEventHandler();

	[Signal]
	public delegate void PenaltyEventHandler(int penalty, Array<string> names, bool give, bool knight);

	[Signal]
	public delegate void RoundFinishedEventHandler();
}
