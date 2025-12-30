using UnityEngine;

public class WallController : MonoBehaviour
{
    [SerializeField] GameObject wall;
    GameObject currentWall;

    /// <summary>
    /// 壁を生成する
    /// </summary>
    /// <param name="player"></param>
    /// <param name="enemy"></param>
    public void CreateWall(Vector2 player, Vector2 enemy)
    {
        Vector2 createPos = Vector2.Lerp(player, enemy, 0.5f);        // プレイヤーと敵との距離の中心を取得
        currentWall = Instantiate(wall, createPos, Quaternion.identity);
    }
}