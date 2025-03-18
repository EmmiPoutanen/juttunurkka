


namespace PrototypeUnitTest
{
    public class UnitTestBase
    {
        // Example unit test. Move tests to suitable folders, for example based on class.
        [Fact]
        public void GetDefaultSurvey_ReturnsCorrectValues()
        {
            var result = Prototype.Survey.GetDefaultSurvey();
            Assert.NotNull(result);
        }
    }
}