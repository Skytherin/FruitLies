using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : MonoBehaviour
{
    private Vector2? ClickedPosition = null;
    
    [HideInInspector]
    public Vector2? OverridePosition = null;

    [HideInInspector]
    public Action OnArrival = null;

    private float speed = 0.05f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Conversation.Started) return;

        if (Input.GetMouseButtonDown(0))
        {
            ClickedPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        var desiredPosition = OverridePosition ?? ClickedPosition;

        if (desiredPosition is {} dp)
        {
            if (Vector2.Distance(transform.position, dp) > 0.1f)
            {
                transform.position = Vector2.Lerp(transform.position, dp, speed);
            }
            else
            {
                ClickedPosition = null;
                OverridePosition = null;
                OnArrival?.Invoke();
                OnArrival = null;
            }
        }
    }
}
