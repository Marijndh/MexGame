using Godot;
using System;
using System.Collections.Generic;

public partial class SceneManager : Node
{
    private Node currentScene = null;

    public override void _Ready()
    {
		Window root = GetTree().Root;
		currentScene = root.GetChild(root.GetChildCount() - 1);
	}
	public void SwitchScene(string scene, Godot.Collections.Dictionary variables = null)
	{
		CallDeferred(nameof(DeferredSwitchScene), "res://Scenes/" + scene + ".tscn", variables);
	}

	private void DeferredSwitchScene(string resPath, Godot.Collections.Dictionary variables = null)
	{
		currentScene.QueueFree();
		PackedScene packedScene = (PackedScene)GD.Load(resPath);
		currentScene = packedScene.Instantiate();
		GetTree().Root.AddChild(currentScene);
		GetTree().CurrentScene = currentScene;

		// Pass variables to the new scene if it has a method to receive them
		if (variables != null && currentScene.HasMethod("SetSceneVariables"))
		{
			currentScene.Call("SetSceneVariables", variables);
		}
	}

	internal void SwitchScene(string v, List<string> playerNames)
	{
		throw new NotImplementedException();
	}
}
