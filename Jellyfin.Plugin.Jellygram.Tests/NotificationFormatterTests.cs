using Jellyfin.Plugin.Jellygram.Notifications;
using Xunit;

namespace Jellyfin.Plugin.Jellygram.Tests;

public sealed class NotificationFormatterTests
{
    [Fact]
    public void NewSeriesAbsorbsChildEvents()
    {
        var seriesId = Guid.NewGuid();
        var seasonId = Guid.NewGuid();
        var items = new[]
        {
            new MediaItem(seriesId, "Series", "Severance", string.Empty, 2022, null, null, null, null, null, null),
            new MediaItem(seasonId, "Season", "Season 1", string.Empty, null, seriesId, "Severance", null, 1, null, null),
            new MediaItem(Guid.NewGuid(), "Episode", "Good News About Hell", string.Empty, null, seriesId, "Severance", seasonId, 1, 1, null)
        };

        var notifications = NotificationFormatter.Build(items, descriptions: false);

        var notification = Assert.Single(notifications);
        Assert.Contains("Nuova serie", notification.Text);
        Assert.Contains("Stagione 01 · 1 episodio", notification.Text);
    }

    [Fact]
    public void NewSeriesShowsLaterSeasonNumberFromEpisodes()
    {
        var seriesId = Guid.NewGuid();
        var items = Enumerable.Range(1, 8)
            .Select(number => new MediaItem(Guid.NewGuid(), "Episode", $"Episode {number}", string.Empty, null, seriesId, "The Bear", Guid.NewGuid(), 5, number, null))
            .Prepend(new MediaItem(seriesId, "Series", "The Bear", string.Empty, 2022, null, null, null, null, null, null));

        var notifications = NotificationFormatter.Build(items, descriptions: false);

        var notification = Assert.Single(notifications);
        Assert.Contains("Stagione 05 · 8 episodi", notification.Text);
    }

    [Fact]
    public void EpisodesAreGroupedIntoRanges()
    {
        var seriesId = Guid.NewGuid();
        var items = Enumerable.Range(1, 4)
            .Select(number => new MediaItem(Guid.NewGuid(), "Episode", $"Episode {number}", string.Empty, null, seriesId, "Slow Horses", Guid.NewGuid(), 2, number, null));

        var notifications = NotificationFormatter.Build(items, descriptions: false);

        var notification = Assert.Single(notifications);
        Assert.Contains("Nuovi episodi", notification.Text);
        Assert.Contains("S02: E01–E04", notification.Text);
        Assert.Contains("+1 altri", notification.Text);
    }
}
