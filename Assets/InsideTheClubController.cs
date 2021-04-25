using System.Collections;
using System.Collections.Generic;
using Assets.Utils;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;

public class InsideTheClubController : MonoBehaviour
{
    void Start()
    {
        Global.WhoHasMouseControl = Mouser.General;

        this.BeginSerial()
            .Wait(SceneTransition.FadeTime)
            .Start(() => EnterTheClub());
    }

    private void EnterTheClub()
    {
        Conversation.Instance.StartConversation(c =>
        {
            c.Add("Cass", "Go get my a juice, Alice. I'm gonna flirt with that guy.");
            c.Add("Alice", "...... Fine");
        });
    }

}
