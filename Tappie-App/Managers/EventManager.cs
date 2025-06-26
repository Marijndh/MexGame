using Godot;
using Godot.Collections;


public partial class EventManager : Node
{
	[Signal]
	public delegate void PopupClosedEventHandler();

	[Signal]
	public delegate void PopupRequestedEventHandler(Node popupNode);

	[Signal]
	public delegate void RollFinishedEventHandler();

	[Signal]
	public delegate void RoundFinishedEventHandler();
}
