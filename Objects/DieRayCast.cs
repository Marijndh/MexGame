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
}
