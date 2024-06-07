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
        //���� ����ĳ��Ʈ ����x
        //Physics2D.queriesStartInColliders = false;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        lineRenderer.sortingOrder = spriteRenderer.sortingOrder;
    }

    private void Update()
    {
        //ù��° (�߻�) ����
        lineRenderer.positionCount = 1; 
        //��ġ �Է�
        lineRenderer.SetPosition(0, transform.position);

        //�浹 ������, right: �ѱ� ����
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, maxRayDistance, layerDetection);

        //�ݻ� �÷���
        bool isReflected = false;

        Vector2 point = Vector2.zero;
        Vector2 normal = Vector2.zero;
        Vector2 dir = Vector2.zero;

        //�浹���� ���
        for(int i = 0; i < reflections; i++)
        {
            lineRenderer.positionCount++;
            
            //�浹 Ȯ��
            if(hit.collider != null)
            {
                //�浹 ���� �Է�
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                isReflected = false;

                //��ֹ� �浹
                if (hit.collider.CompareTag("Obstacle"))
                {
                    //point = (Vector2) hit.point;
                    point = hit.point;
                    normal = (Vector2) hit.normal;

                    //���� ���� ���� ���
                    dir = point - (Vector2)lineRenderer.GetPosition(lineRenderer.positionCount - 2);
                    //�ݻ� ���, ����ĳ��Ʈ �ʱ�ȭ
                    hit = Physics2D.Raycast(point, Vector2.Reflect(dir, normal), maxRayDistance, layerDetection);

                    isReflected = true;
                }
                else break;
            }
            else
            {
                if(isReflected)
                {
                    //�ݻ�� ���� ��ǥ���� �Է�
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, point + Vector2.Reflect(dir, normal) * maxRayDistance);
                    break;
                }
                else
                {
                    //������ �Է�
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
        //LineRenderer �ʱ�ȭ
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
        //��ֹ� �浹 ��
        if(hit.collider.tag.Equals("obstacle"))
        {
            Vector3 pos = hit.point;
            //���� �ݻ�
            Vector3 refDir = Vector3.Reflect(dir, hit.normal);

            CastLaser(refDir, dir);
        }
        else
        {
            //�浹���� �߰�
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
