namespace NightShift.Managers;

public interface IDayNightManager
{
    public void Init();
    public void Apply(TimeOfDay timeOfDay);
}