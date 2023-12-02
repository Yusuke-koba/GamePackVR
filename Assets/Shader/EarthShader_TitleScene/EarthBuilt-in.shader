Shader "EarthBuilt-in/MyShader" {
	Properties {
		_MainTex 		("Texture (RGB)", 2D) = "white" {}
		_AO 		    ("AO map", 2D) = "white" {}
		_SpecColor 		("Specular Color", Color) = (241, 174, 92)
		_Shininess 		("Shininess", Range (0.1, 100)) = 0.078125
		_Mask			("Gloss Mask (RGB)", 2D) = "white" {}
		_BumpMap 		("BumpMap", 2D) = "bump" {}
		_BumpIntensity  ("BumpIntensity", Range(0.1, 10)) = 1
		_Emission		("Night Lights (RGB)", 2D) = "black" {}
		_LightAmount	("Night Lights Intensity", Range(0, 10)) = 1
		_LightColor 	("Night Light Color", Color) = (0.5, 0.5, 0.0, 1)
		_AtmosFalloff 	("Atmosphere Falloff", Range(0.5,8.0)) = 3.0
		_AtmosNear 		("Inner Atmosphere", Color) = (0,0,1,1)
		_AtmosFar 		("Far Atmosphere", Color) = (0,0,1,1)
		_Clouds 		("Clouds (RGB)", 2D) = "black" {}
		_CloudHeight	("Cloud Height", Range(0,.1)) = 0.05
		_CloudSpeed		("Cloud Speed", Range(-.01,.01)) = .001
	}
	//Shader model 3 target
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass{
			Tags{"LightMode" = "ForwardBase"}

			CGPROGRAM
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase

			sampler2D _MainTex, _BumpMap, _Emission, _Clouds, _Mask, _AO;
			float4 _MainTex_ST, _Clouds_ST, _LightColor0;
			fixed _LightAmount, _AtmosFalloff, _CloudSpeed, _CloudHeight, _Shininess, _BumpIntensity;
			fixed3 _AtmosNear, _AtmosFar;
			fixed4 _SpecColor, _LightColor;

			struct appdata {
				float4 vertexOS : POSITION;
				float2 uv : TEXCOORD0;
				float3 normalOS : NORMAL;
				float4 tangent : TANGENT;
			};

			struct v2f {
				float4 vertexCS : SV_POSITION;
				float2 uv_MainTex : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float3 tangentWS : TEXCOORD2;
				float3 binormalWS : TEXCOOED3;
				float3 positionWS : TEXCOORD4;
				float2 uv_Clouds : TEXCOORD5;
			};

			v2f vert (appdata input){
				v2f output;

				output.vertexCS = UnityObjectToClipPos(input.vertexOS);
				output.normalWS = normalize( mul( input.normalOS, (float3x3)unity_WorldToObject) );
				output.tangentWS = normalize( mul( (float3x3)unity_ObjectToWorld, input.tangent.xyz) );
				output.binormalWS = normalize( cross(output.normalWS, output.tangentWS) ) * input.tangent.w;
				output.positionWS = mul( (float3x3)unity_ObjectToWorld, input.vertexOS );
				output.uv_MainTex = TRANSFORM_TEX(input.uv, _MainTex);
				output.uv_Clouds = TRANSFORM_TEX(input.uv, _Clouds);
				return output;
			}


			half4 frag (v2f input) : SV_Target
			{	
				half4 mainTex = tex2D(_MainTex, input.uv_MainTex);
				half4 ao = tex2D(_AO, input.uv_MainTex);
				half4 specMask = tex2D(_Mask, input.uv_MainTex);
				half4 normalMap = tex2D(_BumpMap, input.uv_MainTex);
				//half4 nightLight = tex2D(_Emission, input.uv_MainTex);
				//half4 cloud = tex2D(_Clouds, input.uv_MainTex);

				//normal
				half3 normalData = UnpackNormal(normalMap);
				normalData.xy *= _BumpIntensity;
				half3 normalDir = normalize(input.normalWS);
				half3 tangentDir = normalize(input.tangentWS);
				half3 binormalDir = normalize(input.binormalWS);
				float3x3 TBN = float3x3(tangentDir, binormalDir, normalDir);
				normalDir = normalize( mul(normalData.xyz, TBN) );


			    //diffuse
				half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - input.positionWS);
				half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				half NdotL = dot(normalDir, lightDir);
				half3 diffuseColor = max(0.0, NdotL) * _LightColor0.xyz * mainTex.xyz;
				 
				//specular
				half3 reflectDir = reflect(-lightDir, normalDir);
				half RdotV = dot(reflectDir, viewDir);
				half3 specColor = pow(max(0.0, RdotV), _Shininess) * _LightColor0.xyz * _BumpIntensity * specMask.rgb;

				//AO
				fixed fresnel = 1.0 - saturate(dot(normalDir, half3(0,0,1) ) );
				fixed fresnelPowered = pow(fresnel, _AtmosFalloff);
				fixed3 atmosphere = lerp(_AtmosNear, _AtmosFar, fresnelPowered);

				//Clouds
				half2 uv_Clouds = input.uv_Clouds;
				uv_Clouds.x += fmod(_Time.y * _CloudSpeed, 1);//UV Pan in x axis. Using fmod to dont blow up the half2
				uv_Clouds += ParallaxOffset(1,_CloudHeight,viewDir);
				half3 clouds = tex2D(_Clouds, uv_Clouds); 

				//Emission
				half3 emission = lerp(tex2D(_Emission, input.uv_MainTex) * _LightAmount, 0, clouds) * _LightColor;

				//final
				//half3 finalColor = diffuseColor + specColor;
				half3 baseColor = (diffuseColor + specColor + atmosphere * fresnelPowered);
				half3 finalColor = baseColor + emission + clouds;
				//return half4(diffuseColor, 1.0);
				//return half4(specColor, 1.0);
				return half4(finalColor, 1.0);



				//fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
				
				//Atmosphere
				//fixed fresnel = 1.0 - saturate(dot(normalDir, half3(0,0,1) ) );
				//fixed fresnelPowered = pow(fresnel, _AtmosFalloff);
				
				//fixed3 atmosphere = lerp(_AtmosNear, _AtmosFar, fresnelPowered);
				//fixed3 cleanSky = c.rgb + atmosphere * fresnelPowered;
				
				//Clouds
				//half2 cloudUV = IN.uv_Mask;//IN.uv_Clouds;
				//cloudUV.x += fmod(_Time.y * _CloudSpeed, 1);//UV Pan in x axis. Using fmod to dont blow up the half2
				//cloudUV += ParallaxOffset(1,_CloudHeight,IN.viewDir);
				//half3 clouds = tex2D(_Clouds, cloudUV); 

				//o.Albedo = cleanSky + clouds;//lerp(cleanSky, clouds.rgb, clouds.rgb);
				//o.Normal = 0.5 * lerp(UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex)), half3(0,0,1), half3(clouds));
				//o.Emission = lerp(tex2D(_Emission, IN.uv_MainTex) * _LightAmount, 0, clouds) * _LightColor;
				//o.Specular = _Shininess;
				//o.Gloss =  tex2D(_Mask, IN.uv_MainTex) * (1 - clouds) * _SpecColor;
				//o.Alpha = c.a;
				//return fixed4(atmosphere, 1.0);
			}
			ENDCG
		}
	} 
	//FallBack "Diffuse"
}
