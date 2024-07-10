using Godot;
using System;

public partial class Signals : Node
{
	[Signal]
	public delegate void HideStartUIEventHandler();

	[Signal]
	public delegate void AddPLayerInputEventHandler();

	[Signal]
	public delegate void DeletePlayerInputEventHandler(Node node);
}
