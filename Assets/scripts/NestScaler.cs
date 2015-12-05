using UnityEngine;
using System.Collections;

public class NestScaler : MonoBehaviour
{
	public int Resources;

	protected Vector3 m_OriginalScale;

	public void Start()
	{
		m_OriginalScale = this.transform.localScale;
	}

	public void Update()
	{
		this.transform.localScale = new Vector3(Mathf.Clamp(m_OriginalScale.x + 0.1f*m_OriginalScale.x*Resources, m_OriginalScale.x, 10.0f*m_OriginalScale.x), m_OriginalScale.y + 0.05f*Resources, Mathf.Clamp(m_OriginalScale.z + 0.1f*m_OriginalScale.z*Resources, m_OriginalScale.z, 10.0f*m_OriginalScale.z));
	}
}
