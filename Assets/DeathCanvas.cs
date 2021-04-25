using System.Collections;
using System.Collections.Generic;
using Assets.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using CanvasGroup = UnityEngine.CanvasGroup;

public class DeathCanvas : MonoBehaviour
{
    public static DeathCanvas Instance;

    void Start()
    {
        Instance = this;
        gameObject.SetActive(false);
        GetComponent<CanvasGroup>().alpha = 1.0f;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        Global.WhoHasMouseControl = Mouser.Death;
    }

    public void PlayAgain()
    {
        if (Global.WhoHasMouseControl != Mouser.Death) return;

        SceneTransition.Instance.TransitionTo("InsideTheHouse");
    }

    public void Quit()
    {
        if (Global.WhoHasMouseControl != Mouser.Death) return;

        Application.Quit();
    }
}
