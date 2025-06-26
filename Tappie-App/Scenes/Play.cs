using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class Play : Node3D
{
	private EventManager _eventManager;
	private SceneManager _sceneSwitcher;
	private GameManager _gameManager;

	private bool popUpIsOpen = false;

	private List<Die> dice; 

	private int amountDiceRolled;
	private int amountDiceWantedToRoll;

	private Vector3 highestRollDiePosition = new Vector3(-0.325f, 3.5f, 0.4f);
	private Vector3 lowestRollDiePosition = new Vector3(0.325f, 3.5f, 0.4f);

	private Label _scoreLabel;
	private Label _nameLabel;
	private Label _infoLabel;

	private Button _closeButton;
	private Button _continueButton; 

	// Shake detection variables
	private Vector3 _lastAccel = Vector3.Zero;
	private float _shakeThreshold = 3.0f;
	private float _shakeCooldown = 1.0f;
	private float _shakeTimer = 0f;

	// Drag and drop variables
	private bool isDragging = false;
	private Vector2 dragStartPos;
	private Vector2 dragEndPos;
	private Camera3D camera;

	public override void _Ready()
	{
		camera = GetViewport().GetCamera3D();

		_eventManager = GetNode<EventManager>("/root/EventManager");
		_sceneSwitcher = GetNode<SceneManager>("/root/SceneManager");
		_gameManager = GetNode<GameManager>("/root/GameManager");

		_eventManager.RollFinished += OnRollFinished;
		_eventManager.PopupRequested += OnPopupRequested;
		_eventManager.GameStateChanged += OnGameStateChanged;

		_eventManager.PopupClosed += () => { popUpIsOpen = false; };

		CanvasLayer canvasLayer = GetNode<CanvasLayer>("Canvas");
		_scoreLabel = canvasLayer.GetNode<Label>("Score");
		_nameLabel = canvasLayer.GetNode<Label>("Name");
		_infoLabel = canvasLayer.GetNode<Label>("Info");
		_closeButton = canvasLayer.GetNode<Button>("Close");
		_continueButton = canvasLayer.GetNode<Button>("Continue");

		SetButtonsVisible(false);

		_closeButton.Pressed += () => _sceneSwitcher.SwitchScene("SelectPlayers");
		_continueButton.Pressed += () => { 
			SetButtonsVisible(false);
			_gameManager.StartRound();
		};

		LoadDice();
		_gameManager.StartRound();
	}

	public override void _ExitTree()
	{
		_eventManager.RollFinished -= OnRollFinished;
		_eventManager.PopupRequested -= OnPopupRequested;
		_eventManager.PopupClosed -= () => { popUpIsOpen = false; };
	}

	public override void _PhysicsProcess(double delta)
	{
		bool isMobile = OS.HasFeature("mobile");

		if (_closeButton.Visible || _continueButton.Visible || popUpIsOpen)
			return;

		if (isMobile)
		{
			_shakeTimer -= (float)delta;
			Vector3 currentAccel = Input.GetAccelerometer();
			Vector3 deltaAccel = currentAccel - _lastAccel;

			if (deltaAccel.Length() > _shakeThreshold && _shakeTimer <= 0f)
			{
				ThrowDice(Vector3.Forward);
				_shakeTimer = _shakeCooldown;
			}

			_lastAccel = currentAccel;
		}
		else
		{
			if (Input.IsActionJustPressed("ui_select"))
			{
				ThrowDice(Vector3.Forward, 20);
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (_closeButton.Visible || _continueButton.Visible || popUpIsOpen)
			return;

		if (@event is InputEventMouseButton mouseBtn && mouseBtn.ButtonIndex == MouseButton.Left)
		{
			if (mouseBtn.Pressed)
				StartDrag(mouseBtn.Position);
			else if (isDragging)
				EndDrag(mouseBtn.Position);
		}
		else if (@event is InputEventScreenTouch touch)
		{
			if (touch.Pressed)
				StartDrag(touch.Position);
			else if (isDragging)
				EndDrag(touch.Position);
		}
		else if (@event is InputEventScreenDrag drag)
		{
			if (isDragging)
				UpdateDrag(drag.Position);
		}
	}

	private void StartDrag(Vector2 position)
	{
		isDragging = true;
		dragStartPos = position;
		dragEndPos = position;
	}

	private void UpdateDrag(Vector2 position)
	{
		dragEndPos = position;
	}

	private void EndDrag(Vector2 position)
	{
		dragEndPos = position;
		isDragging = false;

		Vector2 dragVector = dragEndPos - dragStartPos;
		if (dragVector.Length() > 20f)
		{
			Vector3 throwDir = new Vector3(dragVector.X, 0, dragVector.Y).Normalized();
			float strength = Mathf.Min(dragVector.Length() * 0.02f, 35f);
			ThrowDice(throwDir, strength);
		}
	}

	private void OnGameStateChanged(GameState state, Dictionary context)
	{
		switch (state)
		{
			case GameState.RoundStarting:
				_nameLabel.Text = $"{context["Player"]},\nJij bent nu aan de beurt!";
				break;

			case GameState.PlayerFinished:
				Godot.Collections.Dictionary<int, string> messages = new()
				{
					{ 21, "Kassa!\nHet hoogste!" },
					{ 32, "Jammer!\nLager kan niet\n Je bent wel gelijk klaar!" }
				};

				int score = context.ContainsKey("Score") ? context["Score"].AsInt32() : 0; // Fix: Convert Variant to int
				_infoLabel.Text = messages.TryGetValue(score, out string msg)
				? msg
				: $"Eindscore: {score}";
				_nameLabel.Text = $"\nVolgende speler: {context["Player"]}";
				break;

			case GameState.RoundFinished:
				_nameLabel.Text = "De ronde is voorbij.\nWil je opnieuw?";
				SetButtonsVisible(true);
				break;

			case GameState.PlayerTurn:
				_infoLabel.Text = (context["HighScore"].AsInt32() == context["Score"].AsInt32()
				? "Nieuw record: "
				: $"Hoogste score: ") + context["Score"];
				_nameLabel.Text = $"Gooi nog {context["ThrowsLeft"]} keer!";
				break;
		}
	}

	private void LoadDice()
	{
		dice = new List<Die>();

		for (int i = 1; i <= 2; i++)
		{
			Die dieNode = GetNodeOrNull<Die>(i.ToString());
			if (dieNode == null)
			{
				continue;
			}
			dice.Add(dieNode);
		}
	}

	private void ThrowDice(Vector3 direction, float strength = 5f)
	{
		_scoreLabel.Text = "";
		_nameLabel.Text = "";
		_infoLabel.Text = "";
		foreach (Die die in dice)
		{
			if (die.IsRolling) continue;

			die.Reset();
			die.Throw(direction, strength);
			amountDiceWantedToRoll++;
		}
	}

	private void OnRollFinished()
	{
		amountDiceRolled++;
		if (amountDiceRolled == amountDiceWantedToRoll)
		{
			int result = HandleRolledDice();
			_scoreLabel.Text = result + "";
			_gameManager.HandleThrow(result);
		}
	}

	private int GetDiceResult(int highest, int lowest){
		string resultString = highest.ToString() + lowest.ToString();
		return int.Parse(resultString);
	}

	public int HandleRolledDice()
	{
		Die die1 = dice[0];
		Die die2 = dice[1];

		int result;
		if (die1.Value > die2.Value)
		{
			result = GetDiceResult(die1.Value, die2.Value);
			die1.Position = highestRollDiePosition;
			die2.Position = lowestRollDiePosition;
		}
		else if (die1.Value < die2.Value)
		{
			result = GetDiceResult(die2.Value, die1.Value);
			die2.Position = highestRollDiePosition;
			die1.Position = lowestRollDiePosition;
		}
		else
		{
			result = die1.Value * 100;
			die1.Position = highestRollDiePosition;
			die2.Position = lowestRollDiePosition;
		}

		foreach (Die die in dice)
		{
			die.Freeze = true;
			die.SnapRotation();
		}

		return result;
	}


	private void OnPopupRequested(Node popup)
	{
		if (popup != null)
		{
			popUpIsOpen = true;
			AddChild(popup);
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
