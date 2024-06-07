using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VerticalJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public float Vertical { get { return input.y; } }

    public float HandleRange
    {
        get { return handleRange; }
        set { handleRange = Mathf.Abs(value); }
    }

    public float DeadZone
    {
        get { return deadZone; }
        set { deadZone = Mathf.Abs(value); }
    }

    [SerializeField] private float handleRange = 1;
    [SerializeField] private float deadZone = 0;

    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;

    private Canvas canvas;
    private Camera cam;

    //입력 위치
    private Vector2 input = Vector2.zero;

    private void Start()
    {
        HandleRange = handleRange;
        DeadZone = deadZone;
        canvas = GetComponentInParent<Canvas>();

        //기본 세팅
        Vector2 center = new Vector2(0.5f, 0.5f);
        background.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //UI위치
        Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        Vector2 radius = background.sizeDelta / 2;
        //위치값 가공
        input = (eventData.position - position) / (radius * canvas.scaleFactor);
        input = new Vector2(0f, input.y);

        //최대이동 제한
        HandleInput(input.magnitude, input.normalized, radius, cam);
        //위치 이동
        handle.anchoredPosition = input * radius * handleRange;
    }

    private void HandleInput(float magnitude, Vector2 normal, Vector2 radius, Camera cam)
    {
        if (magnitude > deadZone)
        {
            if(magnitude > 1) input = normal;
        }
        else input = Vector2.zero;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        //위치 초기화
        handle.anchoredPosition = Vector2.zero;
    }
}