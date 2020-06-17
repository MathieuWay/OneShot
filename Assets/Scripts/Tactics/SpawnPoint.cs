using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class SpawnPoint : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI numberText = null;
	[SerializeField] private SpriteRenderer spriteRenderer = null;
	[SerializeField] private Sprite pointMove = null;
	[SerializeField] private Sprite pointSelected = null;
	[SerializeField] private Sprite pointDefault = null;
	public int _ID { get; set; }
	public float _Time { get; private set; }
	public Vector2 _Position { get; private set; }
	public GameObject _GameObject { get; private set; }
    /*
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    */
    public void Init(int id, Vector3 root)
	{
		SetID(id);
		_Position = root;
		_GameObject = gameObject;
	}

	public void SetTime(float time)
	{
		_Time = time;
	}

	public void SetID(int id)
	{
		_ID = id;
		numberText.text = (id + 1).ToString();
	}

	public void UpdatePosition(Vector3 root)
	{
		_Position = root;
	}

	public void Select()
	{
		spriteRenderer.sprite = pointSelected;
		//spriteRenderer.color = new Color(1, 0.7f, 0);
	}
	public void Unselect()
	{
		spriteRenderer.sprite = pointDefault;
		//spriteRenderer.color = Color.red;
	}
	public void Grab()
	{
		spriteRenderer.sprite = pointMove;
		//spriteRenderer.color = Color.green;
	}
}