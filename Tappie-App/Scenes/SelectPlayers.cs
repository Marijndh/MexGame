using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
public partial class SelectPlayers : CanvasLayer
{
	private SceneManager _sceneSwitcher;
	private NameInputContainer _container;

	public override void _Ready()
	{
		_sceneSwitcher = GetNode<SceneManager>("/root/SceneManager");
		_container = GetNode<NameInputContainer>("PlayerInputs");

		EventManager.Instance.PopupClosed += _container.SelectFirstEmpty;
	}

	public override void _ExitTree()
	{
		// Clean up event subscriptions
		if (EventManager.Instance != null)
			EventManager.Instance.PopupClosed -= _container.SelectFirstEmpty;
	}

	private void OnContinuePressed()
	{
		(List<string> players, int amountEmpty) = _container.GetNames();

		// Helper function to show a popup and select the first empty slot after popup is closed
		void ShowPopUp(string message)
		{
			Godot.Collections.Dictionary<string, Variant> popUpProperties = new()
			{
				{ "Text", message }
			};
			Node popUp = NodeCreator.CreateNode("PopUp", popUpProperties);
			AddChild(popUp);
		}

		if (players.Count + amountEmpty < 2)
		{
			ShowPopUp("In je eentje spelen \n word lastig denk ik");
			return;
		}

		if (amountEmpty > 0)
		{
			ShowPopUp("Zal je niet iedereen ff \n een naam geven?");
		}
		else
		{
			GetNode<GameManager>("/root/GameManager").SetPlayers(players);
			_sceneSwitcher.SwitchScene("Play");
		}
	}

}
