using UnityEngine;

[CreateAssetMenu(fileName = "New WireAction", menuName = "Inventory/Actions/WireAction")]
public class WireAction : ItemAction
{
    public override void Use(GameObject user)
    {
        Engine engine = FindObjectOfType<Engine>();
        if (engine != null)
        {
            engine.ConnectWire();
            Debug.Log("전선이 엔진에 연결되었습니다.");
        }
        else
        {
            Debug.Log("엔진을 찾을 수 없습니다.");
        }
    }
}