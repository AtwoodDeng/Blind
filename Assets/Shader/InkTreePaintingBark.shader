Shader "Unlit/InkTreePaintingBark"
{
	Properties
	{
		_MainTex ("Main", 2D) = "white" {}
		_Tooniness("Tooniness" , Range(0.1,20) ) = 4
		_CoverTex( "Cover" , 2D) = "white" {}
		_MainColor( "Color" , Color) = (1,1,1,1)
		_Thred("BlackWhiteThred" , float) = 0.25
		_black("black", 2D ) = "white" {}
		_blackScale("black scale" , float) = 2
		_OutColor( "OutColor" , Color) = (1,1,1,1)
		_white("white",2D)= "white" {}
		_whiteScale("white scale" , float) = 2
		_whiteOffset("white offset" , Vector) = (0,0,0,0)
		_InColor( "InColor" , Color) = (1,1,1,1)
		[MaterialToggle]_AlwaysShow("IsAlwaysShow" , float ) = 0
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
			float _blackScale;
			fixed4 _OutColor;
			sampler2D _white;
			float4 _white_ST;
			fixed4 _InColor;
			float _whiteScale;
			float4 _whiteOffset;
			float _AlwaysShow;
			float _Tooniness;

			uniform float4 InkPos[3];
			uniform float InkSca[3];
			uniform float InkAlp[3];
			uniform float InkAng[3];
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
			      float2 findColor = float2(f*2.5f,uv.x/2.0f+uv.y/2.0f) / _blackScale;
			      outline = tex2D(_black, findColor);
			      outline.a = (outline.r + outline.g + outline.b ) / 3;
			      outline *= _OutColor;
			   	}
			   	else {
			      float2 findColor = float2( world.x + world.z , world.y ) / _whiteScale + _whiteOffset.xy ;
			      outline = tex2D(_white, findColor);
			      outline.a = (outline.r + outline.g + outline.b ) / 3;
			      outline *= _InColor;
			   	}

			   	return outline;
			}

			fixed4 GetTooninessColor( fixed4 col )
			{
				fixed4 res = col;
				return floor( col * _Tooniness ) / _Tooniness;
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
				col = GetTooninessColor( col );

				fixed4 style = GetStylizedColor( i.vdotn , i.worldPos.xyz , i.uv );

				float rate = GetCoverRate( i.worldPos) ;
				col *= style;
				
				
				if ( _AlwaysShow == 0 )
					col.a *= rate;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}

	Dependency "OptimizedShader" = "Nature/Tree Creator Bark"
	FallBack "Diffuse"

}
