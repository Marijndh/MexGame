using Godot;
using Godot.Collections;

public class GameStateHandler
{
	private Label _scoreLabel;
	private Label _nameLabel;
	private Label _infoLabel;
	private Button _closeButton;
	private Button _continueButton;
	private ScoreContainer _scoreContainer;

	private GameManager _gameManager;
	private SceneManager _sceneSwitcher;

	public GameStateHandler(CanvasLayer canvasLayer, GameManager gameManager, SceneManager sceneSwitcher)
	{
		_scoreLabel = canvasLayer.GetNode<Label>("Score");
		_nameLabel = canvasLayer.GetNode<Label>("Name");
		_infoLabel = canvasLayer.GetNode<Label>("Info");
		_closeButton = canvasLayer.GetNode<Button>("Close");
		_continueButton = canvasLayer.GetNode<Button>("Continue");
		_scoreContainer = canvasLayer.GetNode<ScoreContainer>("Container");

		_gameManager = gameManager;
		_sceneSwitcher = sceneSwitcher;

		_closeButton.Pressed += () => _sceneSwitcher.SwitchScene("SelectPlayers");
		_continueButton.Pressed += () => {
			SetButtonsVisible(false);
			_gameManager.StartRound();
		};

		EventManager.Instance.GameStateChanged += (state, ctx) => HandleGameStateChange(state, ctx);
	}

	public bool CanThrowDice()
	{
		return _gameManager.CurrentState == GameState.PlayerTurn || _gameManager.CurrentState == GameState.RoundStarting;
	}

	public void HandleGameStateChange(GameState state, Dictionary context)
	{
		GD.Print("GameState changed to: ", state, " with context: ", context.ToString());
		switch (state)
		{
			case GameState.RoundStarting:
				_nameLabel.Text = $"{context["Player"]},\nJij bent nu aan de beurt!";
				break;

			case GameState.PlayerFinished:
				Dictionary<int, string> messages = new()
				{
					{ 21, "Kassa!\nHet hoogste!" },
					{ 32, "Jammer!\nLager kan niet\n Je bent wel gelijk klaar!" }
				};

				int score = context.ContainsKey("Score") ? context["Score"].AsInt32() : 0;
				_infoLabel.Text = messages.TryGetValue(score, out string msg)
					? msg
					: $"Eindscore: {score}";

				_nameLabel.Text = $"\nVolgende speler: {context["Player"]}";
				_scoreContainer.LoadScores();
				_scoreContainer.Show();
				break;

			case GameState.RoundFinished:
				_nameLabel.Text = "De ronde is voorbij.\nWil je opnieuw?";
				SetButtonsVisible(true);
				break;

			case GameState.PlayerTurn:
				_infoLabel.Text = (context["HighScore"].AsInt32() == context["Score"].AsInt32()
					? "Nieuw record: "
					: $"Hoogste score: ") + context["HighScore"];
				_nameLabel.Text = $"Gooi nog {context["ThrowsLeft"]} keer!";
				break;
		}
	}

	private void SetButtonsVisible(bool visible)
	{
		_closeButton.Visible = visible;
		_continueButton.Visible = visible;
		_closeButton.Disabled = !visible;
		_continueButton.Disabled = !visible;
	}
}
