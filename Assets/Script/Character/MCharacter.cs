using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCharacter : MonoBehaviour {

	static public MCharacter Instance{ get { return m_Instance; }}
	static MCharacter m_Instance;
	MCharacter() {
		if ( m_Instance == null ) m_Instance = this;
	}

	Vector3 lastPosition;
	public float deltaDistance;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Shader.SetGlobalVector("MC_POS" , transform.position );

		deltaDistance = (transform.position - lastPosition).magnitude;
		lastPosition = transform.position;
	}
}
