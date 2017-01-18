using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkObject : MBehavior {
	[SerializeField]protected List<Renderer> renders = new List<Renderer>();

	protected List<Vector4> InkPosition = new List<Vector4>();
	protected List<float> InkScale = new List<float>();
	protected List<float> InkAlpha = new List<float>();
	protected List<float> InkAngle = new List<float>();
	[SerializeField] protected AnimationCurve ScaleCurve;
	[SerializeField] protected AnimationCurve AlphaCurve;
	protected List<InkInfo> m_inkList = new List<InkInfo>();

	protected override void MAwake ()
	{
		base.MAwake ();
		if ( renders.Count <= 0 )
		{
//			renders.Add( GetComponent<Renderer>() );
			renders.AddRange( GetComponentsInChildren<Renderer>() );
		}

		Shader.SetGlobalInt("InkNum" , GetTotalInk() );

		for( int i = 0 ; i < GetTotalInk() ; ++i )
		{
			InkPosition.Add( Vector3.zero);
			InkScale.Add( 0 );
			InkAlpha.Add(0);
			InkAngle.Add(0);
		}
	}

	virtual protected int GetTotalInk()
	{
		return 1;
	}

	protected float LastRecordTime;
	protected Vector3 LastRecordPosition;

	protected void Record( Vector3 position )
	{
		m_inkList.Add( new InkInfo( position , ScaleCurve , AlphaCurve ));
		if ( m_inkList.Count > GetTotalInk() )
			m_inkList.RemoveAt(0);
		LastRecordTime = Time.time;
		LastRecordPosition = position;
	}

	protected override void MUpdate ()
	{

		// Update the save data
		for( int i = 0 ; i < GetTotalInk() && i < m_inkList.Count ; ++i ) {
			InkPosition[i] = m_inkList[i].Position;
			InkScale[i] = m_inkList[i].Scale;
			InkAlpha[i] = m_inkList[i].Alpha;
			InkAngle[i] = m_inkList[i].Angle;
		}

		// update the material
		foreach( Renderer r in renders ) {
			r.material.SetVectorArray( "InkPos" , InkPosition );
			r.material.SetFloatArray( "InkSca" , InkScale );
			r.material.SetFloatArray( "InkAlp" , InkAlpha );
			r.material.SetFloatArray( "InkAng" , InkAngle );
		}
	}


}


public class InkInfo
{
	public Vector3 Position;
	public float Scale{
		get { return 1f / m_scale; }
	}
	float m_scale;
	public float Alpha;
	public float Angle;
	float timer;
	float distance;

	AnimationCurve m_scaleCurve;
	AnimationCurve m_alphaCurve;

	public InkInfo( Vector3 _pos, AnimationCurve _scaleCurve , AnimationCurve _alphaCurve )
	{
		Position = _pos;
		timer = 0;
		m_scaleCurve = _scaleCurve;
		m_alphaCurve = _alphaCurve;
		Angle = Random.Range( 0 , Mathf.PI * 2f ) ;
	}


	public void Update( float dt , float ds)
	{
		timer += dt;
		m_scale = m_scaleCurve.Evaluate( timer );
		distance += ds;
		Alpha = m_alphaCurve.Evaluate( distance / 3f );
	}
}

