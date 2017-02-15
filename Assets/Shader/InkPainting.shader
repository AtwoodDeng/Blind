Shader "Unlit/InkPainting"
{
	Properties
	{
		_MainTex ("Main", 2D) = "white" {}
		_CoverTex( "Cover" , 2D) = "white" {}
		_MainColor( "Color" , Color) = (1,1,1,1)
		_Thred("BlackWhiteThred" , float) = 0.25
		_black("black", 2D ) = "white" {}
		_white("white",2D)= "white" {}
		_whiteScale("white scale" , float) = 2
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
			    fixed3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
				UNITY_FOG_COORDS(4)
				float4 vertex : SV_POSITION;
				float vdotn : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _CoverTex;
			float4 _CoverTex_ST;
			fixed4 _MainColor;
			float _Thred;
			sampler2D _black;
			float4 _black_ST;
			sampler2D _white;
			float4 _white_ST;
			float _whiteScale;

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

			fixed4 GetStylizedColor( half vdotn , half3 world, half2 uv )
			{
				float f = pow(vdotn,2);
				fixed4 outline = fixed4(1,1,1,1);
				if ( f < _Thred ) {
			      float2 findColor = float2(f*2.5f,uv.x/2.0f+uv.y/2.0f);
			      outline = tex2D(_black, findColor);
			   	}
			   	else {
			      float2 findColor = float2( world.x , world.y ) / _whiteScale ;
			      outline = tex2D(_white, findColor);
			   	}

			   	return outline;
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _CoverTex);
				o.worldPos = mul(unity_ObjectToWorld , v.vertex);
				float3 viewDir = normalize( mul(unity_WorldToObject, float4(_WorldSpaceCameraPos.xyz, 1)).xyz - v.vertex);
				o.vdotn = dot(normalize(viewDir),v.normal);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _MainColor;

				fixed4 style = GetStylizedColor( i.vdotn , i.worldPos.xyz , i.uv );

				return style;

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
