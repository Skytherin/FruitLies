using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var conversationCanvas = GameObject.Find("ConversationCanvas");
        var canvasGroup = conversationCanvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;
        StartFirstConversation();
    }

    private void StartFirstConversation()
    {
        Conversation.StartConversation(c =>
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
