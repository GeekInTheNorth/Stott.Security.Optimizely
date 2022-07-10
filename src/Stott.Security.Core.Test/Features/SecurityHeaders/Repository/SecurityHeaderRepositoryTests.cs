namespace Stott.Security.Core.Test.Features.SecurityHeaders.Repository;

using System;
using System.Threading.Tasks;

using NUnit.Framework;

using Stott.Security.Core.Entities;
using Stott.Security.Core.Features.SecurityHeaders.Enums;
using Stott.Security.Core.Features.SecurityHeaders.Repository;

[TestFixture]
public class SecurityHeaderRepositoryTests
{
    private TestDataContext _inMemoryDatabase;

    private SecurityHeaderRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _inMemoryDatabase = TestDataContextFactory.Create();

        _repository = new SecurityHeaderRepository(_inMemoryDatabase);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _inMemoryDatabase.Reset();
    }

    [Test]
    public async Task GetAsync_GivenThereAreNoSavedSettings_ThenDefaultSettingsShouldBeReturned()
    {
        // Act
        var settings = await _repository.GetAsync();

        // Assert
        Assert.That(settings, Is.Not.Null);
        Assert.That(settings.XContentTypeOptions, Is.EqualTo(XContentTypeOptions.None));
        Assert.That(settings.XssProtection, Is.EqualTo(XssProtection.None));
        Assert.That(settings.ReferrerPolicy, Is.EqualTo(ReferrerPolicy.None));
        Assert.That(settings.FrameOptions, Is.EqualTo(XFrameOptions.None));
    }

    [Test]
    public async Task GetAsync_GivenThereAreMultipleSavedSettings_ThenThefirstSettingsShouldBeReturned()
    {
        // Arrange
        var settingsOne = new SecurityHeaderSettings
        {
            Id = Guid.NewGuid(),
            FrameOptions = XFrameOptions.SameOrigin,
            ReferrerPolicy = ReferrerPolicy.SameOrigin,
            XContentTypeOptions = XContentTypeOptions.None,
            XssProtection = XssProtection.None,
            CrossOriginEmbedderPolicy = CrossOriginEmbedderPolicy.None,
            CrossOriginOpenerPolicy = CrossOriginOpenerPolicy.None,
            CrossOriginResourcePolicy = CrossOriginResourcePolicy.None,
            IsStrictTransportSecurityEnabled = false,
            IsStrictTransportSecuritySubDomainsEnabled = false,
            StrictTransportSecurityMaxAge = 123,
            ForceHttpRedirect = false
        };

        var settingsTwo = new SecurityHeaderSettings
        {
            Id = Guid.NewGuid(),
            FrameOptions = XFrameOptions.Deny,
            ReferrerPolicy = ReferrerPolicy.NoReferrer,
            XContentTypeOptions = XContentTypeOptions.NoSniff,
            XssProtection = XssProtection.Enabled,
            CrossOriginEmbedderPolicy = CrossOriginEmbedderPolicy.UnsafeNone,
            CrossOriginOpenerPolicy = CrossOriginOpenerPolicy.SameOrigin,
            CrossOriginResourcePolicy = CrossOriginResourcePolicy.SameSite,
            IsStrictTransportSecurityEnabled = true,
            IsStrictTransportSecuritySubDomainsEnabled = true,
            StrictTransportSecurityMaxAge = 456,
            ForceHttpRedirect = true
        };

        _inMemoryDatabase.SecurityHeaderSettings.AddRange(settingsOne, settingsTwo);
        _inMemoryDatabase.SaveChanges();

        // Act
        var settings = await _repository.GetAsync();

        // Assert
        Assert.That(settings, Is.Not.Null);
        Assert.That(settings.XContentTypeOptions, Is.EqualTo(settingsOne.XContentTypeOptions));
        Assert.That(settings.XssProtection, Is.EqualTo(settingsOne.XssProtection));
        Assert.That(settings.ReferrerPolicy, Is.EqualTo(settingsOne.ReferrerPolicy));
        Assert.That(settings.FrameOptions, Is.EqualTo(settingsOne.FrameOptions));
        Assert.That(settings.CrossOriginEmbedderPolicy, Is.EqualTo(settingsOne.CrossOriginEmbedderPolicy));
        Assert.That(settings.CrossOriginOpenerPolicy, Is.EqualTo(settingsOne.CrossOriginOpenerPolicy));
        Assert.That(settings.CrossOriginResourcePolicy, Is.EqualTo(settingsOne.CrossOriginResourcePolicy));
        Assert.That(settings.IsStrictTransportSecurityEnabled, Is.EqualTo(settingsOne.IsStrictTransportSecurityEnabled));
        Assert.That(settings.IsStrictTransportSecuritySubDomainsEnabled, Is.EqualTo(settingsOne.IsStrictTransportSecuritySubDomainsEnabled));
        Assert.That(settings.StrictTransportSecurityMaxAge, Is.EqualTo(settingsOne.StrictTransportSecurityMaxAge));
        Assert.That(settings.ForceHttpRedirect, Is.EqualTo(settingsOne.ForceHttpRedirect));
    }
}
