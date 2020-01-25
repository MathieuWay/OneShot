using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Phase
{
    Tactical,
    Combat
}
public class LevelController : MonoBehaviour
{
    private static LevelController instance;
    public static LevelController Instance
    {
        get
        {
            if (instance != null)
                return instance;
            GameObject inScene = GameObject.Find("LevelController");
            if (inScene)
            {
                instance = inScene.GetComponent<LevelController>();
                return instance;
            }
            else
            {
                Debug.LogError("no LevelController instance in scene");
                return null;
            }
        }
    }

    public delegate void StartCombat();
    public static event StartCombat OnCombat;

    public Phase phase;

    private void Awake()
    {
        instance = this;
        Fader.OnFadeIn += FocusOnPlayer;
    }


    private void FocusOnPlayer()
    {
        if(phase != Phase.Combat)
            CameraController.Instance.FocusOnPlayer();
    }
}
