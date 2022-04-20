Shader "Hidden/DynamicBlurringVisualizer"
{
	Properties{
		[HideInInspector]_MainTex ("Texture", 2D) = "white" {}
		_KernelSize("Kernel Size", Int) = 11
		_visualizeColor("Visualize Color", Int) = 1
		_inRad("Inner Radius", Float) = 0
		_outRad("Outer Radius", Float) = 0
		_eyeXPos("Eye X Position", Float) = 0.5
		_eyeYPos("Eye Y Position", Float) = 0.5
		_softEdge("Soft Edge", Int) = 1
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"

	#define PI 3.14159265359
	#define E 2.71828182846

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float2 _MainTex_TexelSize;
	int _KernelSize;
	
	uniform float _inRad;
	uniform float _outRad;
	uniform float4 _LineColor = (255,255,0,1);
	uniform float _eyeXPos;
	uniform float _eyeYPos;
	uniform int _softEdge;
	uniform int _visualizeColor;

	ENDCG

	SubShader
	{
		Cull Off
		ZWrite Off 
		ZTest Always

		Pass
		{
			Name "Pass"
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_horizontal

			//vertex shader에 첨부될 데이터
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			//fragment shader 데이터 base
			struct v2f
			{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			//vertex shader 데이터 base
			v2f vert(appdata v)
			{
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			//the fragment shader
			fixed4 frag_horizontal(v2f i) : SV_TARGET
			{
				// Eye-tracking variables & FoV variables
				float opacity = 0;
				float base_opacity = 1;
				float X = _eyeXPos;
				float Y = _eyeYPos;
				float L = sqrt((i.uv.x - X)*(i.uv.x - X) + (i.uv.y - (1-Y))*(i.uv.y - (1-Y)));
				float I = _inRad;
				float O = _outRad;
				
				float4 baseColor;

				float4 lineColor = float4(1.0, 1.0, 0.0, 1.0);
				float4 InnerColor = float4(0.0, 0.0, 1.0, 1.0);
				float4 OuterColor = float4(1.0, 0.0, 0.0, 1.0);
				float4 col = tex2D(_MainTex, i.uv);

				if (_softEdge == 1)
				{
					if (L < I - 0.001)
					{
						opacity = 0;
					}
					else if (I + 0.001 < L && L < O - 0.001)
					{
						if (_visualizeColor == 1)
						{
							base_opacity = (L - I) / (O - I);
							for (int num = 0; num < 1; num = num + 1)
							{
								opacity = 0.5 * base_opacity;
							}
							opacity = opacity * _KernelSize / 13;
							baseColor = InnerColor;
						}
					}
					else if (L > O + 0.001)
					{
						if (_visualizeColor == 1)
						{
							opacity = 0.5 * _KernelSize / 13;
							baseColor = OuterColor;
						}
					}
					else
					{
						opacity = 1;
						baseColor = lineColor;
					}
				}
				else
				{
					if (L < I - 0.001)
					{
						opacity = 0;
					}
					else if (I + 0.001 < L)
					{
						if (_visualizeColor == 1)
						{
							base_opacity = (L - I) / (O - I);
							for (int num = 0; num < 1; num = num + 1)
							{
								opacity = 0.5 * base_opacity;
							}
							opacity = opacity * _KernelSize / 13;
							baseColor = OuterColor;
						}
					}
					else
					{
						opacity = 1;
						baseColor = lineColor;
					}
				}
				col = col * (1 - opacity) + baseColor * (opacity);
				
				return col;

			}
			ENDCG
		}

	}
}