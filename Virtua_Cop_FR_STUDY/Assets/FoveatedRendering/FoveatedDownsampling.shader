Shader "Hidden/FoveatedDownsampling"
{
	Properties{
		[HideInInspector]_MainTex ("Texture", 2D) = "white" {}
		_KernelSize("Kernel Size", Int) = 11
		_StandardDeviation("Standard Deviation", Float) = 5.0

		_inRad("Inner Radius", Float) = 0
		_outRad("Outer Radius", Float) = 0
		_loop("Loop Times", Int) = 1
		_eyeXPos("Eye X Position", Float) = 0.5
		_eyeYPos("Eye Y Position", Float) = 0.5
		_softEdge("Soft Edge", Int) = 1
		
		_ResWidth("Res Width", Int) = 1920
		_ResHeight("Res Height", Int) = 1080

	}
	
	CGINCLUDE
	#include "UnityCG.cginc"

	#define PI 3.14159265359
	#define E 2.71828182846

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float2 _MainTex_TexelSize;
	int _KernelSize;
	float _StandardDeviation;
	
	uniform float _inRad;
	uniform float _outRad;
	uniform float _eyeXPos;	
	uniform float _eyeYPos;
	uniform int _loop;
	uniform int _softEdge;

	uniform int _ResWidth;
	uniform int _ResHeight;

	ENDCG

	SubShader
	{
		Cull Off
		ZWrite Off 
		ZTest Always

		Pass
		{
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

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
				float2 screenPos: TEXCOORD1;
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
			fixed4 frag(v2f i) : SV_TARGET
			{
				// Eye-tracking variables & FoV variables
				float opacity = 1;
				float base_opacity = 1;
				float X = _eyeXPos;
				float Y = _eyeYPos;
				float L = sqrt((i.uv.x - X)*(i.uv.x - X) + (i.uv.y - (1-Y))*(i.uv.y - (1-Y)));
				float I = _inRad;
				float O = _outRad;
				int W = _ResWidth;
				int H = _ResHeight;

				int pixel_x = (int)(i.uv.x * W);
				int pixel_y = (int)(i.uv.y * H);
				
				int K = _KernelSize;

				float4 colTemp = float4(0.0, 0.0, 0.0, 1.0);
				float4 colTemp2 = float4(0.0, 1.0, 0.0, 1.0);
				float4 col = tex2D(_MainTex, i.uv);

				int temp = 0;
				v2f o = i;


				temp = pixel_x % K;
				temp = pixel_x - temp + K/2;
				o.uv.x = (float)temp / W;

				temp = pixel_y % K;
				temp = pixel_y - temp + K/2;
				o.uv.y = (float)temp / H;

				if (L < I)
					return tex2D(_MainTex, i.uv);
				else
					return tex2D(_MainTex, o.uv);


				return tex2D(_MainTex, o.uv);

			}
			ENDCG
		}
	}
}