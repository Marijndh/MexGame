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

	private float _lastInputTime = 0f;
	private float _inputTimeout = 3f; // seconds

	public override void _Ready()
	{
		camera = GetViewport().GetCamera3D();

		_sceneManager = GetNode<SceneManager>("/root/SceneManager");
		_gameManager = GetNode<GameManager>("/root/GameManager");

		CanvasLayer canvasLayer = GetNode<CanvasLayer>("Canvas");
		LoadDice();

		_diceManager = new DiceManager(dice);
		_gameStateHandler = new GameStateHandler(canvasLayer, _gameManager, _sceneManager, _diceManager);
		_popupManager = new PopupManager(this, _gameStateHandler);

		_gameManager.Instantiate(_popupManager);
		_gameManager.StartRound();
	}

	public override void _ExitTree()
	{
		_gameStateHandler.Dispose();
		_diceManager.Dispose();
		_popupManager.Dispose();
	}

	private bool _instructionsShown = false;

	public override void _Process(double delta)
	{
		if (_popupManager.CurrentStateHasInstructions() && !_popupManager.PopupIsOpen)
		{
			_lastInputTime += (float)delta;

			if (_lastInputTime >= _inputTimeout && !_instructionsShown)
			{
				GD.Print("Showing instructions due to inactivity.");
				_popupManager.ShowInstructions();
				_instructionsShown = true;
			}
		}
		else
		{
			// Reset flag when popup is open or state changes
			_instructionsShown = false;
			_lastInputTime = 0f;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_gameStateHandler.CanThrowDice()) return;

		if (OS.HasFeature("mobile"))
		{
			_shakeTimer -= (float)delta;
			Vector3 currentAccel = Input.GetAccelerometer();
			Vector3 deltaAccel = currentAccel - _lastAccel;

			if (deltaAccel.Length() > _shakeThreshold && _shakeTimer <= 0f)
			{
				Vector3 throwDirection = GetRandomThrowVector();
				float power = (float)GD.RandRange(12f, 18f);
				_diceManager.ThrowDice(throwDirection, power);

				_shakeTimer = _shakeCooldown;
				_lastAccel = Vector3.Zero;

				_lastInputTime = 0f; // Reset on shake
			}

			_lastAccel = currentAccel;
		}
		else
		{
			if (Input.IsActionJustPressed("ui_select"))
			{
				Vector3 throwDirection = GetRandomThrowVector();
				float power = (float)GD.RandRange(12f, 18f);
				_diceManager.ThrowDice(throwDirection, power);

				_lastInputTime = 0f; // Reset on input
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		Vector2 position;

		if (@event is InputEventScreenTouch touch)
		{
			_lastInputTime = 0f; // Reset on any touch

			if (_popupManager.CanClickThruPopUp()) _gameStateHandler.OnUserTouch();
			if (_gameStateHandler.CanThrowDice())
			{
				position = touch.Position;
				if (touch.Pressed)
					StartDrag(position);
				else if (isDragging)
					EndDrag(position);
			}
		}
		else if (@event is InputEventMouseButton mouseBtn && mouseBtn.ButtonIndex == MouseButton.Left)
		{
			_lastInputTime = 0f; // Reset on any mouse click

			if (_popupManager.CanClickThruPopUp()) _gameStateHandler.OnUserTouch();
			if (_gameStateHandler.CanThrowDice())
			{
				position = mouseBtn.Position;
				if (mouseBtn.Pressed)
					StartDrag(position);
				else if (isDragging)
					EndDrag(position);
			}			
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
	private Vector3 GetRandomThrowVector()
	{
		// ±45 degrees in radians
		float maxAngle = Mathf.DegToRad(45f);

		// Random angle between -45° and 45°
		float angle = (float)(GD.RandRange(-maxAngle, maxAngle));

		// Rotate forward vector around the Y-axis
		Vector3 direction = Vector3.Forward.Rotated(Vector3.Up, angle).Normalized();

		return direction;
	}

}
