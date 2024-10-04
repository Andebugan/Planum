using System.Collections;
using Planum.Model.Filters;

namespace PlanumModel_Test.Filters_Test
{
    public class ValueMatch_Test
    {
        [Theory]
        [ClassData(typeof(Check_internals_int_TestData))]
        public void Check_internals_int_Test(ValueMatch<int> match, bool agg, int compared, Dictionary<CheckType, bool> expected)
        {
            // Arrange
            Dictionary<CheckType, bool> actual = new Dictionary<CheckType, bool>();

            // Assert
            actual[CheckType.Equal] = match.CheckEqual(compared, agg);
            actual[CheckType.Lesser] = match.CheckLesser(compared, agg);
            actual[CheckType.Greater] = match.CheckGreater(compared, agg);
            actual[CheckType.ValueStrInCompared] = match.CheckValueStrInCompared(compared, agg);
            actual[CheckType.ComparedStrInValue] = match.CheckComparedStrInValue(compared, agg);

            // Act
            Assert.Equal(expected[CheckType.Equal], actual[CheckType.Equal]);
            Assert.Equal(expected[CheckType.Lesser], actual[CheckType.Lesser]);
            Assert.Equal(expected[CheckType.Greater], actual[CheckType.Greater]);
            Assert.Equal(expected[CheckType.ValueStrInCompared], actual[CheckType.ValueStrInCompared]);
            Assert.Equal(expected[CheckType.ComparedStrInValue], actual[CheckType.ComparedStrInValue]);
        }

        public enum CheckType
        {
            Equal,
            Lesser,
            Greater,
            ValueStrInCompared,
            ComparedStrInValue
        }

        class Check_internals_int_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach (var testObjectSet in GetTestObject_AND())
                    yield return testObjectSet;
                foreach (var testObjectSet in GetTestObject_NOT())
                    yield return testObjectSet;
                foreach (var testObjectSet in GetTestObject_OR())
                    yield return testObjectSet;
                foreach (var testObjectSet in GetTestObject_IGNORE())
                    yield return testObjectSet;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            IEnumerable<object[]> GetTestObject_AND()
            {
                yield return GetTestObjectArr(ValueMatchType.AND, 0, "0", false, 0, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, true },
                    { CheckType.Lesser, false },
                    { CheckType.Greater, false },
                    { CheckType.ComparedStrInValue, true },
                    { CheckType.ValueStrInCompared, true }
                });
                yield return GetTestObjectArr(ValueMatchType.AND, 0, "0", false, 1, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, false },
                    { CheckType.Lesser, true },
                    { CheckType.Greater, false },
                    { CheckType.ComparedStrInValue, false },
                    { CheckType.ValueStrInCompared, false }
                });
                yield return GetTestObjectArr(ValueMatchType.AND, 0, "0", false, -1, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, false },
                    { CheckType.Lesser, false },
                    { CheckType.Greater, true },
                    { CheckType.ComparedStrInValue, false },
                    { CheckType.ValueStrInCompared, false }
                });
                yield return GetTestObjectArr(ValueMatchType.AND, 0, "0", true, 0, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, true },
                    { CheckType.Lesser, false },
                    { CheckType.Greater, false },
                    { CheckType.ComparedStrInValue, true },
                    { CheckType.ValueStrInCompared, true }
                });
                yield return GetTestObjectArr(ValueMatchType.AND, 0, "0", true, 1, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, false },
                    { CheckType.Lesser, true },
                    { CheckType.Greater, false },
                    { CheckType.ComparedStrInValue, false },
                    { CheckType.ValueStrInCompared, false }
                });
                yield return GetTestObjectArr(ValueMatchType.AND, 0, "0", true, -1, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, false },
                    { CheckType.Lesser, false },
                    { CheckType.Greater, true },
                    { CheckType.ComparedStrInValue, false },
                    { CheckType.ValueStrInCompared, false }
                });
            }

            IEnumerable<object[]> GetTestObject_OR()
            {
                yield return GetTestObjectArr(ValueMatchType.OR, 0, "0", false, 0, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, true },
                    { CheckType.Lesser, false },
                    { CheckType.Greater, false },
                    { CheckType.ComparedStrInValue, true },
                    { CheckType.ValueStrInCompared, true }
                });
                yield return GetTestObjectArr(ValueMatchType.OR, 0, "0", false, 1, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, false },
                    { CheckType.Lesser, true },
                    { CheckType.Greater, false },
                    { CheckType.ComparedStrInValue, false },
                    { CheckType.ValueStrInCompared, false }
                });
                yield return GetTestObjectArr(ValueMatchType.OR, 0, "0", false, -1, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, false },
                    { CheckType.Lesser, false },
                    { CheckType.Greater, true },
                    { CheckType.ComparedStrInValue, false },
                    { CheckType.ValueStrInCompared, false }
                });
                yield return GetTestObjectArr(ValueMatchType.OR, 0, "0", true, 0, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, true },
                    { CheckType.Lesser, true },
                    { CheckType.Greater, true },
                    { CheckType.ComparedStrInValue, true },
                    { CheckType.ValueStrInCompared, true }
                });
                yield return GetTestObjectArr(ValueMatchType.OR, 0, "0", true, 1, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, true },
                    { CheckType.Lesser, true },
                    { CheckType.Greater, true },
                    { CheckType.ComparedStrInValue, true },
                    { CheckType.ValueStrInCompared, true }
                });
                yield return GetTestObjectArr(ValueMatchType.OR, 0, "0", true, -1, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, true },
                    { CheckType.Lesser, true },
                    { CheckType.Greater, true },
                    { CheckType.ComparedStrInValue, true },
                    { CheckType.ValueStrInCompared, true }
                });
            }

            IEnumerable<object[]> GetTestObject_NOT()
            {
                yield return GetTestObjectArr(ValueMatchType.NOT, 0, "0", false, 0, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, false },
                    { CheckType.Lesser, true },
                    { CheckType.Greater, true },
                    { CheckType.ComparedStrInValue, false },
                    { CheckType.ValueStrInCompared, false }
                });
                yield return GetTestObjectArr(ValueMatchType.NOT, 0, "0", false, 1, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, true },
                    { CheckType.Lesser, false },
                    { CheckType.Greater, true },
                    { CheckType.ComparedStrInValue, true },
                    { CheckType.ValueStrInCompared, true }
                });
                yield return GetTestObjectArr(ValueMatchType.NOT, 0, "0", false, -1, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, true },
                    { CheckType.Lesser, true },
                    { CheckType.Greater, false },
                    { CheckType.ComparedStrInValue, true },
                    { CheckType.ValueStrInCompared, true }
                });
                yield return GetTestObjectArr(ValueMatchType.NOT, 0, "0", true, 0, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, false },
                    { CheckType.Lesser, true },
                    { CheckType.Greater, true },
                    { CheckType.ComparedStrInValue, false },
                    { CheckType.ValueStrInCompared, false }
                });
                yield return GetTestObjectArr(ValueMatchType.NOT, 0, "0", true, 1, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, true },
                    { CheckType.Lesser, false },
                    { CheckType.Greater, true },
                    { CheckType.ComparedStrInValue, true },
                    { CheckType.ValueStrInCompared, true }
                });
                yield return GetTestObjectArr(ValueMatchType.NOT, 0, "0", true, -1, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, true },
                    { CheckType.Lesser, true },
                    { CheckType.Greater, false },
                    { CheckType.ComparedStrInValue, true },
                    { CheckType.ValueStrInCompared, true }
                });
            }

            IEnumerable<object[]> GetTestObject_IGNORE()
            {
                yield return GetTestObjectArr(ValueMatchType.IGNORE, 0, "0", false, 0, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, false },
                    { CheckType.Lesser, false },
                    { CheckType.Greater, false },
                    { CheckType.ComparedStrInValue, false },
                    { CheckType.ValueStrInCompared, false }
                });
                yield return GetTestObjectArr(ValueMatchType.IGNORE, 0, "0", false, 1, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, false },
                    { CheckType.Lesser, false },
                    { CheckType.Greater, false },
                    { CheckType.ComparedStrInValue, false },
                    { CheckType.ValueStrInCompared, false }
                });
                yield return GetTestObjectArr(ValueMatchType.IGNORE, 0, "0", false, 1, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, false },
                    { CheckType.Lesser, false },
                    { CheckType.Greater, false },
                    { CheckType.ComparedStrInValue, false },
                    { CheckType.ValueStrInCompared, false }
                });
                yield return GetTestObjectArr(ValueMatchType.IGNORE, 0, "0", true, 0, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, true },
                    { CheckType.Lesser, true },
                    { CheckType.Greater, true },
                    { CheckType.ComparedStrInValue, true },
                    { CheckType.ValueStrInCompared, true }
                });
                yield return GetTestObjectArr(ValueMatchType.IGNORE, 0, "0", true, 1, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, true },
                    { CheckType.Lesser, true },
                    { CheckType.Greater, true },
                    { CheckType.ComparedStrInValue, true },
                    { CheckType.ValueStrInCompared, true }
                });
                yield return GetTestObjectArr(ValueMatchType.IGNORE, 0, "0", true, 1, new Dictionary<CheckType, bool> {
                    { CheckType.Equal, true },
                    { CheckType.Lesser, true },
                    { CheckType.Greater, true },
                    { CheckType.ComparedStrInValue, true },
                    { CheckType.ValueStrInCompared, true }
                });
            }

            object[] GetTestObjectArr(ValueMatchType matchType, int value, string stringValue, bool agg, int compared, Dictionary<CheckType, bool> expected)
            {
                return new object[]
                {
                    new ValueMatch<int>(
                            value: value,
                            stringValue: stringValue,
                            equal: matchType,
                            lesser: matchType,
                            greater: matchType,
                            comparedStrInValue: matchType,
                            valueStrInCompared: matchType),
                    agg,
                    compared,
                    expected
                };
            }
        }
    }
}
