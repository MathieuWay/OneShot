using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace oneShot
{
    [RequireComponent(typeof(Volume))]
    public class PostProcessController : MonoBehaviour
    {
        [Header("Chromatic Aberration")]
        public AnimationCurve chromaticAberrationCurve;
        public float chromaticAberrationDecay = 0.5f;

        [Header("Lens Distortion")]
        public float lensDistortionIntensity;

        [Header("Film Grain")]
        public float filmGrainIntensity;

        public float SlowmotionPPTransitionDuration = 0f;

        private Volume volume = null;
        private VolumeProfile profile = null;
        private ChromaticAberration chromaticAberration = null;
        private LensDistortion lensDistortion = null;
        private FilmGrain filmGrain = null;
        public bool debug;

        private void Awake()
        {
            volume = GetComponent<Volume>();
            profile = volume.profile;
            if(!profile.TryGet<ChromaticAberration>(out chromaticAberration)){
                Debug.LogError("Can't access chromatique aberration from current profile");
            }
            if (!profile.TryGet<LensDistortion>(out lensDistortion))
            {
                Debug.LogError("Can't access lens Distortion from current profile");
            }
            if (!profile.TryGet<FilmGrain>(out filmGrain))
            {
                Debug.LogError("Can't access film Grain from current profile");
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            ComboController.Instance.NextInputEvent += () =>
            {
                float chromaIntensity = chromaticAberrationCurve.Evaluate(CameraController.Instance.comboInputStep);
                chromaticAberration.intensity.Override(chromaIntensity);
                if (!chromaticAberration.active)
                    chromaticAberration.active = true;
            };

            GameTime.Instance.OnStartSlowMotion += (float duration) =>
            {
                StartCoroutine(VolumeComponentTransition(0, lensDistortionIntensity, SlowmotionPPTransitionDuration, lensDistortion));
                StartCoroutine(VolumeComponentTransition(0, filmGrainIntensity, SlowmotionPPTransitionDuration, filmGrain));
                duration -= SlowmotionPPTransitionDuration;
                StartCoroutine(VolumeComponentTransition(lensDistortionIntensity, 0, SlowmotionPPTransitionDuration, lensDistortion, duration));
                StartCoroutine(VolumeComponentTransition(filmGrainIntensity, 0, SlowmotionPPTransitionDuration, filmGrain, duration));
            };
        }

        // Update is called once per frame
        void Update()
        {
            if (chromaticAberration.active)
            {
                float intensity = chromaticAberration.intensity.GetValue<float>();
                if (intensity > 0f)
                {
                    intensity -= chromaticAberrationDecay * Time.deltaTime;
                    chromaticAberration.intensity.Override(intensity);
                }
                else
                {
                    chromaticAberration.active = false;
                }
            }
            /*
             * DEBUG
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (debug)
                {
                    debug = false;
                    StartCoroutine(VolumeComponentTransition(lensDistortionIntensity, 0, SlowmotionPPTransitionDuration, lensDistortion));
                    StartCoroutine(VolumeComponentTransition(filmGrainIntensity, 0, SlowmotionPPTransitionDuration, filmGrain));
                }
                else
                {
                    debug = true;
                    StartCoroutine(VolumeComponentTransition(0, lensDistortionIntensity, SlowmotionPPTransitionDuration, lensDistortion));
                    StartCoroutine(VolumeComponentTransition(0, filmGrainIntensity, SlowmotionPPTransitionDuration, filmGrain));
                }
            }*/
        }

        IEnumerator VolumeComponentTransition(float start, float end, float duration, VolumeComponent component, float delay = 0)
        {
            if(delay != 0)
                yield return new WaitForSeconds(delay);
            float startTime = Time.time;
            float norm = 0;
            while (norm < 1)
            {
                norm = (Time.time - startTime) / duration;
                float intensity = Mathf.Lerp(start, end, norm);
                switch (component)
                {
                    case LensDistortion lensDistortion:
                        lensDistortion.intensity.Override(intensity);
                        break;
                    case FilmGrain filmGrain:
                        filmGrain.intensity.Override(intensity);
                        break;
                }
                yield return null;
            }
        }
    }
}
