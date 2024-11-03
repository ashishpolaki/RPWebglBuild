using HorseRaceCloudCode;
using Microsoft.Extensions.Logging;
using Unity.Services.CloudCode.Apis;

namespace CloudCodeUnitTests
{

    public class JoinRaceTests
    {
        [TestFixture(Category = "Client")]
        public class PassTests
        {
            private IGameApiClient gameApiClient;
            private ILogger<JoinRace> logger;

            private JoinRace joinRace;

            #region Test Cases
            public static IEnumerable<object[]> StartAndEndRaceTimesTestCases
            {
                get
                {
                    return new List<object[]>
                   {
                                      //Start Time , End Time
                       new object[] { "09:00"      , "07:00" },
                       new object[] { "04:00"      , "03:00" },
                    };
                }
            }
            public static IEnumerable<object[]> GenerateRaceTimingsWithIntervalTestCases
            {
                get
                {
                    return new List<object[]>
                   {
                                      //Start Time , End Time   , Interval
                       new object[] { "04:00"      , "07:00"    , 5 },
                       new object[] { "01:00"      , "03:00"    , 10 },
                    };
                }
            }
            public static IEnumerable<object[]> RaceTimingsTestCases
            {
                get
                {
                    return new List<object[]>
                    {
                                       //YYYY-MM-DD HH:MM:SS 
                        new object[] { "2024-10-08 T01:10:00" },
                        new object[] { "2024-10-08 T11:00:00" },
                    };
                }
            }

            public List<DateTime> RaceTimingsList = new List<DateTime>
            {
                               //YYYY-MM-DD HH:MM:SS         
                DateTime.Parse("2024-10-08 T01:00:00"),
                DateTime.Parse("2024-10-08 T02:00:00"),
                DateTime.Parse("2024-10-08 T03:00:00"),
                DateTime.Parse("2024-10-08 T04:00:00"),
                DateTime.Parse("2024-10-08 T05:00:00"),
                DateTime.Parse("2024-10-08 T06:00:00"),
                DateTime.Parse("2024-10-08 T07:00:00"),
                DateTime.Parse("2024-10-08 T08:00:00"),
                DateTime.Parse("2024-10-08 T09:00:00"),
                DateTime.Parse("2024-10-08 T10:00:00"),
                DateTime.Parse("2024-10-08 T11:00:00"),
                DateTime.Parse("2024-10-08 T12:00:00"),
            };

            public static IEnumerable<object[]> PlayerWaitInLobbyTestCases
            {
                get
                {
                    return new List<object[]>
                    {
                        //RaceTime,CurrentTime,LobbyWaitTime
                        new object[] { "01:15", "01:10", 5 },
                        new object[] { "02:50", "02:47", 10 },
                    };
                }
            }
            #endregion


            [SetUp]
            public void Setup()
            {
                joinRace = new JoinRace(gameApiClient, logger);
            }

            #region Methods

            [TestCaseSource(nameof(StartAndEndRaceTimesTestCases))]
            public void AdjustRaceEndTimeIfEarlierThanStartTime(string startRaceTime, string endRaceTime)
            {
                joinRace.AdjustEndTimeIfEarlierThanStartTime(startRaceTime, endRaceTime, out DateTime raceStartTime, out DateTime raceEndTime);
                Assert.That(raceEndTime > raceStartTime, Is.True);
            }

            [TestCaseSource(nameof(GenerateRaceTimingsWithIntervalTestCases))]
            public void CheckIfRaceTimingsGeneratedFromSchedule(string _raceStartTime, string _raceEndTime, int raceInterval)
            {
                DateTime raceStartTime = DateTime.Parse(_raceStartTime);
                DateTime raceEndTime = DateTime.Parse(_raceEndTime);

                List<DateTime> raceTimings = joinRace.GenerateRaceTimingsFromSchedule(raceStartTime, raceEndTime, TimeSpan.FromMinutes(raceInterval));
                TimeSpan span = raceEndTime - raceStartTime;
                int expectedCount = (int)(span.TotalMinutes / raceInterval) + 1;

                Assert.That(raceTimings.Count, Is.EqualTo(expectedCount));
            }

            [TestCaseSource(nameof(RaceTimingsTestCases))]
            public void CheckIfNextRaceToday(string _currentDateTime)
            {
                DateTime currentDateTime = DateTime.Parse(_currentDateTime);
                DateTime? nextRaceDateTime = joinRace.FindNextRaceToday(RaceTimingsList, currentDateTime);
                Assert.That(nextRaceDateTime, Is.Not.Null);

                if (nextRaceDateTime != null)
                {
                    int index = RaceTimingsList.IndexOf(nextRaceDateTime.Value);
                    Assert.That(index, Is.GreaterThanOrEqualTo(0));
                    Assert.That(nextRaceDateTime, Is.EqualTo(RaceTimingsList[index]));
                }
            }

            [TestCaseSource(nameof(PlayerWaitInLobbyTestCases))]
            public void CheckIfPlayerCanWaitInLobby(string _raceTime, string _currentTime, int lobbyWaitTime)
            {
                DateTime raceTime = DateTime.Parse(_raceTime);
                DateTime currentTime = DateTime.Parse(_currentTime);
                bool canWaitInLobby = joinRace.CanPlayerWaitInLobby(raceTime, currentTime, lobbyWaitTime);
                Assert.That(canWaitInLobby, Is.True);
            }
            #endregion

        }

        [TestFixture(Category = "Client")]
        public class FailTests
        {
            private IGameApiClient gameApiClient;
            private ILogger<JoinRace> logger;

            private JoinRace joinRace;

            #region Test Cases
            public static IEnumerable<object[]> StartAndEndRaceTimesTestCases
            {
                get
                {
                    return new List<object[]>
                   {
                                      //Start Time , End Time
                       new object[] { "09:00"      , "07:00" },
                       new object[] { "04:00"      , "03:00" },
                    };
                }
            }
            public static IEnumerable<object[]> GenerateRaceTimingsWithIntervalTestCases
            {
                get
                {
                    return new List<object[]>
                   {
                                      //Start Time , End Time   , Interval
                       new object[] { "04:00"      , "07:00"    , 5 },
                       new object[] { "01:00"      , "03:00"    , 10 },
                    };
                }
            }
            public static IEnumerable<object[]> RaceTimingsTestCases
            {
                get
                {
                    return new List<object[]>
                    {
                                       //YYYY-MM-DD HH:MM:SS 
                        new object[] { "2024-10-09 T01:10:00" },
                        new object[] { "2024-10-11 T11:00:00" },
                    };
                }
            }

            public List<DateTime> RaceTimingsList = new List<DateTime>
            {
                               //YYYY-MM-DD HH:MM:SS         
                DateTime.Parse("2024-10-08 T01:00:00"),
                DateTime.Parse("2024-10-08 T02:00:00"),
                DateTime.Parse("2024-10-08 T03:00:00"),
                DateTime.Parse("2024-10-08 T04:00:00"),
                DateTime.Parse("2024-10-08 T05:00:00"),
                DateTime.Parse("2024-10-08 T06:00:00"),
                DateTime.Parse("2024-10-08 T07:00:00"),
                DateTime.Parse("2024-10-08 T08:00:00"),
                DateTime.Parse("2024-10-08 T09:00:00"),
                DateTime.Parse("2024-10-08 T10:00:00"),
                DateTime.Parse("2024-10-08 T11:00:00"),
                DateTime.Parse("2024-10-08 T12:00:00"),
            };

            public static IEnumerable<object[]> PlayerWaitInLobbyTestCases
            {
                get
                {
                    return new List<object[]>
                    {
                        //RaceTime,CurrentTime,LobbyWaitTime
                        new object[] { "01:15", "01:10", 4 },
                        new object[] { "02:50", "02:47", 2 },
                    };
                }
            }
            #endregion


            [SetUp]
            public void Setup()
            {
                joinRace = new JoinRace(gameApiClient, logger);
            }

            #region Methods
            [TestCaseSource(nameof(StartAndEndRaceTimesTestCases))]
            public void AdjustRaceEndTimeIfEarlierThanStartTime(string startRaceTime, string endRaceTime)
            {
                joinRace.AdjustEndTimeIfEarlierThanStartTime(startRaceTime, endRaceTime, out DateTime raceEndTime, out DateTime raceStartTime);
                Assert.That(raceEndTime > raceStartTime, Is.False);
            }

            [TestCaseSource(nameof(GenerateRaceTimingsWithIntervalTestCases))]
            public void TestRaceTimingsGeneratedFromScheduleAreInvalid(string _raceStartTime, string _raceEndTime, int raceInterval)
            {
                DateTime raceStartTime = DateTime.Parse(_raceStartTime);
                DateTime raceEndTime = DateTime.Parse(_raceEndTime);

                List<DateTime> raceTimings = joinRace.GenerateRaceTimingsFromSchedule(raceEndTime, raceStartTime, TimeSpan.FromMinutes(raceInterval));
                TimeSpan span = raceEndTime - raceStartTime;
                int expectedCount = (int)(span.TotalMinutes / raceInterval) + 1;

                Assert.That(raceTimings.Count, Is.Not.EqualTo(expectedCount));
            }

            [TestCaseSource(nameof(RaceTimingsTestCases))]
            public void CheckIfNextRaceIsNotToday(string _currentDateTime)
            {
                DateTime currentDateTime = DateTime.Parse(_currentDateTime);
                DateTime? nextRaceDateTime = joinRace.FindNextRaceToday(RaceTimingsList, currentDateTime);
                Assert.That(nextRaceDateTime, Is.Null);
            }

            [TestCaseSource(nameof(PlayerWaitInLobbyTestCases))]
            public void CheckIfPlayerCannotJoinLobby(string _raceTime, string _currentTime, int lobbyWaitTime)
            {
                DateTime raceTime = DateTime.Parse(_raceTime);
                DateTime currentTime = DateTime.Parse(_currentTime);
                bool canWaitInLobby = joinRace.CanPlayerWaitInLobby(raceTime, currentTime, lobbyWaitTime);
                Assert.That(canWaitInLobby, Is.False);
            }
            #endregion
        }
    }
}
