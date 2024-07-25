using Godot;

public partial class SceneSwitcher : Node
{
    private Node currentScene = null;

    public override void _Ready()
    {
        var root = GetTree().Root;
        currentScene = root.GetChild(root.GetChildCount() - 1);
    }
    public void SwitchToGame(){
        switch (Global.current_game){
            case "mexen" or "hardcore_mexen":
                SwitchScene("res://Windows/Mexen.tscn");
                break;
        }
    }
    public void SwitchScene(string resPath)
    {
        CallDeferred(nameof(DeferredSwitchScene), resPath);
    }

    private void DeferredSwitchScene(string resPath)
    {
        currentScene.QueueFree();
        var packedScene = (PackedScene)GD.Load(resPath);
        currentScene = packedScene.Instantiate();
        GetTree().Root.AddChild(currentScene);
        GetTree().CurrentScene = currentScene;
    }
}
