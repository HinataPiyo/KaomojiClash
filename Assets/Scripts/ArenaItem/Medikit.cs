namespace ArenaItem
{
    using System.Collections;
    using TMPro;
    using UnityEngine;
    
    public class Medikit : ArenaItemBase
    {
        [SerializeField] Data.Medikit medikitData;
        bool isActive = true;
        SpriteRenderer spriteRenderer;
        TextMeshPro coolTimeText;
        Collider2D col;

        protected override void Awake()
        {
            base.Awake();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            col = GetComponent<Collider2D>();
            coolTimeText = GetComponentInChildren<TextMeshPro>();
            coolTimeText.gameObject.SetActive(false);   // クールタイム非表示
        }

        /// <summary>
        /// 衝突したら回復する
        /// </summary>
        /// <param name="col"></param>
        void OnTriggerEnter2D(Collider2D col)
        {
            if(col.gameObject.CompareTag(Layer.Player))
            {
                IHeal heal = col.GetComponent<IHeal>();
                if(heal != null)
                {
                    float maxHealth = heal.GetMaxHealthAmount;
                    float healAmount = medikitData.GetHealAmountByGrade();
                    heal.Heal(maxHealth * healAmount);      // 回復処理
                    anim.SetTrigger(ANIM_TRIGGER_PLAY);   // アニメーション再生
                    Inactive();                      // 使用不可にする
                }
            }
        }

        /// <summary>
        /// 使用不可にする
        /// </summary>
        void Inactive()
        {
            isActive = false;
            col.enabled = false;
            coolTimeText.gameObject.SetActive(true);
            ChangeSprite();                 // スプライト変更
            StartCoroutine(CoolTimeRoutine());  // クールタイム開始
        }

        /// <summary>
        /// 使用可能にする
        /// </summary>
        void Active()
        {
            isActive = true;
            col.enabled = true;
            coolTimeText.gameObject.SetActive(false);   // クールタイム非表示
            ChangeSprite();                // スプライト変更
            spriteRenderer.color = Color.white;     // 透明度を元に戻す
        }

        /// <summary>
        /// スプライトを変更する
        /// </summary>
        void ChangeSprite()
        {
            spriteRenderer.sprite = medikitData.GetSpriteByActive(isActive);
        }

        /// <summary>
        /// クールタイム処理
        /// </summary>
        IEnumerator CoolTimeRoutine()
        {
            float remainingTime = medikitData.GetCoolTimeByGrade();

            // 透明度を下げる
            Color alpha = spriteRenderer.color;
            alpha.a = 0.5f;

            while(remainingTime > 0f)
            {
                coolTimeText.text = Mathf.CeilToInt(remainingTime).ToString() + "s";
                yield return null;
                remainingTime -= Time.deltaTime;

                // 徐々に透明度を戻す
                alpha.a += Time.deltaTime / medikitData.GetCoolTimeByGrade() * 0.5f;
                spriteRenderer.color = alpha;
            }
            Active();
        }
    }
}