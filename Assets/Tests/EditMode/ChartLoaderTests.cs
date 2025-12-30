using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;

public sealed class ChartLoaderTests
{
    string streamingAssetsPath = default!;

    [SetUp]
    public void SetUp()
    {
        streamingAssetsPath = Application.streamingAssetsPath;
        Directory.CreateDirectory(streamingAssetsPath);
    }

    [TearDown]
    public void TearDown()
    {
        foreach (var file in Directory.GetFiles(streamingAssetsPath, "TestChart*.json"))
        {
            File.Delete(file);
        }
    }

    [Test]
    public void LoadFromStreamingAssets_ReturnsNotesSortedByTime()
    {
        var json = @"{
            \"musicFile\": \"test.mp3\",
            \"bpm\": 120,
            \"offsetSec\": 0.5,
            \"measures\": [
                { \"subdiv\": 16, \"rows\": [\"1000\", \"0001\"] },
                { \"subdiv\": 16, \"rows\": [\"0010\", \"0100\"] }
            ]
        }";

        var path = Path.Combine(streamingAssetsPath, "TestChartSorted.json");
        File.WriteAllText(path, json);

        var chart = ChartLoader.LoadFromStreamingAssets("TestChartSorted.json");

        Assert.That(chart.Bpm, Is.EqualTo(120));
        Assert.That(chart.OffsetSec, Is.EqualTo(0.5f));
        Assert.That(chart.Notes.Count, Is.EqualTo(4));

        for (int i = 1; i < chart.Notes.Count; i++)
        {
            Assert.That(chart.Notes[i].TimeSec, Is.GreaterThanOrEqualTo(chart.Notes[i - 1].TimeSec),
                $"Note at index {i} should not be earlier than note at index {i - 1}");
        }
    }

    [Test]
    public void LoadFromStreamingAssets_AssignsNoteDivisionsFromRows()
    {
        var json = @"{
            \"musicFile\": \"test.mp3\",
            \"bpm\": 150,
            \"offsetSec\": 0.0,
            \"measures\": [
                { \"subdiv\": 16, \"rows\": [\"1000\", \"0100\", \"0010\"] }
            ]
        }";

        var path = Path.Combine(streamingAssetsPath, "TestChartDivisions.json");
        File.WriteAllText(path, json);

        var chart = ChartLoader.LoadFromStreamingAssets("TestChartDivisions.json");

        Assert.That(chart.Notes[0].Division, Is.EqualTo(NoteDivision.Quarter));
        Assert.That(chart.Notes[1].Division, Is.EqualTo(NoteDivision.Eighth));
        Assert.That(chart.Notes[2].Division, Is.EqualTo(NoteDivision.Sixteenth));
    }

    [Test]
    public void LoadFromStreamingAssets_InvalidBpmThrows()
    {
        var json = @"{ \"musicFile\": \"test.mp3\", \"bpm\": 0, \"offsetSec\": 0.0, \"measures\": [] }";
        var path = Path.Combine(streamingAssetsPath, "TestChartInvalidBpm.json");
        File.WriteAllText(path, json);

        var ex = Assert.Throws<InvalidDataException>(() => ChartLoader.LoadFromStreamingAssets("TestChartInvalidBpm.json"));
        StringAssert.Contains("Invalid bpm", ex!.Message);
    }
}
