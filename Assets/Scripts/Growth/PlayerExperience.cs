using UnityEngine;
using System;
using VampireLike.Combat;
using VampireLike.Audio;

namespace VampireLike.Growth
{
    /// <summary>
    /// 플레이어의 경험치, 레벨, 레벨업 필요 경험치, 경험치 보석 획득 범위를 관리한다.
    /// </summary>
    public class PlayerExperience : MonoBehaviour
    {
        public enum LevelUpSource
        {
            Normal,
            BossReward
        }

        // 현재 레벨에서 누적된 경험치다.
        [SerializeField]
        private int currentExperience;

        // 현재 플레이어 레벨이다.
        [SerializeField]
        private int currentLevel = 1;

        // 플레이어 레벨 상한이다. 기본값은 사실상 제한 없음으로 둔다.
        [SerializeField]
        private int maxLevel = int.MaxValue;

        // 다음 레벨까지 필요한 경험치다.
        [SerializeField]
        private int experienceToNextLevel = 5;

        // 레벨업할 때마다 다음 필요 경험치를 얼마나 완만하게 늘릴지 정한다.
        [SerializeField]
        private float nextLevelExperienceMultiplier = 1.09f;

        // 순수 곱연산만 쓰면 후반 요구량이 급격히 튀므로, 작은 고정 증가로 곡선을 안정화한다.
        [SerializeField]
        private int nextLevelFlatIncrease = 1;

        // 레벨업 요구량이 과하게 커져 성장 체감이 끊기지 않도록 상한을 둔다.
        [SerializeField]
        private int maxExperienceToNextLevel = 200;

        // 경험치 보석을 흡수하기 시작하는 반경이다. 자석 강화로 증가한다.
        [SerializeField]
        private float pickupRadius = 0.45f;

        // 경험치 보석 감지에 사용할 레이어 마스크다.
        [SerializeField]
        private LayerMask pickupLayerMask = ~0;

        private LevelUpChoiceUI levelUpChoiceUI;
        private bool hasPendingLevelUpChoice;
        private LevelUpSource pendingLevelUpSource = LevelUpSource.Normal;
        private readonly Collider2D[] pickupResults = new Collider2D[16];

        public int CurrentExperience => currentExperience;
        public int CurrentLevel => currentLevel;
        public int MaxLevel => maxLevel;
        public int ExperienceToNextLevel => experienceToNextLevel;
        public float PickupRadius => pickupRadius;
        public float ExperienceProgress => currentLevel >= maxLevel ? 1f : experienceToNextLevel <= 0 ? 0f : (float)currentExperience / experienceToNextLevel;
        // HUD가 경험치 변화를 즉시 반영할 수 있도록 알림을 보낸다.
        public event Action<int, int, int> ExperienceChanged;

        private void Awake()
        {
            maxLevel = int.MaxValue;

            // 성장 관련 컴포넌트가 빠져 있어도 Player에 자동으로 붙여 학습 프로젝트 설정 부담을 줄인다.
            levelUpChoiceUI = GetComponent<LevelUpChoiceUI>();

            if (levelUpChoiceUI == null)
                levelUpChoiceUI = gameObject.AddComponent<LevelUpChoiceUI>();

            if (GetComponent<PlayerUpgradeController>() == null)
                gameObject.AddComponent<PlayerUpgradeController>();

            if (GetComponent<PlayerExperienceUI>() == null)
                gameObject.AddComponent<PlayerExperienceUI>();
        }

        private void Start()
        {
            NotifyExperienceChanged();
        }

        private void Update()
        {
            CollectNearbyExperienceGems();
        }

        public void AddExperience(int amount)
        {
            // 보석 획득 등 외부에서 경험치를 더할 때 사용하는 진입점이다.
            if (amount <= 0)
                return;

            currentExperience += amount;
            GameSessionStats.RecordExperience(amount);
            CheckLevelUp(LevelUpSource.Normal);
            NotifyExperienceChanged();
            Debug.Log($"Experience: {currentExperience}/{experienceToNextLevel}");
        }

        public void GrantImmediateLevelUp(int statExperienceAmount = 0)
        {
            GrantImmediateLevelUp(statExperienceAmount, LevelUpSource.Normal);
        }

        public void GrantImmediateLevelUp(int statExperienceAmount, LevelUpSource levelUpSource)
        {
            if (currentLevel >= maxLevel)
                return;

            if (statExperienceAmount > 0)
                GameSessionStats.RecordExperience(statExperienceAmount);

            int requiredExperience = Mathf.Max(1, experienceToNextLevel - currentExperience);
            currentExperience += requiredExperience;
            CheckLevelUp(levelUpSource);
            NotifyExperienceChanged();
        }

        public void MultiplyPickupRadius(float multiplier)
        {
            // 자석 강화에서 호출한다. 곱연산으로 획득 범위를 자연스럽게 키운다.
            if (multiplier <= 0f)
                return;

            pickupRadius *= multiplier;
        }

        public void SetMaxLevel(int value)
        {
            maxLevel = value <= 0 ? int.MaxValue : Mathf.Max(1, value);
            currentLevel = Mathf.Min(currentLevel, maxLevel);
            NotifyExperienceChanged();
        }

        private void OnValidate()
        {
            currentExperience = Mathf.Max(0, currentExperience);
            currentLevel = Mathf.Max(1, currentLevel);
            maxLevel = Mathf.Max(currentLevel, maxLevel);
            experienceToNextLevel = Mathf.Max(1, experienceToNextLevel);
            nextLevelExperienceMultiplier = Mathf.Max(1f, nextLevelExperienceMultiplier);
            nextLevelFlatIncrease = Mathf.Max(0, nextLevelFlatIncrease);
            maxExperienceToNextLevel = Mathf.Max(experienceToNextLevel, maxExperienceToNextLevel);
            pickupRadius = Mathf.Max(0.05f, pickupRadius);
        }

        private void CheckLevelUp(LevelUpSource levelUpSource)
        {
            // 한 번에 많은 경험치를 얻어도 필요한 만큼 레벨업을 처리한다.
            while (currentLevel < maxLevel && currentExperience >= experienceToNextLevel)
            {
                currentExperience -= experienceToNextLevel;
                currentLevel++;
                experienceToNextLevel = CalculateNextExperienceRequirement();
                GameSfx.Play(SfxType.LevelUp);
                Debug.Log($"Level Up! Level {currentLevel} / Next EXP: {currentExperience}/{experienceToNextLevel}");
                hasPendingLevelUpChoice = true;
                pendingLevelUpSource = levelUpSource;
            }

            if (currentLevel >= maxLevel)
            {
                currentExperience = Mathf.Min(currentExperience, experienceToNextLevel);
                hasPendingLevelUpChoice = false;
            }

            if (hasPendingLevelUpChoice)
            {
                hasPendingLevelUpChoice = false;
                LevelUpChoiceUI.ChoiceMode choiceMode = pendingLevelUpSource == LevelUpSource.BossReward
                    ? LevelUpChoiceUI.ChoiceMode.BossReward
                    : LevelUpChoiceUI.ChoiceMode.Normal;
                pendingLevelUpSource = LevelUpSource.Normal;
                levelUpChoiceUI.Show(currentLevel, choiceMode);
            }
        }

        private int CalculateNextExperienceRequirement()
        {
            // 곱연산만 사용하면 요구량이 눈덩이처럼 커지므로, 완만한 배율과 작은 고정 증가를 함께 쓴다.
            int nextRequirement = Mathf.CeilToInt(experienceToNextLevel * nextLevelExperienceMultiplier) + nextLevelFlatIncrease;
            return Mathf.Clamp(nextRequirement, 1, maxExperienceToNextLevel);
        }

        private void NotifyExperienceChanged()
        {
            ExperienceChanged?.Invoke(currentLevel, currentExperience, experienceToNextLevel);
        }

        private void CollectNearbyExperienceGems()
        {
            // pickupRadius 안에 들어온 보석을 즉시 삭제하지 않고 플레이어에게 끌어오게 한다.
            if (Time.timeScale <= 0f)
                return;

            int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, pickupRadius, pickupResults, pickupLayerMask);

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hit = pickupResults[i];

                if (hit == null)
                    continue;

                ExperienceGem gem = hit.GetComponentInParent<ExperienceGem>();

                if (gem != null)
                    gem.StartAttract(this);
            }
        }
    }
}
