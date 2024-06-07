using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public int currentStage;

    //다른 스크립트에서 접근
    public static StageManager smInstance = null;

    private void Awake()
    {
        //이미 존재할 시 새로 생긴 오브젝트 파괴
        if(smInstance != null)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        smInstance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
