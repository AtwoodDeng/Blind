// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/InkFloor"
{
	Properties
	{
		_MainTex ("Main", 2D) = "white" {}
		_CoverTex( "Cover" , 2D) = "white" {}
		_MainColor( "Color" , Color) = (1,1,1,1)

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
				UNITY_FOG_COORDS(4)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _CoverTex;
			float4 _CoverTex_ST;
			fixed4 _MainColor;

			uniform float4 InkPos[6];
			uniform float InkSca[6];
			uniform float InkAlp[6];
			uniform float InkAng[6];
			uniform int InkNum;

			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _CoverTex);
				o.worldPos = mul(unity_ObjectToWorld , v.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _MainColor;
				fixed4 dark = fixed4(0,0,0,0);
				float rate = 0;
				float4 worldPos = i.worldPos;

				for( int i = 0 ; i < InkNum ; ++ i )
				{
					if ( InkSca[i] > 0 ) {


						fixed2 uv = fixed2(0,0);
						 
						fixed x = InkPos[i].x - worldPos.x;
						fixed y = InkPos[i].z - worldPos.z;
						x *= InkSca[i];
						y *= InkSca[i];

						fixed alpha = InkAng[i];
						uv.x = x * cos( alpha ) - y * sin( alpha );
						uv.y = x * sin( alpha ) + y * cos( alpha );


						uv.xy += 0.5f;

						fixed4 col = tex2D(_CoverTex , uv );
						rate += col.a * InkAlp[i];
					}
				}

				rate = min( rate , 1);

				col = lerp( _MainColor , col , rate );

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
