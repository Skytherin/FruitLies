using Assets;
using Assets.Utils;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;

public class HomeSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Global.WhoHasMouseControl = Mouser.Cutscene;

        this.BeginSerial()
            .Wait(0.01f)
            .Start()
            .Then(() => StartFirstConversation());

        foreach (var item in new[]
        {
            new {c = "MainCharacter", m = "MCStartMark"},
            new {c = "Dad", m = "DadStartMark"},
            new {c = "Mom", m = "MomStartMark"}
        })
        {
            GameObject.Find(item.c).TeleportTo(GameObject.Find(item.m));
        }

        Debug.Log(Camera.main.pixelRect);
    }

    private void StartFirstConversation()
    {
        Conversation.Instance.StartConversation(c =>
        {
            c.Add(Constants.Names.BF, "Hey, The Orange Peels are playing tonight at Juicy Lucy's. Wanna go?", BoxType.Right);
            c.Add(Constants.Names.MC, "Yeah, they're totally boppers!... But my dad will never let them go.", BoxType.Right);
            c.Add(Constants.Names.BF, "Tell them your coming over my place to study.", BoxType.Right);
            c.Add(Constants.Names.MC,
                "Yeah, ok I guess so. I don't like lying to my parents; they say the best policy is honesty.", BoxType.Right);
            c.Add(Constants.Names.BF, "Don't be a wet noodle, Alice.", BoxType.Right);
            c.Add(Constants.Names.MC, "I'm not a wet noodle, I'm a fresh banana!", BoxType.Right);
        });
    }
}
