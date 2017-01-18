using UnityEngine;
using System.Collections;

public class MBehavior : MonoBehaviour {

	public static bool isPaused = false;
	[SerializeField] bool isAffectByPause = true;

	void Awake()
	{
		MAwake ();
	}

	void Start()
	{
		MStart ();
	}

	void Update()
	{
		if ( !isPaused || !isAffectByPause )
			MUpdate ();
	}

	void OnEnable()
	{
		MOnEnable ();
	}

	void OnDisable()
	{
		MOnDisable ();
	}

	void FixedUpdate()
	{
		if ( !isPaused || !isAffectByPause )
			MFixedUpdate ();
	}

	void OnCollisionEnter( Collision col )
	{
		if ( !isPaused || !isAffectByPause)
		MOnCollisionEnter (  col );
	}
	void OnTriggerEnter(Collider col )
	{
		if ( !isPaused || !isAffectByPause)
		MOnTriggerEnter (col);
	}

	void OnTriggerExit(Collider col )
	{
		if ( !isPaused || !isAffectByPause)
		MOnTriggerExit (col);
	}

	virtual protected void MAwake() {

	}

	// Use this for initialization
	virtual protected void MStart () {
		
	}
	
	// Update is called once per frame
	virtual protected void MUpdate () {
	
	}

	virtual protected void MOnEnable() {
	}

	virtual protected void MOnDisable() {
	}

	virtual protected void MFixedUpdate() {
	}

	virtual protected void MOnCollisionEnter( Collision col )
	{
		
	}
	virtual protected void MOnTriggerEnter( Collider col )
	{
	}

	virtual protected void MOnTriggerExit( Collider col )
	{
	}
}
