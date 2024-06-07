using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VariableJoystick : Joystick
{
    private Vector2 fixedPosition = Vector2.zero;

    protected override void Start()
    {
        base.Start();
        fixedPosition = background.anchoredPosition;

        background.anchoredPosition = fixedPosition;
        background.gameObject.SetActive(true);       
    }

    public override void OnPointerDown(PointerEventData eventData)
    {       
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
    }

    protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        base.HandleInput(magnitude, normalised, radius, cam);
    }
}