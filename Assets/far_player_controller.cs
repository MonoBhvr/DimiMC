using UnityEngine;

public class far_player_controller : MonoBehaviour
{
    public Animator anim;
    Vector3 previousPosition;
    public float speed = 5f;
    void Start()
    {
        // 이전 프레임의 위치를 초기화
        previousPosition = transform.position;
    }

    void Update()
    {
        // 현재 위치와 이전 위치를 비교하여 속도 계산
        Vector3 velocity = (transform.position - previousPosition) / Time.deltaTime;

        // y축을 제외한 x축과 z축 기준 속도
        speed = Mathf.Sqrt(Mathf.Pow(velocity.x, 2) + Mathf.Pow(velocity.z, 2));
        anim.SetBool("Walk", speed > 0.1f);

        // 현재 위치를 다음 프레임을 위해 저장
        previousPosition = transform.position;
    }
}
