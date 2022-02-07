
[System.Serializable]
public class PlayerWeapon
{

    public string name = "Pistol";

    public float damage = 10f;
    public float range = 100f;
    
    public int ammo;

    public PlayerWeapon(string name, float damage, float range)
    {
        this.name = name;
        this.damage = damage;
        this.range = range;
        if (name == "Pistol")
            ammo = -1;
        else
            ammo = 55;
    }
    
    public PlayerWeapon(string name, float damage, float range, int ammo)
    {
        this.name = name;
        this.damage = damage;
        this.range = range;
        this.ammo = ammo;
    }

    public void Shoot()
    {
        ammo--;
    }
}
