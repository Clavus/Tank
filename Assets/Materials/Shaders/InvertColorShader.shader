Shader "Unlit/InvertColorShader"
{
	Properties
	{
		_Color("Tint Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags {
			"IgnoreProjector" = "True"
			"Queue" = "Transparent"
		}

		ZTest Greater
		
		Pass
		{
			ZWrite On
			ColorMask 0
		}

		ZTest Less

		Blend OneMinusDstColor Zero //invert blending, so long as FG color is 1,1,1,1
		BlendOp Add

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			uniform float4 _Color;

			struct vertIn
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
			};

			struct vertOut
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR0;
			};

			vertOut vert (vertIn v)
			{
				vertOut o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color * _Color;
				return o;
			}
			
			half4 frag (vertOut i) : COLOR
			{

				return i.color;
			}
			ENDCG
		}

		// Linear color space correction

		ZTest Equal

		Pass
		{
			Blend Zero DstColor  // multiplies (newly created) bg by itself to simulate a de-gamma in linear color space
			BlendOp Add
			SetTexture[_Color]
			{ 
				constantColor(1,1,1,1)
				combine constant 
			}
		}
		
		Pass
		{
			Blend Zero DstColor  //multiplies (newly created) bg by itself to simulate a de-gamma in linear color space
			BlendOp Add
			SetTexture[_Color]
			{
				constantColor(1,1,1,1)
				combine constant
			}
		}


		/* // GrabPass method
		
		ZWrite Off

		GrabPass{}

		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform float4 _Color;
			sampler2D _GrabTexture;

			// _MainTex
			sampler2D _MainTex;

			// _MainTex tiling and offset properties.
			float4 _MainTex_ST;

			struct vertIn
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct vertOut
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;		// a=distortion intensity multiplier
				half4 texcoord : TEXCOORD0; // xy=distort uv, zw=mask uv
				half4 screenuv : TEXCOORD1; // xy=screenuv, z=distance dependend intensity, w=depth
			};

			vertOut vert(vertIn v)
			{
				vertOut o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex); // Apply texture tiling and offset.
				o.texcoord.zw = v.texcoord;

				half4 screenpos = ComputeGrabScreenPos(o.vertex);
				o.screenuv.xy = screenpos.xy / screenpos.w;

				return o;
			}

			half4 frag(vertOut i) : COLOR
			{
				// get screen space position of current pixel
				half2 uv = i.screenuv.xy;
				half4 color = tex2D(_GrabTexture, uv);
				color.rgb = 1.0 - color.rgb;
				// sample the texture
				//fixed4 col = half4(1.0,_SinTime[0],0.0,0.2);//tex2D(_MainTex, i.uv);
				return color;//tex2D(_GrabTexture, i.vertex.xy) * i.color;
			}
		ENDCG
		}*/

	}
}
