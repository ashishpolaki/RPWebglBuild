using System;
using System.Collections.Generic;
using NUnit.Framework;

public class ScheduleRaceTests
{
    [TestFixture(Category = "Host")]
    public class PassTests
    {
        #region Test Cases
        public static IEnumerable<int> RaceIntervalDigitTestCases
        {
            get
            {
                return new List<int>
                {
                  1,
                  5
                };
            }
        }
        public static IEnumerable<int> LobbyWaitTimeDigitTestCases
        {
            get
            {
                return new List<int>
                {
                  1,
                  6
                };
            }
        }
        public static IEnumerable<object[]> LobbyWaitTimeLessThanRaceIntervalTestCases
        {
            get
            {
                return new List<object[]>
                {
                    //LobbyWaitTime,RaceInterval
                   new object[] { 1, 2 },
                   new object[] { 1, 5 },
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
                    //DateTime
                   new object[] { "11:00 aM" },
                   new object[] { "12:00 PM" },
                   new object[] { "12:00 Pm" }
                };
            }
        }
        public static IEnumerable<object[]> StartAndEndRaceScheduleTestCases
        {
            get
            {
                return new List<object[]>
                {
                    //ScheduleStart,ScheduleEnd
                   new object[] { "11:00 AM", "12:00 PM" },
                   new object[] { "01:00 PM", "03:15 PM" },
                   new object[] { "01:00 PM", "01:07 PM" }
                };
            }
        }
        public static IEnumerable<object[]> RaceIntervalLessThanRaceSchedulesTestCases
        {
            get
            {
                return new List<object[]>
                {
                    //ScheduleStart,ScheduleEnd,RaceInterval
                   new object[] { "11:00 AM", "12:00 PM", 1 },
                   new object[] { "01:00 PM", "03:15 PM", 2 },
                   new object[] { "01:00 PM", "01:07 PM", 6 },
                };
            }
        }
        #endregion

        #region Methods
        [TestCaseSource(nameof(RaceScheduleTimeFormatTestCases))]
        public void RaceSchedule_ValidFormat(string dateTime)
        {
            Assert.That(DateTimeUtils.IsValidDateTimeFormat(dateTime), Is.True);
        }

        [TestCaseSource(nameof(RaceIntervalDigitTestCases))]
        public void RaceInterval_GreaterThanZero(int raceInterval)
        {
            Assert.Greater(raceInterval, 0, StringUtils.RACE_INTERVAL_GREATERTHANZERO);
        }

        [TestCaseSource(nameof(LobbyWaitTimeDigitTestCases))]
        public void LobbyWaitTime_GreaterThanZero(int lobbyWaitTime)
        {
            Assert.Greater(lobbyWaitTime, 0, StringUtils.LOBBY_WAITTIME_GREATERTHANZERO);
        }

        [TestCaseSource((nameof(LobbyWaitTimeLessThanRaceIntervalTestCases)))]
        public void LobbyWaitTime_ShouldBeLessThan_RaceInterval(int lobbyWaitTime, int raceInterval)
        {
            Assert.LessOrEqual(lobbyWaitTime, raceInterval, StringUtils.LOBBYWAITTIME_LESSTHAN_RACEINTERVAL);
        }

        [TestCaseSource(nameof(StartAndEndRaceScheduleTestCases))]
        public void StartAndEndRaceSchedules_AreNotEqual(string scheduleStart, string scheduleEnd)
        {
            DateTime startSchedule = DateTimeUtils.ConvertToUTCTime(scheduleStart, StringUtils.HOUR_MINUTE_AMPM_TIME_FORMAT);
            DateTime endSchedule = DateTimeUtils.ConvertToUTCTime(scheduleEnd, StringUtils.HOUR_MINUTE_AMPM_TIME_FORMAT);
            Assert.That(DateTimeUtils.AreDateTimesEqual(startSchedule, endSchedule), Is.False);
        }

        [TestCaseSource(nameof(RaceIntervalLessThanRaceSchedulesTestCases))]
        public void RaceInterval_LessthanRaceSchedules(string scheduleStart, string scheduleEnd, int raceInterval)
        {
            DateTime startSchedule = DateTimeUtils.ConvertToUTCTime(scheduleStart, StringUtils.HOUR_MINUTE_AMPM_TIME_FORMAT);
            DateTime endSchedule = DateTimeUtils.ConvertToUTCTime(scheduleEnd, StringUtils.HOUR_MINUTE_AMPM_TIME_FORMAT);

            //If end time is less than start time, add a day to end time
            if (startSchedule > endSchedule)
            {
                endSchedule = endSchedule.AddDays(1);
            }
            TimeSpan raceTimeSpan = endSchedule - startSchedule;
            TimeSpan raceIntervalSpan = TimeSpan.FromMinutes(raceInterval);
            Assert.That(raceTimeSpan, Is.GreaterThan(raceIntervalSpan));
        }
        #endregion
    }

    [TestFixture(Category = "Host")]
    public class FailTests
    {

        #region Test Cases
        public static IEnumerable<object[]> RaceScheduleTimeFormatTestCases
        {
            get
            {
                return new List<object[]>
                {
                    //DateTime
                    //invalid formats
                   new object[] { "11:00 M" },
                   new object[] { "12:00 PMM" },
                   new object[] { "12:00 AmM" },
                };
            }
        }
        public static IEnumerable<int> RaceIntervalDigitTestCases
        {
            get
            {
                return new List<int>
                {
                  0,
                  -1
                };
            }
        }
        public static IEnumerable<int> LobbyWaitTimeDigitTestCases
        {
            get
            {
                return new List<int>
                {
                  0,
                  -1
                };
            }
        }
        public static IEnumerable<object[]> LobbyWaitTimeGreaterThanRaceIntervalTestCases
        {
            get
            {
                return new List<object[]>
                {
                    //LobbyWaitTime,RaceInterval
                    new object[] { 2, 1 },
                    new object[] { 5, 3 },
                };
            }
        }

        public static IEnumerable<object[]> StartAndEndRaceScheduleTestCases
        {
            get
            {
                return new List<object[]>
                {
                    //ScheduleStart,ScheduleEnd
                   new object[] { "11:00 AM", "11:00 AM" },
                   new object[] { "01:00 PM", "01:00 PM" },
                };
            }
        }

        public static IEnumerable<object[]> RaceIntervalGreaterThanRaceSchedulesTestCases
        {
            get
            {
                return new List<object[]>
                {
                    //ScheduleStart,ScheduleEnd,RaceInterval
                   new object[] { "11:00 AM", "12:00 PM", 63 },
                   new object[] { "01:00 PM", "01:07 PM", 12 },
                };
            }
        }

        #endregion

        #region Methods
        [TestCaseSource(nameof(RaceScheduleTimeFormatTestCases))]
        public void RaceSchedule_ShouldHaveInValidFormat(string dateTime)
        {
            Assert.That(DateTimeUtils.IsValidDateTimeFormat(dateTime), Is.False);
        }

        [TestCaseSource(nameof(RaceIntervalDigitTestCases))]
        public void RaceInterval_ShouldNotBeGreaterThanZero(int raceInterval)
        {
            Assert.LessOrEqual(raceInterval, 0);
        }

        [TestCaseSource(nameof(LobbyWaitTimeDigitTestCases))]
        public void LobbyWaitTimeNotGreaterThanZero(int lobbyWaitTime)
        {
            Assert.LessOrEqual(lobbyWaitTime, 0);
        }

        [TestCaseSource((nameof(LobbyWaitTimeGreaterThanRaceIntervalTestCases)))]
        public void LobbyWaitTime_GreaterThanRaceInterval(int lobbyWaitTime, int raceInterval)
        {
            Assert.Greater(lobbyWaitTime, raceInterval, StringUtils.LOBBYWAITTIME_LESSTHAN_RACEINTERVAL);
        }

        [TestCaseSource(nameof(StartAndEndRaceScheduleTestCases))]
        public void StartAndEndRaceSchedules_AreEqual(string scheduleStart, string scheduleEnd)
        {
            DateTime startSchedule = DateTimeUtils.ConvertToUTCTime(scheduleStart, StringUtils.HOUR_MINUTE_AMPM_TIME_FORMAT);
            DateTime endSchedule = DateTimeUtils.ConvertToUTCTime(scheduleEnd, StringUtils.HOUR_MINUTE_AMPM_TIME_FORMAT);
            Assert.That(DateTimeUtils.AreDateTimesEqual(startSchedule, endSchedule), Is.True);
        }

        [TestCaseSource(nameof(RaceIntervalGreaterThanRaceSchedulesTestCases))]
        public void RaceInterval_GreaterthanRaceSchedules(string scheduleStart, string scheduleEnd, int raceInterval)
        {
            DateTime startSchedule = DateTimeUtils.ConvertToUTCTime(scheduleStart, StringUtils.HOUR_MINUTE_AMPM_TIME_FORMAT);
            DateTime endSchedule = DateTimeUtils.ConvertToUTCTime(scheduleEnd, StringUtils.HOUR_MINUTE_AMPM_TIME_FORMAT);

            //If end time is less than start time, add a day to end time
            if (startSchedule > endSchedule)
            {
                endSchedule = endSchedule.AddDays(1);
            }
            TimeSpan raceTimeSpan = endSchedule - startSchedule;
            TimeSpan raceIntervalSpan = TimeSpan.FromMinutes(raceInterval);
            Assert.That(raceTimeSpan, Is.LessThan(raceIntervalSpan));
        }
        #endregion
    }
}
