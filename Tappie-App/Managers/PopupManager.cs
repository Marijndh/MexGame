using Godot;
using System.Collections.Generic;

public class PopupManager
{
	private Queue<Node> popupQueue = new();
	private Node currentPopup;
	private Node parent;
	public bool PopupIsOpen => currentPopup != null;

	public PopupManager(Node parentNode)
	{
		parent = parentNode;
		EventManager.Instance.PopupRequested += EnqueuePopup;
		EventManager.Instance.PopupClosed += OnPopupClosed;
	}

	public void Dispose()
	{
		EventManager.Instance.PopupRequested -= EnqueuePopup;
		EventManager.Instance.PopupClosed -= OnPopupClosed;
		if (currentPopup != null)
		{
			currentPopup.QueueFree();
			currentPopup = null;
		}
		popupQueue.Clear();
	}

	public void EnqueuePopup(Node popup)
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
}

