using Godot;
using System;

public partial class PopUp : Node
{
	private void OnCloseButtonPressed()
	{
		EventManager.Instance.EmitSignal(nameof(EventManager.Instance.PopupClosed));
		QueueFree();
	}

}
