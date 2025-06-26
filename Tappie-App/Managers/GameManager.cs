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
	public GameState CurrentState { get; private set; } = GameState.Idle;

	public override void _Ready()
	{
		_eventManager = GetNode<EventManager>("/root/EventManager");

		bool isTest = ProjectSettings.HasSetting("test/test_mode") && (bool)ProjectSettings.GetSetting("test/test_mode");
		if (isTest)
		{
			SetPlayers(new List<string> { "Speler 1", "Speler 2", "Speler 3" });
		}
	}

	private void ChangeState(GameState newState, Dictionary context)
	{
		CurrentState = newState;
		_eventManager.EmitSignal(nameof(_eventManager.GameStateChanged), Variant.From(newState), context);
	}


	public void StartRound()
	{
		playerIndex = 0;
		amountMexx = 0;

		foreach (Player player in players)
			player.Reset();

		players = UtilsManager.ShuffleList(players);
		currentPlayer = players[0];

		// Passing the signal that the player can start their turn
		ChangeState(GameState.RoundStarting, new Dictionary
		{
			{ "Player", currentPlayer.Name },
		});
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
				players.Add(new Player(name));
		}

		if (playersChanged)
			currentKnight = null;

		if (players.Count < 2)
			throw new System.InvalidOperationException("At least two players are required.");

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

				ChangeState(GameState.PlayerFinished, new Dictionary
				{
					{ "Player", currentPlayer.Name },
					{"Score", finishedPlayer.Score },
				});
			}
			else
			{
				ChangeState(GameState.RoundFinished, new Dictionary{});
				DetermineLoser();
			}
		}
		else
		{
			int throwsLeft = currentPlayer.GetThrowsLeft();
			ChangeState(GameState.PlayerTurn, new Dictionary 
			{ 
				{ "ThrowsLeft", throwsLeft },
				{ "Score", result },
				{ "HighScore", currentPlayer.Score}
			});
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
			SendPenaltyPopup(1, new Array<string> { currentPlayer.Name }, true);
		}
		else if (result == 100)
		{
			currentKnight = currentPlayer;
			SendPopup("KnightPopUp", "Je bent nu de nieuwe ridder!");
		}
		else if (result == 600)
		{
			SendPenaltyPopup(1, new Array<string> {}, true, true, true);
		}
		else if (result % 100 == 0)
		{
			if (currentKnight != null)
			{
				int multiplier = result / 100;
				int penalty = knightStrenght * multiplier;
				string name = currentKnight.Name;
				SendPenaltyPopup(penalty, new Array<string> { name }, currentPlayer == currentKnight, true);
			}
		}
	}

	private void SendPenaltyPopup(int penalty, Array<string> names, bool give = false, bool knight = false, bool all = false)
	{
		string joinedNames = string.Join(", ", names);
		string pointWord = penalty == 1 ? "strafpunt" : "strafpunten";
		string getWord = names.Count > 1 ? "krijgen" : "krijgt";
		string text;

		if (all)
		{
			text = "Iedereen krijgt 1 strafpunt!";
		}
		else if (give)
		{
			text = $"Je mag {penalty} {pointWord} uitdelen!";
		}
		else
		{
			text = $"{joinedNames}\n{getWord} {penalty} {pointWord}!";
		}

		string popupType = knight ? "KnightPopUp" : "PopUp";
		SendPopup(popupType, text);
	}

	private void SendPopup(string sceneName, string text)
	{
		var popup = NodeCreator.CreateNode(sceneName, new Godot.Collections.Dictionary<string, Variant> {
			{ "Text", text }
		});

		if (popup != null)
			_eventManager.EmitSignal(nameof(_eventManager.PopupRequested), popup);
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
				lowestScore = player.Score;
		}

		foreach (Player player in players)
		{
			if (player.Score == lowestScore)
				loserNames.Add(player.Name);
		}

		return loserNames;
	}

	private void DetermineLoser()
	{
		Array<string> losers = GetLosers();
		int penalty = CalculatePenalty();
		SendPenaltyPopup(penalty, losers, false, false);
	}
}
