using Godot;
using System;
using System.Collections.Generic;

public partial class MexGameManager : Node{
	private Signals _signals;
    public int playerIndex;
    private MexPlayer currentKnight = null; 

	private int knightStrenght = 1;

	private MexPlayer currentPlayer = null;

	private List<MexPlayer> players;

	private int amountMexx = 0;

	private int penaltyPoints;

	private string mode;



    public MexGameManager(string mode, Signals signals){
		this.mode = mode;
		_signals = signals;
		if (mode == "hardcore_mexen"){
			penaltyPoints = 3;
		}
		else if (mode == "mexen"){
			penaltyPoints = 2;
		}
		// Instantiate important player objects
		players = new List<MexPlayer>();
		foreach(string name in Global.playerNames){
			players.Add(new MexPlayer(name));
		}
        // Shuffle player list
		GameHelper.ShuffleList(players);
		currentPlayer = players[0];
	}

    public string GetCurrentPlayerName(){
        return currentPlayer.name;
    }

    public void HandleDiceResult(int result, Label nameLabel){
		handleSpecialResults(result);
        currentPlayer.addScore(result);
			if (currentPlayer.isFinished) {
				playerIndex++;
				string playerThrowName = currentPlayer.name;
				if (playerIndex < players.Count) {
					currentPlayer = players[playerIndex];
					nameLabel.Text = "Mooie worp, "+ playerThrowName +"!\nDe volgende speler is:\n"+currentPlayer.name;
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
			_signals.EmitSignal(nameof(_signals.Penalty), 1, currentPlayer.name, true, false);
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
				_signals.EmitSignal(nameof(_signals.Penalty), penalty*knightStrenght, currentKnight.name, true, true);
			}
			else _signals.EmitSignal(nameof(_signals.Penalty), penalty*knightStrenght, currentKnight.name, false, true);
		}
	}

    private List<MexPlayer> getLosers(){
		List<MexPlayer> losers = new List<MexPlayer>();
		int lowestScore = int.MaxValue;
		foreach(MexPlayer player in players){
			if(player.score < lowestScore && player.score != 21){
				lowestScore = player.score;
			}
		}
		foreach(MexPlayer player in players){
			if(player.score == lowestScore){
				losers.Add(player);
			}
		}
		return losers;
	}

	private int CalculatePenalty(){
		return penaltyPoints * (amountMexx+1);
	}

	private void RefreshRound(MexPlayer loser){
		playerIndex = 0;
		foreach(MexPlayer player in players){
			player.Reset();
		}
		players.Remove(loser);
		GameHelper.ShuffleList(players);
		players.Insert(0, loser);
		currentPlayer = loser;
	}


    private void DetermineLoser(Label nameLabel){
		List<MexPlayer> losers = getLosers();
        int penalty = CalculatePenalty();
		if (losers.Count > 1){
			_signals.EmitSignal(nameof(_signals.DetermineLoser), penalty);
		}
		else {
            MexPlayer loser = losers[0];
            RefreshRound(loser);
			_signals.EmitSignal(nameof(_signals.Penalty), penalty, loser.name);
			nameLabel.Text = "Jammer "+ loser.name +"!\nJe mag nu wel\nals eerste werpen!";
		}
	}
}