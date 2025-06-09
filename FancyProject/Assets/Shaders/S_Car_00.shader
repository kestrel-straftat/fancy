Shader "S_Car_00"
{
	Properties
	{
		_BC("BC", 2D) = "white" {}
		_EmissionMap("EmissionMap", 2D) = "white" {}
		[Normal]_N("N", 2D) = "bump" {}
		_Metallic("Metallic", 2D) = "white" {}
		_Roughness("Roughness", 2D) = "white" {}
		[Toggle(_USEMETALLIC_ON)] _UseMetallic("UseMetallic", Float) = 0
		[Toggle(_USEEMISSION_ON)] _UseEmission("UseEmission", Float) = 0
		[Toggle(_USEROUGHNESS_ON)] _useRoughness("use Roughness", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#pragma target 3.0
		#pragma shader_feature_local _USEEMISSION_ON
		#pragma shader_feature_local _USEMETALLIC_ON
		#pragma shader_feature_local _USEROUGHNESS_ON
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _N;
		uniform float4 _N_ST;
		uniform sampler2D _BC;
		uniform float4 _BC_ST;
		uniform sampler2D _EmissionMap;
		uniform float4 _EmissionMap_ST;
		uniform sampler2D _Metallic;
		uniform float4 _Metallic_ST;
		uniform sampler2D _Roughness;
		uniform float4 _Roughness_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_N = i.uv_texcoord * _N_ST.xy + _N_ST.zw;
			o.Normal = UnpackNormal( tex2D( _N, uv_N ) );
			float2 uv_BC = i.uv_texcoord * _BC_ST.xy + _BC_ST.zw;
			o.Albedo = tex2D( _BC, uv_BC ).rgb;
			float4 temp_cast_1 = (0.0).xxxx;
			float2 uv_EmissionMap = i.uv_texcoord * _EmissionMap_ST.xy + _EmissionMap_ST.zw;
			#ifdef _USEEMISSION_ON
				float4 staticSwitch9 = tex2D( _EmissionMap, uv_EmissionMap );
			#else
				float4 staticSwitch9 = temp_cast_1;
			#endif
			o.Emission = staticSwitch9.rgb;
			float2 uv_Metallic = i.uv_texcoord * _Metallic_ST.xy + _Metallic_ST.zw;
			#ifdef _USEMETALLIC_ON
				float staticSwitch7 = tex2D( _Metallic, uv_Metallic ).r;
			#else
				float staticSwitch7 = 0.0;
			#endif
			o.Metallic = staticSwitch7;
			float2 uv_Roughness = i.uv_texcoord * _Roughness_ST.xy + _Roughness_ST.zw;
			#ifdef _USEROUGHNESS_ON
				float staticSwitch12 = ( 1.0 - tex2D( _Roughness, uv_Roughness ).r );
			#else
				float staticSwitch12 = 0.0;
			#endif
			o.Smoothness = staticSwitch12;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}