using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

public partial class MexScene : Node3D
{
	private Signals _signals;
	private SceneSwitcher _sceneSwitcher;

	private List<Die> dice; 

	private Die die1;
	private Die die2;

	private int amountDiceRolled;

	private int amountDiceWantedToRoll;

	private Vector3 highestRollDiePosition = new Vector3(-0.5f, 0.5f, -0.3f);

	private Vector3 lowestRollDiePosition = new Vector3(-0.5f, 0.5f, 0.3f);

	private Label _scoreLabel;

	private Label _nameLabel;

	private MexGameManager gameManager;

	public override void _Ready()
	{	
		_signals = GetNode<Signals>("/root/Signals");
		_signals.RollFinished += OnRollFinished;
		_signals.Penalty += PenaltyPopUp;
		_signals.NewKnight += OnNewKnight;
		_scoreLabel = GetChild(0).GetNode<Label>("Score");
		_nameLabel = GetChild(0).GetNode<Label>("Name");
		_sceneSwitcher = GetNode<SceneSwitcher>("/root/SceneSwitcher");

		// Get the dice
		dice = new List<Die>{GetNode<Die>("1"),GetNode<Die>("2")};
		die1 = dice[0];
		die2 = dice[1];

		gameManager = new MexGameManager(Global.current_game, _signals);
		_nameLabel.Text = gameManager.GetCurrentPlayerName() + ",\n Jij bent nu aan de beurt!";
	}

	private void OnNewKnight(string name) {

	}
	
	private void PenaltyPopUp(int penalty, string name, bool give, bool knight){
		Node resultPopUp = null;
		if (knight) {
				if (name == "all") {
					resultPopUp = NodeCreator.getKnightPopUpEverybodyDrinks();
				}
				else resultPopUp = NodeCreator.getKnightPopUp(name, penalty, give);
			}
		else {
			resultPopUp = NodeCreator.getPunishmentPopUp(name, penalty, give);
		}
		if (resultPopUp != null) AddChild(resultPopUp); 
	}

	private void RollDice(){
		foreach(Die die in dice){
			amountDiceWantedToRoll++;
			die.Roll();
		}
	}

	private int GetDiceResult(int highest, int lowest){
		string resultString = highest.ToString() + lowest.ToString();
		return int.Parse(resultString);
	}

	public int FetchRollResultAndSetPosition(){
        int result;
        die1 = dice[0];
		die2 = dice[1];
        if (die1.value > die2.value)
        {
            result = GetDiceResult(die1.value, die2.value);
            die1.Position = highestRollDiePosition;
            die2.Position = lowestRollDiePosition;
        }
        else if (die1.value < die2.value)
        {
            result = GetDiceResult(die2.value, die1.value);
            die2.Position = highestRollDiePosition;
            die1.Position = lowestRollDiePosition;
        }
        else
        {
            result = die1.value * 100;
            die1.Position = highestRollDiePosition;
            die2.Position = lowestRollDiePosition;
        }
		foreach(Die die in dice) {
			die.Freeze = true;
			die.SnapRotation();
		}
		return result;
	}

	private void EnsureDiceExits(){
		if(!IsInstanceValid(die1)||!IsInstanceValid(die2)) {
			die1 = dice[0];
			die2 = dice[1];
		}
	}


	private void OnRollFinished() {
		amountDiceRolled++;
		if (amountDiceRolled == amountDiceWantedToRoll) {
			EnsureDiceExits();
			int result = FetchRollResultAndSetPosition();
			_scoreLabel.Text = result+"";
			gameManager.HandleDiceResult(result, _nameLabel);
		}
	}

	public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
			RollDice();
			_scoreLabel.Text = "";
			_nameLabel.Text = "";
        }
    }
}
