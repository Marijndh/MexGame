using Godot;

public partial class SceneManager : Node
{
    private Node currentScene = null;

    public override void _Ready()
    {
		Window root = GetTree().Root;
		currentScene = root.GetChild(root.GetChildCount() - 1);
	}
	public void SwitchScene(string scene)
	{
		CallDeferred(nameof(DeferredSwitchScene), "res://Scenes/" + scene + ".tscn");
	}

	private void DeferredSwitchScene(string resPath)
	{
		currentScene.QueueFree();
		PackedScene packedScene = (PackedScene)GD.Load(resPath);
		currentScene = packedScene.Instantiate();

		GetTree().Root.AddChild(currentScene);
		GetTree().CurrentScene = currentScene;
	}
}
