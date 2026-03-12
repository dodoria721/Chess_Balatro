
// 원거리 무기 전용 인터페이스
public interface IRange
{
    void Init(float speed, float damage, int per, UnityEngine.Vector3 dir, string ownerTag = null);
}
