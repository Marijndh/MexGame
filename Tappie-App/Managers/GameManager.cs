using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class GameManager : Node
{
	private Player currentKnight = null;
	private Player currentPlayer = null;
	private List<Player> players = new List<Player>();
	private int playerIndex = -1;
	private int amountMexx = 0;
	private int knightStrenght = 1;
	private int penaltyPoints = 2;

	public override void _Ready()
	{
		bool isTest = ProjectSettings.HasSetting("test/test_mode") && (bool)ProjectSettings.GetSetting("test/test_mode");
		if (isTest)
		{
			SetPlayers(new List<string> { "Speler 1", "Speler 2", "Speler 3" });
		}
		EventManager.Instance.DiceThrown += HandleResult;
	}

	public override void _ExitTree()
	{
		EventManager.Instance.DiceThrown -= HandleResult;
	}

	private void ChangeState(GameState newState, Dictionary context)
	{
		EventManager.Instance.EmitSignal(nameof(EventManager.Instance.GameStateChanged), Variant.From(newState), context);
	}

	public void StartRound()
	{
		if (playerIndex == -1)
		{
			playerIndex = GD.RandRange(0, players.Count - 1); // only random at game start
		}		

		amountMexx = 0;

		foreach (Player player in players)
			player.Reset();

		GD.Print("Starting round with players: ", string.Join(", ", players.ConvertAll(p => p.Name)));
		GD.Print("Current player index: ", playerIndex);
		currentPlayer = players[playerIndex];

		ChangeState(GameState.RoundStarting, new Dictionary
		{
			{ "Player", currentPlayer.Name },
		});
	}

	public void SetPlayers(List<string> names)
	{
		players = new List<Player>();

		foreach (string name in names)
		{
			players.Add(new Player(name));
		}

		currentKnight = null;

		if (players.Count < 2)
			throw new System.InvalidOperationException("At least two players are required.");
	}

	public List<Player> GetPlayers() => players;

	public void HandleResult(int result, bool canReroll)
	{
		result = 32;
		if (canReroll && currentPlayer.HasRerolled)
		{
			// Check if reroll is triggered
			bool needsReroll = HandleScore(result);
			if (needsReroll) return;
		}

		currentPlayer.addScore(result);

		if (currentPlayer.IsFinished)
		{
			Player finishedPlayer = currentPlayer;

			// Try to find the next player who is not finished
			int startIndex = playerIndex;
			do
			{
				playerIndex = (playerIndex + 1) % players.Count;
				currentPlayer = players[playerIndex];

				// If we've looped all the way around, break
				if (playerIndex == startIndex)
					break;

			} while (currentPlayer.IsFinished);

			Dictionary finishedStateData = new()
			{
				{ "Score", finishedPlayer.Score }
			};

			if (!currentPlayer.IsFinished)
			{
				finishedStateData.Add("Player", currentPlayer.Name);
			}

			ChangeState(GameState.PlayerFinished, finishedStateData);

			if (!currentPlayer.IsFinished)
			{
				ChangeState(GameState.RoundStarting, new Dictionary
				{
					{ "Player", currentPlayer.Name },
				});
			}
			else
			{
				ChangeState(GameState.RoundFinished, new Dictionary());
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
				{ "HighScore", currentPlayer.Score }
			});
		}
	}

	private bool HandleScore(int score)
	{
		switch (score)
		{
			case 21:
				amountMexx++;
				return false;

			case 32:
				return false;

			case 31:
				SendPenaltyPopup(1, new Array<string> { currentPlayer.Name }, true);
				// Brake because the player can still reroll
				break;

			case 100:
				currentKnight = currentPlayer;
				SendPopup("KnightPopUp", "Je bent nu de nieuwe ridder!");
				// Brake because the player can still reroll
				break;

			case 600:
				SendPenaltyPopup(1, new Array<string> { }, true, true, true);
				return false;
		}
		if (score % 10 == 1 || score % 10 == 2)
		{
			int tens = score / 10;
			int ones = score % 10;

			ChangeState(GameState.CanReroll, new Dictionary
			{
				{ "Tens", tens },
				{ "Ones", ones }
			});

			// Cannot end on 31, so we allow rerolling
			if (score != 31) currentPlayer.HasRerolled = true;

			return true;
		}
		else if (score / 100 > 0 && currentKnight != null)
		{
			int multiplier = score / 100;
			int penalty = knightStrenght * multiplier;
			string name = currentKnight.Name;
			SendPenaltyPopup(penalty, new Array<string> { name }, currentPlayer == currentKnight, true);
			return false;
		}
		return false;
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

	private void SendPopup(string popUp, string text)
	{
		var popup = NodeCreator.CreateNode(popUp, new Godot.Collections.Dictionary<string, Variant> {
			{ "Text", text }
		});

		if (popup != null)
			EventManager.Instance.EmitSignal(nameof(EventManager.Instance.PopupRequested), popup);
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

		// Set the starting player for next round
		if (losers.Count > 0)
		{
			string loserName = losers[0];
			playerIndex = players.FindIndex(p => p.Name == loserName);
		}
	}

}
