using HorseRaceCloudCode;

namespace CloudCodeUnitTests
{
    public class DateTimeUtilsTests
    {
        [TestFixture(Category = "Utils")]
        public class PassTests
        {
            #region Test Cases
            public static List<string> DateTimeStringTestCases = new List<string>
             {
              "2021-09-01 12:00:00",
              "12:00:00",
              "07:00",
            };

            public static IEnumerable<object[]> DateTimeFormatTestCases
            {
                get
                {
                    return new List<object[]>
                {
                    //Latitude,Longitude
                   new object[] { "07:00", "HH:mm" },
                   new object[] { "2021-09-01 12:00:00", "yyyy-MM-dd HH:mm:ss" },
                };
                }
            }
            public static IEnumerable<object[]> DateTimesTestCases
            {
                get
                {
                    return new List<object[]>
                    {
                    //DateTime1, DateTime2
                   new object[] { DateTime.Parse("2021-09-01 12:00:00"), DateTime.Parse("2021-09-01 12:00:00") },
                   new object[] { DateTime.Parse("15:00:00"), DateTime.Parse("15:00:00") },
                };
                }
            }
            #endregion


            #region Methods 
            [TestCaseSource(nameof(DateTimeStringTestCases))]
            public void CheckIfStringIsConvertedToUTC(string dateTimeString)
            {
                DateTime dateTime = DateTimeUtils.ConvertStringToUTCTime(dateTimeString);
                Assert.That(dateTime.Kind, Is.EqualTo(DateTimeKind.Utc), "DateTime is not in UTC format");
            }

            [TestCaseSource(nameof(DateTimeFormatTestCases))]
            public void CheckIfDateTimeFormatIsValid(string dateTimeString, string format)
            {
                bool result = DateTimeUtils.IsValidDateTimeFormat(dateTimeString, format);
                Assert.That(result, Is.True, "DateTime format is invalid");
            }

          
            [TestCaseSource(nameof(DateTimesTestCases))]
            public void CheckIfDateTimesAreEqual(DateTime dateTime1, DateTime dateTime2)
            {
                bool result = DateTimeUtils.AreDateTimesEqual(dateTime1, dateTime2);
                Assert.That(result, Is.True, "DateTimes are not equal");
            }

            [TestCaseSource(nameof(DateTimeStringTestCases))]
            public void CheckIfDateTimeIsValid(string dateTimeString)
            {
                bool result = DateTimeUtils.IsValidDateTime(dateTimeString);
                Assert.That(result, Is.True, "DateTime is invalid");
            }
            #endregion

        }

        [TestFixture(Category = "Utils")]
        public class FailTests
        {
            #region Test Cases
            public static List<string> DateTimeStringTestCases = new List<string>
             {
              "2021-09 12:00:00",
              "54:7:10",
              "7:0 pMm",
            };

            public static IEnumerable<object[]> DateTimeFormatTestCases
            {
                get
                {
                    return new List<object[]>
                {
                    //Latitude,Longitude
                   new object[] { "07:00:00", "HH:mm" },
                   new object[] { "09-01 12:00:00", "yyyy-MM-dd HH:mm:ss" },
                };
                }
            }
            public static IEnumerable<object[]> DateTimesTestCases
            {
                get
                {
                    return new List<object[]>
                    {
                    //DateTime1, DateTime2
                   new object[] { DateTime.Parse("2021-09-01 12:00:00"), DateTime.Parse("2021-09-01 12:00:00") },
                   new object[] { DateTime.Parse("15:00:00"), DateTime.Parse("15:00:00") },
                };
                }
            }
            #endregion


            #region Methods 
            [TestCaseSource(nameof(DateTimeFormatTestCases))]
            public void CheckIfDateTimeFormatIsValid(string dateTimeString, string format)
            {
                bool result = DateTimeUtils.IsValidDateTimeFormat(dateTimeString, format);
                Assert.That(result, Is.False, "DateTime format is valid");
            }


            [TestCaseSource(nameof(DateTimesTestCases))]
            public void CheckIfDateTimesAreEqual(DateTime dateTime1, DateTime dateTime2)
            {
                bool result = DateTimeUtils.AreDateTimesEqual(dateTime1, dateTime2);
                Assert.That(result, Is.True, "DateTimes are not equal");
            }

            [TestCaseSource(nameof(DateTimeStringTestCases))]
            public void CheckIfDateTimeIsValid(string dateTimeString)
            {
                bool result = DateTimeUtils.IsValidDateTime(dateTimeString);
                Assert.That(result, Is.False, "DateTime is valid");
            }
            #endregion

        }
    }
}
