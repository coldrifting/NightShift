namespace NightShift.Managers.Interface;

public interface IDayNightManager
{
    public void Init();
    public void Apply(double currentTime);
}