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

    public bool isFinished = false;
    public Player(string name) {
        this.name = name;
        highestScore = 0;
        scores = new List<int>();
    }

	public void addScore(int score)
	{
		scores.Add(score);

		if (score == 21 || score == 32)
		{
			this.highestScore = score;
			isFinished = true;
			return;
		}

		if (score > this.highestScore)
			this.highestScore = score;

		if (scores.Count == 3)
			isFinished = true;
	}

    public int GetThrowsLeft()
    {
        return 3 - scores.Count;
	}

    public void Reset(){
        scores = new List<int>();
        highestScore = 0;
        isFinished = false;
    }
}