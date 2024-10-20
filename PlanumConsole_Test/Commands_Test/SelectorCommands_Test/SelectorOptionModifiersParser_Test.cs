using System.Collections;
using Planum.Console.Commands.Selector;
using Planum.Model.Filters;

namespace PlanumConsole_Test.SelectorOptionModifiersParser_Test
{
    public class SelectorOptionModifiersParser_Test
    {
        [Theory]
        [ClassData(typeof(ParseSelectorSettings_TestData))]
        public void ParseSelectorSettings_Test(string optionName, ValueMatchType expectedMatchType, MatchFilterType expectedFilterType, string expected)
        {
            // Arrange
            ValueMatchType actualMatchType;
            MatchFilterType actualFilterType;

            // Act
            var actual = SelectorOptionModifiersParser.ParseSelectorSettings(optionName, out actualMatchType, out actualFilterType);

            // Assert
            Assert.Equal(expected, actual);
            Assert.Equal(expectedMatchType, actualMatchType);
            Assert.Equal(expectedFilterType, actualFilterType);
        }

        class ParseSelectorSettings_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                Dictionary<string, ValueMatchType> valueMatchTypes = new Dictionary<string, ValueMatchType> {
                    { "+", ValueMatchType.AND },
                    { "*", ValueMatchType.OR },
                    { "!", ValueMatchType.NOT },
                    { "", ValueMatchType.AND }
                };

                Dictionary<string, MatchFilterType> matchFilterTypes = new Dictionary<string, MatchFilterType> {
                    { "", MatchFilterType.SUBSTRING },
                    { "<", MatchFilterType.LESSER },
                    { "<=", MatchFilterType.LESSER_AND_EQUAL },
                    { "==", MatchFilterType.EQUAL },
                    { ">=", MatchFilterType.GREATER_AND_EQUAL },
                    { ">", MatchFilterType.GREATER }
                };

                foreach (var valueMatch in valueMatchTypes.Keys)
                    foreach (var matchFilter in matchFilterTypes.Keys)
                        yield return new object[] {
                            "test" + valueMatch + matchFilter,
                            valueMatchTypes[valueMatch],
                            matchFilterTypes[matchFilter],
                            "test"
                        };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
