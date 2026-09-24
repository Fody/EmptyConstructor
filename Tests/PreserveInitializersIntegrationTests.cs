using Fody;

public class PreserveInitializersIntegrationTests
{
    static TestResult testResult;

    static PreserveInitializersIntegrationTests()
    {
        var weaver = new ModuleWeaver
        {
            PreserveInitializers = true
        };
        testResult = weaver.ExecuteTestRun("AssemblyToProcess.dll",
            assemblyName: nameof(PreserveInitializersIntegrationTests));
    }

    [Test]
    public async Task ClassWithInitializedFields()
    {
        var instance = testResult.GetInstance("ClassWithInitializedFields");
        await Assert.That((object?)instance.X).IsEqualTo(9);
        await Assert.That((object?)instance.Y).IsEqualTo("aString");
        await Assert.That((object?)instance.Z).IsNotNull();
    }

    [Test]
    public async Task ClassWithInitializedProperties()
    {
        var instance = testResult.GetInstance("ClassWithInitializedProperties");
        await Assert.That((object?)instance.X).IsEqualTo(9);
        await Assert.That((object?)instance.Y).IsEqualTo("aString");
        await Assert.That((object?)instance.Z).IsNotNull();
    }

    [Test]
    public async Task ReproBug143()
    {
        var instance = testResult.GetInstance("Bug143Child");
        await Assert.That((object?)instance).IsNotNull();
    }

#if NET5_0
    [Test]
    public void RecordWithParameter()
    {
        testResult.GetInstance("RecordWithParameter");
    }

    [Test]
    public async Task RecordWithParameterAndInitializedField()
    {
        var instance = testResult.GetInstance("RecordWithParameterAndInitializedField");
        await Assert.That((object?)instance.X).IsEqualTo(9);
    }
#endif
}