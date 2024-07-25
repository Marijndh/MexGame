using Godot;
using System;
using System.Collections.Generic;

public partial class Die : RigidBody3D
{
	public int die_index;
	public int value;
	private Signals _signals;
	private int _rollStrength = 20;

	public bool isRolling = false;

	private List<DieRayCast> _rays = new List<DieRayCast>();

	public override void _Ready()
	{
		die_index = Int32.Parse(Name);
		_signals = GetNode<Signals>("/root/Signals");
		Node3D rayParent = GetNode<Node3D>("RayCasts");
			foreach (Node child in rayParent.GetChildren())
			{
				DieRayCast raycast = child as DieRayCast;
				if (raycast != null)
				{
					_rays.Add(raycast);
				}
			}
	}

    public void Roll(){
		// Reset State
		Sleeping = false;
		Freeze = false;
		Transform3D transform = Transform;
		transform.Origin = Position;
		LinearVelocity = Vector3.Zero;
		AngularVelocity = Vector3.Zero;

		// Random rotation to simulate initial random orientation
		transform.Basis = new Basis(Vector3.Right, (float)GD.RandRange(0, 2 * Mathf.Pi)) * transform.Basis;
		transform.Basis = new Basis(Vector3.Up, (float)GD.RandRange(0, 2 * Mathf.Pi)) * transform.Basis;
		transform.Basis = new Basis(Vector3.Forward, (float)GD.RandRange(0, 2 * Mathf.Pi)) * transform.Basis;
		Transform = transform;

		// Define the target position towards which stones are thrown
		Vector3 targetPosition = new Vector3(0, 0.2f, 0);

		// Add some randomization to the throw direction
		Vector3 randomOffset = new Vector3((float)GD.RandRange(-1, 1), 0, (float)GD.RandRange(-1, 1));
		Vector3 throwDirection = (targetPosition - Position + randomOffset).Normalized();

		// Increase angular velocity for more rolling effect
		Vector3 spinAxis = new Vector3((float)GD.RandRange(-1, 1), (float)GD.RandRange(-1, 1), (float)GD.RandRange(-1, 1)).Normalized();
		float spinAmount = (float)GD.RandRange(-2 * _rollStrength, 2 * _rollStrength); // Increase range for more spin
		Vector3 angularVelocity = spinAxis * spinAmount;
		this.AngularVelocity = angularVelocity;

		// Apply an impulse in the throw direction
		float throwStrength = (float)GD.RandRange(0.8 * _rollStrength, 1.2 * _rollStrength); // Adjust strength
		ApplyCentralImpulse(throwDirection * throwStrength);

		// Set rolling state
		isRolling = true;

	}

	public void OnSleepingStateChanged() {
		if (Sleeping){
			bool landedOnSide = false;
			foreach (DieRayCast ray in _rays){
				if (ray.IsColliding()){
					value = ray.opposite_side;
					isRolling = false;
					landedOnSide = true;
				}
			}
			if(!landedOnSide){
				DieRayCast closestRay = null;
				float minDistance = float.MaxValue;

				foreach (DieRayCast ray in _rays)
				{
					// Get the y-coordinate of the ray's origin
					float yOrigin = ray.GlobalTransform.Origin.Y;

					// Calculate the distance to the y = 0 plane
					float distanceToPlane = Mathf.Abs(yOrigin);

					// Check if this ray is closer to the plane than the previous closest ray
					if (distanceToPlane < minDistance)
					{
						minDistance = distanceToPlane;
						closestRay = ray;
					}
				}
				if (closestRay != null){
					value = closestRay.opposite_side;
				}
			}
		}
	}
}
