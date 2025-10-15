public static class ScoreUtils
{
	// This array defines the score ranking for the game.
	// The lower the index, the higher the score.
	public static readonly int[] ScoreRanking = new int[]
    {
        21,
        600, 500, 400, 300, 200, 100,
        65, 64, 63, 62, 61,
        54, 53, 52, 51,
        43, 42, 41,
        31, 0, 32
    };
	public static int GetDiceResult(int highest, int lowest)
	{
		string resultString = highest.ToString() + lowest.ToString();
		return int.Parse(resultString);
	}

	public static int CalculateRollResult(int die1Value, int die2Value)
	{
		if (die1Value == die2Value)
			return die1Value * 100;
		return die1Value > die2Value
			? GetDiceResult(die1Value, die2Value)
			: GetDiceResult(die2Value, die1Value);
	}
}