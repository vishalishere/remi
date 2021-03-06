using NUnit.Framework;
using ReMi.BusinessEntities.BusinessRules;
using ReMi.Commands.BusinessRules;
using ReMi.CommandValidators.BusinessRules;
using System;
using System.Collections.Generic;
using System.Linq;
using ReMi.TestUtils.UnitTests;

namespace ReMi.CommandValidators.Tests.BusinessRules
{
    public class SaveQueryPermissionRuleCommandValidatorTests : TestClassFor<SaveQueryPermissionRuleCommandValidator>
    {
        protected override SaveQueryPermissionRuleCommandValidator ConstructSystemUnderTest()
        {
            return new SaveQueryPermissionRuleCommandValidator();
        }

        [Test]
        public void Validate_ShouldNotPass_WhenRuleHasEmptyFillouts()
        {
            var command = new SaveQueryPermissionRuleCommand
            {
                Rule = new BusinessRuleDescription
                {
                    AccountTestData = new BusinessRuleAccountTestData(),
                    Parameters = new List<BusinessRuleParameter>
                    {
                        new BusinessRuleParameter
                        {
                            TestData = new BusinessRuleTestData()
                        }
                    }
                }
            };

            var result = Sut.Validate(command);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(12, result.Errors.Count);
            Assert.AreEqual("Rule.Description", result.Errors.ElementAt(0).PropertyName);
            Assert.AreEqual("Rule.ExternalId", result.Errors.ElementAt(1).PropertyName);
            Assert.AreEqual("Rule.Name", result.Errors.ElementAt(2).PropertyName);
            Assert.AreEqual("Rule.Script", result.Errors.ElementAt(3).PropertyName);
            Assert.AreEqual("Rule.AccountTestData.ExternalId", result.Errors.ElementAt(4).PropertyName);
            Assert.AreEqual("Rule.AccountTestData.JsonData", result.Errors.ElementAt(5).PropertyName);
            Assert.AreEqual("Rule.Parameters[0].ExternalId", result.Errors.ElementAt(6).PropertyName);
            Assert.AreEqual("Rule.Parameters[0].Name", result.Errors.ElementAt(7).PropertyName);
            Assert.AreEqual("Rule.Parameters[0].Type", result.Errors.ElementAt(8).PropertyName);
            Assert.AreEqual("Rule.Parameters[0].TestData.ExternalId", result.Errors.ElementAt(9).PropertyName);
            Assert.AreEqual("Rule.Parameters[0].TestData.JsonData", result.Errors.ElementAt(10).PropertyName);
            Assert.AreEqual("QueryId", result.Errors.ElementAt(11).PropertyName);
        }


        [Test]
        public void Validate_ShouldNotPass_WhenRuleHasNoParameters()
        {
            var command = new SaveQueryPermissionRuleCommand
            {
                QueryId = RandomData.RandomInt(int.MaxValue),
                Rule = new BusinessRuleDescription
                {
                    ExternalId = Guid.NewGuid(),
                    Script = "script",
                    Name = RandomData.RandomString(10),
                    Description = RandomData.RandomString(10),
                    AccountTestData = new BusinessRuleAccountTestData
                    {
                        ExternalId = Guid.NewGuid(),
                        JsonData = "account json"
                    }
                }
            };

            var result = Sut.Validate(command);

            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Rule.Parameters", result.Errors.ElementAt(0).PropertyName);
        }

        [Test]
        public void Validate_ShouldNotPass_WhenHasMoreThenOneParameter()
        {
            var command = new SaveQueryPermissionRuleCommand
            {
                QueryId = RandomData.RandomInt(int.MaxValue),
                Rule = new BusinessRuleDescription
                {
                    ExternalId = Guid.NewGuid(),
                    Script = "script",
                    Name = RandomData.RandomString(10),
                    Description = RandomData.RandomString(10),
                    AccountTestData = new BusinessRuleAccountTestData
                    {
                        ExternalId = Guid.NewGuid(),
                        JsonData = "account json"
                    },
                    Parameters = new List<BusinessRuleParameter>
                    {
                        new BusinessRuleParameter
                        {
                            ExternalId = Guid.NewGuid(),
                            Name = RandomData.RandomString(10),
                            Type = RandomData.RandomString(10),
                            TestData = new BusinessRuleTestData
                            {
                                ExternalId = Guid.NewGuid(),
                                JsonData = "parameter json"
                            }
                        },
                        new BusinessRuleParameter
                        {
                            ExternalId = Guid.NewGuid(),
                            Name = RandomData.RandomString(10),
                            Type = RandomData.RandomString(10),
                            TestData = new BusinessRuleTestData
                            {
                                ExternalId = Guid.NewGuid(),
                                JsonData = "parameter json"
                            }
                        }
                    }
                }
            };

            var result = Sut.Validate(command);

            Assert.AreEqual(1, result.Errors.Count);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Rule.Parameters", result.Errors.ElementAt(0).PropertyName);
        }

        [Test]
        public void Validate_ShouldPass_WhenAllFilloutsAreCorrect()
        {
            var command = new SaveQueryPermissionRuleCommand
            {
                QueryId = RandomData.RandomInt(int.MaxValue),
                Rule = new BusinessRuleDescription
                {
                    ExternalId = Guid.NewGuid(),
                    Script = "script",
                    Name = RandomData.RandomString(10),
                    Description = RandomData.RandomString(10),
                    AccountTestData = new BusinessRuleAccountTestData
                    {
                        ExternalId = Guid.NewGuid(),
                        JsonData = "account json"
                    },
                    Parameters = new List<BusinessRuleParameter>
                    {
                        new BusinessRuleParameter
                        {
                            ExternalId = Guid.NewGuid(),
                            Name = RandomData.RandomString(10),
                            Type = RandomData.RandomString(10),
                            TestData = new BusinessRuleTestData
                            {
                                ExternalId = Guid.NewGuid(),
                                JsonData = "parameter json"
                            }
                        }
                    }
                }
            };

            var result = Sut.Validate(command);

            Assert.AreEqual(0, result.Errors.Count);
            Assert.IsTrue(result.IsValid);
        }
    }
}
