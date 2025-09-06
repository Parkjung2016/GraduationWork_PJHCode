using BIS.Core.Utility;
using BIS.UI;
using Main.Runtime.Combat;
using UnityEngine.UI;
using YTH.Boss;

namespace PJH.Runtime.UI
{
    public class BossHPProgressBarUI : ProgressBaseUI
    {
        public enum Progress
        {
            HealthBar,
        }

        private float previousHealth;
        private Boss _boss;
        private Health _bossHealth;

        private void Start()
        {
            _boss = FindAnyObjectByType<Boss>();

            if (_boss != null)
            {
                _bossHealth = _boss.HealthCompo;
                _bossHealth.OnDeath += HandleDeath;
                _bossHealth.OnApplyDamaged += HandleApplyDamage;
                previousHealth = _bossHealth.CurrentHealth;
            }
        }

        private void HandleApplyDamage(float damage)
        {
            float currentHealth = _bossHealth.CurrentHealth - damage;
            float healthDelta = previousHealth - currentHealth;

            float totalWidth = 610;
            float healthRatio = healthDelta / (_bossHealth.MaxHealth - 0);

            float newX = (totalWidth * (previousHealth / _bossHealth.MaxHealth)) - (totalWidth * 0.5f) - 15;
            float newWidth = (totalWidth * healthRatio) + 5.2f;

            //var ui = BIS.Manager.Managers.UI.MakeSupItem<HealthSpaceUI>(GetImage((int)Progress.HealthBar).transform,
            //    "HealthSpace");
            //ui.PlayEffect(newWidth, newX, Color.red);

            ValueUpdate(currentHealth, _bossHealth.MaxHealth, 0);

            previousHealth = currentHealth;
        }

        private void HandleDeath()
        {
            Util.UIFadeOut(gameObject, true);
        }

        public override void SetUp()
        {
            Bind<Image>(typeof(Progress));
            _fill = Get<Image>((int)Progress.HealthBar);
        }

        private void OnDestroy()
        {
            if (_bossHealth != null)
            {
                _bossHealth.OnDeath -= HandleDeath;
                _bossHealth.OnApplyDamaged -= HandleApplyDamage;
            }
        }
    }
}