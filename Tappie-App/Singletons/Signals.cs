using Godot;
using System;
using System.Collections.Generic;

public partial class Signals : Node
{
	[Signal]
	public delegate void HideStartUIEventHandler();

	[Signal]
	public delegate void AddPLayerInputEventHandler();

	[Signal]
	public delegate void DeletePlayerInputEventHandler(Node node);

	[Signal]
	public delegate void SelectFirstEmptyEventHandler();

	[Signal]

	public delegate void RollFinishedEventHandler();

	[Signal]
	public delegate void PenaltyEventHandler(int penalty, string name, bool give, bool knight);

	[Signal]
	public delegate void DetermineLoserEventHandler(int penalty);

	[Signal]
	public delegate void NewKnightEventHandler(string name);

}
