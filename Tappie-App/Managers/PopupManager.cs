using Godot;
using Godot.Collections;
using System.Collections.Generic;

public class PopupManager
{
	private Queue<PopUp> popupQueue = new();
	private PopUp currentPopup;
	private Node parent;
	private GameStateHandler gameStateHandler;
	public bool PopupIsOpen => currentPopup != null;

	public PopupManager(Node parent, GameStateHandler gameStateHandler = null)
	{
		this.parent = parent;
		this.gameStateHandler = gameStateHandler;
		EventManager.Instance.PopupClosed += OnPopupClosed;
		EventManager.Instance.GameStateChanged += OnGameStateChanged;
	}

	public void Dispose()
	{
		EventManager.Instance.PopupClosed -= OnPopupClosed;
		EventManager.Instance.GameStateChanged -= OnGameStateChanged;
		if (currentPopup != null)
		{
			currentPopup.QueueFree();
			currentPopup = null;
		}
		popupQueue.Clear();
	}

	public void AddPopUp(string popUp, string text = "", bool overrides = true)
	{
		Node node;
		Godot.Collections.Dictionary<string, Variant> dict = new();

		if (text != "")
		{
			dict.Add("Text", text);
		}
		node = NodeCreator.CreateNode(popUp, dict);

		if (node is PopUp popUpNode)
		{
			popUpNode.Overrides = overrides;
			EnqueuePopup(popUpNode);
		}
		else
		{
			GD.PrintErr($"Node {popUp} is not a PopUp type.");
		}

	}

	private void EnqueuePopup(PopUp popup)
	{
		popupQueue.Enqueue(popup);
		if (!PopupIsOpen) ShowNext();
	}

	public void OnPopupClosed()
	{
		currentPopup = null;
		ShowNext();
	}

	private void ShowNext()
	{
		if (popupQueue.Count == 0) return;
		currentPopup = popupQueue.Dequeue();
		parent.AddChild(currentPopup);
	}

	private void OnGameStateChanged(GameState state, Dictionary context)
	{
		if (currentPopup != null)
		{
			currentPopup.QueueFree();
			currentPopup = null;
		}
	}

	public bool CurrentStateHasInstructions()
	{
		if (gameStateHandler == null)
			return false;

		switch (gameStateHandler.CurrentState)
		{
			case GameState.RoundStarting:
			case GameState.IsRerolling:
			case GameState.PlayerFinished:
				return true;
			default:
				return false;
		}
	} 

	public void ShowInstructions()
	{
		if (gameStateHandler == null)
			return;

		switch (gameStateHandler.CurrentState)
		{
			case GameState.RoundStarting:
			case GameState.IsRerolling:
				// TODO add swipe and shake instructions
				break;
			case GameState.PlayerFinished:
				AddPopUp("ClickInstruction", overrides: false); 
				break;
		}
	}

	public bool CanClickThruPopUp()
	{
		if (currentPopup == null)
			return true;
		return !currentPopup.Overrides;
	}
}

