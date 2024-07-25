using Godot;
using System;

public partial class ChooseGame : CanvasLayer
{
	private SceneSwitcher _sceneSwitcher;
	public override void _Ready()
	{
		_sceneSwitcher = GetNode<SceneSwitcher>("/root/SceneSwitcher");
	}

	private void OnMexenPressed(){
		Node popup = NodeCreator.getMexenInfo();
		AddChild(popup);
	}

	private void OnHardcoreMexenPressed(){
		Node popup = NodeCreator.getHardcoreMexenInfo();
		AddChild(popup);
	}

	private void OnBackPressed(){
		_sceneSwitcher.SwitchScene("res://Windows/SelectPlayerNames.tscn");
	}
}
