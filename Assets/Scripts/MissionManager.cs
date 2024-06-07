using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    [Serializable]
    public struct msTime
    {
        public int min;
        public int sec;
    }

    [Serializable]
    public struct mission
    {
        public msTime clearTime;
        public int bulletCount;
    }
    /*
    1. kill target
    2. use helicopter num
    3. dont kill none-target
     */
    public mission[] missions;

    public int[] getClearStar(int stage)
    {
        int[] star = { 1, 0, 0 };

        if (isClearTimeLimit(stage)) star[1] = 1;
        if (isClearBulletCount(stage)) star[2] = 1;

        return star;
    }

    public  bool isClearTimeLimit(int stage)
    {
        mission m = missions[stage - 1];
        if (convertToSec(m.clearTime) >= (int) (GameManager.Instance.stageEndTime - GameManager.Instance.stageStartTime))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool isClearBulletCount(int stage)
    {
        if (stage > 9) stage = 9;
        mission m = missions[stage];
        if (m.bulletCount >= GameManager.Instance.shotCount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int convertToSec(msTime time)
    {
        return time.sec + time.min * 60;
    }
}
