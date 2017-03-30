using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FarmAnimal : MBehavior {

	public enum State
	{
		Idle,
		Walk,
		WalkMove,
		Run,
		RunMove,
		Stand,
	}

	private AStateMachine<State,LogicEvents> m_stateMachine;
	[SerializeField] MinMax stateChangeInterval;
	[SerializeField] float walkSpeed = 1f;
	[SerializeField] float runSpeed = 2f;
	[SerializeField] float turnSpeed = 1000f;
	[SerializeField] Animation m_animation;
	[SerializeField] string standClip = "stand";
	[SerializeField] string walkClip = "walk";
	[SerializeField] string runClip = "run";
	[SerializeField] string idleClip = "idle";

	protected override void MAwake ()
	{
		base.MAwake ();
		if (m_animation == null)
			m_animation = GetComponent<Animation> ();
		m_animation.Play ();
		InitStateMachine ();
	}

	float stateMachineTimer = 0;
	float stateChangeTime = 0;
	Vector3 direction;
	void InitStateMachine()
	{
		m_stateMachine = new AStateMachine<State, LogicEvents> (State.Stand);

		m_stateMachine.AddEnter (State.Stand, delegate() {
			stateChangeTime = stateChangeInterval.Rand;
			stateMachineTimer = 0;
			m_animation.CrossFade(standClip,0.2f);
		});

		m_stateMachine.AddUpdate (State.Stand, delegate() {
			stateMachineTimer += Time.deltaTime;
			if ( stateMachineTimer > stateChangeTime ){
				if ( Random.Range(0,1f) > 0.5f )
				{
					m_stateMachine.State = State.Idle;
				}else 
					m_stateMachine.State = State.Walk;
			}
		});

		m_stateMachine.AddEnter (State.Walk, delegate() {
			float turnTo = Random.Range( 0 , 360f );
			float turnTime = Mathf.Repeat( transform.rotation.eulerAngles.y - turnTo , 360f ) / turnSpeed;

			transform.DORotate( new Vector3( 0 , turnTo , 0 ) , turnTime ).OnComplete(delegate() {
				m_stateMachine.State = State.WalkMove;	
			});
			m_animation.CrossFade(walkClip,0.2f);
		});

		m_stateMachine.AddEnter (State.WalkMove, delegate() {
			stateChangeTime = stateChangeInterval.Rand;
			stateMachineTimer = 0;
		});


		m_stateMachine.AddUpdate (State.WalkMove, delegate() {
			transform.position += - transform.forward * walkSpeed * Time.deltaTime;
			stateMachineTimer += Time.deltaTime;
			if ( stateMachineTimer > stateChangeTime ){
				m_stateMachine.State = State.Stand;
			}
		});

		m_stateMachine.AddEnter (State.Idle, delegate() {
			stateChangeTime = stateChangeInterval.Rand;
			stateMachineTimer = 0;
			m_animation.CrossFade(idleClip,0.2f);
		});

		m_stateMachine.AddUpdate (State.Idle, delegate() {
			stateMachineTimer += Time.deltaTime;
			if ( stateMachineTimer > stateChangeTime ){
				if ( Random.Range(0,1f) > 0.5f )
				{
					m_stateMachine.State = State.Stand;
				}else 
					m_stateMachine.State = State.Walk;
			}
		});

		m_stateMachine.AddEnter (State.Run, delegate() {
			Vector3 oriAngle = transform.eulerAngles;
			GameObject player = GameObject.FindWithTag("Player");
			Vector3 toPlayer = player.transform.position - transform.position;
			transform.forward = toPlayer;
			Vector3 toAngle = transform.eulerAngles;
			transform.eulerAngles = oriAngle;

			float turnTo = toAngle.y;
			float turnTime = Mathf.Repeat( transform.rotation.eulerAngles.y - turnTo , 360f ) / turnSpeed;

			transform.DORotate( new Vector3( 0 , turnTo , 0 ) , turnTime ).OnComplete(delegate() {
				m_stateMachine.State = State.RunMove;	
			});
			m_animation.CrossFade(runClip,0.2f);
		});

		m_stateMachine.AddEnter (State.RunMove, delegate() {
			stateChangeTime = stateChangeInterval.Rand;
			stateMachineTimer = 0;
		});


		m_stateMachine.AddUpdate (State.RunMove, delegate() {
			transform.position += - transform.forward * runSpeed * Time.deltaTime;
			stateMachineTimer += Time.deltaTime;
			if ( stateMachineTimer > stateChangeTime ){
				m_stateMachine.State = State.Stand;
			}
		});


	}

	protected override void MUpdate ()
	{
		base.MUpdate ();
		m_stateMachine.Update ();
	}

	protected override void MOnTriggerEnter (Collider col)
	{
		base.MOnTriggerEnter (col);
		if (col.tag == "Player" ) {
			if ( m_stateMachine.State != State.Run && m_stateMachine.State != State.RunMove )
				m_stateMachine.State = State.Run;
		}
	}

	void OnGUI()
	{
		GUILayout.Label ("State " + m_stateMachine.State);
	}
}
