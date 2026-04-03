using SOSG.Monster;

public class MonsterInfo
{
    public readonly MonsterDataSO MonsterData;
    public bool IsUnlocked { get; private set; }
    public bool IsEquipped => EquippedTier >= 0;
    public int EquippedTier { get; private set; }

    public MonsterInfo(MonsterDataSO monsterData, bool isUnlocked)
    {
        MonsterData = monsterData;
        IsUnlocked = isUnlocked;
        SetUnequipped();
    }

    public void SetUnlocked()
    {
        IsUnlocked = true;
    }

    public void SetEquipped(int tier)
    {
        EquippedTier = tier;
    }

    public void SetUnequipped()
    {
        EquippedTier = -1;
    }
}