using HorseRaceCloudCode;
using Microsoft.Extensions.Logging;
using Unity.Services.CloudCode.Apis;

namespace CloudCodeUnitTests
{
    public class VenueCheckInTests
    {
        [TestFixture(Category = "Client")]
        public class PassTests
        {
            private IGameApiClient gameApiClient;
            private ILogger<VenueCheckIn> logger;

            private VenueCheckIn venueCheckIn;

            [SetUp]
            public void Setup()
            {
                venueCheckIn = new VenueCheckIn(gameApiClient, logger);
            }

            #region Test Cases
            public static IEnumerable<object[]> DateTimesTestCases
            {
                get
                {
                    return new List<object[]>
                   {
                                      //YYYY-MM-DD HH:MM:SS    
                       new object[] { "2024-10-08 T01:58:08" },
                       new object[] { "2024-10-08 T11:05:08" },
                       new object[] { "2024-10-08 T11:05:09" },
                    };
                }
            }
            public static IEnumerable<object[]> LastCheckInIntervalTestCases
            {
                get
                {
                    return new List<object[]>
                    {                //YYYY-MM-DD  HH:MM:SS  ,  HH:MM(LastCheckInTime)
                       new object[] { "2024-10-08 T01:58:00" , "01:45" },
                       new object[] { "2024-10-08 T04:12:00" , "04:00" },
                    };
                }
            }
            public static List<PlayerVenueCheckIn> VenueCheckInsList = new List<PlayerVenueCheckIn>
             {
               new PlayerVenueCheckIn
               {
                   Count = 1,
                   Date = "08",
                   LastCheckInTime = "01:00"
               },
               new PlayerVenueCheckIn
               {
                   Count = 5,
                   Date = "08",
                   LastCheckInTime = "11:00"
               }
             };
            #endregion

            #region Methods
            [TestCaseSource(nameof(DateTimesTestCases))]
            public void TestNextCheckInTime(string dateTIme)
            {
                DateTime currentDateTime = DateTime.Parse(dateTIme);
                TimeSpan timeSpan = venueCheckIn.GetNextCheckInTime(currentDateTime, HostConfig.venueCheckInInterval);

                DateTime nextCheckInTime = currentDateTime.Add(timeSpan);
                bool isNextCheckInTimeValid = nextCheckInTime.Minute % HostConfig.venueCheckInInterval == 0;

                Assert.That(isNextCheckInTimeValid, Is.True);
            }

            [TestCaseSource(nameof(DateTimesTestCases))]
            public void CheckIfPlayerAlreadyCheckedInToday(string _dateTime)
            {
                DateTime currentDateTime = DateTime.Parse(_dateTime);
                bool isAlreadyCheckedIn = venueCheckIn.IsAlreadyCheckedInToday(VenueCheckInsList, currentDateTime, out int index);


                Assert.That(isAlreadyCheckedIn, Is.True);
                bool condition = (VenueCheckInsList[index].Date == currentDateTime.ToString(StringUtils.DAY_FORMAT));
                Assert.IsTrue(condition);
            }

            [TestCaseSource(nameof(LastCheckInIntervalTestCases))]
            public void CheckIfPlayerAlreadyCheckedInCurrentInterval(string _dateTime, string lastCheckInTime)
            {
                DateTime dateTime = DateTime.Parse(_dateTime);
                bool condition = venueCheckIn.IsAlreadyCheckedInCurrentInterval(lastCheckInTime, dateTime);
                Assert.That(condition, Is.True);
            }

            [Test]
            public void IsPlayerAddedToVenueCheckInList()
            {
                int countBeforeAdd = VenueCheckInsList.Count;
                venueCheckIn.AddPlayerCheckInToList(VenueCheckInsList, DateTime.UtcNow);
                Assert.IsTrue(VenueCheckInsList.Count == countBeforeAdd + 1);
            }

            [Test]
            public void CheckIfPlayerVenueCheckInUpdated()
            {
                PlayerVenueCheckIn playerCheckIn = new PlayerVenueCheckIn
                {
                    Count = 1,
                    Date = "08",
                    LastCheckInTime = "01:00"
                };
                DateTime dateTime = DateTime.UtcNow;
                venueCheckIn.UpdatePlayerCheckIn(playerCheckIn, dateTime);
                Assert.That(playerCheckIn.Count, Is.EqualTo(2));
            }
            #endregion
        }

        [TestFixture(Category = "Client")]
        public class FailTests
        {
            private IGameApiClient gameApiClient;
            private ILogger<VenueCheckIn> logger;

            private VenueCheckIn venueCheckIn;

            [SetUp]
            public void Setup()
            {
                venueCheckIn = new VenueCheckIn(gameApiClient, logger);
            }

            #region Test Cases
            public static IEnumerable<object[]> DateTimesTestCases
            {
                get
                {
                    return new List<object[]>
                   {
                                      //YYYY-MM-DD HH:MM:SS    
                       new object[] { "2024-10-08 T01:58:08" },
                       new object[] { "2024-10-08 T11:05:08" },
                       new object[] { "2024-10-08 T11:05:09" },
                    };
                }
            }
            public static IEnumerable<object[]> LastCheckInIntervalTestCases
            {
                get
                {
                    return new List<object[]>
                    {                //YYYY-MM-DD  HH:MM:SS  ,  HH:MM(LastCheckInTime)
                       new object[] { "2024-10-08 T01:58:00" , "01:43" },
                       new object[] { "2024-10-08 T04:00:00" , "03:59" },
                    };
                }
            }
            public static List<PlayerVenueCheckIn> PlayerVenueCheckInsList = new List<PlayerVenueCheckIn>
             {
               new PlayerVenueCheckIn
               {
                   Count = 1,
                   Date = "09",
                   LastCheckInTime = "01:00"
               },
               new PlayerVenueCheckIn
               {
                   Count = 5,
                   Date = "10",
                   LastCheckInTime = "11:00"
               }
             };
            #endregion

            #region Methods
            [TestCaseSource(nameof(DateTimesTestCases))]
            public void TestNextCheckInTimeIsInvalid(string dateTIme)
            {
                DateTime currentDateTime = DateTime.Parse(dateTIme);
                int interval = HostConfig.venueCheckInInterval / 2;
                TimeSpan timeSpan = venueCheckIn.GetNextCheckInTime(currentDateTime, interval);

                DateTime nextCheckInTime = currentDateTime.Add(timeSpan);
                bool isNextCheckInTimeValid = nextCheckInTime.Minute % HostConfig.venueCheckInInterval == 0;
                Assert.That(isNextCheckInTimeValid, Is.False);
            }

            [TestCaseSource(nameof(DateTimesTestCases))]
            public void VerifyPlayerNotCheckedInToday(string _dateTime)
            {
                DateTime currentDateTime = DateTime.Parse(_dateTime);
                bool isAlreadyCheckedIn = venueCheckIn.IsAlreadyCheckedInToday(PlayerVenueCheckInsList, currentDateTime, out int index);
                Assert.That(isAlreadyCheckedIn, Is.False);
            }

            [TestCaseSource(nameof(LastCheckInIntervalTestCases))]
            public void EnsurePlayerCheckInIntervalInvalid(string _dateTime, string lastCheckInTime)
            {
                DateTime dateTime = DateTime.Parse(_dateTime);
                bool condition = venueCheckIn.IsAlreadyCheckedInCurrentInterval(lastCheckInTime, dateTime);
                Assert.That(condition, Is.False);
            }
            #endregion
        }
    }

}
