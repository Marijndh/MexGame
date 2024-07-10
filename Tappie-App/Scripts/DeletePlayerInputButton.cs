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
		_signals.EmitSignal(nameof(Signals.DeletePlayerInput), GetParent<Node>());
	}
}
