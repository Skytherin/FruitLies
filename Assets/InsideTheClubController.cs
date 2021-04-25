using System.Collections;
using System.Collections.Generic;
using Assets.Utils;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;

public class InsideTheClubController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Global.WhoHasMouseControl = Mouser.General;

        this.BeginSerial()
            .Wait(SceneTransition.FadeTime)
            .Then(EnterTheClub);
    }

    private void EnterTheClub()
    {
        Conversation.Instance.StartConversation("", c =>
        {
            c.Add("Cass", "Go get my a juice, Alice. I'm gonna flirt with that guy.");
            c.Add("Cass", "...... Fine");
        });
    }

}
