using Godot;
using System;
using System.Runtime.InteropServices;

public partial class SelectPlayerNames : CanvasLayer
{
	private Signals _signals;
	private SceneSwitcher _sceneSwitcher;
	private VBoxContainer _playerInputs;
	public override void _Ready()
	{
		_signals = GetNode<Signals>("/root/Signals");
		_signals.AddPLayerInput += AddPLayerInput;
		_signals.DeletePlayerInput += DeletePlayerInput;

		_playerInputs = GetNode<VBoxContainer>("PlayerInputs");
	}

	private void OnContinuePressed(){
		//Save all player names in the global variable
		//TODO add correct path
		_sceneSwitcher.SwitchScene("");
	}

	private void AddDeleteButton(int index){
				// Load the custom scene
        PackedScene myCustomScene = (PackedScene)ResourceLoader.Load("res://Windows/DeletePlayerInputButton.tscn");
        
        // Instance the custom scene
        Node myCustomObject = myCustomScene.Instantiate();

		_playerInputs.GetChild(index).AddChild(myCustomObject);
	}

	private void AddPLayerInput(){
		if (_playerInputs.GetChildCount() <= 8) {
		// Load the custom scene
        PackedScene scene = (PackedScene)ResourceLoader.Load("res://Windows/NameInput.tscn");
        
        // Instance the custom scene
        Node nameInput = scene.Instantiate();
        
        // Add the custom object at the second to last position
        int index = _playerInputs.GetChildCount();
		nameInput.Name = "Speler " + index;
        _playerInputs.AddChild(nameInput);
        _playerInputs.MoveChild(nameInput, index-1);
		AddDeleteButton(index-2);
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
			GD.Print(child.Name);
			if (child.Name == "TextureButton") {
				parent.RemoveChild(child);
				break;
			}
		}
	}

	private void DeletePlayerInput(Node node){
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
				// Load the custom scene
        		PackedScene scene = (PackedScene)ResourceLoader.Load("res://Windows/AddPlayerInputButton.tscn");
        
        		// Instance the custom scene
        		Node addButton = scene.Instantiate();

				 _playerInputs.AddChild(addButton);
			}
		}
		if (_playerInputs.GetChildCount() == 2) {
			RemoveDeleteButton(0);	
		}
			
	}
}
