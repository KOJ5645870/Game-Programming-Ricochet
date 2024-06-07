using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    [SerializeField] GameObject controlUISet;
    [SerializeField] GameObject clearUISet;
    [SerializeField] GameObject fadeScreen;
    private Image fadeScreenImage;
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] audioClips;

    public GameObject[] stages;
    private GameObject stageObject;

    [HideInInspector] public int curStage = 0;
    [HideInInspector] public float stageStartTime;
    [HideInInspector] public float stageEndTime;
    [HideInInspector] public int shotCount = 0;

    public Text timeText;
    public Text bulletText;
    public Image[] crackImages;


    //페이드 아웃 코루틴
    IEnumerator FadeOut()
    {
        fadeScreenImage.gameObject.SetActive(true);
        while (true)
        {
            //0.01초마다 알파값 증가
            fadeScreenImage.color = new Color(0, 0, 0, fadeScreenImage.color.a + 0.01f);
            yield return new WaitForSeconds(0.01f);

            if (fadeScreenImage.color.a >= 1.0f)
            {
                FadeInScreen();
                EnableStage(curStage);
                yield break;
            }
        }               
    }
    //페이드 인 코루틴
    IEnumerator FadeIn()
    {
        while(true)
        {
            //0.01초마다 알파값 감소
            fadeScreenImage.color = new Color(0, 0, 0, fadeScreenImage.color.a - 0.01f);
            yield return new WaitForSeconds(0.01f);

            if (fadeScreenImage.color.a <= 0.0f)
            {
                fadeScreenImage.gameObject.SetActive(false);
                yield break;
            }
        }      
    }

    IEnumerator EnableCrack(Image image, float delay)
    {
        yield return new WaitForSeconds(delay);

        audioSource.clip = audioClips[0];
        audioSource.Play();
        image.color = new Color(200, 200, 200, 1);
    }

    public static GameManager Instance
    {
        get
        {
            // 인스턴스가 없는 경우 할당
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        fadeScreenImage = fadeScreen.GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();

        if (_instance == null)
        {
            _instance = this;
        }

        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        //씬 전환 시 인스턴스 파괴x.
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        curStage = GameObject.Find("StageManager").GetComponent<StageManager>().currentStage;
        EnableStage(curStage);
    }

    public void FadeInScreen()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeOutScreen()
    {
        fadeScreenImage.gameObject.SetActive(true);
        StartCoroutine(FadeOut());
    }

    public void EnableStage(int index)
    {
        //FadeOutScreen();

        //현재 스테이지 파괴
        if(stageObject != null) GameObject.Destroy(stageObject);
        //스테이지 할당
        stageObject = GameObject.Instantiate(stages[index - 1]);
        stageObject.transform.position = Vector3.zero;

        //모든 스테이지 비활성화
        /*
        foreach (GameObject stage in stages)
        {
            stage.SetActive(false);
        }
        */
        //해당 스테이지 활성화
        //stages[index - 1].SetActive(true);

        //현재 스테이지 저장
        curStage = index;

        //UI세팅
        controlUISet.SetActive(true);
        clearUISet.SetActive(false);
        foreach(Image i in crackImages)
        {
            i.color = new Color(200, 200, 200, 0);
        }

        if (index < 4) setGuideLine(true);
        else setGuideLine(false);

        //클리어 조건 초기화
        stageStartTime = Time.time;
        shotCount = 0;

    }

    public bool IsClearStage()
    {
        //적이 모두 죽었을 때
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            return true;
        }
        else return false;
    }

    //클리어 UI 활성화
    public void ClearStage()
    {
        stageEndTime = Time.time;
        SetResult();

        controlUISet.SetActive(false);
        clearUISet.SetActive(true);
    }

    private void SetResult()
    {
        MissionManager missionManager = GameObject.Find("MissionManager").GetComponent<MissionManager>();

        //결과 텍스트
        int time = (int) (stageEndTime - stageStartTime);
        int min = time / 60;
        int sec = time % 60;

        timeText.text = string.Format("{0:D2}:{1:D2}", min, sec) + " / " + 
            string.Format("{0:D2}:{1:D2}", missionManager.missions[curStage].clearTime.min, missionManager.missions[curStage].clearTime.sec);
        bulletText.text = shotCount + " / " + missionManager.missions[curStage].bulletCount;

        //결과 이미지
        StartCoroutine(EnableCrack(crackImages[0], 1));

        if (missionManager.isClearTimeLimit(curStage))
        {
            StartCoroutine(EnableCrack(crackImages[1], 1.5f));
        }

        if (missionManager.isClearBulletCount(curStage))
        {
            StartCoroutine(EnableCrack(crackImages[2], 2));
        }
    }

    public void setGuideLine(bool isActive)
    {
        foreach(GameObject line in GameObject.FindGameObjectsWithTag("Line"))
        {
            line.SetActive(isActive);
        }
    }

    public void ShotBullet()
    {
        //씬 내의 모든 스나이퍼 대상
        foreach (GameObject sniper in GameObject.FindGameObjectsWithTag("Sniper"))
        {
            //활성화된 스나이퍼에서 총알 발사
            if(sniper.activeSelf)
            {
                Sniper sniperLogic = sniper.GetComponent<Sniper>();
                sniperLogic.ShotBullet();
                break;
            }
        }
    }

    //메인메뉴로 나가기
    public void ExitInGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    //Next버튼 클릭
    public void LoadNextStage()
    {
        //다음 스테이지가 존재할 때
        if (curStage < stages.Length)
        {
            curStage++;
            StartCoroutine(FadeOut());
        }
        //마지막 스테이지일 때
        else
        {
            ExitInGame();
        }
    }

    public void RetryStage()
    {
        StartCoroutine(FadeOut());
    }
}
