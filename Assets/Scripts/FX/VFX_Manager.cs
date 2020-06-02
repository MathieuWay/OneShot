using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace oneShot
{
	public class VFX_Manager : MonoBehaviour
	{
		public static VFX_Manager Instance { get; private set; }

		[SerializeField] private VFX_Data[] vfxDatas = null;

		[System.Serializable]
		public class VFX_Data
		{
			[SerializeField] private string vfxName = string.Empty;
			[SerializeField] private GameObject vfxPrefab = null;
			public string VfxName { get => vfxName; }
			public GameObject VfxPrefab { get => vfxPrefab; }
		}

		public void PlayVFX(string vfxName, Vector3 position)
		{
			GameObject instance = Instantiate(GetVFX(vfxName), position, Quaternion.identity);
			//Destruction VFX après 10s par défaut
			Destroy(instance, 10);
		}

		private void Awake()
		{
			if(Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
		}
		private GameObject GetVFX(string vfxName)
		{
			for (int i = 0; i < vfxDatas.Length; i++)
			{
				if(vfxDatas[i].VfxName == vfxName)
				{
					return vfxDatas[i].VfxPrefab;
				}
			}

			Debug.LogError("ERROR: VFX NOT FOUND");
			return null;
		}
	}
}