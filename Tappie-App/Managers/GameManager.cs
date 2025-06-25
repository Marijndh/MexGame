using Godot;

using System.Collections.Generic;

public partial class GameManager : Node
{
	private EventManager _eventManager;

    private Player currentKnight = null; 

	private Player currentPlayer = null;

	private List<Player> players;

	private int playerIndex;

	private int amountMexx = 0;

	private int knightStrenght = 1;

	private int penaltyPoints = 2;

	public override void _Ready(){
		_eventManager = GetNode<EventManager>("/root/EventManager");
	}
	public void SetPlayers(List<string> names)
	{
		players = new List<Player>();
		foreach (string name in names)
		{
			Player player = new Player(name);
			players.Add(player);
		}
		players = UtilsManager.ShuffleList(players);
		currentPlayer = players[0];
	}

	public string GetCurrentPlayerName(){
        return currentPlayer.Name;
    }

	public void HandleDiceResult(int result, Label nameLabel)
	{
		handleSpecialResults(result);
		currentPlayer.addScore(result);
		if (currentPlayer.isFinished)
		{
			playerIndex++;
			string playerThrowName = currentPlayer.Name;
			if (playerIndex < players.Count)
			{
				currentPlayer = players[playerIndex];
				nameLabel.Text = "Mooie worp, " + playerThrowName + "!\nDe volgende speler is:\n" + currentPlayer.Name;
			}
			else
			{
				nameLabel.Text = "Mooie worp, " + playerThrowName + "!\nDe ronde is voorbij";
				DetermineLoser(nameLabel);
			}
		}
	}

	private void handleSpecialResults(int result)
	{
		if (result == 21)
		{
			amountMexx++;
		}
		else if (result == 31)
		{
			_eventManager.EmitSignal(nameof(_eventManager.Penalty), 1, currentPlayer.Name, true, false);
		}
		else if (result == 100)
		{
			currentKnight = currentPlayer;
		}
		else if (result == 600)
		{
			_eventManager.EmitSignal(nameof(_eventManager.Penalty), 1, "all", true, false);
		}
		else if (result % 100 == 0)
		{
			if (currentKnight != null && currentPlayer == currentKnight)
			{
				_eventManager.EmitSignal(nameof(_eventManager.Penalty), knightStrenght, currentKnight.Name, true, true);
			}
			else _eventManager.EmitSignal(nameof(_eventManager.Penalty), knightStrenght, currentKnight.Name, false, true);
		}
	}

	private List<Player> getLosers()
	{
		List<Player> losers = new List<Player>();
		int lowestScore = int.MaxValue;
		foreach (Player player in players)
		{
			if (player.Score < lowestScore && player.Score != 21)
			{
				lowestScore = player.Score;
			}
		}
		foreach (Player player in players)
		{
			if (player.Score == lowestScore)
			{
				losers.Add(player);
			}
		}
		return losers;
	}

	private int CalculatePenalty()
	{
		return penaltyPoints * (amountMexx + 1);
	}

	private void RefreshRound(Player loser)
	{
		playerIndex = 0;
		foreach (Player player in players)
		{
			player.Reset();
		}
		players.Remove(loser);
		UtilsManager.ShuffleList(players);
		players.Insert(0, loser);
		currentPlayer = loser;
	}


	private void DetermineLoser(Label nameLabel)
	{
		List<Player> losers = getLosers();
		int penalty = CalculatePenalty();
		if (losers.Count > 1)
		{
			_eventManager.EmitSignal(nameof(_eventManager.DetermineLoser), penalty);
		}
		else
		{
			Player loser = losers[0];
			RefreshRound(loser);
			_eventManager.EmitSignal(nameof(_eventManager.Penalty), penalty, loser.Name);
			nameLabel.Text = "Jammer " + loser.Name + "!\nJe mag nu wel\nals eerste werpen!";
		}
	}
}