using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Helicopter : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    bool isClicked;

    //black -> white �ڷ�ƾ
    IEnumerator ToWhite()
    {
        while (true)
        {
            //0.01�ʸ��� ��ü�� ����
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
    //white -> black �ڷ�ƾ
    IEnumerator ToBlack()
    {
        while (true)
        {
            //0.01�ʸ��� ��ü�� ����
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

    //�巡�� �̵�
    public void OnMouseDrag()
    {
        isClicked = true;

        //Ŭ�� ��ġ
        Vector3 point = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        //Ŭ����ġ -> ���� ��ġ ��ȯ
        Vector3 objPoint = Camera.main.ScreenToWorldPoint(point);

        transform.position = objPoint;
    }
}
