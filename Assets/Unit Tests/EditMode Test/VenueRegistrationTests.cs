using System.Collections.Generic;
using NUnit.Framework;

public class VenueRegistrationTests
{
    [TestFixture(Category = "Host")]
    public class PassTests
    {
        #region Test Cases
        public static IEnumerable<object[]> LocationStringPassTestCases
        {
            get
            {
                return new List<object[]>
                {
                    //Latitude,Longitude
                   new object[] { "19.087", "1" },
                   new object[] { "0", "0" }
                };
            }
        }

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
        [TestCaseSource(nameof(LocationStringPassTestCases))]
        public void CheckLocationStringIsNotEmpty(string latitude, string longitude)
        {
            bool condition = !StringUtils.IsStringEmpty(latitude) && !StringUtils.IsStringEmpty(longitude);
            Assert.That(condition, Is.True);
        }

        [TestCaseSource(nameof(LocationValidPassTestCases))]
        public void CheckLocationValid_PassTest(double latitude, double longitude)
        {
            bool result = GPS.IsValidGpsLocation(latitude, longitude);
            //assert
            Assert.That(result, Is.True);
        }

        [TestCaseSource(nameof(RadiusPassTestCases))]
        public void RadiusWithinLimit_PassTest(float radius)
        {
            Assert.That(radius, Is.InRange(HostConfig.radiusMin, HostConfig.radiusMax));
        }
        #endregion

    }

    [TestFixture(Category = "Host")]
    public class FailTests
    {
        #region TestCases

        public static IEnumerable<object[]> LocationStringFailTestCases
        {
            get
            {
                return new List<object[]>
                {
                    //Latitude,Longitude
                   new object[] { "", "" },
                   new object[] { "19.087", "" },
                   new object[] { "", "1" }
                };
            }
        }
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
                };
            }
        }

        #endregion

        #region Methods
        [TestCaseSource(nameof(LocationStringFailTestCases))]
        public void CheckLocationString_FailTest(string latitude, string longitude)
        {
            bool condition = StringUtils.IsStringEmpty(latitude) || StringUtils.IsStringEmpty(longitude);
            Assert.That(condition, Is.True);
        }

        [TestCaseSource(nameof(LocationValidFailTestCases))]
        public void CheckLocationValid_FailTest(double latitude, double longitude)
        {
            bool condition = GPS.IsValidGpsLocation(latitude, longitude);
            Assert.That(condition, Is.False);
        }

        [TestCaseSource(nameof(RadiusFailTestCases))]
        public void RadiusWithinLimit_FailTest(float radius)
        {
            Assert.That(radius, Is.Not.InRange(HostConfig.radiusMin, HostConfig.radiusMax));
        }
        #endregion
    }
}
