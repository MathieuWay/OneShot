using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    private Animator anim;

	//EVENT FADER
	public delegate void EventFadeIn();
	public static event EventFadeIn OnFadeIn;

	public delegate void EventFadeOut();
	public static event EventFadeOut OnFadeOut;

	private static Fader instance;
    public static Fader Instance
    {
        get{
            if (instance != null)
                return instance;
            GameObject inScene = GameObject.Find("Fader");
            if(inScene)
            {
                instance = inScene.GetComponent<Fader>();
                return instance;
            }
            else
            {
                Debug.LogError("no Fader instance in scene");
                return null;
            }
        }
    }

    public void FadeIn()
    {
        anim.SetTrigger("fadeIn");
    }

    public void FadeOut()
    {
        anim.SetTrigger("fadeOut");
    }

    private void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        instance = this;
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        //StartCoroutine(DelayEvent("in"));
    }
    /*
    public IEnumerator DelayEvent(string state)
    {
        yield return new WaitForSeconds(1f);
        if (state == "in")
            OnFadeIn();
        else
            OnFadeOut();
    }
    */
}
