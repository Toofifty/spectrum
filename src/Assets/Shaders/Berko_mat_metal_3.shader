
Shader "Berko/Berko_mat_metal_3" {
Properties {
	_Color ("Main Color", Color) = (0.5, 0.5, 0.5, 0)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	//Glossines
	_Gloss ("Glossiness", float) = 0.5	
	//Specular
	_Spec ("Specular Power", float) = 0.5	
	_SpecTex ("Specular (R), Glossiness (G), Reflection (B), Parallax (A)", 2D) = "white" {}
	
	_BumpMap ("Normalmap", 2D) = "bump" {}
    _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
    _ReflectColor ("Reflection Color", Color) = (255,255,255,0.5)
	_Cube ("Reflection Cubemap", Cube) = "" {}
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 400
	
CGPROGRAM
#pragma surface surf BlinnPhongSpecMap
#pragma target 3.0
sampler2D _MainTex;
sampler2D _SpecTex;
sampler2D _BumpMap;
samplerCUBE _Cube;
fixed4 _ReflectColor;
half _Spec;
half _Gloss;
float _RimPower;

struct Input {
	float2 uv_MainTex;
	float3 viewDir;
	float3 worldRefl;
	INTERNAL_DATA
	
};

struct SurfaceOut {
	fixed3 Albedo;
	fixed3 Normal;
	fixed3 Emission;
	half Specular;
	fixed3 Gloss;
	fixed Alpha;
	
};

void surf (Input IN, inout SurfaceOut o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 texS = tex2D(_SpecTex, IN.uv_MainTex);
	//difuse
	o.Albedo = tex.rgb; //* _Color.rgb;
	//normal
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
	//rim
	half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
    //gloss
    o.Gloss = texS.g *_Gloss* pow (rim, _RimPower);
	//alpha
	o.Alpha = tex.a; //* _Color.a;
	//specular
	o.Specular = texS.r *_Spec;
	//Cubemap
	float3 worldRefl = WorldReflectionVector (IN, o.Normal);
	fixed4 reflcol = texCUBE (_Cube, worldRefl);
	reflcol *= texS.b * pow (rim, _RimPower);
	o.Emission = reflcol.rgb * _ReflectColor.rgb;
	
	
}
inline fixed4 LightingBlinnPhongSpecMap (SurfaceOut s, fixed3 lightDir, half3 viewDir, fixed atten)
{
	half3 h = normalize (lightDir + viewDir);
	fixed diff = max (0, dot (s.Normal, lightDir));
	float nh = max (0, dot (s.Normal, h));
	float spec = pow (nh, s.Specular*128.0);
	fixed4 c;
	c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec * s.Gloss) * (atten * 2);
	c.a = s.Alpha + _LightColor0.a * (0.2989f * s.Gloss.r + 0.5870f * s.Gloss.g + 0.1140f * s.Gloss.b) * spec * atten;
	return c;
}
ENDCG
}

Fallback "Berko/Berko_mat_metal_3"
}
