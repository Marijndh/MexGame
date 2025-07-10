using Godot;
using Godot.Collections;
using System.Collections.Generic;

public class PopupManager
{
	private Queue<Node> popupQueue = new();
	private Node currentPopup;
	private Node parent;
	private GameStateHandler gameStateHandler;
	public bool PopupIsOpen => currentPopup != null;

	public PopupManager(Node parent, GameStateHandler gameStateHandler)
	{
		this.parent = parent;
		this.gameStateHandler = gameStateHandler;
		EventManager.Instance.PopupRequested += EnqueuePopup;
		EventManager.Instance.PopupClosed += OnPopupClosed;
		EventManager.Instance.GameStateChanged += OnGameStateChanged;
	}

	public void Dispose()
	{
		EventManager.Instance.PopupRequested -= EnqueuePopup;
		EventManager.Instance.PopupClosed -= OnPopupClosed;
		EventManager.Instance.GameStateChanged -= OnGameStateChanged;
		if (currentPopup != null)
		{
			currentPopup.QueueFree();
			currentPopup = null;
		}
		popupQueue.Clear();
	}

	private void EnqueuePopup(Node popup)
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
	
	private void InstantiateInstruction(string name)
	{
		Node popUp = NodeCreator.CreateNode(name, null);
		EnqueuePopup(popUp);
	}

	public void ShowInstructions()
	{
		switch (gameStateHandler.CurrentState)
		{
			case GameState.RoundStarting:
			case GameState.IsRerolling:
				// TODO add swipe and shake instructions
				break;
			case GameState.PlayerFinished:
				InstantiateInstruction("ClickInstruction"); 
				break;
				
		}
	}
}

