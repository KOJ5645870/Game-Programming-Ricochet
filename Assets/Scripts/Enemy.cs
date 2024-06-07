using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem deathParticle;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    //black to red �ڷ�ƾ
    IEnumerator ToRed()
    {
        while (true)
        {
            //0.01�ʸ��� r�� ����
            spriteRenderer.color = new Color(spriteRenderer.color.r + 0.01f, 0, 0);
            yield return new WaitForSeconds(0.01f);

            if (!gameObject.activeSelf) yield break;

            if (spriteRenderer.color.r >= 0.99f)
            {
                StartCoroutine(ToBlack());
                yield break;
            }
        }
    }
    //red -> black �ڷ�ƾ
    IEnumerator ToBlack()
    {
        while (true)
        {
            //0.01�ʸ��� r�� ����
            spriteRenderer.color = new Color(spriteRenderer.color.r - 0.01f, 0, 0);
            yield return new WaitForSeconds(0.01f);

            if(!gameObject.activeSelf) yield break;

            if (spriteRenderer.color.r <= 0.01f)
            {
                StartCoroutine(ToRed());
                yield break;
            }
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        StartCoroutine (ToRed());
    }

    public void Kill()
    {
        if(gameObject.activeSelf)
        {
            ParticleSystem particle = Instantiate(deathParticle, transform.position, Quaternion.identity);
            deathParticle.transform.position = transform.position;
            deathParticle.Play();
            audioSource.Play();

            gameObject.SetActive(false);
        }
    }

    public void Revive()
    {
        gameObject.SetActive(true);
    }
}
