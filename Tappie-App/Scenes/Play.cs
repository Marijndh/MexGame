using Godot;
using Godot.Collections;
using System;
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

	private Vector3 highestRollDiePosition = new Vector3(-0.5f, 0.5f, -0.3f);
	private Vector3 lowestRollDiePosition = new Vector3(-0.5f, 0.5f, 0.3f);

	private Label _scoreLabel;
	private Label _nameLabel;

	private Button _closeButton;
	private Button _continueButton;

	public override void _Ready()
	{	
		_eventManager = GetNode<EventManager>("/root/EventManager");
		_sceneSwitcher = GetNode<SceneManager>("/root/SceneManager");
		_gameManager = GetNode<GameManager>("/root/GameManager");

		_eventManager.RollFinished += OnRollFinished;
		_eventManager.Penalty += PenaltyPopUp;
		_eventManager.NewKnight += OnNewKnight;
		_eventManager.RoundFinished += () => SetButtonsVisible(true);

		_eventManager.PopupOpened += () => { popUpIsOpen = true; };
		_eventManager.PopupClosed += () => { popUpIsOpen = false; };

		CanvasLayer canvasLayer = GetNode<CanvasLayer>("Canvas");
		_scoreLabel = canvasLayer.GetNode<Label>("Score");
		_nameLabel = canvasLayer.GetNode<Label>("Name");
		_closeButton = canvasLayer.GetNode<Button>("Close");
		_continueButton = canvasLayer.GetNode<Button>("Continue");

		SetButtonsVisible(false);

		_closeButton.Pressed += () => _sceneSwitcher.SwitchScene("SelectPlayers");
		_continueButton.Pressed += () => { 
			SetButtonsVisible(false);
			_scoreLabel.Text = "";
			_gameManager.StartRound(_nameLabel);
		};

		LoadDice();
		_gameManager.StartRound(_nameLabel);
	}

	public override void _ExitTree()
	{
		_eventManager.RollFinished -= OnRollFinished;
		_eventManager.Penalty -= PenaltyPopUp;
		_eventManager.NewKnight -= OnNewKnight;
		_eventManager.RoundFinished -= () => SetButtonsVisible(true);
		_eventManager.PopupOpened -= () => { popUpIsOpen = true; };
		_eventManager.PopupClosed -= () => { popUpIsOpen = false; };
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

	private void OnNewKnight()
	{
		Node knightPopUp = NodeCreator.CreateNode(
			"KnightPopUp",
			new Godot.Collections.Dictionary<string, Variant>
			{
			{ "Text", "Je bent nu de nieuwe ridder!" }
			}
		);

		if (knightPopUp != null)
			AddChild(knightPopUp);
	}

	private void PenaltyPopUp(int penalty, Array<string> names, bool give, bool knight)
	{
		Node resultPopUp = null;
		string joinedNames = string.Join(", ", names);
		string pointWord = penalty == 1 ? "strafpunt" : "strafpunten";
		string text;

		if (knight)
		{
			if (joinedNames == "all")
			{
				text = $"Iedereen krijgt 1 strafpunt!";
			}
			else
			{
				text = give
					? $"{joinedNames}\nMag {penalty} {pointWord} uitdelen!"
					: $"{joinedNames}\nKrijgen {penalty} {pointWord}!";
			}

			resultPopUp = NodeCreator.CreateNode(
				"KnightPopUp",
				new Godot.Collections.Dictionary<string, Variant>
				{
				{ "Text", text }
				}
			);
		}
		else
		{
			text = give
				? $"{joinedNames}\nMag {penalty} \n {pointWord} \n uitdelen!"
				: $"{joinedNames}\nKrijgt {penalty} \n {pointWord}!";

			resultPopUp = NodeCreator.CreateNode(
				"PopUp",
				new Godot.Collections.Dictionary<string, Variant>
				{
				{ "Text", text }
				}
			);
		}

		if (resultPopUp != null)
			AddChild(resultPopUp);
	}

	private int GetDiceResult(int highest, int lowest){
		string resultString = highest.ToString() + lowest.ToString();
		return int.Parse(resultString);
	}

	public int FetchRollResultAndSetPosition()
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


	private void OnRollFinished() {
		amountDiceRolled++;
		if (amountDiceRolled == amountDiceWantedToRoll) {
			int result = FetchRollResultAndSetPosition();
			_scoreLabel.Text = result+"";
			_gameManager.HandleThrow(result);
		}
	}

	private void RollDice()
	{
		foreach (Die die in dice)
		{
			amountDiceWantedToRoll++;
			if (die.IsRolling)
			{
				continue;
			}
			die.Roll();
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed && !popUpIsOpen && !_closeButton.Visible && !_continueButton.Visible)
		{
			RollDice();
			_scoreLabel.Text = "";
			_nameLabel.Text = "";
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
