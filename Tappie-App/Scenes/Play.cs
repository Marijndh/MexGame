using Godot;
using Godot.Collections;
using System.Collections.Generic;

public partial class Play : Node3D
{
	private SceneManager _sceneManager;
	private GameManager _gameManager;
	private GameStateHandler _gameStateHandler;
	private PopupManager _popupManager;
	private DiceManager _diceManager;

	private List<Die> dice; 

	private Vector3 highestRollDiePosition = new Vector3(-0.325f, 3.5f, 0.4f);
	private Vector3 lowestRollDiePosition = new Vector3(0.325f, 3.5f, 0.4f);

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

	// Game state handling variables
	private Queue<(GameState, Dictionary)> gameStateQueue = new();
	private float minDisplayTime = 3f;
	private float currentStateTimer = 0f;
	private bool waitingForUserInput = false;


	public override void _Ready()
	{
		camera = GetViewport().GetCamera3D();

		_sceneManager = GetNode<SceneManager>("/root/SceneManager");
		_gameManager = GetNode<GameManager>("/root/GameManager");

		CanvasLayer canvasLayer = GetNode<CanvasLayer>("Canvas");
		LoadDice();

		_diceManager = new DiceManager(dice, highestRollDiePosition, lowestRollDiePosition);
		_gameStateHandler = new GameStateHandler(canvasLayer, _gameManager, _sceneManager);
		_popupManager = new PopupManager(this);
						
		_gameManager.StartRound();
	}


	public override void _PhysicsProcess(double delta)
	{
		bool isMobile = OS.HasFeature("mobile");

		// Waiting for user input to show next game state
		if (waitingForUserInput)
		{
			currentStateTimer += (float)delta;
			if (currentStateTimer >= minDisplayTime)
				ShowNextGameState();
		}

		if (!_gameStateHandler.CanThrowDice())
			return;

		if (isMobile)
		{
			_shakeTimer -= (float)delta;
			Vector3 currentAccel = Input.GetAccelerometer();
			Vector3 deltaAccel = currentAccel - _lastAccel;

			if (deltaAccel.Length() > _shakeThreshold && _shakeTimer <= 0f)
			{
				_diceManager.ThrowDice(Vector3.Forward, 10);
				_shakeTimer = _shakeCooldown;
			}

			_lastAccel = currentAccel;
		}
		else
		{
			if (Input.IsActionJustPressed("ui_select"))
			{
				_diceManager.ThrowDice(Vector3.Forward, 10);
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (!_gameStateHandler.CanThrowDice())
			return;

		Vector2 position;

		if (@event is InputEventScreenTouch touch)
		{
			position = touch.Position;
			if (waitingForUserInput)
				ShowNextGameState();

			if (touch.Pressed)
				StartDrag(position);
			else if (isDragging)
				EndDrag(position);
		}
		else if (!OS.HasFeature("mobile") && @event is InputEventMouseButton mouseBtn && mouseBtn.ButtonIndex == MouseButton.Left)
		{
			position = mouseBtn.Position;
			if (waitingForUserInput)
				ShowNextGameState();

			if (mouseBtn.Pressed)
				StartDrag(position);
			else if (isDragging)
				EndDrag(position);
		}
	}
	private void StartDrag(Vector2 position)
	{
		isDragging = true;
		dragStartPos = position;
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
			_diceManager.ThrowDice(throwDir, strength);
		}
	}

	private void ShowNextGameState()
	{
		if (gameStateQueue.Count == 0)
		{
			waitingForUserInput = false;
			return;
		}

		_gameStateHandler.HandleGameStateChange(gameStateQueue.Dequeue().Item1, gameStateQueue.Dequeue().Item2);

		// Start waiting again
		currentStateTimer = 0f;
		waitingForUserInput = true;
	}
	private void OnGameStateChanged(GameState state, Dictionary context)
	{
		gameStateQueue.Enqueue((state, context));
		if (!waitingForUserInput)
		{
			ShowNextGameState();
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
}
