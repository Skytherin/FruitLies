using System.Collections;
using System.Collections.Generic;
using Assets.Utils;
using UnityEngine;

public class JuicyLucyEntrance : MonoBehaviour
{
    void OnMouseDown()
    {
        if (Global.WhoHasMouseControl == Mouser.General)
        {
            SceneTransition.Instance.TransitionTo("InsideTheClub");
        }
    }
}
