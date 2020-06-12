using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace oneShot
{
	public class BulletBehaviour : MonoBehaviour
	{
		[SerializeField] private Rigidbody2D rb = null;
		[SerializeField] private float speed = 100;

		private void Start()
		{
			rb.velocity = transform.up * speed;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if(collision.CompareTag("Obstacle") || collision.CompareTag("Player"))
			{
				Destroy(gameObject);
			}
		}
	}
}