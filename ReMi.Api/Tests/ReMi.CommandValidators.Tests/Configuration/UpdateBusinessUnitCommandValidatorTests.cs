using NUnit.Framework;
using ReMi.Commands.Configuration;
using ReMi.CommandValidators.Configuration;
using System;
using ReMi.TestUtils.UnitTests;

namespace ReMi.CommandValidators.Tests.Configuration
{
    public class UpdateBusinessUnitCommandValidatorTests : TestClassFor<UpdateBusinessUnitCommandValidator>
    {
        protected override UpdateBusinessUnitCommandValidator ConstructSystemUnderTest()
        {
            return new UpdateBusinessUnitCommandValidator();
        }

        [Test]
        public void Validate_ShouldReturnThreeError_WhenCommandIsEmpty()
        {
            var command = new UpdateBusinessUnitCommand();

            var result = Sut.Validate(command);

            Assert.AreEqual(3, result.Errors.Count, "error list size");
            Assert.True(result.Errors[0].PropertyName.Contains("ExternalId"));
            Assert.True(result.Errors[1].PropertyName.Contains("Description"));
            Assert.True(result.Errors[2].PropertyName.Contains("Name"));
        }


        [Test]
        public void Validate_ShouldNotReturnErrors_WhenCommandIsOk()
        {
            var command = new UpdateBusinessUnitCommand
            {
                Description = RandomData.RandomString(34),
                ExternalId = Guid.NewGuid(),
                Name = RandomData.RandomString(10)
            };

            var result = Sut.Validate(command);

            Assert.AreEqual(0, result.Errors.Count, "error list size");
        }
    }
}
