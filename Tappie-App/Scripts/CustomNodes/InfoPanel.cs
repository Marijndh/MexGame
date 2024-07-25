using Godot;
using System;

public partial class InfoPanel : CanvasLayer
{
	private SceneSwitcher _sceneSwitcher;
	public override void _Ready()
	{
		_sceneSwitcher = GetNode<SceneSwitcher>("/root/SceneSwitcher");
	}

	private void OnContinueButtonPressed(){
		if (Global.games.Contains(Name)) { 
			Global.current_game = Name; 
			_sceneSwitcher.SwitchToGame();
		}

	}	

	private void OnCloseButtonPressed(){
		QueueFree();
	}
}
