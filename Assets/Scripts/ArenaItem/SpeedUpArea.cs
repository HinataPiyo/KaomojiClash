namespace ArenaItem
{
    using UnityEngine;
    
    public class SpeedUpArea : ArenaItemBase
    {
        [SerializeField] Data.SpeedUpArea speedUpAreaData;
        ParticleSystem particle;
        RangeLine2D rangeLine;
        bool isInside = false;
        Movement insideTarget = null;
        CircleCollider2D col;
        
        protected override void Awake()
        {
            rangeLine = GetComponentInChildren<RangeLine2D>();
            particle = GetComponentInChildren<ParticleSystem>();
            col = GetComponent<CircleCollider2D>();
        }

        void Start()
        {
            // 効果範囲の設定
            float r = speedUpAreaData.GetEffectRange();
            var shape = particle.shape;
            rangeLine.Initialize(r);
            SetColliderRadius(r);
            shape.radius = speedUpAreaData.GetEffectRange();
        }

        /// <summary>
        /// コライダーに入ったらスピードアップ適用
        /// </summary>
        /// <param name="col"></param>
        void OnTriggerEnter2D(Collider2D col)
        {
            if(col.gameObject.CompareTag(Layer.Player))
            {
                // スピードアップ処理
                insideTarget = col.GetComponent<Movement>();
                ApplySpeedUp(insideTarget);
            }
        }

        /// <summary>
        /// コライダー内にいる間スピードアップを維持
        /// </summary>
        /// <param name="col"></param>
        void OnTriggerStay2D(Collider2D col)
        {
            if(col.gameObject.CompareTag(Layer.Player))
            {
                // スピードアップ処理
                ApplySpeedUp(insideTarget);
            }
        }

        /// <summary>
        /// コライダーから出たらスピードアップ解除
        /// </summary>
        /// <param name="col"></param>
        void OnTriggerExit2D(Collider2D col)
        {
            if(col.gameObject.CompareTag(Layer.Player))
            {
                // スピードアップ解除処理
                insideTarget.ResetSpeedUp();
                RemoveSpeedUp();
            }
        }

        /// <summary>
        /// スピードアップを適用する
        /// </summary>
        void ApplySpeedUp(Movement move)
        {
            isInside = true;
            rangeLine.SetColor(Color.red);
            move.SpeedUp(speedUpAreaData.GetSpeedMultiplier());
        }

        /// <summary>
        /// スピードアップを解除する
        /// </summary>
        void RemoveSpeedUp()
        {
            isInside = false;
            rangeLine.ResetColor();

            insideTarget = null;
        }

        /// <summary>
        /// コライダーの半径を設定する
        /// </summary>
        /// <param name="r"></param>
        void SetColliderRadius(float r)
        {
            col.radius = r;
        }


    }
}