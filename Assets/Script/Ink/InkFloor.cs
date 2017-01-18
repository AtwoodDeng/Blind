using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkFloor : InkObject {

	static int totalInk = 10;

	[SerializeField] float InkIntervalMax = 0.5f;
	[SerializeField] float InkIntervalMin = 0.1f;
	[SerializeField] float minInkDistance = 0.5f;
	[SerializeField] Transform target;

	protected override int GetTotalInk ()
	{
		return totalInk;
	}

	override protected void MAwake()
	{
		base.MAwake();

		if ( target == null )
			target = MCharacter.Instance.transform;

	}

	protected override void MStart ()
	{
		base.MStart ();
		StartCoroutine( InkCreateor( ));
	}

	IEnumerator InkCreateor( )
	{
		Vector3 lastRecordPos = Vector3.one * 999f;
		while( true )
		{
			Vector3 mcPos = MCharacter.Instance.transform.position;
			if ( (lastRecordPos - mcPos ).magnitude > minInkDistance )
			{
				Record( mcPos );
				lastRecordPos = mcPos;

			}
			yield return new WaitForSeconds(Random.Range( InkIntervalMin , InkIntervalMax ));
		}
	}

	Vector3 targetLastPosition;

	protected override void MUpdate ()
	{

		float ds = 0;
		ds = ( target.position - targetLastPosition).magnitude;
		targetLastPosition = target.position;
		// Update the ink data
		foreach( InkInfo info in m_inkList ) {
			info.Update( Time.deltaTime , ds );
		}

		base.MUpdate ();
	}
}