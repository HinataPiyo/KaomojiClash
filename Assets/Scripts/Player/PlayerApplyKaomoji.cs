using TMPro;
using UnityEngine;
using Constants.Global;
using System.Collections.Generic;

public class PlayerApplyKaomoji : MonoBehaviour
{
    [SerializeField] TextMeshPro faceText;
    [SerializeField] CapsuleCollider2D col;

    PlayerData data;
    string current_Kaomoji;     // 現在の顔文字

    // 総合ステータス
    public float Speed { get; private set; }
    public float Power { get; private set; }
    public float Guard { get; private set; }
    public float Stamina { get; private set; }

    public List<KaomojiPartData> KaomojiPartDatas { get; private set; } = new List<KaomojiPartData>();



    public void Initialize(PlayerData data)
    {
        this.data = data;
        TotalParameter();
        faceText.text = BuildKaomoji();
        SetColliderSize();
        GetComponent<Mental>().Initialize(Stamina, data);
    }

    

    /// <summary>
    /// 顔文字の組み立て
    /// </summary>
    /// <returns>設定された顔文字のPartsを合体させたもの</returns>
    string BuildKaomoji()
    {
        KAOMOJI K = data.Kaomoji;
        string left_faceline = SeparatePart(data.Status.mentalData?.faceline, 0);
        string right_faceline = SeparatePart(data.Status.mentalData?.faceline, 1);
        string left_eye = SeparatePart(K.eyes?.Data.part, 0);
        string right_eye = SeparatePart(K.eyes?.Data.part, 1);
        string mouth = K.mouth?.Data.part;
        // string left_hands = SeparatePart(K.hands.Data.part, 0);
        // string right_hands = SeparatePart(K.hands.Data.part, 1);

        string merged = left_faceline + left_eye + mouth + right_eye + right_faceline;
        current_Kaomoji = merged;
        return merged;
    }

    /// <summary>
    /// パーツのステータスを合計
    /// </summary>
    void TotalParameter()
    {
        KAOMOJI K = data.Kaomoji;
        if(K == null) return;

        KaomojiPartData[] datas = new KaomojiPartData[]
        { K.eyes, K.mouth, K.hands, K.decoration_first, K.decoration_second};

        foreach(var part in datas)
        {
            if(part == null) continue;
            KaomojiPartData copy = Instantiate(part);
            KaomojiPartDatas.Add(copy);       // 装備中の記号パーツデータを保持しておく

            // SOが設定されていなければ0を返す
            Speed += part.Data == null ? 0f : part.Data.speed.GetParameterByLevel(0);
            Power += part.Data == null ? 0f : part.Data.power.GetParameterByLevel(0);
            Guard += part.Data == null ? 0f : part.Data.guard.GetParameterByLevel(0);
            Stamina += part.Data == null ? 0f : part.Data.stamina.GetParameterByLevel(0);
        }
    }

    /// <summary>
    /// 記号全体のステータスを更新する
    /// </summary>
    public void UpdateTotalParameter()
    {
        Speed = Power = Guard = Stamina = 0f;
        foreach(var part in KaomojiPartDatas)
        {
            int level = part.Data.levelDetail.Level;
            Speed += part.Data.speed.GetParameterByLevel(level);
            Power += part.Data.power.GetParameterByLevel(level);
            Guard += part.Data.guard.GetParameterByLevel(level);
            Stamina += part.Data.stamina.GetParameterByLevel(level);
        }
    }

    /// <summary>
    /// パーツの左右分割
    /// </summary>
    /// <param name="part">SOで設定した記号</param>
    /// <param name="index">左右どちらか(0=左側、1=右側)</param>
    /// <returns></returns>
    string SeparatePart(string part, int index)
    {
        // index: 0=左側、1=右側
        // partがnullまたは空文字、indexが範囲外の場合は空文字を返す
        if(string.IsNullOrEmpty(part) || part.Length <= index)
        {
            return "";
        }

        return part[index].ToString();
    }

    /// <summary>
    /// コライダーサイズ設定
    /// </summary>
    void SetColliderSize()
    {
        KAOMOJI K = data.Kaomoji;
        if(K == null) return;

        Vector2 size = col.size;
        size.x = KAOMOJI.ColiderXSize * current_Kaomoji.Length;
        col.size = size;
    }
}