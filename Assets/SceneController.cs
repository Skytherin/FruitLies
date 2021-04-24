using System.Collections;
using System.Collections.Generic;
using Assets.ProceduralAnimationLibrary.Tweeners;
using Assets.Utils;
using Assets.Utils.ProceduralAnimationLibrary.Tweeners;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (var canvasName in new[] {"ConversationCanvas"})
        {
            var canvas = GameObject.Find(canvasName);
            var canvasGroup = canvas.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0.0f;
        }

        Global.WhoHasMouseControl = Mouser.General;

        this.BeginSerial()
            .Wait(0.01f)
            .Start(() => StartFirstConversation());
    }

    private void StartFirstConversation()
    {
        Conversation.Instance.StartConversation("", c =>
        {
            c.Add("Cass", "Hey, The Apple Sauce Jam Band is playing tonight. You want to go see them?");
            c.Add("Alice", "Yeah, they're totally boppers!... But my dad will never let them go.");
            c.Add("Cass", "Tell them your coming over my place to study.");
            c.Add("Alice",
                "Yeah, ok I guess so. I don't like lying to my parents; they say the best policy is honesty.");
            c.Add("Cass", "Don't be a wet noodle, Alice.");
            c.Add("Alice", "I'm not a wet noodle, I'm a fresh banana!");
        });
    }
}
