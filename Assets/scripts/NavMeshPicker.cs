using UnityEngine;
using System.Collections;

public class NavMeshPicker : MonoBehaviour
{
	public Camera PickerCamera;
	public string PickButton = "Fire1";

	protected Vector3 m_LastPickedPosition;
	protected NavMeshAgent m_NavAgent;

	public void Awake()
	{
		if(!PickerCamera) {
			if(Debug.isDebugBuild) {
				Debug.Log(this.transform.name + " [NavMeshPicker]: No camera assigned, using main camera.");
			}

			PickerCamera = Camera.main;
		}

		m_NavAgent = GetComponent<NavMeshAgent>();
	}

	public void Update()
	{
		RaycastHit hit_info;

		if(m_NavAgent && PickerCamera) {
			if(Input.GetButton(PickButton) && Physics.Raycast(PickerCamera.ScreenPointToRay(Input.mousePosition), out hit_info)) {
				m_LastPickedPosition = hit_info.point;
				m_NavAgent.SetDestination(m_LastPickedPosition);
			}
		}
	}
}
