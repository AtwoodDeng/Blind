Shader "Unlit/InkCube"
{
	Properties
	{
		_MainTex ("Main", 2D) = "white" {}
		_CoverTex( "Cover" , 2D) = "white" {}
		_MainColor( "Color" , Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

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

			uniform float4 InkPos[64];
			uniform float InkSca[64];
			uniform float InkAlp[64];
			uniform float InkAng[64];
			uniform int InkNum;

			// return the alpha of the cover
			fixed GetCoverRate( float4 worldPos )
			{
				fixed rate = 0;
				float2 myPos = ComputeScreenPos( worldPos );

				for( int i = 0 ; i < InkNum ; ++ i )
				{
					if ( InkSca[i] > 0 ) {
						fixed2 uv = fixed2(0,0);

						float dis = distance( InkPos[i].xyz , worldPos.xyz );
						float gama = ( InkPos[i].x - worldPos.x + InkPos[i].y - worldPos.y + InkPos[i].z - worldPos.z) * 0.5;

						fixed2 oriPos = fixed2( dis * cos( gama ) , dis * sin(gama ));

						oriPos *= InkSca[i];


						fixed alpha = InkAng[i];
						uv.x = oriPos.x * cos( alpha ) - oriPos.y * sin( alpha );
						uv.y = oriPos.x * sin( alpha ) + oriPos.y * cos( alpha );

						uv.xy += 0.5;

						fixed4 col = tex2D(_CoverTex , uv );
						rate = lerp( rate , 1 , col.a * InkAlp[i]);
					}
				}

				rate = min( rate , 1);

				return rate;
			}
			
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

				float rate = GetCoverRate( i.worldPos) ;
				col = lerp( _MainColor , col , rate );
				col.a = rate;


				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
