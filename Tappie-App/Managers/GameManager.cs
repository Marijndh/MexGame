using Godot;

using System.Collections.Generic;

public partial class GameManager : Node{
	private EventManager _eventManager;

	private int playerIndex;

    private Player currentKnight = null; 

	private int knightStrenght = 1;

	private Player currentPlayer = null;

	private List<Player> players;

	private int amountMexx = 0;

	private int penaltyPoints;

	private string mode;



    public GameManager(){
		penaltyPoints = 2;
		_eventManager = GetNode<EventManager>("/root/EventManager");

		// Shuffle player list
		UtilsManager.ShuffleList(players);
		currentPlayer = players[0];
	}

    public string GetCurrentPlayerName(){
        return currentPlayer.Name;
    }

    public void HandleDiceResult(int result, Label nameLabel){
		handleSpecialResults(result);
        currentPlayer.addScore(result);
			if (currentPlayer.isFinished) {
				playerIndex++;
				string playerThrowName = currentPlayer.Name;
				if (playerIndex < players.Count) {
					currentPlayer = players[playerIndex];
					nameLabel.Text = "Mooie worp, "+ playerThrowName +"!\nDe volgende speler is:\n"+currentPlayer.Name;
				}
				else {
					nameLabel.Text = "Mooie worp, "+ playerThrowName +"!\nDe ronde is voorbij";
					DetermineLoser(nameLabel);
				}
			}
    }
	
	private void handleSpecialResults(int result){
		if (result == 21) {
			amountMexx++;
		}
		else if (result == 31){
			_signals.EmitSignal(nameof(_signals.Penalty), 1, currentPlayer.Name, true, false);
		}
		else if (result == 100){
			if (currentKnight == currentPlayer && mode == "hardcore_mexen") knightStrenght++;
			else currentKnight = currentPlayer;
		}
		else if (result == 600){
			_signals.EmitSignal(nameof(_signals.Penalty), 1, "all", true, false);
		}
		else if (result % 100 == 0){
			int penalty = 0;
			if (mode == "mexen") penalty = 1;
			else if (mode == "hardcore_mexen") penalty = result / 100;
			if (currentKnight != null && currentPlayer == currentKnight) {
				_signals.EmitSignal(nameof(_signals.Penalty), penalty*knightStrenght, currentKnight.Name, true, true);
			}
			else _signals.EmitSignal(nameof(_signals.Penalty), penalty*knightStrenght, currentKnight.Name, false, true);
		}
	}

    private List<Player> getLosers(){
		List<Player> losers = new List<Player>();
		int lowestScore = int.MaxValue;
		foreach(Player player in players){
			if(player.Score < lowestScore && player.Score != 21){
				lowestScore = player.Score;
			}
		}
		foreach(Player player in players){
			if(player.Score == lowestScore){
				losers.Add(player);
			}
		}
		return losers;
	}

	private int CalculatePenalty(){
		return penaltyPoints * (amountMexx+1);
	}

	private void RefreshRound(Player loser){
		playerIndex = 0;
		foreach(Player player in players){
			player.Reset();
		}
		players.Remove(loser);
		UtilsManager.ShuffleList(players);
		players.Insert(0, loser);
		currentPlayer = loser;
	}


    private void DetermineLoser(Label nameLabel){
		List<Player> losers = getLosers();
        int penalty = CalculatePenalty();
		if (losers.Count > 1){
			_signals.EmitSignal(nameof(_signals.DetermineLoser), penalty);
		}
		else {
            Player loser = losers[0];
            RefreshRound(loser);
			_signals.EmitSignal(nameof(_signals.Penalty), penalty, loser.Name);
			nameLabel.Text = "Jammer "+ loser.Name +"!\nJe mag nu wel\nals eerste werpen!";
		}
	}
}