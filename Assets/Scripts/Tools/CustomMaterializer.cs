using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WizardUtils;

public class CustomMaterializer : MonoBehaviour
{
    public Renderer Target;
    public string TextureFileName;
    public Texture2D FallbackTexture;
    public string ShaderTextureReference = "_MainTex";

    public bool LoadOnAwake;

    private void Awake()
    {
        if (LoadOnAwake) LoadFromDisk();
    }

    private Material instancedMaterial;
    public void LoadFromDisk()
    {
        if (instancedMaterial == null)
        {
            instancedMaterial = new Material(Target.material);
        }

        Texture2D loadedTexture = CustomTextureHelper.Load(TextureFileName, FallbackTexture);

        instancedMaterial.SetTexture(ShaderTextureReference, loadedTexture);

        Target.material = instancedMaterial;
    }
}
