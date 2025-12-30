using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class ChartToolsTests
{
    readonly List<string> createdFiles = new();

    [TearDown]
    public void TearDown()
    {
        foreach (var file in createdFiles)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }

        createdFiles.Clear();
    }

    [Test]
    public void ChartLoader_LoadsNotesWithCorrectTiming()
    {
        var chartJson = new ChartJson
        {
            musicFile = "song.ogg",
            bpm = 120,
            offsetSec = 0.5f,
            measures = new[]
            {
                new ChartJson.Measure
                {
                    subdiv = 16,
                    rows = new[] { "1000", "0100", "0010", "0001" }
                }
            }
        };

        var fileName = "chart_loader_test.json";
        var path = WriteChartJson(fileName, chartJson);

        var chart = ChartLoader.LoadFromStreamingAssets(fileName);

        Assert.That(chart.Notes.Count, Is.EqualTo(4));
        Assert.That(chart.Bpm, Is.EqualTo(120));

        var secPerMeasure = (60.0 / chart.Bpm) * 4.0; // 2.0
        Assert.That(chart.Notes[0].Lane, Is.EqualTo(Lane.Left));
        Assert.That(chart.Notes[0].TimeSec, Is.EqualTo(0.5f).Within(1e-6));
        Assert.That(chart.Notes[0].Division, Is.EqualTo(NoteDivision.Quarter));

        Assert.That(chart.Notes[1].Lane, Is.EqualTo(Lane.Down));
        Assert.That(chart.Notes[1].TimeSec, Is.EqualTo(0.5f + secPerMeasure * (1f / 16f)).Within(1e-6));
        Assert.That(chart.Notes[1].Division, Is.EqualTo(NoteDivision.Sixteenth));

        Assert.That(chart.Notes[2].Lane, Is.EqualTo(Lane.Up));
        Assert.That(chart.Notes[2].TimeSec, Is.EqualTo(0.5f + secPerMeasure * (2f / 16f)).Within(1e-6));
        Assert.That(chart.Notes[2].Division, Is.EqualTo(NoteDivision.Eighth));

        Assert.That(chart.Notes[3].Lane, Is.EqualTo(Lane.Right));
        Assert.That(chart.Notes[3].TimeSec, Is.EqualTo(0.5f + secPerMeasure * (3f / 16f)).Within(1e-6));
        Assert.That(chart.Notes[3].Division, Is.EqualTo(NoteDivision.Sixteenth));
    }

    [Test]
    public void ChartLoader_InvalidBpm_Throws()
    {
        var fileName = "chart_loader_invalid_bpm.json";
        var chartJson = new ChartJson
        {
            musicFile = "song.ogg",
            bpm = 0,
            offsetSec = 0f,
            measures = Array.Empty<ChartJson.Measure>()
        };

        WriteChartJson(fileName, chartJson);

        Assert.Throws<InvalidDataException>(() => ChartLoader.LoadFromStreamingAssets(fileName));
    }

    [Test]
    public void ChartLoader_BpmAboveMax_Throws()
    {
        var fileName = "chart_loader_invalid_high_bpm.json";
        var chartJson = new ChartJson
        {
            musicFile = "song.ogg",
            bpm = 1001,
            offsetSec = 0f,
            measures = Array.Empty<ChartJson.Measure>()
        };

        WriteChartJson(fileName, chartJson);

        Assert.Throws<InvalidDataException>(() => ChartLoader.LoadFromStreamingAssets(fileName));
    }

    [Test]
    public void ChartLoader_AcceptsUpperBoundBpm()
    {
        var fileName = "chart_loader_upper_bound_bpm.json";
        var chartJson = new ChartJson
        {
            musicFile = "song.ogg",
            bpm = 1000,
            offsetSec = 0f,
            measures = new[]
            {
                new ChartJson.Measure
                {
                    subdiv = 4,
                    rows = new[] { "1000", "0000", "0000", "0000" }
                }
            }
        };

        WriteChartJson(fileName, chartJson);

        var chart = ChartLoader.LoadFromStreamingAssets(fileName);

        Assert.That(chart.Bpm, Is.EqualTo(1000));
        Assert.That(chart.Notes, Is.Not.Empty);
    }

    [Test]
    public void ChartRecorder_SavesUsingCorrectedSubdivision()
    {
        var chart = new Chart("song.ogg", 120, 0f, Array.Empty<Note>());
        var recorder = new ChartRecorder(enable: true, recordedFileName: "chart_recorder_test.json", recordSubdiv: 12);

        SetPrivateField(recorder, "isRecording", true);
        recorder.OnKeyPressed(Lane.Left, 0.1f);
        recorder.OnKeyPressed(Lane.Right, 2.05f);

        InvokeSave(recorder, chart);

        var path = Path.Combine(Application.streamingAssetsPath, "chart_recorder_test.json");
        var json = File.ReadAllText(path);
        var parsed = JsonUtility.FromJson<ChartJson>(json);

        Assert.That(parsed.measures.Length, Is.EqualTo(2));
        Assert.That(parsed.measures[0].subdiv, Is.EqualTo(16));
        Assert.That(parsed.measures[1].subdiv, Is.EqualTo(16));

        Assert.That(parsed.measures[0].rows[1][0], Is.EqualTo('1'));
        Assert.That(parsed.measures[1].rows[0][3], Is.EqualTo('1'));
    }

    string WriteChartJson(string fileName, ChartJson chartJson)
    {
        var path = Path.Combine(Application.streamingAssetsPath, fileName);
        Directory.CreateDirectory(Application.streamingAssetsPath);
        File.WriteAllText(path, JsonUtility.ToJson(chartJson, prettyPrint: true));
        createdFiles.Add(path);
        return path;
    }

    static void SetPrivateField<T>(T instance, string fieldName, object value)
    {
        var field = typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)
                    ?? throw new MissingFieldException(typeof(T).Name, fieldName);
        field.SetValue(instance, value);
    }

    static void InvokeSave(ChartRecorder recorder, Chart chart)
    {
        var method = typeof(ChartRecorder).GetMethod("Save", BindingFlags.Instance | BindingFlags.NonPublic)
                     ?? throw new MissingMethodException(nameof(ChartRecorder), "Save");
        method.Invoke(recorder, new object[] { chart });
    }
}
