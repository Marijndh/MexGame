using System.Collections.Generic;

public class Player {

    private string name;
    public string Name {
        get { return name; }
        set { name = value; }
	}

	private int score;
    public int Score
    {
        get { return score; }
        set { score = value; }
    }

	private List<int> scores;

    public bool isFinished = false;
    public Player(string name) {
        this.name = name;
        score = 0;
        scores = new List<int>();
    }

	public void addScore(int score)
	{
		scores.Add(score);

		if (score == 21 || score == 32)
		{
			this.score = score;
			isFinished = true;
			return;
		}

		if (score > this.score)
			this.score = score;

		if (scores.Count == 3)
			isFinished = true;
	}

    public void Reset(){
        scores = new List<int>();
        score = 0;
        isFinished = false;
    }
}