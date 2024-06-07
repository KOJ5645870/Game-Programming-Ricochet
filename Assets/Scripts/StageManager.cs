using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public int currentStage;

    //�ٸ� ��ũ��Ʈ���� ����
    public static StageManager smInstance = null;

    private void Awake()
    {
        //�̹� ������ �� ���� ���� ������Ʈ �ı�
        if(smInstance != null)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        smInstance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
