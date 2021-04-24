using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Conversation : MonoBehaviour
{
    public static bool Started { get; private set; } = false;
    private readonly ConversationFlow ConversationFlow = new ConversationFlow();
    private int ConversationIndex = -1;
    private GameObject ConversationCanvas;
    private CanvasGroup CanvasGroup;
    private TextMeshProUGUI Who;
    private TextMeshProUGUI What;
    private Button Q1;
    private Button Q2;
    private Button Q3;

    public static void StartConversation(Action<ConversationFlow> script)
    {
        GameObject.Find("ConversationController").GetComponent<Conversation>().StartConversationFromScript(script);
    }

    private void StartConversationFromScript(Action<ConversationFlow> script)
    {
        Assert.IsFalse(Started, "Attempted to start two conversations in a row!");
        ConversationFlow.Items.Clear();
        script(ConversationFlow);
        StartConversationInternal();
    }

    private void Initialize()
    {
        if (ConversationCanvas == null)
        {
            ConversationCanvas = GameObject.Find("ConversationCanvas");
            CanvasGroup = ConversationCanvas.GetComponent<CanvasGroup>();
            Who = ConversationCanvas.transform.Find("Who").GetComponent<TextMeshProUGUI>();
            What = ConversationCanvas.transform.Find("What").GetComponent<TextMeshProUGUI>();
            Q1 = ConversationCanvas.transform.Find("Q1").GetComponent<Button>();
            Q2 = ConversationCanvas.transform.Find("Q2").GetComponent<Button>();
            Q3 = ConversationCanvas.transform.Find("Q3").GetComponent<Button>();
        }
    }

    private void StartConversationInternal()
    {
        Initialize();
        Started = true;
        Q1.transform.gameObject.SetActive(false);
        Q2.transform.gameObject.SetActive(false);
        Q3.transform.gameObject.SetActive(false);
        CanvasGroup.alpha = 1.0f;
        ConversationIndex = 0;
        ShowCurrentIndex();
    }

    private void ShowCurrentIndex()
    {
        var item = ConversationFlow.Items[ConversationIndex];
        Who.text = item.Who;
        What.text = item.Text;

        foreach (var a in item.Answers.Zip(new[]{ Q1, Q2, Q3 }, (answer, button) => new {answer,button})
            .Select((it,index) => new { it.answer, it.button, index}))
        {
            a.button.transform.gameObject.SetActive(true);
            a.button.GetComponentInChildren<TextMeshProUGUI>().text = a.answer;
            a.button.onClick = new Button.ButtonClickedEvent();
            a.button.onClick.AddListener(() => Answer(a.index));
        }
    }

    private void Answer(int which)
    {
        Q1.transform.gameObject.SetActive(false);
        Q2.transform.gameObject.SetActive(false);
        Q3.transform.gameObject.SetActive(false);
        AdvanceConversation();
    }

    public void Update()
    {
        if (ConversationIndex >= 0 && Input.GetMouseButtonDown(0))
        {
            if (Q1.transform.gameObject.activeSelf ||
                Q1.transform.gameObject.activeSelf ||
                Q1.transform.gameObject.activeSelf)
            {
                return;
            }

            AdvanceConversation();
        }
    }

    private void AdvanceConversation()
    {
        ConversationIndex += 1;
        if (ConversationIndex >= ConversationFlow.Items.Count)
        {
            LeaveConversation();
            return;
        }

        ShowCurrentIndex();
    }

    private void LeaveConversation()
    {
        Q1.transform.gameObject.SetActive(true);
        Q2.transform.gameObject.SetActive(true);
        Q3.transform.gameObject.SetActive(true);
        CanvasGroup.alpha = 0.0f;
        ConversationIndex = -1;
        Started = false;
    }
}

public class ConversationFlow
{
    public List<ConversationItem> Items = new List<ConversationItem>();

    public void Add(string who, string what) => Items.Add(new ConversationItem(who, what));
}

public class ConversationItem
{
    public string Who;
    public string Text;
    public List<string> Answers = new List<string>();
    public string Backreference;

    public ConversationItem(string who, string text)
    {
        Who = who;
        Text = text;
    }
}