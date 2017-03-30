using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inkCube : InkObject {


	static int totalInk = 5;

	[SerializeField] MinMax createDistance;
	bool isTouch = false;

	protected override void MStart ()
	{
		base.MStart ();
	}

	protected override int GetTotalInk ()
	{
		return totalInk;
	}

	protected override void MOnCollisionEnter (Collision col)
	{
		base.MOnCollisionEnter (col);
	}

	protected override void MUpdate ()
	{
		base.MUpdate ();

		foreach( InkInfo info in m_inkList )
		{
			info.Update( Time.deltaTime , isTouch? MCharacter.Instance.deltaDistance : Time.deltaTime * 0.5f );
		}
	}

	void OnCollisionEnter( Collision col )
	{
		if ( col.gameObject.layer == LayerMask.NameToLayer("Player") ) {
			Debug.Log("Collision Enter");
			isTouch = true;
		}
	}

	float nextDistance = 0;
	void OnCollisionStay( Collision col )
	{
		if ( col.gameObject.layer == LayerMask.NameToLayer("Player") ) {
			if ( (LastRecordPosition - col.contacts[0].point).magnitude > nextDistance )
			{
				Record( col.contacts[0].point );
				nextDistance = createDistance.Rand;
			}
		}
	}

	void OnCollisionExit( Collision col )
	{
		if ( col.gameObject.layer == LayerMask.NameToLayer("Player") ) {
			Debug.Log("Collision Exit");
			isTouch = false;
		}
	}



}
