Shader "AboubiSuit"
{
	Properties
	{
		_MainTex("Diffuse", 2D) = "white" {}
		_Metallic("Metallic", Float) = 0
		_Smoothness("Smoothness", Float) = 0
		[Toggle(_USE_TEX)] _use_Tex("Use Texture", Float) = 1
		_Color("Color", Color) = (0,0,0,0)
		_EmissiveColor("Emissive Color", Color) = (0.5990566,0.8961561,1,0)
		_EmissiveBoost("Emissive Boost", Float) = 1
		_StunTimescale("Stun Timescale", Float) = 100
		[Toggle(_STUNNED)] _Stunned("Stunned", Float) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma shader_feature_local _USE_TEX
		#pragma shader_feature_local _STUNNED
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv2_MainTex;
		};

		uniform sampler2D _MainTex;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float4 _Color;
		uniform float4 _EmissiveColor;
		uniform float _EmissiveBoost;
		uniform float _StunTimescale;

		void surf(Input i, inout SurfaceOutputStandard o)
		{
			#ifdef _USE_TEX
				o.Albedo = tex2D(_MainTex, i.uv2_MainTex).rgb;
			#else
				o.Albedo = _Color;
			#endif
			
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			
			#ifdef _STUNNED
				float t = _Time.y * _StunTimescale;
				o.Emission = (o.Albedo * (saturate(sin(t)) * _EmissiveColor * _EmissiveBoost)).rgb;
			#else
				o.Emission = 0;
			#endif
			
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}