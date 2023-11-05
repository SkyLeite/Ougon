namespace OugonPack.Test;

using NUnit.Framework;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        OugonPack ougonPack = new OugonPack();
        var result = ougonPack.LoadAndDecomp("/mnt/hdd/projects/Ougon/OugonPack.Test/00.LZR");
        Assert.AreNotEqual(result.Length, 0);
    }
}
