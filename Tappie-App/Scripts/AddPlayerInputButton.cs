using Godot;
using System;

public partial class AddPlayerInputButton : TextureButton
{
	private Signals _signals;

    public override void _Ready()
    {
        _signals = GetNode<Signals>("/root/Signals");
    }

    private void OnPressed()
	{
		_signals.EmitSignal(nameof(Signals.AddPLayerInput));
	}
}
