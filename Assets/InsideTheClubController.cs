using System.Collections;
using System.Collections.Generic;
using Assets.Utils;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;

public class InsideTheClubController : MonoBehaviour
{
    void Start()
    {
        Global.WhoHasMouseControl = Mouser.Cutscene;

        var audio = GetComponent<AudioSource>();
        audio.volume = 0.0f;

        this.BeginSerial()
            .Then(SceneTransition.FadeTime, ratio => audio.volume = ratio)
            .Start()
            .Then(() => EnterTheClub());
    }

    private void EnterTheClub()
    {
        Conversation.Instance.StartConversation(c =>
        {
            c.Add("Cass", "Go get me a juice, Alice. I'm gonna flirt with that guy.");
            c.Add("Alice", "...... Fine");
        })
        .Then(() =>
        {
            this.MoveToMark("Cass", "CassMark", 1.0f);
        });
    }
}
