Shader "Spectrum/Specular with Rim"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_EmissionLM ("Emission (Lightmapper)", Float) = 0
	    _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
		_Prominence ("Light Prominence", Range(0, 100)) = 1
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 400
		CGPROGRAM
		
		#pragma surface surf SpectrumSpecular alpha

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _Illum;
		fixed4 _Color;
		half _Shininess;
		float4 _RimColor;
		float _RimPower;
		float _Prominence;
		
		half4 LightingSpectrumSpecular (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
			half3 h = normalize (lightDir + viewDir);
			half diff = max (0, dot (s.Normal, lightDir));
			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, 48.0);
			half4 c;
			c.rgb = (_LightColor0.rgb * diff + _LightColor0.rgb * spec) * (atten * 2);
			return c;
		}

		struct Input {
			float2 uv_MainTex;
			float2 uv_Illum;
			float2 uv_BumpMap;
			float3 viewDir;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 c = tex * _Color;
			o.Albedo = c.rgb;
			o.Gloss = tex.a;
			o.Specular = _Shininess;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Alpha = max(0.0, normalize(_LightColor0.rgb)) * _Prominence + ((1.0 - dot(normalize(IN.viewDir), o.Normal)) * _RimColor.a);//normalize(_LightColor0.rgb) * _Prominence;
			half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.Emission = c.rgb * tex2D(_Illum, IN.uv_Illum).a + _RimColor.rgb * pow (rim, _RimPower);
		}
		
		ENDCG
	}
}