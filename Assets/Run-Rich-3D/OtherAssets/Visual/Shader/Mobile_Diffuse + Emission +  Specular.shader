Shader "Mobile/Diffuse + Emission +  Specular" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_EmissionColor ("Emission Color", Color) = (0,0,0,1)
		[NoScaleOffset] _EmissionMap ("Emission", 2D) = "white" {}
		[PowerSlider(5.0)] _Gloss ("Gloss", Range(0, 1)) = 0.078125
		[PowerSlider(5.0)] _Shininess ("Shininess", Range(0, 1)) = 0.078125
	}
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _EmissionMap;
		fixed4 _Color;
		fixed4 _EmissionColor;
		half _Gloss;
		half _Shininess;

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Metallic = 0;
			o.Smoothness = saturate(_Gloss + _Shininess);
			o.Emission = tex2D(_EmissionMap, IN.uv_MainTex).rgb * _EmissionColor.rgb;
		}
		ENDCG
	}
	Fallback "Mobile/VertexLit"
}
