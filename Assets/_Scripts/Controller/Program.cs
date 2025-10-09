using MineCombat;

public static class Program
{
    public static void Main(string[] args)
    {
        Damage damage1 = new("mc_common", 10);
        Damage damage2 = new("mc_magic", 10);

        EventManager.Bind("DamageProcess", (Damage dmg) => {
            if (!DamageTypes.Ignore(dmg.type, "mc_bypass_armor"))
            {
                dmg.value *= 0.5;
            }
            dmg.AddModifier("mc_test", DamageModifiers.CreateAdd(-2, 10, (Tags)"mc_bypass_test"));
        });

        Console.WriteLine($"Damage1 is {damage1.Get()}\nDamage2 is {damage2.Get()}");
    }
}