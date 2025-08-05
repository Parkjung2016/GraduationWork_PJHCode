using Cysharp.Threading.Tasks;
using FMODUnity;
using PJH.Runtime.BossSkill.BossSkills.ShadowClones;
using UnityEngine;

namespace PJH.Runtime.BossSkill.BossSkills
{
    [CreateAssetMenu(menuName = "SO/BossSkill/Skills/ShadowCloneSkill")]
    public class ShadowCloneSkillSO : BossSkillSO
    {
        public PoolTypeSO shadowClonePoolType;
        public PoolManagerSO poolManager;
        public EventReference spawnShadowCloneSound;
        public float lifeTime = 5f;

        [Range(1, 4)] public int cloneCount = 2;

        public override void ActivateSkill()
        {
            for (int i = 1; i <= cloneCount; i++)
            {
                bool isLeft = i % 2 == 0;
                MoveDirection moveDirection = isLeft ? MoveDirection.Left : MoveDirection.Right;
                SpawnShadowClone(moveDirection);
            }
        }

        private async void SpawnShadowClone(MoveDirection moveDirection)
        {
            RuntimeManager.PlayOneShot(spawnShadowCloneSound, _boss.transform.position);
            ShadowClone shadowClone = poolManager.Pop(shadowClonePoolType) as ShadowClone;
            shadowClone.GetCompo<ShadowCloneMovement>().AIPathCompo.enabled = false;
            shadowClone.SetLifeTime(lifeTime);
            await UniTask.Yield();
            shadowClone.transform.SetPositionAndRotation(_boss.transform.position, _boss.transform.rotation);
            shadowClone.GetCompo<ShadowCloneMovement>().AIPathCompo.enabled = true;
            shadowClone.StartBehaviour(moveDirection);
        }
    }
}