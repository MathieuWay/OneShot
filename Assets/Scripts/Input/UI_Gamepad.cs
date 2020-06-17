using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace oneShot
{
	public class UI_Gamepad : MonoBehaviour
	{
		public static UI_Gamepad Instance { get; private set; }

		[System.Serializable]
		public class UI_Input
		{
			[SerializeField] private InputName inputName = InputName.X;
			[SerializeField] private Sprite inputSprite = null;
            [SerializeField] private Sprite inputKeyboardSprite = null;
            public InputName InputName { get => inputName; }
			public Sprite InputSprite {
                get
                {
                    if(Gamepad.Instance.type == ControllerType.Gamepad)
                        return inputSprite;
                    else
                        return inputKeyboardSprite;
                }
            }
        }

		[SerializeField] private UI_Input[] uiInputs = null;
		private Dictionary<InputName, UI_Input> uiInputDic;


		public Sprite GetInputSprite(InputName inputName)
        {
            return uiInputDic[inputName].InputSprite;
        }

		private void Awake()
		{
			if(Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;

			uiInputDic = new Dictionary<InputName, UI_Input>();

			for (int i = 0; i < uiInputs.Length; i++)
			{
				uiInputDic.Add(uiInputs[i].InputName, uiInputs[i]);
			}
		}
	}
}