using System.Collections.Generic;
using Godot;

public partial class SelectPlayers : CanvasLayer
{
	private EventManager _signals;
	private SceneManager _sceneSwitcher;
	private VBoxContainer _playerInputs;
	private List<string> playerNames; 
	public override void _Ready()
	{
		_signals = GetNode<EventManager>("/root/EventManager");

		_sceneSwitcher = GetNode<SceneManager>("/root/SceneManager");

		_playerInputs = GetNode<VBoxContainer>("PlayerInputs");

		if (playerNames.Count > 0) {
			foreach (string playerName in playerNames) {GD.Print("" + playerName); }
			_playerInputs.GetChild(0).GetChild(0).GetNode<LineEdit>("LineEdit").Text = playerNames[0];
			AddDeleteButton(0);
			for (int i = 2; i <= playerNames.Count; i++) {
				Node nameInput = NodeCreator.CreateNode(
					"NameInput",
					new Dictionary<string, object>
					{
						{ "LineEdit", playerNames[i - 1] } 
					},
					nodeName: i.ToString()
				);
				_playerInputs.AddChild(nameInput);
				_playerInputs.MoveChild(nameInput, i-1);
				AddDeleteButton(i-1);
			}
		}
	}

	private void SelectFirstEmpty(){
		List<int> emptyInputs = GetEmptyNameInputs();
		Node child = _playerInputs.GetChild(emptyInputs[0]);
		if(child.GetChild(0).HasNode("LineEdit")) {
           	LineEdit lineEdit = child.GetChild(0).GetNode<LineEdit>("LineEdit");
           	lineEdit.GrabFocus();
        }
	}

	private void AddPlayerName(string name, int index) {
		if (index >= 0 && index < playerNames.Count) playerNames[index] = name;
		else playerNames.Add(name);
	}

	private List<int> GetEmptyNameInputs(){ 
		List<int> result = new List<int>();
		int amount_players = _playerInputs.GetChildCount();
		for (int i = 0; i < amount_players; i++) {
			Node child = _playerInputs.GetChild(i);
			if (child != null && child.Name != "AddPlayerInputButton") {
				string name = child.GetChild(0).GetNode<LineEdit>("LineEdit").Text;
				if (name == "") {
					result.Add(i);
				}
			}
		}
		return result;
	}

	private void AddPlayerNames(){ 
		int amount_players = _playerInputs.GetChildCount();
		for (int i = 0; i < amount_players; i++) {
			Node child = _playerInputs.GetChild(i);
			if (child != null && child.Name != "AddPlayerInputButton") {
				string name = child.GetChild(0).GetNode<LineEdit>("LineEdit").Text;
				if (name != "") {
					AddPlayerName(name, i);
				}
			}
		}
	}
	private void OnContinuePressed(){
		int amount_players= _playerInputs.GetChildCount();
		if (amount_players == 2){
			Node popUp = NodeCreator.CreateNode(
				"PopUp",
				new Dictionary<string, object>
				{
					{ "Text", "In je eentje spelen \n word lastig denk ik" }
				}
			);
			AddChild(popUp);
			return;
		}
		List<int> emptyInputs = GetEmptyNameInputs();
		if (emptyInputs.Count > 0){
			Node popUp = NodeCreator.CreateNode(
				"PopUp",
				new Dictionary<string, object>
				{
					{ "Text", "Zal je niet iedereen ff \n een naam geven?" }
				}
			);
			AddChild(popUp);
		}
		else {
			AddPlayerNames();
			_sceneSwitcher.SwitchScene("Play", playerNames);
		}
		
	}

	private void AddDeleteButton(int index){
		Node deleteButton = NodeCreator.CreateNode("DeletePlayerInputButton");
		_playerInputs.GetChild(index).AddChild(deleteButton);
	}

	private void AddPLayerInput(){
		if (IsInstanceValid(_playerInputs) && _playerInputs.GetChildCount() <= 8) {
			int index = _playerInputs.GetChildCount();
			if (index == 2) AddDeleteButton(0);
			Node nameInput = NodeCreator.CreateNode(
				"NameInput.tscn",
				new Dictionary<string, object>
				{
					{ "LineEdit", $"Player {index}" }
				},
				index.ToString()
			);
			_playerInputs.AddChild(nameInput);
			_playerInputs.MoveChild(nameInput, index-1);
			AddDeleteButton(index-1);
			if (nameInput.GetChild(0).HasNode("LineEdit")) {
            	LineEdit lineEdit = nameInput.GetChild(0).GetNode<LineEdit>("LineEdit");
            	lineEdit.GrabFocus();
        	}
			// If limit is reached, remove add button
			if (index == 8) {
				Node addbutton = _playerInputs.GetChild(8);
				_playerInputs.RemoveChild(addbutton);
				AddDeleteButton(index-1);
			}
			
		}
	} 

	private void RemoveDeleteButton(int index){
		Node parent = _playerInputs.GetChild(index);
		int count = parent.GetChildCount();
		for (int i = 0; i < count; i++) {
			Node child = parent.GetChild(i);
			if (child.Name == "TextureButton") {
				parent.RemoveChild(child);
				break;
			}
		}
	}

	private void DeletePlayerInput(Node node){
		if (!IsInstanceValid(_playerInputs)){ return; }
		
		_playerInputs.RemoveChild(node);
		
		int child_count = _playerInputs.GetChildCount();
		// Add addplayerbutton if children < 8 and it is not already there
		if (child_count < 8){
			bool addButtonVisible = false;
			for (int i = 0; i < child_count; i++) {
				if (_playerInputs.GetChild(i).Name == "AddPlayerInputButton"){
					addButtonVisible = true;
				}
			}
			if (!addButtonVisible){
				Node addButton = NodeCreator.CreateNode("AddPlayerInputButton");
				_playerInputs.AddChild(addButton);
			}
		}
		if (_playerInputs.GetChildCount() == 2) {
			RemoveDeleteButton(0);	
		}
			
	}
}
