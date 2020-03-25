using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpriteLoader : MonoBehaviour
{
    public Texture2D tex;
    public Color tint = Color.white;
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
        GetComponent<Renderer>().material.SetColor("_Tint", tint);
        //Debug.Log(tex.width / tex.height);
        if (tex)
        {
            GetComponent<Renderer>().material.SetTexture("_MainTex", tex);
            if (tex.width > tex.height)
            {
                transform.localScale = new Vector3(1, (float)(tex.height) / (float)(tex.width), 1);
            }
            else
            {
                transform.localScale = new Vector3((float)(tex.width) / (float)(tex.height), 1, 1);
            }
        }
        transform.localScale *= ScaleFactor;
    }
}
