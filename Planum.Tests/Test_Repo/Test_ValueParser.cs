using System.Collections;
using Planum.Parser;

namespace Planum.Tests
{
    public class Test_ValueParser
    {
        public class TryParseGuidTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                Guid guid = Guid.Empty;
                string guidString = guid.ToString();

                yield return new object[] { guid, guidString, true };
                yield return new object[] { guid, guidString.Remove(0, guidString.Length / 2), false };
                yield return new object[] { guid, string.Empty, false };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TryParseGuidTestData))]
        public void Test_TryParseGuid(Guid value, string data, bool expected)
        {
            // Arrange
            Guid parsedValue = Guid.Empty;

            // Act
            var result = ValueParser.TryParse(ref parsedValue, data);

            // Assert
            Assert.Equal(expected, result);
            if (result)
                Assert.Equal(value, parsedValue);
        }

        [Theory]
        [InlineData(0, "test", false)]
        [InlineData(0, "", true)]
        [InlineData(0, "0", true)]
        [InlineData(10, "10", true)]
        [InlineData(10, "1 0", false)]
        [InlineData(100, "100", true)]
        public void Test_TryParseInt(int value, string data, bool expected)
        {
            // Arrange
            int parsedValue = 0;

            // Act
            var result = ValueParser.TryParse(ref parsedValue, data);

            // Assert
            Assert.Equal(expected, result);
            if (result)
                Assert.Equal(value, parsedValue);
        }

        [Theory]
        [InlineData(1.0, "1.0", true)]
        [InlineData(12.34, "12.34", true)]
        [InlineData(1.0, "test", false)]
        [InlineData(0.0, "", true)]
        public void Test_TryParseFloat(float value, string data, bool expected)
        {
            // Arrange
            float parsedValue = 0;

            // Act
            var result = ValueParser.TryParse(ref parsedValue, data);

            // Assert
            Assert.Equal(expected, result);
            if (result)
                Assert.Equal(value, parsedValue);
        }

        [Theory]
        [InlineData(true, "", false)]
        [InlineData(true, "1234", false)]
        [InlineData(true, "test", false)]
        [InlineData(false, "", false)]
        [InlineData(false, "1234", false)]
        [InlineData(false, "test", false)]
        [InlineData(true, "true", true)]
        [InlineData(true, "t", true)]
        [InlineData(true, "yes", true)]
        [InlineData(true, "y", true)]
        [InlineData(false, "false", true)]
        [InlineData(false, "f", true)]
        [InlineData(false, "no", true)]
        [InlineData(false, "n", true)]
        [InlineData(true, "1", true)]
        [InlineData(false, "0", true)]
        public void Test_TryParseBool(bool value, string data, bool expected)
        {
            // Arrange
            bool parsedValue = false;

            // Act
            var result = ValueParser.TryParse(ref parsedValue, data);

            // Assert
            Assert.Equal(expected, result);
            if (result)
                Assert.Equal(value, parsedValue);
        }

        public class TryParseTimeSpanTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { TimeSpan.Zero, "", true };
                yield return new object[] { TimeSpan.Zero, "test", false };
                yield return new object[] { new TimeSpan(1, 2, 3, 0), "1.2:3", true };
                yield return new object[] { new TimeSpan(1, 2, 0, 0), "1.2", true };
                yield return new object[] { new TimeSpan(0, 2, 3, 0), "2:3", true };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TryParseTimeSpanTestData))]
        public void Test_TryParseTimeSpan(TimeSpan value, string data, bool expected)
        {
            // Arrange
            TimeSpan parsedValue = TimeSpan.Zero;

            // Act
            var result = ValueParser.TryParse(ref parsedValue, data);

            // Assert
            Assert.Equal(expected, result);
            if (result)
                Assert.Equal(value, parsedValue);
        }

        public class TryParseFullTimeSpanTestData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { 0, 0, TimeSpan.Zero, "", true };
                yield return new object[] { 0, 3, TimeSpan.Zero, "3", true };
                yield return new object[] { 1, 3, TimeSpan.Zero, "1 3", true };
                yield return new object[] { 0, 0, TimeSpan.Zero, "test", false };

                yield return new object[] { 0, 0, new TimeSpan(1, 2, 3, 0), "1.2:3", true };
                yield return new object[] { 0, 0, new TimeSpan(1, 2, 0, 0), "1.2", true };
                yield return new object[] { 0, 0, new TimeSpan(0, 2, 3, 0), "2:3", true };

                yield return new object[] { 0, 1, new TimeSpan(1, 2, 3, 0), "1 1.2:3", true };
                yield return new object[] { 1, 3, new TimeSpan(1, 2, 3, 0), "1 3 1.2:3", true };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TryParseFullTimeSpanTestData))]
        public void Test_TryParseFullTimeSpan(int years, int months, TimeSpan timeSpan, string data, bool expected)
        {
            // Arrange
            int parsedYears = 0;
            int parsedMonths = 0;
            TimeSpan parsedTimeSpan = TimeSpan.Zero;

            // Act
            var result = ValueParser.TryParse(ref parsedTimeSpan, ref parsedMonths, ref parsedYears, data);

            // Assert
            Assert.Equal(expected, result);
            if (result)
            {
                Assert.Equal(timeSpan, parsedTimeSpan);
                Assert.Equal(years, parsedYears);
                Assert.Equal(months, parsedMonths);
            }
        }

        public class TryParseDateTimeTestData : IEnumerable<object[]>
        {
            public DateTime GetDayOfWeek(DayOfWeek day, int step)
            {
                DateTime date = DateTime.Today.AddDays(step);
                while (date.DayOfWeek != day)
                    date = date.AddDays(step);
                return date;
            }

            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { DateTime.Today, "", true };
                yield return new object[] { DateTime.Today, "test", false };
                yield return new object[] { new DateTime(2024, 5, 10), "10.5.24", true};
                yield return new object[] { new DateTime(2024, 5, 10), "10.5.2024", true};
                yield return new object[] { new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 12, 30, 0), "12:30", true};
                yield return new object[] { new DateTime(DateTime.Today.Year, 10, 1, 12, 30, 0), "12:30 1.10", true};
                yield return new object[] { new DateTime(2024, 10, 1, 12, 30, 0), "12:30 1.10.2024", true};
                yield return new object[] { DateTime.Today.AddDays(-1), "yesterday", true};
                yield return new object[] { DateTime.Today.AddDays(1), "tomorrow", true};
                yield return new object[] { DateTime.Today, "today", true};
                yield return new object[] { DateTime.Today.AddDays(-1).AddHours(12).AddMinutes(30), "12:30 yesterday", true};
                yield return new object[] { DateTime.Today.AddDays(1).AddHours(12).AddMinutes(30), "12:30 tomorrow", true};
                yield return new object[] { DateTime.Today.AddHours(12).AddMinutes(30), "12:30 today", true};
                yield return new object[] { DateTime.Today.AddDays(1), "next", false};
                yield return new object[] { DateTime.Today.AddDays(1).AddHours(12).AddMinutes(30), "12:30 next", false};
                yield return new object[] { DateTime.Today.AddDays(-1), "previous", false};
                yield return new object[] { DateTime.Today.AddDays(-1).AddHours(12).AddMinutes(30), "12:30 previous", false};
                // return next/previous days of week
                Dictionary<DayOfWeek, string> daysOfWeek = new Dictionary<DayOfWeek, string>()
                {
                    { DayOfWeek.Monday, "monday" },
                    { DayOfWeek.Tuesday, "tuesday" },
                    { DayOfWeek.Wednesday, "wednesday" },
                    { DayOfWeek.Thursday, "thursday" },
                    { DayOfWeek.Friday, "friday" },
                    { DayOfWeek.Saturday, "saturday" },
                    { DayOfWeek.Sunday, "sunday" }
                };

                foreach (var dayOfWeek in daysOfWeek.Keys)
                {
                yield return new object[] { GetDayOfWeek(dayOfWeek, 1), "next " + daysOfWeek[dayOfWeek], true};
                yield return new object[] { GetDayOfWeek(dayOfWeek, 1).AddHours(12).AddMinutes(30), "12:30 next " + daysOfWeek[dayOfWeek] , true};
                yield return new object[] { GetDayOfWeek(dayOfWeek, -1), "previous " + daysOfWeek[dayOfWeek], true};
                yield return new object[] { GetDayOfWeek(dayOfWeek, -1).AddHours(12).AddMinutes(30), "12:30 previous " + daysOfWeek[dayOfWeek], true};

                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(TryParseDateTimeTestData))]
        public void Test_TryParseDateTime(DateTime value, string data, bool expected)
        {
            // Arrange
            DateTime parsedValue = new DateTime();

            // Act
            var result = ValueParser.TryParse(ref parsedValue, data);

            // Assert
            Assert.Equal(expected, result);
            if (result)
                Assert.Equal(value, parsedValue);
        }
    }
}
