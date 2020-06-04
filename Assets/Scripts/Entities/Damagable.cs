public interface Damagable
{
    void ApplyDamage(float damage);

    void OnRepairStart(Pirate pirate);

    void OnRepairEnd();
}