using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
public partial class SelectPlayers : CanvasLayer
{
	private SceneManager _sceneSwitcher;
	private NameInputContainer _container;
	private PopupManager _popupManager;

	public override void _Ready()
	{
		_sceneSwitcher = GetNode<SceneManager>("/root/SceneManager");
		_container = GetNode<NameInputContainer>("PlayerInputs");

		EventManager.Instance.PopupClosed += _container.SelectFirstEmpty;

		_popupManager = new PopupManager(this);
	}

	public override void _ExitTree()
	{
		EventManager.Instance.PopupClosed -= _container.SelectFirstEmpty;
	}

	private void OnContinuePressed()
	{
		(List<string> players, int amountEmpty) = _container.GetNames();

		

		if (players.Count + amountEmpty < 2)
		{
			_popupManager.AddPopUp("PopUp","In je eentje spelen \n word lastig denk ik");
			return;
		}

		if (amountEmpty > 0)
		{
			_popupManager.AddPopUp("PopUp","Zal je niet iedereen ff \n een naam geven?");
		}
		else
		{
			GetNode<GameManager>("/root/GameManager").SetPlayers(players);
			_sceneSwitcher.SwitchScene("Play");
		}
	}

}
