using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Sniper : MonoBehaviour
{
    private VerticalJoystick joystick;

    [SerializeField] private GameObject bullet;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    public float rotateSpeed = 20;


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        joystick = GameObject.FindObjectOfType<VerticalJoystick>();
    }

    void Update()
    {
        //���̽�ƽ ȸ��
        transform.Rotate(0, 0, joystick.Vertical * Time.deltaTime * rotateSpeed);

        /*
        if (Input.GetKeyDown(KeyCode.Mouse0) && canShot())
        {
            Laser laserLogic = gameObject.GetComponent<Laser>();
            //�̵� ��� ����
            int amount = laserLogic.lineRenderer.GetPositions(new Vector3[laserLogic.reflections]);
            Vector3[] positions = new Vector3[amount];
            for (int i = 0; i < amount; i++)          
            {
                positions[i] = (laserLogic.lineRenderer.GetPosition(i));
            }
            GameObject bulletObj = Instantiate(bullet, transform.position, transform.rotation);
            Bullet bulletLogic = bulletObj.GetComponent<Bullet>();
            //�Ѿ˿� �̵���� ����
            bulletLogic.positions = positions;
        }
        */
    }

    public void ShotBullet()
    {
        if (canShot())
        {
            Laser laserLogic = gameObject.GetComponent<Laser>();
            //�̵� ��� ����
            int amount = laserLogic.lineRenderer.GetPositions(new Vector3[laserLogic.reflections]);
            Vector3[] positions = new Vector3[amount];

            for (int i = 0; i < amount; i++)
            {
                positions[i] = (laserLogic.lineRenderer.GetPosition(i));
            }

            GameObject bulletObj = Instantiate(bullet, transform.position, transform.rotation);
            Bullet bulletLogic = bulletObj.GetComponent<Bullet>();
            //��ź Ÿ�� ����
            bulletLogic.type = "reflect";
            //�Ѿ˿� �̵���� ����
            bulletLogic.positions = positions;

            //�Ѿ� �߻� ���� ���
            audioSource.Play();
            //�Ѿ� �߻� Ƚ�� ����
            GameManager.Instance.shotCount++;
        }
    }

    private bool canShot()
    {
        return GameObject.FindGameObjectsWithTag("Bullet").Length == 0;
    }
}
