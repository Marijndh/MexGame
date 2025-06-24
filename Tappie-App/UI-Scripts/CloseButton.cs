using Godot;
using System;

public partial class CloseButton : TextureButton
{
		private void OnPressed() {
		QueueFree();
	}
}
