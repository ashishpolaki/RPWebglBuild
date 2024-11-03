using HorseRaceCloudCode;
using System.Globalization;

namespace CloudCodeUnitTests
{

    public class RaceScheduleTests
    {
        [TestFixture(Category = "Host")]
        public class PassTests
        {
            #region Test Cases
            public static IEnumerable<object[]> LobbyWaitTimeVsRaceIntervalTestCases
            {
                get
                {
                    return new List<object[]>
                {
                    //LobbyWaitTime,RaceInterval
                   new object[] { 1, 2 },
                   new object[] { 1, 5 },
                };
                }
            }
            public static IEnumerable<object[]> RaceScheduleTimeFormatTestCases
            {
                get
                {
                    return new List<object[]>
                {
                   new object[] { "11:00" },
                   new object[] { "12:00" },
                };
                }
            }
            public static IEnumerable<object[]> RaceStartAndEndTimeCombinations
            {
                get
                {
                    return new List<object[]>
                {
                    //ScheduleStart,ScheduleEnd
                   new object[] { "11:00", "12:00" },
                   new object[] { "01:00", "03:15" },
                   new object[] { "01:00", "01:07" }
                };
                }
            }
            public static IEnumerable<object[]> RaceScheduleWithIntervalsTestCases
            {
                get
                {
                    return new List<object[]>
                {
                    //ScheduleStart,ScheduleEnd,RaceInterval
                   new object[] { "11:00", "12:00", 1 },
                   new object[] { "01:00", "03:15", 2 },
                   new object[] { "01:00", "01:07", 7 },
                };
                }
            }
            #endregion

            #region Methods
            [TestCaseSource(nameof(RaceScheduleTimeFormatTestCases))]
            public void RaceSchedule_ValidFormat(string dateTime)
            {
                Assert.That(DateTimeUtils.IsValidDateTimeFormat(dateTime, StringUtils.HOUR_MINUTE_FORMAT), Is.True);
            }

            [TestCaseSource((nameof(LobbyWaitTimeVsRaceIntervalTestCases)))]
            public void LobbyWaitTime_ShouldBeLessThan_RaceInterval(int lobbyWaitTime, int raceInterval)
            {
                Assert.Less(lobbyWaitTime, raceInterval, "Lobby Wait Time should be less than Race Interval");
            }

            [TestCaseSource(nameof(RaceStartAndEndTimeCombinations))]
            public void StartAndEndRaceSchedules_AreNotEqual(string scheduleStart, string scheduleEnd)
            {
                DateTime startSchedule = DateTime.ParseExact(scheduleStart, StringUtils.HOUR_MINUTE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime endSchedule = DateTime.ParseExact(scheduleEnd, StringUtils.HOUR_MINUTE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None);
                Assert.That(DateTimeUtils.AreDateTimesEqual(startSchedule, endSchedule), Is.False);
            }
            [TestCaseSource(nameof(RaceScheduleWithIntervalsTestCases))]
            public void RaceInterval_LessthanRaceSchedules(string scheduleStart, string scheduleEnd, int raceInterval)
            {
                DateTime startSchedule = DateTime.ParseExact(scheduleStart, StringUtils.HOUR_MINUTE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime endSchedule = DateTime.ParseExact(scheduleEnd, StringUtils.HOUR_MINUTE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None);

                //If end time is less than start time, add a day to end time
                if (startSchedule > endSchedule)
                {
                    endSchedule = endSchedule.AddDays(1);
                }
                TimeSpan raceTimeSpan = endSchedule - startSchedule;
                TimeSpan raceIntervalSpan = TimeSpan.FromMinutes(raceInterval);
                Assert.That(raceTimeSpan, Is.GreaterThanOrEqualTo(raceIntervalSpan), "The Race Interval should be less than the race schedule.");
            }
            #endregion

        }

        [TestFixture(Category = "Host")]
        public class FailTests
        {
            #region Test Cases
            public static IEnumerable<object[]> LobbyWaitTimeVsRaceIntervalTestCases
            {
                get
                {
                    return new List<object[]>
                {
                    //LobbyWaitTime,RaceInterval
                   new object[] { 7, 2 },
                   new object[] { 8, 5 },
                   new object[] { 5, 5 }
                };
                }
            }
            public static IEnumerable<object[]> RaceScheduleTimeFormatTestCases
            {
                get
                {
                    return new List<object[]>
                {
                   new object[] { "11:00 PM" },
                   new object[] { "12:00 am" },
                   new object[] {  "1:00"  }
                };
                }
            }
            public static IEnumerable<object[]> RaceStartAndEndTimeCombinations
            {
                get
                {
                    return new List<object[]>
                {
                    //ScheduleStart,ScheduleEnd
                   new object[] { "11:00", "11:00" },
                   new object[] { "01:00", "01:00" },
                   new object[] { "01:50", "01:50" }
                };
                }
            }
            public static IEnumerable<object[]> RaceScheduleWithIntervalsTestCases
            {
                get
                {
                    return new List<object[]>
                {
                    //ScheduleStart,ScheduleEnd,RaceInterval
                   new object[] { "11:00", "12:00", 129 },
                   new object[] { "01:00", "01:07", 9 },
                };
                }
            }
            #endregion

            #region Methods
            [TestCaseSource(nameof(RaceScheduleTimeFormatTestCases))]
            public void RaceSchedule_InValidFormat(string dateTime)
            {
                Assert.That(DateTimeUtils.IsValidDateTimeFormat(dateTime, StringUtils.HOUR_MINUTE_FORMAT), Is.False);
            }

            [TestCaseSource((nameof(LobbyWaitTimeVsRaceIntervalTestCases)))]
            public void Check_LobbyWaitTimeExceedsRaceInterval(int lobbyWaitTime, int raceInterval)
            {
                Assert.GreaterOrEqual(lobbyWaitTime, raceInterval, "Lobby Wait Time should not be less than Race Interval");
            }

            [TestCaseSource(nameof(RaceStartAndEndTimeCombinations))]
            public void StartAndEndRaceSchedules_AreEqual(string scheduleStart, string scheduleEnd)
            {
                DateTime startSchedule = DateTime.ParseExact(scheduleStart, StringUtils.HOUR_MINUTE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime endSchedule = DateTime.ParseExact(scheduleEnd, StringUtils.HOUR_MINUTE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None);
                Assert.That(DateTimeUtils.AreDateTimesEqual(startSchedule, endSchedule), Is.True);
            }

            [TestCaseSource(nameof(RaceScheduleWithIntervalsTestCases))]
            public void Check_RaceScheduleIsLessThanInterval(string scheduleStart, string scheduleEnd, int raceInterval)
            {
                DateTime startSchedule = DateTime.ParseExact(scheduleStart, StringUtils.HOUR_MINUTE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None);
                DateTime endSchedule = DateTime.ParseExact(scheduleEnd, StringUtils.HOUR_MINUTE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None);

                //If end time is less than start time, add a day to end time
                if (startSchedule > endSchedule)
                {
                    endSchedule = endSchedule.AddDays(1);
                }
                TimeSpan raceTimeSpan = endSchedule - startSchedule;
                TimeSpan raceIntervalSpan = TimeSpan.FromMinutes(raceInterval);
                Assert.That(raceTimeSpan < raceIntervalSpan, Is.True);
            }
            #endregion
        }
    }

}
