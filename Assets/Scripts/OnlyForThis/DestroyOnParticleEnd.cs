using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OnlyForThis
{
    /// <summary>
    /// エフェクト(ParticleSystem)がすべて破棄されているか確認し
    /// 破棄されていたら、自身（Particleの親）を削除する処理
    /// </summary>
    public class DestroyOnParticleEnd : MonoBehaviour
    {
        List<ParticleSystem> particleSystems;

        void Awake()
        {
            // 子にある ParticleSystem をすべて取得
            particleSystems = GetComponentsInChildren<ParticleSystem>().ToList();
        }

        void Update()
        {
            // すべてのParticleが終了したか確認
            if (particleSystems.All(p => !p.IsAlive(true)))
            {
                Destroy(gameObject);
            }
        }
    }
}