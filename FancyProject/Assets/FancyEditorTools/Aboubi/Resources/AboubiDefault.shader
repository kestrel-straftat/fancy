Shader "AboubiDefault"
{
	Properties
	{
		_MainTex("Diffuse", 2D) = "white" {}
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
		#pragma shader_feature_local _STUNNED
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		
		uniform sampler2D _MainTex;
		uniform float4 _EmissiveColor;
		uniform float _EmissiveBoost;
		uniform float _StunTimescale;

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input i, inout SurfaceOutputStandard o)
		{
			float4 color = tex2D(_MainTex, i.uv_MainTex);
			o.Albedo = color.rgb;
			
			#ifdef _STUNNED
				float t = _Time.y * _StunTimescale;
				o.Emission = (color * (saturate(sin(t)) * _EmissiveColor * _EmissiveBoost)).rgb;
			#else
				o.Emission = 0;
			#endif
			
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}