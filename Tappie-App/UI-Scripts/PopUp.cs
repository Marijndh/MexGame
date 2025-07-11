using Godot;
using System;

public partial class PopUp : Node
{
	private bool overrides = false;
	public bool Overrides
	{
		get => overrides;
		set
		{
			if (overrides == value) return;
			overrides = value;
		}
	}

	private bool closesOnClick;

	public override void _Ready()
	{
		Button closeButton = GetNodeOrNull<Button>("CloseButton");
		if (closeButton != null)
		{
			closeButton.Pressed += OnCloseButtonPressed;
		}
		else closesOnClick = true; // If no close button, default to closing on click
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventScreenTouch touch)
		{
			if (closesOnClick)
			{
				EventManager.Instance.EmitSignal(nameof(EventManager.Instance.PopupClosed));
				QueueFree();
			}
		}
		else if (@event is InputEventMouseButton mouseBtn && mouseBtn.ButtonIndex == MouseButton.Left)
		{
			if (closesOnClick)
			{
				EventManager.Instance.EmitSignal(nameof(EventManager.Instance.PopupClosed));
				QueueFree();
			}
		}
	}

	private void OnCloseButtonPressed()
	{
		EventManager.Instance.EmitSignal(nameof(EventManager.Instance.PopupClosed));
		QueueFree();
	}
}
