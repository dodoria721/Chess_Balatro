using UnityEngine;

/*카메라 플레이어 따라다니게*/

public class MainCameraController : MonoBehaviour
{
    // 플레이어의 위치 정보
    [SerializeField] private Transform player;
    // 카메라의 부드러움의 정보 숫자가 낮을 수록 더 부드럽다, 높을 수 록 반응이 빨라 지며 좀 딱딱해진다.
    [SerializeField] private float smoothing = 0.2f;

    private void LateUpdate()
    {
        // 플레이어의 위치가 모두 업데이트된 후 카메라 위치를 조정합니다.
        if (player != null)
        {
            // z 좌표는 유지 하면서, (x,y)의 자표를 플레이어의 위치로 정함
            Vector3 targetPos = new Vector3(player.position.x, player.position.y, transform.position.z);
            // 해당 좌표로 이동, 선형보간을 이용한 부드러운 움직임
            transform.position = Vector3.Lerp(transform.position, targetPos, smoothing);
        }
    }
}