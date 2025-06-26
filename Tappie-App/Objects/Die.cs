using Godot;
using System;
using System.Collections.Generic;

public partial class Die : RigidBody3D
{
	private int value;
	public int Value => value;

	private EventManager _eventManager;
	private int _rollStrength = 20;

	private bool _isRolling = false;
	public bool IsRolling => _isRolling;

	private List<DieRayCast> _rays = new List<DieRayCast>();

	private float _rollingTimeout = 2.5f; // Seconds
	private float _rollingTime = 0f;

	public override void _Ready()
	{
		_eventManager = GetNode<EventManager>("/root/EventManager");
		Node3D rayParent = GetNode<Node3D>("RayCasts");
		foreach (Node child in rayParent.GetChildren())
		{
			if (child is DieRayCast raycast)
			{
				_rays.Add(raycast);
			}
		}
		SleepingStateChanged += OnSleepingStateChanged;
	}

	public void Reset()
	{
		Sleeping = false;
		Freeze = false;
		LinearVelocity = Vector3.Zero;
		AngularVelocity = Vector3.Zero;
		_isRolling = true;
		_rollingTime = 0f;

		Transform3D t = Transform;
		t.Basis = new Basis(Vector3.Right, GD.Randf() * Mathf.Tau) *
				  new Basis(Vector3.Up, GD.Randf() * Mathf.Tau) *
				  new Basis(Vector3.Forward, GD.Randf() * Mathf.Tau);
		Transform = t;
	}

	public void Throw(Vector3 direction, float strength)
	{
		if (direction == Vector3.Zero)
			direction = Vector3.Forward;

		direction = direction.Normalized();

		Vector3 impulse = direction * strength;
		ApplyCentralImpulse(impulse);

		Vector3 spinAxis = new Vector3(1, 1, 1).Normalized();
		ApplyTorqueImpulse(spinAxis * strength * 5f);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isRolling)
		{
			bool almostStill = LinearVelocity.Length() < 0.5f && AngularVelocity.Length() < 0.5f;
			if (almostStill)
				_rollingTime += (float)delta;
			else
				_rollingTime = 0f;

			if (_rollingTime > _rollingTimeout)
			{
				EvaluateResult();
				_rollingTime = 0f;
			}
		}
	}

	public void SnapRotation()
	{
		Vector3 rotationDegrees = RotationDegrees;
		float SnapAngle = 90.0f;

		if (Value == 6)
			rotationDegrees.Y = 90;
		else
			rotationDegrees.Y = 0;

		rotationDegrees.X = Mathf.Round(rotationDegrees.X / SnapAngle) * SnapAngle;
		rotationDegrees.Z = Mathf.Round(rotationDegrees.Z / SnapAngle) * SnapAngle;

		RotationDegrees = rotationDegrees;
	}

	private void OnSleepingStateChanged()
	{
		if (Sleeping && _isRolling)
		{
			EvaluateResult();
		}
	}

	private void EvaluateResult()
	{
		if (!_isRolling)
			return;

		DieRayCast closestRay = null;
		float minDistance = float.MaxValue;

		foreach (DieRayCast ray in _rays)
		{
			float yOrigin = ray.GlobalTransform.Origin.Y;
			float distanceToPlane = Mathf.Abs(yOrigin);

			if (distanceToPlane < minDistance)
			{
				minDistance = distanceToPlane;
				closestRay = ray;
			}
		}

		if (closestRay != null)
		{
			value = closestRay.opposite_side;
			_isRolling = false;
			_eventManager.EmitSignal(nameof(_eventManager.RollFinished));
		}
	}
}
