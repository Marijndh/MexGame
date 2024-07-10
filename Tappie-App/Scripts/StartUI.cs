using Godot;

public partial class StartUI : CanvasLayer
{
	private SceneSwitcher _switcher;

    public override void _Ready()
    {
        _switcher = GetNode<SceneSwitcher>("/root/SceneSwitcher");
    }

    private void OnStartButtonPressed()
	{
		_switcher.SwitchScene("res://Windows/SelectPlayerNames.tscn");
	}

}
