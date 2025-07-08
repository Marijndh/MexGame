using Godot;
using System.Collections.Generic;

public class DiceManager
{
	private List<Die> dice;
	private Vector3 highest, lowest;
	private int amountDiceRolled = 0;
	private int amountDiceWantedToRoll = 0;


	public DiceManager(List<Die> dice, Vector3 highPos, Vector3 lowPos)
	{
		this.dice = dice;
		highest = highPos;
		lowest = lowPos;

		EventManager.Instance.DieRolled += DieRolled;
	}

	public void ThrowDice(Vector3 dir, float strength)
	{
		foreach (Die die in dice)
		{
			if (die.IsRolling) continue;
			die.Reset();
			die.Throw(dir, strength);
			amountDiceWantedToRoll++;
		}
	}

	public void DieRolled()
	{
		amountDiceRolled++;
		if (amountDiceRolled == amountDiceWantedToRoll)
		{
			Die die1 = dice[0];
			Die die2 = dice[1];

			int result = ScoreUtils.CalculateRollResult(die1.Value, die2.Value);

			if (die1.Value > die2.Value)
			{
				die1.Position = highest;
				die2.Position = lowest;
			}
			else if (die1.Value < die2.Value)
			{
				die2.Position = highest;
				die1.Position = lowest;
			}
			else
			{
				die1.Position = highest;
				die2.Position = lowest;
			}

			foreach (Die die in dice)
			{
				die.Freeze = true;
				die.SnapRotation();
			}
			GD.Print($"Dice rolled: {die1.Value}, {die2.Value} => Result: {result}");
			EventManager.Instance.EmitSignal(nameof(EventManager.Instance.DiceThrown), result);
		}
	}
}
