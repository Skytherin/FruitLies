using System.Collections;
using System.Collections.Generic;
using Assets.ProceduralAnimationLibrary.Tweeners;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    void Start()
    {
        Instance = this;

        var canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1.0f;

        this.BeginSerial()
            .Then(0.5f, ratio => canvasGroup.alpha = 1.0f - ratio)
            .Then(() => gameObject.SetActive(false))
            .Start();
    }

    public void TransitionTo(string nextScene)
    {
        var canvasGroup = GetComponent<CanvasGroup>();
        gameObject.SetActive(true);
        this.BeginSerial()
            .Then(0.5f, ratio => canvasGroup.alpha = ratio)
            .Then(() => SceneManager.LoadScene(nextScene))
            .Start();
    }
}
