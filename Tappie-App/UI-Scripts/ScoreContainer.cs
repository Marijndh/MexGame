using Godot;
using System;
using System.Collections.Generic;

public partial class ScoreContainer : HBoxContainer
{
	private VBoxContainer _leftContainer;
	private VBoxContainer _rightContainer;

	private GameManager _gameManager;
	public override void _Ready()
	{
		_leftContainer = GetNode<VBoxContainer>("Left");
		_rightContainer = GetNode<VBoxContainer>("Right");

		_gameManager = GetNode<GameManager>("/root/GameManager");
		LoadScores();
	}

	public void LoadScores()
	{
		List<Player> players = _gameManager.GetPlayers();
		int playerCount = players.Count;

		int minScore = int.MaxValue;
		for (int i = 0; i < playerCount; i++)
			if (players[i].Score < minScore)
				minScore = players[i].Score;

		UpdateContainer(_leftContainer, players, 0, playerCount / 2, minScore);
		UpdateContainer(_rightContainer, players, playerCount / 2, playerCount, minScore);
	}

	private void UpdateContainer(VBoxContainer container, List<Player> players, int start, int end, int minScore)
	{
		// Update or create ScoreBars
		for (int i = start; i < end; i++)
		{
			ScoreBar scoreBar = (ScoreBar)NodeCreator.CreateNode("ScoreBar");
			if (i - start < container.GetChildCount())
			{
				scoreBar = container.GetChild<ScoreBar>(i - start);
			}
			else
			{
				scoreBar = new ScoreBar();
				container.AddChild(scoreBar);
			}
			var player = players[i];
			bool isLast = player.Score == minScore;
			scoreBar.SetData(player.Score, i, player.Name, isLast);
		}
		// Remove extra ScoreBars
		while (container.GetChildCount() > end - start)
		{
			var child = container.GetChild(container.GetChildCount() - 1);
			container.RemoveChild(child);
			child.QueueFree();
		}
	}
}
