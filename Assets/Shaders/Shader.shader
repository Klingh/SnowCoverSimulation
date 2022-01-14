Shader "Custom/Shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Amount ("Extrusion multiplier", Range(0,10)) = 0.0
        _HeightMap("Height", 2D) = "white" {}
        _EdgeLength("Edge length", Range(2,50)) = 5
        _Phong("Phong Strengh", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows addshadow vertex:vert tessellate:tessEdge// tessphong:_Phong
        #include "Tessellation.cginc"

        struct appdata
        {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
        };

        void dispNone(inout appdata v) { }

        float _Phong;
        float _EdgeLength;

        

        sampler2D _MainTex;
        float _Amount;
        sampler2D _HeightMap;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        //UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        //UNITY_INSTANCING_BUFFER_END(Props)


        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Main texture and heightmap is set to the same in the compute shader
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            // Height Value from the Heightmap, could be used
            //fixed height = tex2D(_HeightMap, IN.uv_MainTex).r;
           
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            o.Albedo =  (1.0f - smoothstep(0.15, 0.7, c)) * _Color + smoothstep(0.15, 0.7, c);
        }

        void vert(inout appdata_base v)
        {
            float3 normal = float3(0, 1, 0);

            fixed height = tex2Dlod(_HeightMap, float4(v.texcoord.xy, 0, 0)).r;

            v.normal = normalize(v.normal);
            v.vertex.xyz += normal * height * 2;
        }

        float4 tessEdge(appdata_base v0, appdata_base v1, appdata_base v2)
        {
            return UnityEdgeLengthBasedTess(v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
        }

        ENDCG
    }
    FallBack "Diffuse"
}
