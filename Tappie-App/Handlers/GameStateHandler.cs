using Godot;
using Godot.Collections;
using System.Collections.Generic;

public class GameStateHandler
{
	private GameState _currentState = GameState.Idle;
	public GameState CurrentState => _currentState;

	private Label _scoreLabel;
	private Label _nameLabel;
	private Label _infoLabel;
	private Button _closeButton;
	private Button _continueButton;
	private Button _noButton;
	private Button _yesButton;
	private ScoreContainer _scoreContainer;

	private GameManager _gameManager;
	private SceneManager _sceneSwitcher;
	private DiceManager _diceManager;

	private Queue<(GameState, Dictionary)> gameStateQueue = new();
	private bool _waitingForUserInput = false;

	public GameStateHandler(CanvasLayer canvasLayer, GameManager gameManager, SceneManager sceneSwitcher, DiceManager diceManager)
	{
		_scoreLabel = canvasLayer.GetNode<Label>("Score");
		_nameLabel = canvasLayer.GetNode<Label>("Name");
		_infoLabel = canvasLayer.GetNode<Label>("Info");
		_closeButton = canvasLayer.GetNode<Button>("Close");
		_continueButton = canvasLayer.GetNode<Button>("Continue");
		_noButton = canvasLayer.GetNode<Button>("No");
		_yesButton = canvasLayer.GetNode<Button>("Yes");
		_scoreContainer = canvasLayer.GetNode<ScoreContainer>("Container");

		_gameManager = gameManager;
		_sceneSwitcher = sceneSwitcher;
		_diceManager = diceManager;

		SetupButtons();

		EventManager.Instance.GameStateChanged += OnGameStateChanged;
	}

	public void Dispose()
	{
		EventManager.Instance.GameStateChanged -= OnGameStateChanged;
	}

	private void SetupButtons()
	{
		_closeButton.Pressed += () => _sceneSwitcher.SwitchScene("SelectPlayers");
		_continueButton.Pressed += () => {
			SetButtonsVisible(false);
			_gameManager.StartRound();
		};
		_yesButton.Pressed += () =>
		{
			_diceManager.PrepareReroll();
			SetButtonsVisible(false);
			HandleGameStateChange(GameState.IsRerolling, new Dictionary());
		};
		_noButton.Pressed += () =>
		{
			SetButtonsVisible(false);
			_diceManager.EmitRollResult(false);
		};
	}

	public void ShowNextGameState()
	{
		if (gameStateQueue.Count == 0 || _waitingForUserInput)
			return;

		(GameState state, Dictionary context) = gameStateQueue.Dequeue();
		HandleGameStateChange(state, context);
	}

	private void HandleGameStateChange(GameState state, Dictionary context)
	{
		ResetUI();
		_currentState = state;
		GD.Print("GameState changed to: ", state);

		switch (state)
		{
			case GameState.RoundStarting:
				HandleRoundStarting(context);
				break;

			case GameState.CanReroll:
				HandleRerollDie(context);
				break;

			case GameState.PlayerFinished:
				HandlePlayerFinished(context);
				break;

			case GameState.RoundFinished:
				HandleRoundFinished();
				break;

			case GameState.PlayerTurn:
				HandlePlayerTurn(context);
				break;
		}
	}

	private void HandleRoundStarting(Dictionary context)
	{
		if (context.TryGetValue("Player", out Variant player))
			_nameLabel.Text = $"{player},\nJij bent nu aan de beurt!";
		_diceManager.PrepareThrow();
	}

	private void HandleRerollDie(Dictionary context)
	{
		if (context.TryGetValue("Tens", out Variant tens) && context.TryGetValue("Ones", out Variant ones))
			_nameLabel.Text = $"Je mag de {tens} opnieuw gooien,\n De {ones} blijft staan!";
		SetButtonGroupVisible(true, _yesButton, _noButton);
	}

	private void HandlePlayerFinished(Dictionary context)
	{
		if (context.TryGetValue("Player", out Variant player))
		{
			_nameLabel.Text = $"\nVolgende speler: {player}";
			Godot.Collections.Dictionary<int, string> messages = new()
			{
				{ 21, "Kassa!\nHet hoogste!" },
				{ 32, "Jammer!\nLager kan niet\n Je bent wel gelijk klaar!" }
			};

			int score = context.TryGetValue("Score", out Variant scoreVal) ? (int)scoreVal : 0;
			string message = messages.TryGetValue(score, out string msg) ? msg : $"Eindscore: {score}";
			_infoLabel.Text = message;
		}
		else
		{
			_nameLabel.Text = "De ronde is voorbij! \nWie heeft er verloren?";
		}

			_scoreContainer.LoadScores();
		_scoreContainer.Show();
		_waitingForUserInput = true;
	}

	private void HandleRoundFinished()
	{
		_nameLabel.Text = "Wil je opnieuw spelen?";
		SetButtonGroupVisible(true, _continueButton, _closeButton);
	}

	private void HandlePlayerTurn(Dictionary context)
	{
		int score = context.TryGetValue("Score", out Variant scoreVal) ? (int)scoreVal : 0;
		int highScore = context.TryGetValue("HighScore", out Variant highScoreVal) ? (int)highScoreVal : 0;
		int throwsLeft = context.TryGetValue("ThrowsLeft", out Variant throwsLeftVal) ? (int)throwsLeftVal : 0;

		_infoLabel.Text = score == highScore
			? "Nieuw record: " + highScore
			: $"Hoogste score: {highScore}";

		_nameLabel.Text = $"Gooi nog {throwsLeft} keer!";
		_scoreLabel.Text = score.ToString();
	}

	private void OnGameStateChanged(GameState state, Dictionary context)
	{
		gameStateQueue.Enqueue((state, context));
		ShowNextGameState();
	}

	public void OnUserTouch()
	{
		if (_waitingForUserInput)
		{
			_waitingForUserInput = false;
			ShowNextGameState();
		}
	}

	public bool CanThrowDice()
	{
		return _currentState == GameState.PlayerTurn || _currentState == GameState.RoundStarting || _currentState == GameState.IsRerolling;
	}

	private void ResetUI()
	{
		_scoreLabel.Text = "";
		_nameLabel.Text = "";
		_infoLabel.Text = "";
		SetButtonsVisible(false);
		_scoreContainer.Hide();
	}

	private void SetButtonGroupVisible(bool visible, params Button[] buttons)
	{
		foreach (Button btn in buttons)
		{
			btn.Visible = visible;
			btn.Disabled = !visible;
		}
	}
	
	private void SetButtonsVisible(bool visible)
	{
		SetButtonGroupVisible(visible, _closeButton, _continueButton, _yesButton, _noButton);
	}

}
