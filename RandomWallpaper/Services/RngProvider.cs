using RandomWallpaper.Contracts;

namespace RandomWallpaper.Services;

public class RngProvider : IRngProvider
{
    public int Generate(int minValue, int maxValue)
    {
        return Random.Shared.Next(minValue, maxValue);
    }
}