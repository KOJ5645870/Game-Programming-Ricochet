using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public int reflections;
    public float maxRayDistance;
    public LayerMask layerDetection;

    private void Start()
    {
        //내부 레이캐스트 감지x
        //Physics2D.queriesStartInColliders = false;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        lineRenderer.sortingOrder = spriteRenderer.sortingOrder;
    }

    private void Update()
    {
        //첫번째 (발사) 지점
        lineRenderer.positionCount = 1; 
        //위치 입력
        lineRenderer.SetPosition(0, transform.position);

        //충돌 데이터, right: 총구 방향
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, maxRayDistance, layerDetection);

        //반사 플래그
        bool isReflected = false;

        Vector2 point = Vector2.zero;
        Vector2 normal = Vector2.zero;
        Vector2 dir = Vector2.zero;

        //충돌지점 계산
        for(int i = 0; i < reflections; i++)
        {
            lineRenderer.positionCount++;
            
            //충돌 확인
            if(hit.collider != null)
            {
                //충돌 지점 입력
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                isReflected = false;

                //장애물 충돌
                if (hit.collider.CompareTag("Obstacle"))
                {
                    //point = (Vector2) hit.point;
                    point = hit.point;
                    normal = (Vector2) hit.normal;

                    //현재 진행 방향 계산
                    dir = point - (Vector2)lineRenderer.GetPosition(lineRenderer.positionCount - 2);
                    //반사 계산, 레이캐스트 초기화
                    hit = Physics2D.Raycast(point, Vector2.Reflect(dir, normal), maxRayDistance, layerDetection);

                    isReflected = true;
                }
                else break;
            }
            else
            {
                if(isReflected)
                {
                    //반사된 다음 목표지점 입력
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, point + Vector2.Reflect(dir, normal) * maxRayDistance);
                    break;
                }
                else
                {
                    //일직선 입력
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, transform.position + transform.right * maxRayDistance);
                    break;
                }
            }
        }
    }

    /*
    public GameObject laserObj;
    private LineRenderer laserLine;
    private List<Vector3> lasers = new List<Vector3>();

    public Laser(Vector3 pos, Vector3 dir, float startwidth, float endWidth, 
        Color startColor, Color endColor, Material mat)
    {
        //LineRenderer 초기화
        laserLine = new LineRenderer();
        laserObj = new GameObject();
        laserObj.name = "Laser";

        laserLine = this.laserObj.AddComponent(typeof(LineRenderer)) as LineRenderer;
        laserLine.startWidth = startwidth;
        laserLine.endWidth = endWidth;
        laserLine.startColor = startColor;
        laserLine.endColor = endColor;
        laserLine.material = mat;

        CastLaser(pos, dir);
    }

    private void CastLaser(Vector3 pos, Vector3 dir)
    {
        lasers.Add(pos);

        Ray ray = new Ray(pos, dir);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 100, 1)) 
        {
            CheckHit(hit, dir);
        }
        else
        {
            lasers.Add(ray.GetPoint(100));
            UpdateLaser();
        }
    }

    private void CheckHit(RaycastHit hit, Vector3 dir)
    {
        //장애물 충돌 시
        if(hit.collider.tag.Equals("obstacle"))
        {
            Vector3 pos = hit.point;
            //법선 반사
            Vector3 refDir = Vector3.Reflect(dir, hit.normal);

            CastLaser(refDir, dir);
        }
        else
        {
            //충돌지점 추가
            lasers.Add(hit.point);
            UpdateLaser();
        }
    }

    private void UpdateLaser()
    {
        int count = 0;
        laserLine.positionCount = lasers.Count;

        foreach(Vector3 index in lasers)
        {
            laserLine.SetPosition(count, index);
            count++;
        }
    }
    */
}
