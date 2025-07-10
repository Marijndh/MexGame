using Godot;
using System;
using System.Collections.Generic;

public partial class ScoreContainer : HBoxContainer
{
	private VBoxContainer _leftContainer;
	private VBoxContainer _rightContainer;

	// TODO Set in some sort of setting
	public int MAX_PLAYERS = 8;

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

		// Sort players by custom ranking (lower index = better score)
		players.Sort((a, b) =>
		{
			int aRank = Array.IndexOf(ScoreUtils.ScoreRanking, a.Score);
			int bRank = Array.IndexOf(ScoreUtils.ScoreRanking, b.Score);

			if (aRank == -1) aRank = ScoreUtils.ScoreRanking.Length;
			if (bRank == -1) bRank = ScoreUtils.ScoreRanking.Length;

			return aRank != bRank ? aRank.CompareTo(bRank) : b.Score.CompareTo(a.Score);
		});

		// Lowest score is now the last one in the sorted list
		int lowestScore = players[^1].Score;

		int mid = (players.Count + 1) / 2;
		UpdateContainer(_leftContainer, players, 0, mid, lowestScore);
		UpdateContainer(_rightContainer, players, mid, players.Count, lowestScore);
	}


	private void UpdateContainer(VBoxContainer container, List<Player> players, int start, int end, int lowestScore)
	{
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
			bool isLast = player.Score == lowestScore;
			scoreBar.SetData(player.Score, i, player.Name, isLast, player.IsFinished);
		}

		// Remove extra children
		while (container.GetChildCount() > end - start)
		{
			Node extra = container.GetChild(container.GetChildCount() - 1);
			container.RemoveChild(extra);
			extra.QueueFree();
		}
	}

}
