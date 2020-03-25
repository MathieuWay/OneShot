using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
public class SpriteLoader : MonoBehaviour
{
    //public Texture2D tex;
    //public Color tint = Color.white;
    [Range(0.5f,10f)]
    public float ScaleFactor = 1f;
    private void OnEnable()
    {
        ApplyTextureAndResizeQuad();
    }
    private void OnValidate()
    {
        ApplyTextureAndResizeQuad();
    }

    private void ApplyTextureAndResizeQuad()
    {
        if (GetComponent<Renderer>())
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer.sharedMaterial)
            {
                Texture tex = renderer.sharedMaterial.GetTexture("_MainTex");
                if (tex)
                {
                    if (tex.width > tex.height)
                    {
                        transform.localScale = new Vector3(1, (float)(tex.height) / (float)(tex.width), 1);
                    }
                    else
                    {
                        transform.localScale = new Vector3((float)(tex.width) / (float)(tex.height), 1, 1);
                    }
                    transform.localScale *= ScaleFactor;
                }
            }
            else
            {
                if (gameObject.scene.name != null)
                {
                    Debug.Log("Generating a new Material");
                    var material = new Material(Shader.Find("Customs/SpriteShadow"));
                    AssetDatabase.CreateAsset(material, "Assets/Materials/SpriteShadow/" + transform.name + ".mat");
                    renderer.sharedMaterial = material;
                }
            }
        }
    }
}

#endif
