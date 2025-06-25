using Godot;
using System;

public partial class PopUp : Node
{
	private EventManager _eventManager;
	public override void _Ready()
	{
		_eventManager = GetNode<EventManager>("/root/EventManager");
	}
	private void OnCloseButtonPressed()
	{
		_eventManager.EmitSignal(nameof(_eventManager.PopupClosed));
		QueueFree();
	}

}
