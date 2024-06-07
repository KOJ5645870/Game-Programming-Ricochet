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


    //���̵� �ƿ� �ڷ�ƾ
    IEnumerator FadeOut()
    {
        fadeScreenImage.gameObject.SetActive(true);
        while (true)
        {
            //0.01�ʸ��� ���İ� ����
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
    //���̵� �� �ڷ�ƾ
    IEnumerator FadeIn()
    {
        while(true)
        {
            //0.01�ʸ��� ���İ� ����
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
            // �ν��Ͻ��� ���� ��� �Ҵ�
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
        //�� ��ȯ �� �ν��Ͻ� �ı�x.
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

        //���� �������� �ı�
        if(stageObject != null) GameObject.Destroy(stageObject);
        //�������� �Ҵ�
        stageObject = GameObject.Instantiate(stages[index - 1]);
        stageObject.transform.position = Vector3.zero;

        //��� �������� ��Ȱ��ȭ
        /*
        foreach (GameObject stage in stages)
        {
            stage.SetActive(false);
        }
        */
        //�ش� �������� Ȱ��ȭ
        //stages[index - 1].SetActive(true);

        //���� �������� ����
        curStage = index;

        //UI����
        controlUISet.SetActive(true);
        clearUISet.SetActive(false);
        foreach(Image i in crackImages)
        {
            i.color = new Color(200, 200, 200, 0);
        }

        if (index < 4) setGuideLine(true);
        else setGuideLine(false);

        //Ŭ���� ���� �ʱ�ȭ
        stageStartTime = Time.time;
        shotCount = 0;

    }

    public bool IsClearStage()
    {
        //���� ��� �׾��� ��
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            return true;
        }
        else return false;
    }

    //Ŭ���� UI Ȱ��ȭ
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

        //��� �ؽ�Ʈ
        int time = (int) (stageEndTime - stageStartTime);
        int min = time / 60;
        int sec = time % 60;

        timeText.text = string.Format("{0:D2}:{1:D2}", min, sec) + " / " + 
            string.Format("{0:D2}:{1:D2}", missionManager.missions[curStage].clearTime.min, missionManager.missions[curStage].clearTime.sec);
        bulletText.text = shotCount + " / " + missionManager.missions[curStage].bulletCount;

        //��� �̹���
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
        //�� ���� ��� �������� ���
        foreach (GameObject sniper in GameObject.FindGameObjectsWithTag("Sniper"))
        {
            //Ȱ��ȭ�� �������ۿ��� �Ѿ� �߻�
            if(sniper.activeSelf)
            {
                Sniper sniperLogic = sniper.GetComponent<Sniper>();
                sniperLogic.ShotBullet();
                break;
            }
        }
    }

    //���θ޴��� ������
    public void ExitInGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    //Next��ư Ŭ��
    public void LoadNextStage()
    {
        //���� ���������� ������ ��
        if (curStage < stages.Length)
        {
            curStage++;
            StartCoroutine(FadeOut());
        }
        //������ ���������� ��
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
