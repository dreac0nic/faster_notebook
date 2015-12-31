using System.Collections;
﻿using UnityEngine;

public class SimpleSteerable : MonoBehaviour
{
	/*
	public Vector2 Velocity { get { return m_Velocity; } }
	public Vector2 Heading { get { return m_Heading; } }
	public Vector2 Right { get { return m_Right; } }
	public Vector2 Position { get { return new Vector2(this.transform.position.x, this.transform.position.z); } }

	public float Mass = 1.0f;
	public float MaximumSpeed = 20.0f;
	public float MaximumForce = 2.0f;
	public float MaximumTurnRate = 0.02f;

	protected Vector2 m_Velocity;
	protected Vector2 m_Heading;
	protected Vector2 m_Right;

	public void Update()
	{
		Vector2 steering_force = Vector2.zero; // TODO: Calculate steering force
		Vector2 acceleration = steering_force/Mass;

		m_Velocity += acceleration*Time.deltaTime;

		m_Velocity = Vector2.ClampMagnitude(m_Velocity, MaximumSpeed);
		this.transform.position += new Vector3(m_Velocity.x, this.transform.position.y, m_Velocity.y)*Time.deltaTime;

		if(m_Velocity.sqrMagnitude > 0.00001f) {
			m_Heading = m_Velocity.normalized;
			m_Right = Vector3.Cross(m_Heading, Vector3.up).normalized;

			this.transform.rotation = Quaternion.LookRotation(m_Heading);
		}
	}
}

public class SteeringBehaviors
{
	public float WanderRadius = 4.0f;
	public float WanderDistance = 2.0f;
	public float WanderJitter = 1.5f;

	protected Vector2 m_WanderTarget;
	protected SimpleSteerable m_Steerable;

	public SteeringBehaviors(SimpleSteerable agent)
	{
		m_WanderTarget = new Vector2(1.0f, 0.0f);
		m_Steerable = agent;
	}

	public Vector2 Seek(Vector2 target)
	{
		Vector2 desired_velocity = m_Steerable.MaximumSpeed*(target - m_Steerable.Position).normalized;

		return (desired_velocity - m_Steerable.Velocity);
	}

	public Vector2 Flee(Vector2 target, double flee_distance = 100.0)
	{
		Vector2 desired_velocity = m_Steerable.MaximumSpeed*(m_Steerable.Position - target).normalized;
		flee_distance *= flee_distance;

		if((m_Steerable.Position - target).sqrMagnitude > flee_distance) {
			return Vector2.zero;
		}

		return (desired_velocity - m_Steerable.Velocity);
	}

	public Vector2 Arrive(Vector2 target, float stopping_speed = 0.4f)
	{
		Vector2 to_target = target - m_Steerable.Position;

		if(to_target.sqrMagnitude > 0.0) {
			float speed = to_target.magnitude/stopping_speed;

			return (speed*to_target/speed - m_Steerable.Velocity);
		}

		return Vector2.zero;
	}

	public Vector2 Pursuit(SteeringBehaviors evader)
	{
		Vector2 to_target = evader.Position - m_Steerable.Position;

		float relative_heading = Vector2.Dot(m_Steerable.Heading, evader.Heading);

		if((Vector2.Dot(to_target, m_Steerable.Heading) > 0) && (relative_heading < -0.95f)) {
			return Seek(evader.Position);
		}

		float look_ahead_time = to_target.magnitude/(m_Steerable.MaximumSpeed + evader.Velocity.magnitude); // TODO: Possibly include turn around time?

		return Seek(evader.Position + evader.Velocity*look_ahead_time);
	}

	public Vector2 Evade(SteeringBehaviors pursuer)
	{
		Vector2 to_pursuer = pursuer.Position - m_Steerable.Position;

		float look_head_time = to_pursuer.magnitude/(m_Steerable.MaximumSpeed + pursuer.Velocity.magnitude);

		return Flee(pursuer.Position + pursuer.Velocity*look_ahead_time);
	}

	public Vector2 Wander()
	{
		Vector3 target;

		// Calculate wandering.
		m_WanderTarget += new Vector2(WanderJitter*Random.Range(-1, 1), WanderJitter*Random.Range(-1, 1));
		m_WanderTarget.Normalize();

		m_WanderTarget *= WanderRadius;
		target = new Vector3(WanderDistance, 0.0f, 0.0f) + new Vector3(m_WanderTarget.x, 0.0f, m_WanderTarget.y);

		target = m_Steerable.transform.TransformDirection(target);

		return new Vector2(target.x, target.z) - m_Steerable.Position;
	}
	//*/
}
