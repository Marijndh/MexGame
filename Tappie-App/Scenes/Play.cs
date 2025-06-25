using Godot;
using System.Collections.Generic;

public partial class Play : Node3D
{
	private EventManager _eventManager;
	private SceneManager _sceneSwitcher;
	private GameManager _gameManager;

	private List<Die> dice; 

	private int amountDiceRolled;
	private int amountDiceWantedToRoll;

	private Vector3 highestRollDiePosition = new Vector3(-0.5f, 0.5f, -0.3f);
	private Vector3 lowestRollDiePosition = new Vector3(-0.5f, 0.5f, 0.3f);

	private Label _scoreLabel;
	private Label _nameLabel;	

	public override void _Ready()
	{	
		_eventManager = GetNode<EventManager>("/root/EventManager");
		_sceneSwitcher = GetNode<SceneManager>("/root/SceneManager");
		_gameManager = GetNode<GameManager>("/root/GameManager");

		_eventManager.RollFinished += OnRollFinished;
		_eventManager.Penalty += PenaltyPopUp;
		_eventManager.NewKnight += OnNewKnight;

		_scoreLabel = GetChild(0).GetNode<Label>("Score");
		_nameLabel = GetChild(0).GetNode<Label>("Name");		

		LoadDice();
	}

	private void LoadDice()
	{
		dice = new List<Die>();

		for (int i = 1; i <= 2; i++)
		{
			Die dieNode = GetNodeOrNull<Die>(i.ToString());
			if (dieNode == null)
			{
				GD.PrintErr($"Die node '{i}' not found!");
				continue;
			}
			dice.Add(dieNode);
		}
	}

	private void OnNewKnight(string name) {

	}

	private void PenaltyPopUp(int penalty, string name, bool give, bool knight)
	{
		Node resultPopUp = null;
		string text;

		if (knight)
		{
			text = name == "all"
				? "Iedereen krijgt 1 strafpunt!"
				: (give
					? $"{name}\n Mag {penalty} strafpunt(en) uitdelen!"
					: $"{name}\n Krijgt {penalty} strafpunt(en)!");

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
				? $"{name}\n Mag {penalty} strafpunt(en) uitdelen!"
				: $"{name}\n Krijgt {penalty} strafpunt(en)!";

			resultPopUp = NodeCreator.CreateNode(
				"PunishmentPopUp",
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
		GD.Print("Roll finished!");
		amountDiceRolled++;
		if (amountDiceRolled == amountDiceWantedToRoll) {
			GD.Print("All dice rolled!");
			int result = FetchRollResultAndSetPosition();
			_scoreLabel.Text = result+"";
			//_gameManager.HandleDiceResult(result, _nameLabel);
		}
	}

	private void RollDice()
	{
		foreach (Die die in dice)
		{
			amountDiceWantedToRoll++;
			die.Roll();
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
