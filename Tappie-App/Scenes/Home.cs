using Godot;

public partial class StartUI : CanvasLayer
{
	private SceneManager _switcher;

    public override void _Ready()
    {
        _switcher = GetNode<SceneManager>("/root/SceneManager");
    }

    private void OnStartButtonPressed()
	{
		_switcher.SwitchScene("SelectPlayers");
	}

}
