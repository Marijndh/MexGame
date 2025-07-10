using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class DiceManager
{
	private List<Die> dice;
	private List<Vector3> targetPositions = new();
	private Vector3 centerPosition = new Vector3(0, 3.5f, 0.4f);
	private int amountDiceRolled = 0;
	private int amountDiceWantedToRoll = 0;

	public DiceManager(List<Die> dice)
	{
		this.dice = dice;

		CalculateTargetPositions(dice.Count);

		EventManager.Instance.DieRolled += DieRolled;
	}

	public void Dispose()
	{
		EventManager.Instance.DieRolled -= DieRolled;
	}

	private void CalculateTargetPositions(int diceCount)
	{
		targetPositions.Clear();

		float spacing = 0.65f;
		float startX = centerPosition.X - ((diceCount - 1) * spacing / 2);

		for (int i = 0; i < diceCount; i++)
		{
			float x = startX + i * spacing;
			targetPositions.Add(new Vector3(x, centerPosition.Y, centerPosition.Z));
		}
	}

	public void ThrowDice(Vector3 dir, float strength) 
	{
		EventManager.Instance.EmitSignal(nameof(EventManager.Instance.GameStateChanged), Variant.From(GameState.DiceRolling), new Dictionary { });

		amountDiceRolled = 0;
		amountDiceWantedToRoll = 0;

		foreach (Die die in dice)
		{
			if (die.IsRolling || !die.Visible) continue;
			die.Reset();
			die.Throw(dir, strength);
			amountDiceWantedToRoll++;
		}
	}

	public void PrepareReroll()
	{
		if (dice == null || dice.Count == 0)
			return;

		// Find die with lowest value without full sorting
		Die highestDie = dice[0];
		foreach (Die die in dice)
		{
			if (die.Value > highestDie.Value)
				highestDie = die;
		}

		// Update dice states
		foreach (Die die in dice)
		{
			if (die != highestDie)
			{
				die.Deactivate();
			}			
		}

		// Recalculate position for single die
		CalculateTargetPositions(1);
		highestDie.Position = targetPositions[0];
	}

	public void EmitRollResult(bool canReroll = true)
	{
		int result = ScoreUtils.CalculateRollResult(dice[0].Value, dice[1].Value); // TODO make calculate result more generic
		EventManager.Instance.EmitSignal(nameof(EventManager.Instance.DiceThrown), result, canReroll);
	}


	public void DieRolled()
	{
		amountDiceRolled++;
		if (amountDiceRolled < amountDiceWantedToRoll)
			return;

		foreach (Die die in dice)
		{
			die.Activate();
		}
		List<Die> sortedDice = dice.OrderByDescending(d => d.Value).ToList();
		CalculateTargetPositions(dice.Count);
		Debug.Print($"Dice rolled: {string.Join(", ", sortedDice.Select(d => d.Value))}");

		for (int i = 0; i < sortedDice.Count; i++)
		{
			sortedDice[i].Position = targetPositions[i];
			sortedDice[i].Freeze = true;
			sortedDice[i].SnapRotation();
		}

		EmitRollResult();		
	}
}
