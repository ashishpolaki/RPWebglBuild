using HorseRaceCloudCode;

namespace CloudCodeUnitTests
{
    public class VenueRegistrationTests
    {
        [TestFixture(Category = "Host")]
        public class PassTests
        {
            #region Test Cases
            public static IEnumerable<object[]> LocationValidPassTestCases
            {
                get
                {
                    return new List<object[]>
                {
                    //Latitude,Longitude
                   new object[] { 19.087, 1 },
                   new object[] { 0, 0 },
                   new object[] { 90, 180 }
                };
                }
            }
            public static IEnumerable<float> RadiusPassTestCases
            {
                get
                {
                    return new List<float>
                {
                    1,
                    5,
                    10,
                    100
                };
                }
            }
            #endregion

            #region Methods
            [TestCaseSource(nameof(LocationValidPassTestCases))]
            public void CheckLocationIsValid(double latitude, double longitude)
            {
                bool result = Utils.IsValidGpsLocation(latitude, longitude);
                //assert
                Assert.That(result, Is.True, "InValid GPS Location");
            }

            [TestCaseSource(nameof(RadiusPassTestCases))]
            public void CheckIfRadiusWithinLimit(float radius)
            {
                Assert.That(radius, Is.InRange(HostConfig.radiusMin, HostConfig.radiusMax), "Radius is out of range");
            }
            #endregion

        }

        public class FailTests
        {
            #region TestCases
            public static IEnumerable<object[]> LocationValidFailTestCases
            {
                get
                {
                    return new List<object[]>
                {
                    //Latitude,Longitude
                   new object[] { 91, 180 },
                   new object[] { 90, 181 },
                   new object[] { 91, 181 }
                };
                }
            }
            public static IEnumerable<float> RadiusFailTestCases
            {
                get
                {
                    return new List<float>
                {
                    0,
                    101,
                    -1
                };
                }
            }
            #endregion

            #region Methods
            [TestCaseSource(nameof(LocationValidFailTestCases))]
            public void CheckIfLocationIsInValid(double latitude, double longitude)
            {
                bool condition = Utils.IsValidGpsLocation(latitude, longitude);
                Assert.That(condition, Is.False, "GPS should be invalid.");
            }

            [TestCaseSource(nameof(RadiusFailTestCases))]
            public void RadiusWithinLimit_FailTest(float radius)
            {
                string message = $"Radius should not be within {HostConfig.radiusMin} and {HostConfig.radiusMax}";
                Assert.That(radius, Is.Not.InRange(HostConfig.radiusMin, HostConfig.radiusMax));
            }
            #endregion
        }
    }

}
