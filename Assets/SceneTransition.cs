using System.Collections;
using System.Collections.Generic;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;
    public const float FadeTime = 2.0f;

    void Start()
    {
        Instance = this;

        var canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1.0f;

        this.BeginSerial()
            .Then(FadeTime, ratio => canvasGroup.alpha = 1.0f - ratio)
            .Then(() => gameObject.SetActive(false))
            .Start();
    }

    public CallbackThing<bool> FadeOutAndIn()
    {
        gameObject.SetActive(true);
        var canvasGroup = GetComponent<CanvasGroup>();
        return this.BeginSerial()
            .Then(FadeTime, ratio => canvasGroup.alpha = ratio)
            .Then(FadeTime, ratio => canvasGroup.alpha = 1.0f - ratio)
            .Then(() => gameObject.SetActive(false))
            .Start();
    }


    public void TransitionTo(string nextScene)
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        gameObject.SetActive(true);
        this.BeginSerial()
            .Then(FadeTime, ratio => canvasGroup.alpha = ratio)
            .Then(() => SceneManager.LoadScene(nextScene))
            .Start();
    }
}
