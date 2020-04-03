using UnityEngine;

[ExecuteInEditMode]
public class QuadTextureRepeat : MonoBehaviour
{
	//Texture Offset
	public Vector2 textureOffset = Vector2.zero;
	private Vector2 _currentTextureOffset = Vector2.zero;

	//Texture Scale
	public Vector2 textureScale = Vector2.one;
	private Vector2 _currentTextureScale = Vector2.one;

	private Vector3 _currentScale = Vector3.zero;


	private void Update()
	{
		if(_currentScale == transform.localScale && _currentTextureOffset == textureOffset && _currentTextureScale == textureScale) return;

		_currentScale = transform.localScale;
		_currentTextureOffset = textureOffset;

		CalculateQuadUvs();

		Renderer renderer = GetComponent<Renderer>();
		if (renderer != null && renderer.sharedMaterial.mainTexture.wrapMode != TextureWrapMode.Repeat)
		{
			renderer.sharedMaterial.mainTexture.wrapMode = TextureWrapMode.Repeat;
		}
	}
	public void CalculateQuadUvs()
	{
#if UNITY_EDITOR
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh meshCopy = Instantiate(meshFilter.sharedMesh);
		meshFilter.mesh = meshCopy;
		Mesh mesh = meshCopy;
		mesh.uv = SetupUvMap(mesh.uv);
#else
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;
		mesh.uv = SetupUvMap(mesh.uv);
#endif

		mesh.name = "Quad Repeat";
	}

	private Vector2[] SetupUvMap(Vector2[] meshUVs)
	{
		float left = textureOffset.x;

		float right = transform.localScale.x;
		if (textureScale.x > 0)
		{
			right /= textureScale.x;
		}
		right += textureOffset.x;

		float bottom = textureOffset.y;

		float top = transform.localScale.y;
		if(textureScale.y > 0)
		{
			top /= textureScale.y;
		}
		top += textureOffset.y;

		meshUVs[0] = new Vector2(left, bottom);
		meshUVs[1] = new Vector2(right, bottom);
		meshUVs[2] = new Vector2(left, top);
		meshUVs[3] = new Vector2(right, top);

		return meshUVs;
	}
}
