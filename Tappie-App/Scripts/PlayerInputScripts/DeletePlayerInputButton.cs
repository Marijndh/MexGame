using Godot;
using System;

public partial class DeletePlayerInputButton : TextureButton
{
	private Signals _signals;

    public override void _Ready()
    {
        _signals = GetNode<Signals>("/root/Signals");
    }

    private void OnPressed()
	{
        int index = GetParent().Name.ToString().ToInt();
        if (index >= 0 && index < Global.playerNames.Count && GetParent().GetChild(0).GetNode<LineEdit>("LineEdit").Text == Global.playerNames[index]) Global.playerNames.RemoveAt(index);
		_signals.EmitSignal(nameof(Signals.DeletePlayerInput), GetParent<Node>());
	}
}
