using Assets;
using Assets.Utils;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;

public class OutsideTheClubSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Global.WhoHasMouseControl = Mouser.Cutscene;

        var mc = GameObject.Find("MainCharacter");
        var cass = GameObject.Find("Cass");
        var cassMarkStart = GameObject.Find("CassMarkStart");
        var cassMarkEnd = GameObject.Find("CassMarkEnd");
        var mcMarkStart = GameObject.Find("MCMarkStart");
        var mcMarkEnd = GameObject.Find("MCMarkEnd");

        cass.TeleportTo(cassMarkStart);
        mc.TeleportTo(mcMarkStart);

        cass.LookLeft();
        mc.LookLeft();

        this.BeginSerial()
            .Wait(SceneTransition.FadeTime)
            .Start()
            .Then(() => this.BeginParallel()
                .MoveTo(cass, cassMarkEnd, 1.0f)
                .MoveTo(mc, mcMarkEnd, 1.0f)
                .Then(() => BeginConversationWithCass())
                .Start());
    }

    private void BeginConversationWithCass()
    {
        Conversation.Instance.StartConversation(c =>
        {
            c.Add(Constants.Names.BF, "Oh no, that bouncer won't let us in!", BoxType.Left);
            c.Add(Constants.Names.MC, "No worries, I'll handle him.", BoxType.Left);
            c.Add(Constants.Names.BF, "What are you gonna do?", BoxType.Left);
            c.Add(Constants.Names.MC, "Lie our way in, of course.", BoxType.Left);
            c.Add(Constants.Names.BF, "But you're a terrible liar!", BoxType.Left);
            c.Add(Constants.Names.MC, "I lied to mom and dad. Everything is permitted now.", BoxType.Left);
        });
    }
}
