using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class GameManager : Node
{
	private EventManager _eventManager;

	private Player currentKnight = null;

	private Player currentPlayer = null;

	private List<Player> players = new List<Player>();

	private int playerIndex;

	private int amountMexx = 0;

	private int knightStrenght = 1;

	private int penaltyPoints = 2;

	private Label nameLabel;

	public override void _Ready()
	{
		_eventManager = GetNode<EventManager>("/root/EventManager");
	}

	public void StartRound(Label nameLabel)
	{
		this.nameLabel = nameLabel;

		playerIndex = 0;
		amountMexx = 0;
		currentPlayer = players[0];
		foreach (Player player in players)
		{
			player.Reset();
		}
		players = UtilsManager.ShuffleList(players);
		currentPlayer = players[0];
		nameLabel.Text = currentPlayer.Name + ",\n Jij bent nu aan de beurt!";
	}

	public void SetPlayers(List<string> names)
	{
		if (players == null)
			players = new List<Player>();

		var oldNamesSet = new HashSet<string>(players.ConvertAll(p => p.Name));
		var newNamesSet = new HashSet<string>(names);

		bool playersChanged = !oldNamesSet.SetEquals(newNamesSet);

		players.RemoveAll(p => !newNamesSet.Contains(p.Name));

		foreach (string name in names)
		{
			if (!players.Exists(p => p.Name == name))
			{
				players.Add(new Player(name));
			}
		}

		if (playersChanged)
		{
			currentKnight = null;
		}

		if (players.Count < 2)
		{
			throw new System.InvalidOperationException("At least two players are required.");
		}

		players = UtilsManager.ShuffleList(players);
		currentPlayer = players[0];
	}

	public List<Player> GetPlayers() => players;

	public void HandleThrow(int result)
	{
		HandleResult(result);
		currentPlayer.addScore(result);
		if (currentPlayer.isFinished)
		{
			playerIndex++;
			Player finishedPlayer = currentPlayer;
			if (playerIndex < players.Count)
			{
				currentPlayer = players[playerIndex];

				Godot.Collections.Dictionary<int, string> messages = new ()
				{
					{ 21, "Kassa!\nHet hoogste!" },
					{ 32, "Jammer!\nLager kan niet" }
				};

				string extraText = messages.TryGetValue(finishedPlayer.Score, out string message)
					? message
					: $"Eindscore: {finishedPlayer.Score}";

				nameLabel.Text = $"{extraText}\nVolgende speler:\n{currentPlayer.Name}";
			}
			else
			{
				nameLabel.Text = "De ronde is voorbij \n Wil je opnieuw?";
				DetermineLoser();
				_eventManager.EmitSignal(nameof(_eventManager.RoundFinished));
			}
		}
		else
		{
			int throwsLeft = currentPlayer.GetThrowsLeft();
			string throwText = throwsLeft == 1 ? "worp" : "worpen";

			string scoreText = currentPlayer.Score == result
				? "Nieuwe hoge score:"
				: $"Hoogste score: {currentPlayer.Score}";

			nameLabel.Text = $"Nog {throwsLeft} {throwText}!\n\n{scoreText}";
		}
	}

	private void HandleResult(int result)
	{
		if (result == 21)
		{
			amountMexx++;
		}
		else if (result == 31)
		{
			_eventManager.EmitSignal(nameof(_eventManager.Penalty), 1, new Array<string>() { currentPlayer.Name }, true, false);
		}
		else if (result == 100)
		{
			currentKnight = currentPlayer;
			_eventManager.EmitSignal(nameof(_eventManager.NewKnight));
		}
		else if (result == 600)
		{
			_eventManager.EmitSignal(nameof(_eventManager.Penalty), 1, new Array<string>() { "all" }, true, false);
		}
		else if (result % 100 == 0)
		{
			if (currentKnight != null)
			{
				int multiplier = result / 100;
				if (currentPlayer == currentKnight)
				{
					_eventManager.EmitSignal(nameof(_eventManager.Penalty), knightStrenght * multiplier, new Array<string> { currentKnight.Name }, true, true);
				}
				else
				{
					_eventManager.EmitSignal(nameof(_eventManager.Penalty), knightStrenght * multiplier, new Array<string> { currentKnight.Name }, false, true);
				}
			}
		}
	}

	private int CalculatePenalty()
	{
		return penaltyPoints * (amountMexx + 1);
	}

	private Array<string> GetLosers()
	{
		var loserNames = new Array<string>();
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
				loserNames.Add(player.Name);
			}
		}
		return loserNames;
	}

	private void DetermineLoser()
	{
		Array<string> losers = GetLosers();
		int penalty = CalculatePenalty();		
		_eventManager.EmitSignal(nameof(_eventManager.Penalty), penalty, losers, false, false);
	}
}