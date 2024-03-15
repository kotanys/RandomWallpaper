namespace RandomWallpaper.Contracts;

public interface IRngProvider
{
    int Generate(int minValue, int maxValue);
}