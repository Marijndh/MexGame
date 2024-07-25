using Godot;
using System;
using System.Collections.Generic;

public partial class Mexen : Node3D
{
	private Signals _signals;
	private SceneSwitcher _sceneSwitcher;
	private int throw_index;

	private int roll_index;

	private List<Die> dice; 

	private Vector3 highest_pos = new Vector3(0, 0.2f, 0.3f);

	private Vector3 lowest_pos = new Vector3(0, 0.2f, -0.3f);
	public override void _Ready()
	{	
		throw_index = 0;
		roll_index = 0;
		_signals = GetNode<Signals>("/root/Signals");

		_sceneSwitcher = GetNode<SceneSwitcher>("/root/SceneSwitcher");

		dice = new List<Die>{GetNode<Die>("1"),GetNode<Die>("2")};
	}

	private int GetValue(int highest, int lowest){
			string resultString = highest.ToString() + lowest.ToString();
			
			return int.Parse(resultString);
	}

	private int FetchRollResultAndSetPosition(Die die1, Die die2){
        int result;
        if (die1.value > die2.value)
        {
            result = GetValue(die1.value, die2.value);
            die1.Position = highest_pos;
            die2.Position = lowest_pos;
        }
        else if (die1.value < die2.value)
        {
            result = GetValue(die2.value, die1.value);
            die2.Position = highest_pos;
            die1.Position = lowest_pos;
        }
        else
        {
            result = die1.value * 100;
            die1.Position = highest_pos;
            die2.Position = lowest_pos;
        }
        GD.Print("" + result);
		return result;
	}

	private void RollDice(){
		foreach(Die die in dice) {
			die.Roll();
		}
		Die die1 = dice[0];
		Die die2 = dice[1];
		int result = FetchRollResultAndSetPosition(die1, die2);
	}

	public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
			RollDice();
        }
    }
}
