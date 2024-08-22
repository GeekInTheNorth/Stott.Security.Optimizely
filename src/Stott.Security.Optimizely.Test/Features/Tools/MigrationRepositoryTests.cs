using System;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Entities;
using Stott.Security.Optimizely.Features.Tools;

namespace Stott.Security.Optimizely.Test.Features.Tools;

[TestFixture]
public sealed class MigrationRepositoryTests
{
    private Mock<ICspDataContext> _mockDataContext;

    private Lazy<ICspDataContext> _lazyDataContext;

    private MigrationRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _mockDataContext = new Mock<ICspDataContext>();
        _lazyDataContext = new Lazy<ICspDataContext>(() => _mockDataContext.Object);
        
        _repository = new MigrationRepository(_lazyDataContext);
    }

    [Test]
    [TestCaseSource(typeof(MigrationRepositoryTestCases), nameof(MigrationRepositoryTestCases.GetInvalidArgumentsTestCases))]
    public async Task GivenTheSettingsOrModifiedIsNullOrEmpty_ThenChangesWillNotBeMade(SettingsModel settings, string modifiedBy)
    {
        // Act
        await _repository.SaveAsync(settings, modifiedBy);

        // Assert
        _mockDataContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}