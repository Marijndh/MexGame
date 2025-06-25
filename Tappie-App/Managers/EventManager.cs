using Godot;
using System;
using System.Collections.Generic;

public partial class EventManager : Node
{
	[Signal]
	public delegate void PopupClosedEventHandler();

	[Signal]

	public delegate void NewKnightEventHandler(string name);

	[Signal]

	public delegate void RollFinishedEventHandler();

	[Signal]
	public delegate void PenaltyEventHandler(int penalty, string name, bool give, bool knight);

	[Signal]
	public delegate void DetermineLoserEventHandler(int penalty);

}
