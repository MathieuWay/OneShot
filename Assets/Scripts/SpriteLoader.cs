using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[ExecuteInEditMode]
public class SpriteLoader : MonoBehaviour
{
    public Sprite sprite;
    private Sprite currentSprite;
    //public Color tint = Color.white;
    [Range(0.5f, 10f)]
    public float ScaleFactor = 1f;
    private void Update()
    {
        if (GetComponent<Renderer>() && currentSprite != sprite && sprite != null)
        {
            UpdateSprite();
        }
    }

    private void OnEnable()
    {
        ApplyTextureAndResizeQuad();

        if (GetComponent<Renderer>() && currentSprite != sprite && sprite != null)
            UpdateSprite();
    }
    private void OnValidate()
    {
        ApplyTextureAndResizeQuad();

        if (GetComponent<Renderer>() && currentSprite != sprite && sprite != null)
            UpdateSprite();
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
                    UpdateQuadSize(tex);
            }
            else
            {
                /*if (gameObject.scene.name != null)
                {
                    Debug.Log("Generating a new Material");
                    var material = new Material(Shader.Find("Customs/SpriteShadow"));
                    AssetDatabase.CreateAsset(material, "Assets/Materials/SpriteShadow/" + transform.name + ".mat");
                    renderer.sharedMaterial = material;
                }*/
            }
        }
    }

    private void UpdateQuadSize(Texture tex)
    {
        float width = 1;
        float height = 1;
        if (tex.width > tex.height)
            height = (float)(tex.height) / (float)(tex.width);
        else
            width = (float)(tex.width) / (float)(tex.height);
        transform.localScale = new Vector3(width * ScaleFactor, height * ScaleFactor, 1);
    }

    private void UpdateSprite()
    {
        Renderer renderer = GetComponent<Renderer>();
        var croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
        var pixels = sprite.texture.GetPixels((int)sprite.rect.x,
                                                (int)sprite.rect.y,
                                                (int)sprite.rect.width,
                                                (int)sprite.rect.height);
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();
        croppedTexture.filterMode = FilterMode.Point;
        renderer.sharedMaterial.SetTexture("_MainTex", croppedTexture);
        UpdateQuadSize(croppedTexture);
        currentSprite = sprite;
    }
}

#endif
