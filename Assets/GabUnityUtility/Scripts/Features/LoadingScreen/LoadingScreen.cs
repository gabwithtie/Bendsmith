using GabUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LoadingScreen : MonoSingleton<LoadingScreen>
{
    /// <summary>
    /// this animator should have 2 animations
    /// animation[1] = entry anim
    /// animation[2] = exit anim
    /// The animation should control the "enabled" property of the signaller object
    /// </summary>
    [SerializeField] private Animation blocker_animator;
    [SerializeField] private AnimationClip entry_anim;
    [SerializeField] private AnimationClip exit_anim;

    public static void LoadScreen(Action do_when_loading)
    {Instance.StopAllCoroutines();
        Instance.StartCoroutine(Instance.Load(do_when_loading));
    }

    IEnumerator Load(Action do_when_loading)
    {
        Instance.blocker_animator.Play(Instance.entry_anim.name);

        while (blocker_animator.IsPlaying(entry_anim.name))
        {
            yield return null;
        }

        do_when_loading.Invoke();

        Instance.blocker_animator.Play(Instance.exit_anim.name);
    }
}
