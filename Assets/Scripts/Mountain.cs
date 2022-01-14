using UnityEngine;

public class Mountain : MonoBehaviour
{
    public ComputeShader shader; 
    public Texture2D normalMap;
    public Material mountainMaterial;

    public float precipitation = 0.35f;
    public float lastSnowfall = 1.0f;

    public float airTemp = 1.0f;
    public float rainTemp = 2.0f;
    public float meltTemp = 0.0f;
    
    private RenderTexture tex;

    // Start is called before the first frame update
    void Start()
    {
        //Creates the rendertexture used
        tex = new RenderTexture(1024, 1024, 24);
        tex.enableRandomWrite = true;
        tex.Create();

        //Sets the textures in the material to the rendertexture
        mountainMaterial.SetTexture("_HeightMap", tex);
        mountainMaterial.SetTexture("_MainTex", tex);

        //Sets the snow cover
        SetSnow();
    }

    void Update()
    {
        Snow();
    }

    /// <summary>
    /// Changes the snow cover
    /// Prepares the values for the compute shader, and starts it
    /// </summary>
    public void Snow()
    {
        int kernelHandle = shader.FindKernel("Snow");
        shader.SetTexture(kernelHandle, "inTexture", tex);
        shader.SetTexture(kernelHandle, "normalMap", normalMap);

        shader.SetFloat("t", Time.time);
        shader.SetFloat("d", 24.0f);
        shader.SetFloat("Ri", 1.0f);

        shader.SetFloat("precipitation", precipitation);
        shader.SetFloat("lastSnowfall", 0);
        shader.SetFloat("albedo", 0.4f);

        shader.SetFloat("airTemp", airTemp);
        shader.SetFloat("rainTemp", rainTemp);
        shader.SetFloat("meltTemp", meltTemp);

        shader.SetFloat("cellArea", 1.0f);

        shader.Dispatch(kernelHandle, 1024 / 8, 1024 / 8, 1);
    }

    /// <summary>
    /// Sets the snow cover
    /// Prepares the values for the compute shader, and starts it
    /// </summary>
    public void SetSnow()
    {
        int kernelHandle = shader.FindKernel("SetSnow");
        shader.SetTexture(kernelHandle, "inTexture", tex);
        shader.SetTexture(kernelHandle, "normalMap", normalMap);

        shader.Dispatch(kernelHandle, 1024 / 8, 1024 / 8, 1);
    }
}