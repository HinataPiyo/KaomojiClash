using TMPro;
using UnityEngine;
using Constants.Global;

public class ApplyKaomoji : MonoBehaviour
{
    [SerializeField] CharacterData data;
    [SerializeField] TextMeshPro faceText;

    public float Speed { get; private set; }
    public float Power { get; private set; }
    public float Guard { get; private set; }
    public float Stamina { get; private set; }


    void Awake()
    {
        TotalParameter();
        faceText.text = BuildKaomoji();
    }

    string BuildKaomoji()
    {
        KAOMOJI K = data.Kaomoji;
        string left_faceline = SeparatePart(K.mentalData?.faceline, 0);
        string right_faceline = SeparatePart(K.mentalData?.faceline, 1);
        string left_eye = SeparatePart(K.eyes?.Data.part, 0);
        string right_eye = SeparatePart(K.eyes?.Data.part, 1);
        string mouth = K.mouth?.Data.part;
        // string left_hands = SeparatePart(K.hands.Data.part, 0);
        // string right_hands = SeparatePart(K.hands.Data.part, 1);

        string merged = left_faceline + left_eye + mouth + right_eye + right_faceline;
        return merged;
    }

    void TotalParameter()
    {
        KAOMOJI K = data.Kaomoji;
        if(K == null) return;

        KaomojiPartData[] datas = new KaomojiPartData[]
        { K.eyes, K.mouth, K.hands, K.decoration_first, K.decoration_second};

        foreach(var part in datas)
        {
            if(part == null) continue;

            Speed += part.Data == null ? 0f : part.Data.speed.GetSpeedByLevel();
            Power += part.Data == null ? 0f : part.Data.power.GetSpeedByLevel();
            Guard += part.Data == null ? 0f : part.Data.guard.GetSpeedByLevel();
            Stamina += part.Data == null ? 0f : part.Data.stamina.GetSpeedByLevel();
        }

    }

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
}