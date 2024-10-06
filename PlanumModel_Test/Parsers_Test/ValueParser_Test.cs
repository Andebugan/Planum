using System.Collections;
using Planum.Parser;

namespace PlanumModel_Test.Parsers_Test
{
    public class ValueParser_Test
    {
        [Theory]
        [ClassData(typeof(TryParse_Guid_TestData))]
        public void TryParse_Guid_Test(string data, Guid expectedValue, bool expectedResult)
        {
            // Arrange
            Guid actualValue = Guid.Empty;

            // Act
            var actualResult = ValueParser.TryParse(ref actualValue, data);

            // Assert
            Assert.Equal(expectedResult, actualResult);
            if (expectedResult)
                Assert.Equal(expectedValue, actualValue);
        }

        class TryParse_Guid_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "", Guid.Empty, false };
                var guid = Guid.NewGuid();
                yield return new object[] { guid.ToString(), guid, true};
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TryParse_int_TestData))]
        public void TryParse_int_Test(string data, int expectedValue, bool expectedResult)
        {
            // Arrange
            int actualValue = 0;

            // Act
            var actualResult = ValueParser.TryParse(ref actualValue, data);

            // Assert
            Assert.Equal(expectedResult, actualResult);
            if (expectedResult)
                Assert.Equal(expectedValue, actualValue);
        }

        class TryParse_int_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "a", 0, false };
                yield return new object[] { "0", 0, true };
                yield return new object[] { "-1", -1, true };
                yield return new object[] { "1", 1, true };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TryParse_float_TestData))]
        public void TryParse_float_Test(string data, float expectedValue, bool expectedResult)
        {
            // Arrange
            float actualValue = 0;

            // Act
            var actualResult = ValueParser.TryParse(ref actualValue, data);

            // Assert
            Assert.Equal(expectedResult, actualResult);
            if (expectedResult)
                Assert.Equal(expectedValue, actualValue);
        }

        class TryParse_float_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "a", 0.0, false };
                yield return new object[] { "1.1", 1.1, true };
                yield return new object[] { "1,001.1", 1001.1, true };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TryParse_bool_TestData))]
        public void TryParse_bool_Test(string data, bool expectedValue, bool expectedResult)
        {
            // Arrange
            bool actualValue = false;

            // Act
            var actualResult = ValueParser.TryParse(ref actualValue, data);

            // Assert
            Assert.Equal(expectedResult, actualResult);
            if (expectedResult)
                Assert.Equal(expectedValue, actualValue);
        }

        class TryParse_bool_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "test", false, false };
                yield return new object[] { "true", true, true };
                yield return new object[] { "t", true, true };
                yield return new object[] { "yes", true, true };
                yield return new object[] { "y", true, true };
                yield return new object[] { "1", true, true };
                yield return new object[] { "false", false, true };
                yield return new object[] { "f", false, true };
                yield return new object[] { "0", false, true };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TryParse_TimeSpan_TestData))]
        public void TryParse_TimeSpan_Test(string data, TimeSpan expectedValue, bool expectedResult)
        {
            // Arrange
            TimeSpan actualValue = TimeSpan.Zero;

            // Act
            var actualResult = ValueParser.TryParse(ref actualValue, data);

            // Assert
            Assert.Equal(expectedResult, actualResult);
            if (expectedResult)
                Assert.Equal(expectedValue, actualValue);
        }

        class TryParse_TimeSpan_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "a", TimeSpan.Zero, false };
                yield return new object[] { "1.10:15", new TimeSpan(1, 10, 15, 0), true };
                yield return new object[] { "1.10", new TimeSpan(1, 10, 0, 0), true };
                yield return new object[] { "10:15", new TimeSpan(0, 10, 15, 0), true };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TryParse_DateTime_TestData))]
        public void TryParse_DateTime_Test(string data, DateTime expectedValue, bool expectedResult)
        {
            // Arrange
            DateTime actualValue = DateTime.MinValue;

            // Act
            var actualResult = ValueParser.TryParse(ref actualValue, data);

            // Assert
            Assert.Equal(expectedResult, actualResult);
            if (expectedResult)
                Assert.Equal(expectedValue, actualValue);
        }

        class TryParse_DateTime_TestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { "test", DateTime.MinValue, false };

                // normal dates
                yield return new object[] { "1.10.24", new DateTime(2024, 10, 1), true };
                yield return new object[] { "1.10.2024", new DateTime(2024, 10, 1), true };
                yield return new object[] { "01.10.2024", new DateTime(2024, 10, 1), true };
                yield return new object[] { "1.10", new DateTime(DateTime.Now.Year, 10, 1), true };

                yield return new object[] { "12:00 1.10.24", new DateTime(2024, 10, 1, 12, 0, 0), true };
                yield return new object[] { "12:00 1.10.2024", new DateTime(2024, 10, 1, 12, 0, 0), true };
                yield return new object[] { "12:00 01.10.2024", new DateTime(2024, 10, 1, 12, 0, 0), true };
                yield return new object[] { "12:00 1.10", new DateTime(DateTime.Now.Year, 10, 1, 12, 0, 0), true };

                // parse day moves
                var now = DateTime.Now;
                yield return new object[] { "12:00 yesterday", new DateTime(now.Year, now.Month, now.Day - 1, 12, 0, 0), true };
                yield return new object[] { "12:00 today", new DateTime(now.Year, now.Month, now.Day, 12, 0, 0), true };
                yield return new object[] { "12:00 tomorrow", new DateTime(now.Year, now.Month, now.Day + 1, 12, 0, 0), true };

                var currentDay = DateTime.Today.AddHours(12);

                yield return new object[] { "12:00 previous monday", GetDayOfWeek(currentDay, DayOfWeek.Monday, -1), true };
                yield return new object[] { "12:00 previous tuesday", GetDayOfWeek(currentDay, DayOfWeek.Tuesday, -1), true };
                yield return new object[] { "12:00 previous wednesday", GetDayOfWeek(currentDay, DayOfWeek.Wednesday, -1), true };
                yield return new object[] { "12:00 previous thursday", GetDayOfWeek(currentDay, DayOfWeek.Thursday, -1), true };
                yield return new object[] { "12:00 previous friday", GetDayOfWeek(currentDay, DayOfWeek.Friday, -1), true };
                yield return new object[] { "12:00 previous saturday", GetDayOfWeek(currentDay, DayOfWeek.Saturday, -1), true };
                yield return new object[] { "12:00 previous sunday", GetDayOfWeek(currentDay, DayOfWeek.Sunday, -1), true };

                yield return new object[] { "12:00 next monday", GetDayOfWeek(currentDay, DayOfWeek.Monday, 1), true };
                yield return new object[] { "12:00 next tuesday", GetDayOfWeek(currentDay, DayOfWeek.Tuesday, 1), true };
                yield return new object[] { "12:00 next wednesday", GetDayOfWeek(currentDay, DayOfWeek.Wednesday, 1), true };
                yield return new object[] { "12:00 next thursday", GetDayOfWeek(currentDay, DayOfWeek.Thursday, 1), true };
                yield return new object[] { "12:00 next friday", GetDayOfWeek(currentDay, DayOfWeek.Friday, 1), true };
                yield return new object[] { "12:00 next saturday", GetDayOfWeek(currentDay, DayOfWeek.Saturday, 1), true };
                yield return new object[] { "12:00 next sunday", GetDayOfWeek(currentDay, DayOfWeek.Sunday, 1), true };
            }

            DateTime GetDayOfWeek(DateTime date, DayOfWeek dayOfWeek, int direction)
            {
                date = date.AddDays(direction);
                while (date.DayOfWeek != dayOfWeek)
                    date = date.AddDays(direction);
                return date;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
