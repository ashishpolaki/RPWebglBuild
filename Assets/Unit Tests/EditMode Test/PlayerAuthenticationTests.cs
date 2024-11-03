using System.Collections.Generic;
using NUnit.Framework;
using UGS;

public class PlayerAuthenticationTests
{
    [TestFixture(Category = "PlayerAccountValidation")]
    public class PassTests
    {
        #region Setup Data
        private Authentication authentication;

        [SetUp]
        public void Setup()
        {
            authentication = new Authentication();
        }
        #endregion

        #region Test Cases
        public static IEnumerable<string> UsernamePassTestCases
        {
            get
            {
                return new List<string>
            {
                "IronMan",
                "Ash_",
            };
            }
        }
        public static IEnumerable<string> PasswordPassTestCases
        {
            get
            {
                return new List<string>
            {
                "IronMan%123",
                "IronMan@123",
                "IronMan#123"
            };
            }
        }
        public static IEnumerable<string> PlayerNamePassTestCases
        {
            get
            {
                return new List<string>
            {
                "IronMan",
                "_",
            };
            }
        }
        #endregion

        #region Methods
        [TestCaseSource(nameof(UsernamePassTestCases))]
        public void UsernameCriteria(string username)
        {
            // Act
            var result = authentication.CheckUsernameCriteria(username);

            // Assert
            Assert.That(result, Is.True);
        }

        [TestCaseSource(nameof(PasswordPassTestCases))]
        public void PasswordCriteria(string password)
        {
            // Act
            var result = authentication.CheckPasswordCriteria(password);

            // Assert
            Assert.That(result, Is.True);
        }
        #endregion
    }

    [TestFixture(Category = "PlayerAccountValidation")]
    public class FailTests
    {
        #region Setup Data
        private Authentication authentication;

        [SetUp]
        public void Setup()
        {
            authentication = new Authentication();
        }
        #endregion

        #region Test Cases
        public static IEnumerable<string> UsernameFailTestCases
        {
            get
            {
                return new List<string>
            {
                "IronMan$",
                "Thor#123"
            };
            }
        }

        public static IEnumerable<string> PasswordFailTestCases
        {
            get
            {
                return new List<string>
            {
                "Thor",
                "THOR@123",
                "thor#123"
            };
            }
        }

        public static IEnumerable<string> PlayerNameFailTestCases
        {
            get
            {
                return new List<string>
            {
                "",
                "Ashish @123",
            };
            }
        }
        #endregion

        #region Methods
        [TestCaseSource(nameof(UsernameFailTestCases))]
        public void UsernameCriteria(string username)
        {
            // Act
            var result = authentication.CheckUsernameCriteria(username);

            // Assert
            Assert.That(result, Is.False);
        }

        [TestCaseSource(nameof(PasswordFailTestCases))]
        public void PasswordCriteria(string password)
        {
            // Act
            var result = authentication.CheckPasswordCriteria(password);

            // Assert
            Assert.That(result, Is.False);
        }
        #endregion
    }
}
