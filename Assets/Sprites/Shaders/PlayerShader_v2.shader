Shader "Custom/PlayerShader_v2" {
		Properties{
			_MColor("Main Color", Color) = (1,1,1,1)
			_RColor("Reload Color", Color) = (1,1,0,1)
			_InvulWidth("Invul side width", Range(0,0.5)) = 0.1
			_InvulAlpha("Alpha of invul", Range(0,1)) = 1
			_InvulDarkness("darkness of invul", Range(0,1)) = 0.8
			[Toggle]_Reload("Is reloading", Float) = 0
			[Toggle]_Invul("Invulnerable", Float) = 0
			_MainTex("Alpha nask", 2D) = "white" {}
			_Alpha("Total Alpha", Range(0,1)) = 1
		}
			SubShader
		{
			Tags{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}
			LOD 100
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
		{
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

			struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
		};


		sampler2D _MainTex;
		float4	_MColor;
		float4	_RColor;
		float	_Percent;
		float	_InvulWidth;
		float	_Reload;
		float	_Invul;
		float	_Alpha;
		float	_InvulAlpha;
		float	_InvulDarkness;
		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{



			fixed4 texColor = tex2D(_MainTex, i.uv);

		float4 col = float4(1, 1, 1, 1);


		// sample the texture

		if (_Reload < 0.5)
			col = _MColor;
		else
			col = _RColor;
				
		col.a = _Alpha;

		if (_Invul > 0.5 && (i.uv.x < _InvulWidth || i.uv.y < _InvulWidth || i.uv.x >(1 - _InvulWidth) || i.uv.y >(1 - _InvulWidth)))
		{
			col.a *= _InvulAlpha;
			col.rgb *= _InvulDarkness;
			
		}

		
		return col * texColor;
		}

			ENDCG
		}
		}
	}