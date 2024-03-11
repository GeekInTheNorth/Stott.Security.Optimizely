using Microsoft.AspNetCore.Http;

using NUnit.Framework;

using Stott.Security.Optimizely.Extensions;

namespace Stott.Security.Optimizely.Test.Extensions;

[TestFixture]
public sealed class HeaderDictionaryExtensionsTests
{
    [Test]
    [TestCaseSource(typeof(HeaderDictionaryExtensionsTestCases), nameof(HeaderDictionaryExtensionsTestCases.ValidHeaderTestCases))]
    public void AddOrUpdateHeader_GivenHeaderValueDoesNotExist_ThenHeaderIsAdded(string headerName, string headerValue)
    {
        // Arrange
        var headers = new HeaderDictionary();

        // Act
        headers.AddOrUpdateHeader(headerName, headerValue);

        // Assert
        Assert.That(headers.ContainsKey(headerName), Is.True);
        Assert.That(headers[headerName], Is.EqualTo(headerValue));
    }

    [Test]
    [TestCaseSource(typeof(HeaderDictionaryExtensionsTestCases), nameof(HeaderDictionaryExtensionsTestCases.ValidHeaderTestCases))]
    public void AddOrUpdateHeader_GivenHeaderValueDoesExist_ThenHeaderIsUpdated(string headerName, string headerValue)
    {
        // Arrange
        var headers = new HeaderDictionary();
        headers.Append(headerName, "Old Value");

        // Act
        headers.AddOrUpdateHeader(headerName, headerValue);

        // Assert
        Assert.That(headers.ContainsKey(headerName), Is.True);
        Assert.That(headers[headerName], Is.EqualTo(headerValue));
    }

    [Test]
    [TestCaseSource(typeof(HeaderDictionaryExtensionsTestCases), nameof(HeaderDictionaryExtensionsTestCases.InvalidHeaderTestCases))]
    public void AddOrUpdateHeader_GivenHeaderNameOrValueIsNullOrEmpty_ThenHeaderIsNotUpdated(string headerName, string headerValue)
    {
        // Arrange
        var headers = new HeaderDictionary();

        // Act
        headers.AddOrUpdateHeader(headerName, headerValue);

        // Assert
        Assert.That(headers.ContainsKey(headerName), Is.False);
    }
}