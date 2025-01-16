using System.Collections.Generic;

public class MexPlayer : Player {

    public int score;
    private List<int> scores;

    public bool isFinished = false;
    public MexPlayer(string name) {
        this.name = name;
        score = 0;
        scores = new List<int>();
    }

    public void addScore(int score){
        scores.Add(score);
        if(scores.Count == 3){
            isFinished = true;
        }
        if (score == 21 ||  score == 32) {
            this.score = score;
            isFinished = true;
        }
        else if (score > this.score) this.score = score;
    }

    public void Reset(){
        scores = new List<int>();
        score = 0;
        isFinished = false;
    }
}