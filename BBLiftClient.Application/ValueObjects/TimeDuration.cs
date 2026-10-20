namespace BBLiftClient.Application.ValueObjects;

public record TimeDuration
{
    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }
    
    private TimeDuration(int hours,int minutes, int seconds)
    {
        Hours = hours;
        Minutes = minutes;
        Seconds = seconds;
    }

    public static TimeDuration Create(int hours, int minutes, int seconds)
    {
        if (hours < 0 || minutes < 0 || seconds < 0)
            throw new ArgumentException("Zaman 0 Olamaz");

        if (seconds >= 60)
        {
            minutes += seconds / 60;
            seconds = seconds % 60;
        }
        if (minutes >= 60)
        {
            hours += minutes / 60;
            minutes = minutes % 60;
        }
        return new TimeDuration(hours, minutes, seconds);
    }
    
    public override string ToString()
    {
        if (Hours > 0)
            return $"{Hours:D2}:{Minutes:D2}:{Seconds:D2}";
            
        return $"{Minutes:D2}:{Seconds:D2}";
    }
}
