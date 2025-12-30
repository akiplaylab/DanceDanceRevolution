using NUnit.Framework;
using UnityEngine;

/// <summary>
/// テストを書くときの雛形サンプルです。
/// 必要に応じてクラス名やテスト対象を変更してください。
/// </summary>
public sealed class TestTemplate
{
    [SetUp]
    public void SetUp()
    {
        // 各テストの前処理をここに書きます。
    }

    [TearDown]
    public void TearDown()
    {
        // 各テストの後処理をここに書きます。
    }

    [Test]
    public void SampleTest()
    {
        // Arrange: テストに必要な初期状態を準備
        var obj = new GameObject("sample");

        // Act: テスト対象の処理を実行
        var isActive = obj.activeSelf;

        // Assert: 結果を検証
        Assert.IsTrue(isActive);
    }
}
