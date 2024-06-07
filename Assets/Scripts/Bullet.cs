using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bullet : MonoBehaviour
{
    //도탄 충돌 tag
    public float speed;    //총알 속도
    public string type = "normal";

    [HideInInspector]
    public Vector3[] positions;
    private int curPositionIndex = 0;

    private Rigidbody2D rigid;

    private float cameraWidth;
    private float cameraHeight;

    [SerializeField]
    private ParticleSystem bulletParticle;

    [SerializeField]
    private AudioClip[] audioClips;
    private AudioSource audioSource;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        cameraHeight = Camera.main.orthographicSize;
        cameraWidth = Camera.main.aspect * cameraHeight;

        //rigid.velocity = transform.right.normalized * speed;
    }

    void Update()
    {
        if(type.Equals("reflect"))
        {
            if(curPositionIndex < positions.Length)
            {
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, positions[curPositionIndex], step);
                //Debug.Log(positions[curPositionIndex]);
                //현재 목표 지점 도착 시
                if (Vector3.Distance(positions[curPositionIndex], transform.position) == 0f)
                {
                    //다음 목표 지점 & 방향 설정
                    curPositionIndex++;
                    if(curPositionIndex < positions.Length)
                    {
                        LookAt(positions[curPositionIndex]);
                    }

                    //최초 발사 이외의 경우 파티클 생성
                    if (curPositionIndex != 1)
                    {
                        ParticleSystem particle = Instantiate(bulletParticle, transform.position, Quaternion.identity);
                        bulletParticle.transform.position = transform.position;
                        bulletParticle.Play();
                        PlayRicochetSound();
                    }
                }
            }
            //마지막 경로에 도착 시 오브젝트 파괴
            else
            {   
                SetDestroy();
            }
        }
        if (isOutOfCamera()) SetDestroy();

        //현재 진행방향

        RaycastHit2D hit = Physics2D.Raycast(rigid.position, transform.right, 0.5f);

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Enemy"))
        {
            Enemy enemyLogic = hit.collider.gameObject.GetComponent<Enemy>();
            enemyLogic.Kill();
        }
    }

    //총알 방향 제어
    private void LookAt(Vector3 target)
    {
        Vector2 newPos = target - transform.position;
        float rotZ = Mathf.Atan2(newPos.y, newPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
    }

    private bool isOutOfCamera()
    {
        //카메라 범위 밖으로 나갈 시 true 반환
        if (Mathf.Abs(transform.position.x) > cameraWidth) return true;
        if (Mathf.Abs(transform.position.y) > cameraHeight) return true;
        return false;
    }

    private void SetDestroy()
    {
        //총알 파괴 전 클리어 여부 판단
        if (GameManager.Instance != null && GameManager.Instance.IsClearStage())
        {
            GameManager.Instance.ClearStage();
        }
        Destroy(gameObject);
    }

    //랜덤 도탄 사운드
    private void PlayRicochetSound()
    {     
        audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
        audioSource.Play();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Obstacle"))
        {
            Vector2 normal = collision.contacts[0].normal;
            Vector2 dir = Vector2.Reflect(rigid.velocity, normal).normalized;

            rigid.velocity = dir * speed;
        }
    }
}
