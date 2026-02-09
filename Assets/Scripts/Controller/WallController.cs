using UnityEngine;
using UI.Battle;

public class WallController : MonoBehaviour
{
    [SerializeField] GameObject wall;
    Wall currentWall;

    /// <summary>
    /// 壁を生成する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="enemy"></param>
    public Vector2 CreateWall(Vector2 player, Vector2 enemy)
    {
        Vector2 createPos = Vector2.Lerp(player, enemy, 0.5f);        // プレイヤーと敵との距離の中心を取得
        GameObject obj = Instantiate(wall, createPos, Quaternion.identity);
        currentWall = obj.GetComponent<Wall>();
        AudioManager.I.PlaySE("SetWall");
        CameraShake.I.ApplyShake(1, 2f, 0.5f);
        CreateArenaItems();     // 壁生成時にアイテムも生成
        PayUsageFee();

        return createPos;
    }

    public Wall GetWall()
    {
        if(currentWall == null) return null;
        return currentWall;
    }

    /// <summary>
    /// 壁にArenaItemを生成
    /// </summary>
    public void CreateArenaItems()
    {
        if(currentWall == null) return;
        currentWall.CreateArenaItem();
    }

    /// <summary>
    /// 生成されていた壁を破棄
    /// </summary>
    public void DestroyWall()
    {
        if(currentWall == null) return;
        currentWall.ClearArenaItems();   // 破棄前にアイテムも破棄
        Destroy(currentWall.gameObject);
    }

    /// <summary>
    /// ArenaItemの使用料を支払う
    /// </summary>
    public void PayUsageFee()
    {
        int fee = currentWall.ArenaItemSettingData.GetUsageFee();
        Money.Sub(fee);
        Context.I.UpdateMoneyDisplay();
    }
}