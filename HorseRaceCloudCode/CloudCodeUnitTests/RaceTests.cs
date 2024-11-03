using HorseRaceCloudCode;
using Microsoft.Extensions.Logging;
using Unity.Services.CloudCode.Apis;

namespace CloudCodeUnitTests
{
    public class RaceTests
    {
        [TestFixture(Category = "Race")]
        public class PassTests
        {
            private IGameApiClient gameApiClient;
            private ILogger<RaceStart> logger;
            private IPushClient pushClient;

            private RaceStart raceStart;
            [SetUp]
            public void Setup()
            {
                raceStart = new RaceStart(gameApiClient, pushClient, logger);
            }

            #region Test Cases
            public static List<int> LobbyPlayersCountTestCases = new List<int>
            {
                 2, 3,
            };
            #endregion

            #region Methods
            [TestCaseSource(nameof(LobbyPlayersCountTestCases))]
            public void CheckHostLobbyPlayers(int count)
            {
                bool condition = raceStart.CheckLobbyPlayers(count, out string lobbyPlayersErrorMessage);
                Assert.IsTrue(condition, lobbyPlayersErrorMessage);
            }
            #endregion
        }

        [TestFixture(Category = "Race")]
        public class FailTests
        {
            private IGameApiClient gameApiClient;
            private ILogger<RaceStart> logger;
            private IPushClient pushClient;

            private RaceStart raceStart;
            [SetUp]
            public void Setup()
            {
                raceStart = new RaceStart(gameApiClient, pushClient, logger);
            }

            #region Test Cases
            public static List<int> LobbyPlayersCountTestCases = new List<int>
            {
                 1, 13,14
            };
            #endregion

            #region Methods
            [TestCaseSource(nameof(LobbyPlayersCountTestCases))]
            public void CheckHostLobbyPlayers(int count)
            {
                bool condition = raceStart.CheckLobbyPlayers(count, out string lobbyPlayersErrorMessage);
                Assert.IsFalse(condition);
            }
            #endregion
        }
    }
}
