using Godot;
using System;

public partial class PopUp : CanvasLayer
{
	private Signals _signals;
	public override void _Ready()
    {
        _signals = GetNode<Signals>("/root/Signals");
    }
	private void OnCloseButtonPressed(){
		string text = GetChild(1).GetNode<Label>("Text").Text;
		QueueFree();
		if (text == "Zal je niet iedereen ff \n een naam geven?"){
			_signals.EmitSignal(nameof(Signals.SelectFirstEmpty));
		}
	}
}
