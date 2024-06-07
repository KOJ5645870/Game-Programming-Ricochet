using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Helicopter : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    bool isClicked;

    //black -> white 코루틴
    IEnumerator ToWhite()
    {
        while (true)
        {
            //0.01초마다 전체값 증가
            float n = spriteRenderer.color.r + 0.02f;
            spriteRenderer.color = new Color(n, n, n);
            yield return new WaitForSeconds(0.01f);

            if (!gameObject.activeSelf) yield break;

            if (spriteRenderer.color.r >= 0.98f)
            {
                StartCoroutine(ToBlack());
                yield break;
            }

            if(isClicked)
            {
                spriteRenderer.color = Color.black;
                yield break;
            }
        }
    }
    //white -> black 코루틴
    IEnumerator ToBlack()
    {
        while (true)
        {
            //0.01초마다 전체값 감소
            float n = spriteRenderer.color.r - 0.02f;
            spriteRenderer.color = new Color(n, n, n);
            yield return new WaitForSeconds(0.01f);

            if (!gameObject.activeSelf) yield break;

            if (spriteRenderer.color.r <= 0.02f)
            {
                StartCoroutine(ToWhite());
                yield break;
            }

            if (isClicked)
            {
                spriteRenderer.color = Color.black;
                yield break;
            }
        }
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        isClicked = false;
        StartCoroutine(ToWhite());
    }

    //드래그 이동
    public void OnMouseDrag()
    {
        isClicked = true;

        //클릭 위치
        Vector3 point = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        //클릭위치 -> 월드 위치 변환
        Vector3 objPoint = Camera.main.ScreenToWorldPoint(point);

        transform.position = objPoint;
    }
}
