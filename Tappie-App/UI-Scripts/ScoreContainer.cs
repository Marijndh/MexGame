using Godot;
using System;
using System.Collections.Generic;

public partial class ScoreContainer : HBoxContainer
{
	private VBoxContainer _leftContainer;
	private VBoxContainer _rightContainer;

	private int MAX_PLAYERS = 8;

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
		// Order players by custom ranking: lower index in ScoreRanking is better
		players.Sort((a, b) =>
		{
			int aRank = Array.IndexOf(ScoreUtils.ScoreRanking, a.Score);
			int bRank = Array.IndexOf(ScoreUtils.ScoreRanking, b.Score);

			// If not found in ranking, treat as worst (after all ranked)
			if (aRank == -1) aRank = ScoreUtils.ScoreRanking.Length;
			if (bRank == -1) bRank = ScoreUtils.ScoreRanking.Length;

			if (aRank != bRank)
				return aRank.CompareTo(bRank); // lower rank is better
											   // If same rank, keep higher score first
			return b.Score.CompareTo(a.Score);
		});
		int playerCount = players.Count;

		// Find the minimum score among all players using ScoreRanking
		int minScore = int.MaxValue;
		for (int i = 0; i < playerCount; i++)
		{
			int rank = Array.IndexOf(ScoreUtils.ScoreRanking, players[i].Score);
			if (rank == -1) rank = ScoreUtils.ScoreRanking.Length;
			if (rank < minScore)
				minScore = players[i].Score;
		}

		int halve = (playerCount + 1) / 2;
		UpdateContainer(_leftContainer, players, 0, halve, minScore);
		UpdateContainer(_rightContainer, players, halve, playerCount, minScore);
	}

	private void UpdateContainer(VBoxContainer container, List<Player> players, int start, int end, int minScore)
	{
		// Update or create ScoreBars
		for (int i = start; i < end; i++)
		{
			ScoreBar scoreBar;
			if (i - start < container.GetChildCount())
			{
				scoreBar = container.GetChild<ScoreBar>(i - start);
			}
			else
			{
				scoreBar = (ScoreBar)NodeCreator.CreateNode("ScoreBar", nodeName: i.ToString());
				container.AddChild(scoreBar);
			}
			Player player = players[i];
			bool isLast = player.Score == minScore;
			scoreBar.SetData(player.Score, i, player.Name, isLast);
		}
		// Remove extra ScoreBars
		while (container.GetChildCount() > end - start)
		{
			Node child = container.GetChild(container.GetChildCount() - 1);
			container.RemoveChild(child);
			child.QueueFree();
		}
	}
}
