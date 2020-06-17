using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimEventTimeline : MonoBehaviour
{
    [System.Serializable]
    public class AnimTriggerEvent
    {
        public float Time;
        public AnimationClip Clip;
    }
    public List<AnimTriggerEvent> triggers = new List<AnimTriggerEvent>();
    AnimTriggerEvent currentTrigger;
    Animator anim = null;

    // Start is called before the first frame update
    void Awake()
    {
        if (triggers.Count == 0)
            this.enabled = false;
        anim = GetComponent<Animator>();
        List<AnimationClip> clips = new List<AnimationClip>(anim.runtimeAnimatorController.animationClips);
        int i = 0;
        while (i < triggers.Count)
        {
            if (!clips.Contains(triggers[i].Clip))
                break;
            i++;
        }

        if(i != triggers.Count)
        {
            Debug.LogError("Clip not found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if(GameTime.Instance)
        //    anim.speed = GameTime.Instance.TimeSpeed;
        float currentTime = 0f;
        if (UI_Timeline.Instance)
            currentTime = UI_Timeline.Instance.GetCurrentTime();

        currentTrigger = GetCurrentClipTrigger(currentTime);
        if(currentTrigger != null)
        {
            anim.Play(currentTrigger.Clip.name, 0, GetNormalizedTime(currentTime, currentTrigger));
        }
    }

    private AnimTriggerEvent GetCurrentClipTrigger(float time)
    {
        int i = triggers.Count-1;
        while(i >= 0)
        {
            if (time >= triggers[i].Time)
                return triggers[i];
            i--;
        }
        return triggers[0];
    }

    private float GetNormalizedTime(float time, AnimTriggerEvent trigger)
    {
        return Mathf.Clamp((time - trigger.Time) / trigger.Clip.length, 0, 1);
    }
}

