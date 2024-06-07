using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class MainMenu : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField] private GameObject sniper;
    [SerializeField] private GameObject bullet;
    [SerializeField] private Text startText;
    [SerializeField] private Text exitText;

    [SerializeField] private GameObject menuUISet;
    [SerializeField] private GameObject stageUISet;

    private Vector3 targetPos;
    private Vector3 startPos;
    private Vector3 exitPos;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        //UI위치 -> 월드 위치
        startPos = Camera.main.ScreenToWorldPoint(startText.transform.position);
        exitPos = Camera.main.ScreenToWorldPoint(exitText.transform.position);
    }

    void Update()
    {
        //버튼 타겟 설정됨
        if(targetPos != Vector3.zero)
        {
            Vector2 dir = targetPos - sniper.transform.position;
            float z = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            float preZ = sniper.transform.rotation.z;
            //자연스러운 회전(선형보간)
            sniper.transform.rotation = Quaternion.Lerp(sniper.transform.rotation, Quaternion.Euler(0, 0, z), Time.deltaTime * 10);

            //Debug.Log(preZ + " " + sniper.transform.rotation.z);
            //선형보간의 오차값 처리
            if (Mathf.Abs(preZ - sniper.transform.rotation.z) <= 0.001f)
            {
                ShotBullet();
                //대상 위치별 함수 호출
                if (targetPos == startPos) Invoke("OpenStageUI", 0.25f);
                if (targetPos == exitPos) Invoke("ExitGame", 0.3f);
                targetPos = Vector3.zero;
            }
        }
    }

    private void ShotBullet()
    {
        GameObject bulletObj = Instantiate(bullet, sniper.transform.position, sniper.transform.rotation);
        Bullet bulletLogic = bulletObj.GetComponent<Bullet>();
        Rigidbody2D rigid = bulletObj.GetComponent<Rigidbody2D>();
        //일반 타입 설정
        bulletLogic.type = "normal";
        rigid.velocity = sniper.transform.right * bulletLogic.speed;
        audioSource.Play();
    }

    public void OpenStageUI()
    {
        menuUISet.SetActive(false);
        stageUISet.SetActive(true);
    }

    public void OpenMenuUI()
    {
        menuUISet.SetActive(true);
        stageUISet.SetActive(false);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
    public void ClickStartButton()
    {
        targetPos = startPos;
    }

    public void ClickExitButton()
    {
        targetPos = exitPos;
    }

    public void LoadStage(int level)
    {     
        //스테이지 매니저에 로드할 스테이지 값 저장
        StageManager.smInstance.currentStage = level;

        if(SceneManager.GetActiveScene() != SceneManager.GetSceneByName("InGame"))
            SceneManager.LoadScene("InGame");
    }
}
