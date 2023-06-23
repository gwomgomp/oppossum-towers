public class TimedStatusEffect {
    public StatusEffect statusEffect;
    public Tower originTower;

    private float timer = 0.0f;

    private float appliedStacks = 0;
    private float slowPercentage = 0;
    public float AppliedStacks { get => appliedStacks; }

    public float SlowPercentage { get => slowPercentage; }
    public void ApplyStack() {
        if (appliedStacks < statusEffect.maxStacks) {
            appliedStacks += 1f;
        }

        slowPercentage = statusEffect.slowPercentage * appliedStacks;
    }

    public TimedStatusEffect(StatusEffect statusEffect, Tower originTower) {
        this.statusEffect = statusEffect;
        this.originTower = originTower;
    }

    public void UpdateTimer(float timeDelta) {
        timer += timeDelta;
    }

    public bool HasEnded() {
        if (timer >= statusEffect.duration) {
            return true;
        }

        return false;
    }

    public bool Equals(TimedStatusEffect other) {
        if (other.statusEffect == this.statusEffect && other.originTower == this.originTower) {
            return true;
        }

        return false;
    }

    public void RefreshTimer() {
        timer = 0.0f;
    }
}
