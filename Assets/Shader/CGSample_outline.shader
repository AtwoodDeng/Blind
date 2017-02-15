// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/CGSample_outline" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_black("black", 2D ) = "white" {}
		_white("white",2D)= "white" {}
	}
	SubShader {
		Pass {
			Tags { "RenderType"="Opaque" }
			LOD 200
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _black;
			sampler2D _white;
			fixed4 _Color;			
			
			struct appdata {
			    float4 vertex : POSITION;
			    fixed3 normal : NORMAL;
			    half2 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 vertex : POSITION;
				half2 uv : TEXCOORD0;
				float vdotn : TEXCOORD1;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				float3 viewDir = normalize( mul(unity_WorldToObject, float4(_WorldSpaceCameraPos.xyz, 1)).xyz - v.vertex);
				o.vdotn = dot(normalize(viewDir),v.normal);
				return o;
			}


			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 diff = tex2D(_MainTex, i.uv)*_Color;
				float f = pow(i.vdotn,2);


				fixed4 outline = fixed4(1,1,1,1);
				 if(f<0.25)
			   {
			      float2 findColor = float2(f*2.5f,i.uv.x/2.0f+i.uv.y/2.0f);
			      outline = tex2D(_black, findColor);
			   }
			   else
			   {
			     float2 findColor = float2((1-f)*1.666f, i.uv.x/2.0f+0.5f);
			      outline = tex2D(_white, 1-findColor);
			   }
				
				return diff*outline;
			}
			
			ENDCG
		}
	} 
	Fallback "diffuse"
	CustomEditor "CustomShaderGUI"
}
