using System.Collections.Generic;

public class Player {

    private string name;
    public string Name {
        get { return name; }
        set { name = value; }
	}

	private int highestScore;
    public int Score
    {
        get { return highestScore; }
        set { highestScore = value; }
    }

	private List<int> scores;

    private bool isFinished = false;

    public bool IsFinished
    {
        get { return isFinished; }
	}

	private bool hasRerolled = false;

    public bool HasRerolled
    {
        get { return hasRerolled; }
        set { hasRerolled = value; }
    }
	public Player(string name) {
        this.name = name;
        highestScore = 0;
        scores = new List<int>();
    }

	public void addScore(int score)
	{
		scores.Add(score);
        hasRerolled = false;

		if (score == 21 || score == 32)
		{
			this.highestScore = score;
			isFinished = true;
			return;
		}

		if (score > this.highestScore)
			this.highestScore = score;

		if (scores.Count == 1)
			isFinished = true;
	}

    public int GetThrowsLeft()
    {
        return 1 - scores.Count;
	}

    public void Reset(){
        scores = new List<int>();
        highestScore = 0;
        isFinished = false;
    }
}