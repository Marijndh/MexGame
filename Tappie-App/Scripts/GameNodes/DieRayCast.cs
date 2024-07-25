using Godot;
using System;

public partial class DieRayCast : RayCast3D
{
	[Export]
	public int opposite_side;
	public override void _Ready()
	{
		if (Owner is CollisionObject3D collisionObject)
		{
			AddException(collisionObject);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
