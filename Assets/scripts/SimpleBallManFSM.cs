using UnityEngine;
using System.Collections;

public class SimpleBallManFSM : MonoBehaviour
{
	public abstract class State
	{
		public abstract void Enter(SimpleBallManFSM agent);
		public abstract void Run(SimpleBallManFSM agent);
		public abstract void Leave(SimpleBallManFSM agent);
	}

	public class GatherCrystalsState : State
	{
		protected static GatherCrystalsState instance;

		public static GatherCrystalsState Instance {
			get {
				if(instance == null) {
					instance = new GatherCrystalsState();
				}

				return instance;
			}
		}

		protected GatherCrystalsState() {}

		public override void Enter(SimpleBallManFSM agent)
		{
			Debug.Log(agent.transform.name + " is going to gather crystals!");
		}

		public override void Run(SimpleBallManFSM agent)
		{
			Collider[] entities = Physics.OverlapSphere(agent.transform.position, agent.InteractionRadius, agent.InteractableLayers);

			foreach(Collider entity in entities) {
				Transform crystal = entity.transform;

				while(crystal != null && crystal.tag != "Crystal") {
					crystal = crystal.parent;
				}

				if(crystal != null) {
					agent.Hunger -= 1 + (int)Mathf.Floor(Random.Range(0, 2));
					agent.Exhaustion -= 3 + (int)Mathf.Floor(Random.Range(0, 2));
					agent.CrystalsHeld += 1 + (int)Mathf.Floor(Random.Range(0, 1));

					Debug.Log(agent.transform.name + " gathered a crystal!");

					if(agent.CrystalsHeld >= 25 || agent.Exhaustion < 5) {
						agent.ChangeState(ReturnToNestState.Instance);
					} else if(agent.Hunger < 3) {
						agent.ChangeState(EatFoodState.Instance);
					}

					return;
				}
			}

			agent.ChangeState(FindCrystalState.Instance);

			Debug.Log(agent.transform.name + " couldn't find a crystal.");
		}

		public override void Leave(SimpleBallManFSM agent)
		{
			Debug.Log(agent.transform.name + " stopped gathering crystals.");
		}
	}

	public class FindCrystalState : State
	{
		protected static FindCrystalState instance;

		public static FindCrystalState Instance {
			get {
				if(instance == null) {
					instance = new FindCrystalState();
				}

				return instance;
			}
		}

		protected FindCrystalState() {}

		public override void Enter(SimpleBallManFSM agent)
		{
			NavMeshAgent nav_agent = agent.GetComponent<NavMeshAgent>();

			if(nav_agent) {
				nav_agent.ResetPath();
			}

			Debug.Log(agent.transform.name + " is looking for crystals!");
		}

		public override void Run(SimpleBallManFSM agent)
		{
			NavMeshAgent nav_agent = agent.GetComponent<NavMeshAgent>();
			Collider[] entities = Physics.OverlapSphere(agent.transform.position, agent.InteractionRadius, agent.InteractableLayers);
			Transform crystal = null;

			foreach(Collider entity in entities) {
				crystal = entity.transform;

				while(crystal != null && crystal.tag != "Crystal") {
					crystal = crystal.parent;
				}

				if(crystal != null) {
					break;
				}
			}

			if(crystal != null) {
				Debug.Log(agent.transform.name + " found a crystal!");

				agent.ChangeState(GatherCrystalsState.Instance);

				return;
			}

			if(nav_agent) {
				if(!nav_agent.hasPath) {
					entities = Physics.OverlapSphere(agent.transform.position, agent.SearchRadius, agent.InteractableLayers);

					foreach(Collider entity in entities) {
						crystal = entity.transform;

						while(crystal != null && crystal.tag != "Crystal") {
							crystal = crystal.parent;
						}

						if(crystal != null) {
							break;
						}
					}

					if(crystal != null) {
						Debug.Log(agent.transform.name + " sees a crystal (" + crystal.name + ") and is heading toward it!");

						nav_agent.SetDestination(crystal.position);
					}
				} else {
					Debug.Log(agent.transform.name + " is heading towards a crystal!");
				}
			} else {
				Debug.LogError(agent.transform.name + " [FindCrystalState]: No NavMeshAgent found!");
			}
		}

		public override void Leave(SimpleBallManFSM agent)
		{
			NavMeshAgent nav_agent = agent.GetComponent<NavMeshAgent>();

			if(nav_agent) {
				nav_agent.ResetPath();
			}

			Debug.Log(agent.transform.name + " has stopped looking for crystals.");
		}
	}

	public class EatFoodState : State
	{
		protected static EatFoodState instance;

		public static EatFoodState Instance {
			get {
				if(instance == null) {
					instance = new EatFoodState();
				}

				return instance;
			}
		}

		protected EatFoodState() {}

		public override void Enter(SimpleBallManFSM agent)
		{
			Debug.Log(agent.transform.name + " is going to eat food!");
		}

		public override void Run(SimpleBallManFSM agent)
		{
			Collider[] entities = Physics.OverlapSphere(agent.transform.position, agent.InteractionRadius, agent.InteractableLayers);

			foreach(Collider entity in entities) {
				Transform food = entity.transform;

				while(food != null && food.tag != "Food") {
					food = food.parent;
				}

				if(food != null) {
					++agent.Hunger;
					++agent.Exhaustion;

					Debug.Log(agent.transform.name + " ate some food!");

					if(agent.Hunger >= 10) {
						agent.ChangeState(FindCrystalState.Instance);
					}

					return;
				}
			}

			agent.ChangeState(FindFoodState.Instance);
		}

		public override void Leave(SimpleBallManFSM agent)
		{
			Debug.Log(agent.transform.name + " has stopped eating food.");
		}
	}

	public class FindFoodState : State
	{
		protected static FindFoodState instance;

		public static FindFoodState Instance {
			get {
				if(instance == null) {
					instance = new FindFoodState();
				}

				return instance;
			}
		}

		protected FindFoodState() {}

		public override void Enter(SimpleBallManFSM agent)
		{
			NavMeshAgent nav_agent = agent.GetComponent<NavMeshAgent>();

			if(nav_agent) {
				Collider[] entities = Physics.OverlapSphere(agent.transform.position, agent.SearchRadius, agent.InteractableLayers);

				nav_agent.ResetPath();

				foreach(Collider entity in entities) {
					Transform food = entity.transform;

					while(food != null && food.tag != "Food") {
						food = food.parent;
					}

					if(food != null) {
						nav_agent.SetDestination(food.position);
						Debug.Log(agent.transform.name + " sees food (" + food.name + ") and is heading toward it!");

						return;
					}
				}

				Debug.Log(agent.transform.name + " doesn't see any food...");
			} else {
				Debug.LogError(agent.transform.name + " [FindFoodState]: No NavMeshAgent found!");
			}
		}

		public override void Run(SimpleBallManFSM agent)
		{
			NavMeshAgent nav_agent = agent.GetComponent<NavMeshAgent>();
			Collider[] entities = Physics.OverlapSphere(agent.transform.position, agent.InteractionRadius, agent.InteractableLayers);
			Transform food = null;

			foreach(Collider entity in entities) {
				food = entity.transform;

				while(food != null && food.tag != "Food") {
					food = food.parent;
				}

				if(food != null) {
					break;
				}
			}

			if(food != null) {
				Debug.Log(agent.transform.name + " found food!");

				if(nav_agent && nav_agent.hasPath) {
					nav_agent.Stop();
				}

				agent.ChangeState(EatFoodState.Instance);
			} else if(nav_agent != null && nav_agent.hasPath) {
				Debug.Log(agent.transform.name + " is heading towards food!");
			} else {
				Debug.LogError(agent.transform.name + " [FindFoodState]: No NavMeshAgent found!");
			}
		}

		public override void Leave(SimpleBallManFSM agent)
		{
			NavMeshAgent nav_agent = agent.GetComponent<NavMeshAgent>();

			if(nav_agent) {
				nav_agent.ResetPath();
			}

			Debug.Log(agent.transform.name + " has stopped looking for food.");
		}
	}

	public class ReturnToNestState : State
	{
		protected static ReturnToNestState instance;

		public static ReturnToNestState Instance {
			get {
				if(instance == null) {
					instance = new ReturnToNestState();
				}

				return instance;
			}
		}

		protected ReturnToNestState() {}

		public override void Enter(SimpleBallManFSM agent)
		{
			NavMeshAgent nav_agent = agent.GetComponent<NavMeshAgent>();

			if(nav_agent) {
				Collider[] entities = Physics.OverlapSphere(agent.transform.position, agent.SearchRadius, agent.InteractableLayers);

				nav_agent.ResetPath();

				foreach(Collider entity in entities) {
					NestScaler nest = entity.GetComponentInParent<NestScaler>();

					if(nest != null) {
						nav_agent.SetDestination(nest.transform.position);
						Debug.Log(agent.transform.name + " sees its nest (" + nest.transform.name + ") and is heading toward it!");

						return;
					}
				}

				Debug.Log(agent.transform.name + " doesn't see his home. :'(");
			} else {
				Debug.LogError(agent.transform.name + " [ReturnToNestState]: No NavMeshAgent found!");
			}
		}

		public override void Run(SimpleBallManFSM agent)
		{
			NavMeshAgent nav_agent = agent.GetComponent<NavMeshAgent>();
			Collider[] entities = Physics.OverlapSphere(agent.transform.position, agent.InteractionRadius, agent.InteractableLayers);

			foreach(Collider entity in entities) {
				NestScaler nest = entity.GetComponentInParent<NestScaler>();

				if(nest != null) {
					nest.Resources += agent.CrystalsHeld;
					agent.CrystalsHeld = 0;

					Debug.Log(agent.transform.name + " deposited his crystals in his nest.");

					if(agent.Exhaustion < 5) {
						agent.ChangeState(SleepState.Instance);
					} else if(agent.Hunger >= 8) {
						agent.ChangeState(FindCrystalState.Instance);
					} else {
						agent.ChangeState(FindFoodState.Instance);
					}

					return;
				}
			}

			if(nav_agent && nav_agent.hasPath) {
				Debug.Log(agent.transform.name + " is heading towards his nest!");
			} else {
				Debug.Log(agent.transform.name + " is lost...");
			}
		}

		public override void Leave(SimpleBallManFSM agent)
		{
			NavMeshAgent nav_agent = agent.GetComponent<NavMeshAgent>();

			if(nav_agent) {
				nav_agent.ResetPath();
			}

			Debug.Log(agent.transform.name + " is done going to his nest.");
		}
	}

	public class SleepState : State
	{
		protected static SleepState instance;

		public static SleepState Instance {
			get {
				if(instance == null) {
					instance = new SleepState();
				}

				return instance;
			}
		}

		protected SleepState() {}

		public override void Enter(SimpleBallManFSM agent)
		{
			Debug.Log(agent.transform.name + " is going to sleep.");
		}

		public override void Run(SimpleBallManFSM agent)
		{
			Collider[] entities = Physics.OverlapSphere(agent.transform.position, agent.InteractionRadius, agent.InteractableLayers);

			foreach(Collider entity in entities) {
				NestScaler nest = entity.GetComponentInParent<NestScaler>();

				if(nest != null) {
					if(agent.Exhaustion < 50) {
						if(agent.Hunger > 0) {
							--agent.Hunger;
						}

						agent.Exhaustion += 5;

						Debug.Log(agent.transform.name + " zzzzzzzz ...");
					} else if(agent.Hunger < 8){
						agent.ChangeState(FindFoodState.Instance);
					} else {
						agent.ChangeState(FindCrystalState.Instance);
					}

					return;
				}
			}

			agent.ChangeState(ReturnToNestState.Instance);
		}

		public override void Leave(SimpleBallManFSM agent)
		{
			Debug.Log(agent.transform.name + " has stopped sleeping.");
		}
	}

	public float TickRate = 10.0f;
	public float InteractionRadius = 2.5f;
	public float SearchRadius = 50.0f;
	public LayerMask InteractableLayers;
	public int Hunger = 10;
	public int Exhaustion = 50;
	public int CrystalsHeld;

	protected float m_NextActTime;
	protected State m_CurrentState;

	public void Start()
	{
		ChangeState(FindCrystalState.Instance);
	}

	public void Update()
	{
		if(Time.time >= m_NextActTime) {
			m_CurrentState.Run(this);
			m_NextActTime = Time.time + 1.0f/TickRate;
		}
	}

	public void ChangeState(State new_state)
	{
		if(m_CurrentState != null) {
			m_CurrentState.Leave(this);
		}

		m_CurrentState = new_state;

		if(m_CurrentState != null) {
			m_CurrentState.Enter(this);
		}
	}
}
