using System.Collections.Generic;
using System.Linq;

using Moq;

using NUnit.Framework;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Csp;
using Stott.Security.Optimizely.Features.Csp.Dtos;
using Stott.Security.Optimizely.Features.Csp.Settings;

namespace Stott.Security.Optimizely.Test.Features.Csp;

[TestFixture]
public sealed class CspOptimizerTests
{
    private Mock<ICspSettings> _mockSettings;

    [SetUp]
    public void SetUp()
    {
        _mockSettings = new Mock<ICspSettings>();
        _mockSettings.Setup(x => x.IsNonceEnabled).Returns(false);
    }

    [Test]
    public void GivenThereNoScriptSrcDirectiveButDefaultSrcExists_ThenScriptSrcShouldMatchDefaultSrc()
    {
        // Arrange
        var sources = new List<string> { "'self'", "https://example.com" };
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.DefaultSource, sources),
            new(CspConstants.Directives.ScriptSourceElement, GenerateSources(250)),
            new(CspConstants.Directives.StyleSourceElement, GenerateSources(250))
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var scriptsGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.ScriptSource)));
        var scriptSrc = scriptsGroup.FirstOrDefault(x => x.Directive.Equals(CspConstants.Directives.ScriptSource));

        // Assert
        Assert.That(scriptsGroup, Has.Count.EqualTo(2));
        Assert.That(scriptSrc, Is.Not.Null);
        Assert.That(scriptSrc.Sources, Is.EquivalentTo(sources));
    }

    [Test]
    public void GivenThereIsNoScriptDirectivesOrDefaultSrc_ThenScriptSrcShouldMatchSelf()
    {
        // Arrange
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.ScriptSourceElement, GenerateSources(250)),
            new(CspConstants.Directives.StyleSourceElement, GenerateSources(250))
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var scriptsGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.ScriptSource)));
        var scriptSrc = scriptsGroup?.FirstOrDefault(x => x.Directive.Equals(CspConstants.Directives.ScriptSource));

        // Assert
        Assert.That(scriptsGroup, Has.Count.EqualTo(2));
        Assert.That(scriptSrc, Is.Not.Null);
        Assert.That(scriptSrc.Sources, Has.Count.EqualTo(1));
        Assert.That(scriptSrc.Sources[0], Is.EqualTo(CspConstants.Sources.Self));
    }

    [Test]
    [TestCase(115, false, false)]
    [TestCase(110, true, false)]
    [TestCase(109, true, true)]
    public void GivenScriptSourcesDoNotExceedMaxHeaderSize_ThenScriptSourcesShouldNotBeSimplified(int numberOfSources, bool useNonce, bool useStrictDynamic)
    {
        // Arrange
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.ScriptSource, GenerateSources(numberOfSources, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.ScriptSourceElement, GenerateSources(117, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.ScriptSourceAttribute, GenerateSources(117, false, false)),
            new(CspConstants.Directives.StyleSource, GenerateSources(100, useNonce, useStrictDynamic))
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var scriptsGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.ScriptSource)));

        // Assert
        Assert.That(scriptsGroup, Has.Count.EqualTo(3));
        Assert.That(scriptsGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ScriptSource)), Is.EqualTo(1));
        Assert.That(scriptsGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ScriptSourceElement)), Is.EqualTo(1));
        Assert.That(scriptsGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ScriptSourceAttribute)), Is.EqualTo(1));
    }

    [Test]
    [TestCase(113, false, false)]
    [TestCase(109, true, false)]
    [TestCase(108, true, true)]
    public void GivenScriptSourcesDoNotExceedMaxHeaderSizeAndThereIsAReportTo_ThenScriptSourcesShouldNotBeSimplified(int numberOfSources, bool useNonce, bool useStrictDynamic)
    {
        // Arrange
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.ScriptSource, GenerateSources(numberOfSources, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.ScriptSourceElement, GenerateSources(117, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.ScriptSourceAttribute, GenerateSources(117, false, false)),
            new(CspConstants.Directives.StyleSource, GenerateSources(100, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.ReportTo, "report-url-header")
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var scriptsGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.ScriptSource)));

        // Assert
        Assert.That(scriptsGroup, Has.Count.EqualTo(4));
        Assert.That(scriptsGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ScriptSource)), Is.EqualTo(1));
        Assert.That(scriptsGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ScriptSourceElement)), Is.EqualTo(1));
        Assert.That(scriptsGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ScriptSourceAttribute)), Is.EqualTo(1));
        Assert.That(scriptsGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ReportTo)), Is.EqualTo(1));
    }

    [Test]
    [TestCase(116, false, false)]
    [TestCase(112, true, false)]
    [TestCase(111, true, true)]
    public void GivenScriptSourcesExceedMaxHeaderSize_ThenScriptSourcesShouldBeSimplified(int numberOfSources, bool useNonce, bool useStrictDynamic)
    {
        // Arrange
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.ScriptSource, GenerateSources(numberOfSources, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.ScriptSourceElement, GenerateSources(117, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.ScriptSourceAttribute, GenerateSources(117, false, false)),
            new(CspConstants.Directives.StyleSource, GenerateSources(100, useNonce, useStrictDynamic))
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var scriptsGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.ScriptSource)));

        // Assert
        Assert.That(scriptsGroup, Has.Count.EqualTo(1));
        Assert.That(scriptsGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ScriptSource)), Is.EqualTo(1));
    }

    [Test]
    [TestCase(114, false, false)]
    [TestCase(111, true, false)]
    [TestCase(109, true, true)]
    public void GivenScriptSourcesExceedMaxHeaderSizeAndThereIsAReportTo_ThenScriptSourcesShouldBeSimplifiedToScriptSrcAndReportTo(int numberOfSources, bool useNonce, bool useStrictDynamic)
    {
        // Arrange
        _mockSettings.Setup(x => x.IsNonceEnabled).Returns(useNonce);
        _mockSettings.Setup(x => x.IsStrictDynamicEnabled).Returns(useStrictDynamic);

        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.ScriptSource, GenerateSources(numberOfSources, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.ScriptSourceElement, GenerateSources(117, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.ScriptSourceAttribute, GenerateSources(117, false, false)),
            new(CspConstants.Directives.StyleSource, GenerateSources(100, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.ReportTo, "report-url-header")
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var scriptsGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.ScriptSource)));

        // Assert
        Assert.That(scriptsGroup, Has.Count.EqualTo(2));
        Assert.That(scriptsGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ScriptSource)), Is.EqualTo(1));
        Assert.That(scriptsGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ReportTo)), Is.EqualTo(1));
    }

    [Test]
    public void GivenThereIsNoStyleSrcDirectivesButDefaultSrcExists_ThenStyleSrcShouldMatchDefaultSrc()
    {
        // Arrange
        var sources = new List<string> { "'self'", "https://example.com" };
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.DefaultSource, sources),
            new(CspConstants.Directives.StyleSourceElement, GenerateSources(250)),
            new(CspConstants.Directives.ScriptSourceElement, GenerateSources(250))
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var styleGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.StyleSource)));
        var styleSrc = styleGroup.FirstOrDefault(x => x.Directive.Equals(CspConstants.Directives.StyleSource));

        // Assert
        Assert.That(styleGroup, Has.Count.EqualTo(2));
        Assert.That(styleSrc, Is.Not.Null);
        Assert.That(styleSrc.Sources, Is.EquivalentTo(sources));
    }

    [Test]
    public void GivenThereIsNoStyleSrcDirectivesOrDefaultSrc_ThenStyleSrcShouldMatchSelf()
    {
        // Arrange
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.StyleSourceElement, GenerateSources(250)),
            new(CspConstants.Directives.ScriptSourceElement, GenerateSources(250))
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var styleGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.StyleSource)));
        var styleSrc = styleGroup.FirstOrDefault(x => x.Directive.Equals(CspConstants.Directives.StyleSource));

        // Assert
        Assert.That(styleGroup, Has.Count.EqualTo(2));
        Assert.That(styleSrc, Is.Not.Null);
        Assert.That(styleSrc.Sources, Has.Count.EqualTo(1));
        Assert.That(styleSrc.Sources[0], Is.EqualTo(CspConstants.Sources.Self));
    }

    [Test]
    [TestCase(115, false, false)]
    [TestCase(110, true, false)]
    [TestCase(109, true, true)]
    public void GivenStyleSourcesDoNotExceedMaxHeaderSize_ThenStyleSourcesShouldNotBeSimplified(int numberOfSources, bool useNonce, bool useStrictDynamic)
    {
        // Arrange
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.StyleSource, GenerateSources(numberOfSources, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.StyleSourceElement, GenerateSources(117, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.StyleSourceAttribute, GenerateSources(117, false, false)),
            new(CspConstants.Directives.ScriptSource, GenerateSources(100, useNonce, useStrictDynamic))
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var styleGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.StyleSource)));

        // Assert
        Assert.That(styleGroup, Has.Count.EqualTo(3));
        Assert.That(styleGroup.Count(x => x.Directive.Equals(CspConstants.Directives.StyleSource)), Is.EqualTo(1));
        Assert.That(styleGroup.Count(x => x.Directive.Equals(CspConstants.Directives.StyleSourceElement)), Is.EqualTo(1));
        Assert.That(styleGroup.Count(x => x.Directive.Equals(CspConstants.Directives.StyleSourceAttribute)), Is.EqualTo(1));
    }

    [Test]
    [TestCase(114, false, false)]
    [TestCase(109, true, false)]
    [TestCase(108, true, true)]
    public void GivenStyleSourcesDoNotExceedMaxHeaderSizeAndThereIsAReportTo_ThenStyleSourcesShouldNotBeSimplified(int numberOfSources, bool useNonce, bool useStrictDynamic)
    {
        // Arrange
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.StyleSource, GenerateSources(numberOfSources, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.StyleSourceElement, GenerateSources(117, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.StyleSourceAttribute, GenerateSources(117, false, false)),
            new(CspConstants.Directives.ScriptSource, GenerateSources(100, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.ReportTo, "report-url-header")
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var styleGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.StyleSource)));

        // Assert
        Assert.That(styleGroup, Has.Count.EqualTo(4));
        Assert.That(styleGroup.Count(x => x.Directive.Equals(CspConstants.Directives.StyleSource)), Is.EqualTo(1));
        Assert.That(styleGroup.Count(x => x.Directive.Equals(CspConstants.Directives.StyleSourceElement)), Is.EqualTo(1));
        Assert.That(styleGroup.Count(x => x.Directive.Equals(CspConstants.Directives.StyleSourceAttribute)), Is.EqualTo(1));
        Assert.That(styleGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ReportTo)), Is.EqualTo(1));
    }

    [Test]
    [TestCase(116, false, false)]
    [TestCase(112, true, false)]
    [TestCase(111, true, true)]
    public void GivenStyleSourcesExceedMaxHeaderSize_ThenStyleSourcesShouldBeSimplified(int numberOfSources, bool useNonce, bool useStrictDynamic)
    {
        // Arrange
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.StyleSource, GenerateSources(numberOfSources, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.StyleSourceElement, GenerateSources(117, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.StyleSourceAttribute, GenerateSources(117, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.ScriptSource, GenerateSources(100, useNonce, useStrictDynamic))
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var styleGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.StyleSource)));

        // Assert
        Assert.That(styleGroup, Has.Count.EqualTo(1));
        Assert.That(styleGroup.Count(x => x.Directive.Equals(CspConstants.Directives.StyleSource)), Is.EqualTo(1));
    }

    [Test]
    [TestCase(115, false, false)]
    [TestCase(111, true, false)]
    [TestCase(110, true, true)]
    public void GivenStyleSourcesExceedMaxHeaderSizeAndThereIsAReportTo_ThenStyleSourcesShouldBeSimplifiedToStyleSrcAndReportTo(int numberOfSources, bool useNonce, bool useStrictDynamic)
    {
        // Arrange
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.StyleSource, GenerateSources(numberOfSources, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.StyleSourceElement, GenerateSources(117, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.StyleSourceAttribute, GenerateSources(117, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.ScriptSource, GenerateSources(100, useNonce, useStrictDynamic)),
            new(CspConstants.Directives.ReportTo, "report-url-header")
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var styleGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.StyleSource)));

        // Assert
        Assert.That(styleGroup, Has.Count.EqualTo(2));
        Assert.That(styleGroup.Count(x => x.Directive.Equals(CspConstants.Directives.StyleSource)), Is.EqualTo(1));
        Assert.That(styleGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ReportTo)), Is.EqualTo(1));
    }

    [Test]
    public void GivenThereIsNoChildSrcDirectivesButDefaultSrcExists_ThenChildSrcShouldMatchDefaultSrc()
    {
        // Arrange
        var sources = new List<string> { "'self'", "https://example.com" };
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.DefaultSource, sources),
            new(CspConstants.Directives.FrameSource, GenerateSources(250)),
            new(CspConstants.Directives.ScriptSource, GenerateSources(250))
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var frameGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.ChildSource)));
        var childSrc = frameGroup?.FirstOrDefault(x => x.Directive.Equals(CspConstants.Directives.ChildSource));

        // Assert
        Assert.That(frameGroup, Is.Not.Null);
        Assert.That(frameGroup, Has.Count.EqualTo(2));
        Assert.That(childSrc, Is.Not.Null);
        Assert.That(childSrc.Sources, Is.EquivalentTo(sources));
    }

    [Test]
    public void GivenThereIsNoChildSrcDirectivesOrDefaultSrc_ThenChildSrcShouldMatchSelf()
    {
        // Arrange
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.FrameSource, GenerateSources(250)),
            new(CspConstants.Directives.ScriptSource, GenerateSources(250))
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var frameGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.ChildSource)));
        var childSrc = frameGroup?.FirstOrDefault(x => x.Directive.Equals(CspConstants.Directives.ChildSource));

        // Assert
        Assert.That(frameGroup, Has.Count.EqualTo(2));
        Assert.That(childSrc, Is.Not.Null);
        Assert.That(childSrc.Sources, Has.Count.EqualTo(1));
        Assert.That(childSrc.Sources[0], Is.EqualTo(CspConstants.Sources.Self));
    }

    [Test]
    [TestCase(false, false)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public void GivenFrameSourcesDoNotExceedMaxHeaderSize_ThenFrameSourcesShouldNotBeSimplified(bool useNonce, bool useStrictDynamic)
    {
        // Arrange
        var sources = GenerateSources(70, useNonce, useStrictDynamic);
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.FencedFrameSource, sources),
            new(CspConstants.Directives.FrameSource, sources),
            new(CspConstants.Directives.ChildSource, sources),
            new(CspConstants.Directives.WorkerSource, sources)
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var frameGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.ChildSource)));

        // Assert
        Assert.That(frameGroup, Has.Count.EqualTo(4));
        Assert.That(frameGroup.Count(x => x.Directive.Equals(CspConstants.Directives.FencedFrameSource)), Is.EqualTo(1));
        Assert.That(frameGroup.Count(x => x.Directive.Equals(CspConstants.Directives.FrameSource)), Is.EqualTo(1));
        Assert.That(frameGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ChildSource)), Is.EqualTo(1));
        Assert.That(frameGroup.Count(x => x.Directive.Equals(CspConstants.Directives.WorkerSource)), Is.EqualTo(1));
    }

    [Test]
    [TestCase(false, false)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public void GivenFrameSourcesDoNotExceedMaxHeaderSizeAndThereIsAReportTo_ThenFrameSourcesShouldNotBeSimplified(bool useNonce, bool useStrictDynamic)
    {
        // Arrange
        var sources = GenerateSources(70, useNonce, useStrictDynamic);
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.FencedFrameSource, sources),
            new(CspConstants.Directives.FrameSource, sources),
            new(CspConstants.Directives.ChildSource, sources),
            new(CspConstants.Directives.WorkerSource, sources),
            new(CspConstants.Directives.ReportTo, "report-url-header")
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var frameGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.ChildSource)));

        // Assert
        Assert.That(frameGroup, Has.Count.EqualTo(5));
        Assert.That(frameGroup.Count(x => x.Directive.Equals(CspConstants.Directives.FencedFrameSource)), Is.EqualTo(1));
        Assert.That(frameGroup.Count(x => x.Directive.Equals(CspConstants.Directives.FrameSource)), Is.EqualTo(1));
        Assert.That(frameGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ChildSource)), Is.EqualTo(1));
        Assert.That(frameGroup.Count(x => x.Directive.Equals(CspConstants.Directives.WorkerSource)), Is.EqualTo(1));
        Assert.That(frameGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ReportTo)), Is.EqualTo(1));
    }

    [Test]
    [TestCase(false, false)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public void GivenFrameSourcesExceedMaxHeaderSize_ThenFrameSourcesShouldBeSimplified(bool useNonce, bool useStrictDynamic)
    {
        // Arrange
        var sources = GenerateSources(250, useNonce, useStrictDynamic);
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.FencedFrameSource, sources),
            new(CspConstants.Directives.FrameSource, sources),
            new(CspConstants.Directives.ChildSource, sources),
            new(CspConstants.Directives.WorkerSource, sources)
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var frameGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.ChildSource)));

        // Assert
        Assert.That(frameGroup, Has.Count.EqualTo(1));
        Assert.That(frameGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ChildSource)), Is.EqualTo(1));
    }

    [Test]
    public void GivenFrameSourcesExceedMaxHeaderSizeAndThereIsAReportTo_ThenFrameSourcesShouldBeSimplifiedToChildSrcAndReportTo()
    {
        // Arrange
        var sources = GenerateSources(250);
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.StyleSource, sources),
            new(CspConstants.Directives.StyleSourceElement, sources),
            new(CspConstants.Directives.StyleSourceAttribute, sources),
            new(CspConstants.Directives.ReportTo, "report-url-header")
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var frameGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.ChildSource)));

        // Assert
        Assert.That(frameGroup, Has.Count.EqualTo(2));
        Assert.That(frameGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ChildSource)), Is.EqualTo(1));
        Assert.That(frameGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ReportTo)), Is.EqualTo(1));
    }

    [Test]
    [TestCase(CspConstants.Directives.ConnectSource)]
    [TestCase(CspConstants.Directives.FontSource)]
    [TestCase(CspConstants.Directives.ImageSource)]
    [TestCase(CspConstants.Directives.ManifestSource)]
    [TestCase(CspConstants.Directives.MediaSource)]
    [TestCase(CspConstants.Directives.ObjectSource)]
    public void GivenOtherFetchSourceIsAbsentButDefaultSrcExists_ThenOtherFetchSourceShouldMatchDefaultSrc(string missingDirective)
    {
        // Arrange
        var sources = new List<string> { "'self'", "https://example.com" };
        var otherSources = GenerateSources(80);
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.DefaultSource, sources),
            new(CspConstants.Directives.ConnectSource, otherSources),
            new(CspConstants.Directives.FontSource, otherSources),
            new(CspConstants.Directives.ImageSource, otherSources),
            new(CspConstants.Directives.ManifestSource, otherSources),
            new(CspConstants.Directives.MediaSource, otherSources),
            new(CspConstants.Directives.ObjectSource, otherSources)
        };

        directives = directives.Where(x => !x.Directive.Equals(missingDirective)).ToList();

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var otherGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.Equals(missingDirective)));
        var otherSource = otherGroup?.FirstOrDefault(x => x.Directive.Equals(missingDirective));

        // Assert
        Assert.That(otherSource, Is.Not.Null);
        Assert.That(otherSource.Sources, Is.EquivalentTo(sources));
    }

    [Test]
    [TestCase(CspConstants.Directives.ConnectSource)]
    [TestCase(CspConstants.Directives.FontSource)]
    [TestCase(CspConstants.Directives.ImageSource)]
    [TestCase(CspConstants.Directives.ManifestSource)]
    [TestCase(CspConstants.Directives.MediaSource)]
    [TestCase(CspConstants.Directives.ObjectSource)]
    public void GivenOtherFetchSourceIsAbsentAndDefaultSrcDoesNotExist_ThenChildSrcShouldMatchSelf(string missingDirective)
    {
        // Arrange
        var otherSources = GenerateSources(80);
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.ConnectSource, otherSources),
            new(CspConstants.Directives.FontSource, otherSources),
            new(CspConstants.Directives.ImageSource, otherSources),
            new(CspConstants.Directives.ManifestSource, otherSources),
            new(CspConstants.Directives.MediaSource, otherSources),
            new(CspConstants.Directives.ObjectSource, otherSources)
        };

        directives = directives.Where(x => !x.Directive.Equals(missingDirective)).ToList();

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var otherGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.Equals(missingDirective)));
        var otherSource = otherGroup?.FirstOrDefault(x => x.Directive.Equals(missingDirective));

        // Assert
        Assert.That(otherSource, Is.Not.Null);
        Assert.That(otherSource.Sources, Has.Count.EqualTo(1));
        Assert.That(otherSource.Sources[0], Is.EqualTo(CspConstants.Sources.Self));
    }

    [Test]
    public void GivenOtherFetchSourcesDoNotExceedMaxHeaderSize_ThenOtherFetchSourcesWillNotBeSplitIntoSeparateGroups()
    {
        // Arrange
        var sources = GenerateSources(45);
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.ConnectSource, sources),
            new(CspConstants.Directives.FontSource, sources),
            new(CspConstants.Directives.ImageSource, sources),
            new(CspConstants.Directives.ManifestSource, sources),
            new(CspConstants.Directives.MediaSource, sources),
            new(CspConstants.Directives.ObjectSource, sources)
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var otherGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.ConnectSource)));

        // Assert
        Assert.That(otherGroup, Has.Count.EqualTo(6));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ConnectSource)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.FontSource)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ImageSource)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ManifestSource)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.MediaSource)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ObjectSource)), Is.EqualTo(1));
    }

    [Test]
    public void GivenOtherFetchSourcesDoNotExceedMaxHeaderSizeAndReportToExists_ThenOtherFetchSourcesWillNotBeSplitIntoSeparateGroups()
    {
        // Arrange
        var sources = GenerateSources(45);
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.ConnectSource, sources),
            new(CspConstants.Directives.FontSource, sources),
            new(CspConstants.Directives.ImageSource, sources),
            new(CspConstants.Directives.ManifestSource, sources),
            new(CspConstants.Directives.MediaSource, sources),
            new(CspConstants.Directives.ObjectSource, sources),
            new(CspConstants.Directives.ReportTo, "report-url-header")
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var otherGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.ConnectSource)));

        // Assert
        Assert.That(otherGroup, Has.Count.EqualTo(7));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ConnectSource)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.FontSource)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ImageSource)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ManifestSource)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.MediaSource)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ObjectSource)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ReportTo)), Is.EqualTo(1));
    }

    [Test]
    public void GivenOtherFetchSourcesExceedMaxHeaderSize_ThenOtherFetchSourcesShouldBeSplitAcrossMultipleGroups()
    {
        // Arrange
        var sources = GenerateSources(90);
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.ConnectSource, sources),
            new(CspConstants.Directives.FontSource, sources),
            new(CspConstants.Directives.ImageSource, sources),
            new(CspConstants.Directives.ManifestSource, sources),
            new(CspConstants.Directives.MediaSource, sources),
            new(CspConstants.Directives.ObjectSource, sources)
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var otherGroups = result.Where(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.ConnectSource))
                                         || x.Any(y => y.Directive.Equals(CspConstants.Directives.FontSource))
                                         || x.Any(y => y.Directive.Equals(CspConstants.Directives.ImageSource))
                                         || x.Any(y => y.Directive.Equals(CspConstants.Directives.ManifestSource))
                                         || x.Any(y => y.Directive.Equals(CspConstants.Directives.MediaSource))
                                         || x.Any(y => y.Directive.Equals(CspConstants.Directives.ObjectSource))).ToList();

        // Assert
        Assert.That(result, Has.Count.GreaterThan(5));
        Assert.That(otherGroups, Has.Count.GreaterThan(1));
        Assert.That(otherGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.ConnectSource))), Is.EqualTo(1));
        Assert.That(otherGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.FontSource))), Is.EqualTo(1));
        Assert.That(otherGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.ImageSource))), Is.EqualTo(1));
        Assert.That(otherGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.ManifestSource))), Is.EqualTo(1));
        Assert.That(otherGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.MediaSource))), Is.EqualTo(1));
        Assert.That(otherGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.ObjectSource))), Is.EqualTo(1));
    }

    [Test]
    public void GivenOtherFetchSourcesExceedMaxHeaderSizeWithReportTo_ThenOtherFetchSourcesShouldBeSplitAcrossMultipleGroupsEachWithReportTo()
    {
        // Arrange
        var sources = GenerateSources(90);
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.ConnectSource, sources),
            new(CspConstants.Directives.FontSource, sources),
            new(CspConstants.Directives.ImageSource, sources),
            new(CspConstants.Directives.ManifestSource, sources),
            new(CspConstants.Directives.MediaSource, sources),
            new(CspConstants.Directives.ObjectSource, sources),
            new(CspConstants.Directives.ReportTo, "report-url-header")
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var otherGroups = result.Where(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.ConnectSource))
                                         || x.Any(y => y.Directive.Equals(CspConstants.Directives.FontSource))
                                         || x.Any(y => y.Directive.Equals(CspConstants.Directives.ImageSource))
                                         || x.Any(y => y.Directive.Equals(CspConstants.Directives.ManifestSource))
                                         || x.Any(y => y.Directive.Equals(CspConstants.Directives.MediaSource))
                                         || x.Any(y => y.Directive.Equals(CspConstants.Directives.ObjectSource))).ToList();

        // Assert
        Assert.That(result, Has.Count.GreaterThan(5));
        Assert.That(otherGroups, Has.Count.GreaterThan(1));
        Assert.That(otherGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.ConnectSource))), Is.EqualTo(1));
        Assert.That(otherGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.FontSource))), Is.EqualTo(1));
        Assert.That(otherGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.ImageSource))), Is.EqualTo(1));
        Assert.That(otherGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.ManifestSource))), Is.EqualTo(1));
        Assert.That(otherGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.MediaSource))), Is.EqualTo(1));
        Assert.That(otherGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.ObjectSource))), Is.EqualTo(1));
        Assert.That(otherGroups.All(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.ReportTo))), Is.True);
    }

    [Test]
    public void GivenStandAloneDirectivesDoNotExceedHeaderSize_ThenStandAloneSourcesWillNotBeSplitIntoSeparateGroups()
    {
        // Arrange
        var sources = GenerateSources(50);
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.BaseUri, sources),
            new(CspConstants.Directives.FormAction, sources),
            new(CspConstants.Directives.FrameAncestors, sources),
            new(CspConstants.Directives.UpgradeInsecureRequests, sources),
            new(CspConstants.Directives.Sandbox, sources)
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var otherGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.BaseUri)));

        // Assert
        Assert.That(otherGroup, Has.Count.EqualTo(5));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.BaseUri)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.FormAction)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.FrameAncestors)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.UpgradeInsecureRequests)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.Sandbox)), Is.EqualTo(1));
    }

    [Test]
    public void GivenStandAloneDirectivesDoNotExceedHeaderSizeWithReportTo_ThenStandAloneSourcesWillNotBeSplitIntoSeparateGroups()
    {
        // Arrange
        var sources = GenerateSources(50);
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.BaseUri, sources),
            new(CspConstants.Directives.FormAction, sources),
            new(CspConstants.Directives.FrameAncestors, sources),
            new(CspConstants.Directives.UpgradeInsecureRequests, sources),
            new(CspConstants.Directives.Sandbox, sources),
            new(CspConstants.Directives.ReportTo, "report-url-header")
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var otherGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.BaseUri)));

        // Assert
        Assert.That(otherGroup, Has.Count.EqualTo(6));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.BaseUri)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.FormAction)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.FrameAncestors)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.UpgradeInsecureRequests)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.Sandbox)), Is.EqualTo(1));
        Assert.That(otherGroup.Count(x => x.Directive.Equals(CspConstants.Directives.ReportTo)), Is.EqualTo(1));
    }

    [Test]
    public void GivenStandAloneDirectivesDoExceedHeaderSize_ThenStandAloneSourcesWillBeSplitIntoSeparateGroups()
    {
        // Arrange
        var sources = GenerateSources(95);
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.BaseUri, sources),
            new(CspConstants.Directives.FormAction, sources),
            new(CspConstants.Directives.FrameAncestors, sources),
            new(CspConstants.Directives.UpgradeInsecureRequests, sources),
            new(CspConstants.Directives.Sandbox, sources)
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var standAloneGroups = result.Where(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.BaseUri))
                                              || x.Any(y => y.Directive.Equals(CspConstants.Directives.FormAction))
                                              || x.Any(y => y.Directive.Equals(CspConstants.Directives.FrameAncestors))
                                              || x.Any(y => y.Directive.Equals(CspConstants.Directives.UpgradeInsecureRequests))
                                              || x.Any(y => y.Directive.Equals(CspConstants.Directives.Sandbox))).ToList();

        // Assert
        Assert.That(result, Has.Count.GreaterThan(5));
        Assert.That(standAloneGroups, Has.Count.GreaterThan(1));
        Assert.That(standAloneGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.BaseUri))), Is.EqualTo(1));
        Assert.That(standAloneGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.FormAction))), Is.EqualTo(1));
        Assert.That(standAloneGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.FrameAncestors))), Is.EqualTo(1));
        Assert.That(standAloneGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.UpgradeInsecureRequests))), Is.EqualTo(1));
        Assert.That(standAloneGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.Sandbox))), Is.EqualTo(1));
    }

    [Test]
    public void GivenStandAloneDirectivesDoExceedHeaderSizeWithReportTo_ThenStandAloneSourcesWillBeSplitIntoSeparateGroupsEachWithReportTo()
    {
        // Arrange
        var sources = GenerateSources(95);
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.BaseUri, sources),
            new(CspConstants.Directives.FormAction, sources),
            new(CspConstants.Directives.FrameAncestors, sources),
            new(CspConstants.Directives.UpgradeInsecureRequests, sources),
            new(CspConstants.Directives.Sandbox, sources),
            new(CspConstants.Directives.ReportTo, "report-url-header")
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var standAloneGroups = result.Where(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.BaseUri))
                                              || x.Any(y => y.Directive.Equals(CspConstants.Directives.FormAction))
                                              || x.Any(y => y.Directive.Equals(CspConstants.Directives.FrameAncestors))
                                              || x.Any(y => y.Directive.Equals(CspConstants.Directives.UpgradeInsecureRequests))
                                              || x.Any(y => y.Directive.Equals(CspConstants.Directives.Sandbox))).ToList();

        // Assert
        Assert.That(result, Has.Count.GreaterThan(5));
        Assert.That(standAloneGroups, Has.Count.GreaterThan(1));
        Assert.That(standAloneGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.BaseUri))), Is.EqualTo(1));
        Assert.That(standAloneGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.FormAction))), Is.EqualTo(1));
        Assert.That(standAloneGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.FrameAncestors))), Is.EqualTo(1));
        Assert.That(standAloneGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.UpgradeInsecureRequests))), Is.EqualTo(1));
        Assert.That(standAloneGroups.Count(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.Sandbox))), Is.EqualTo(1));
        Assert.That(standAloneGroups.All(x => x.Any(y => y.Directive.Equals(CspConstants.Directives.ReportTo))), Is.True);
    }

    [Test]
    [TestCase(73, false)]
    [TestCase(74, true)]
    public void WhenContentWouldExceedMaximumHeaderThreshold_ThenNoContentShouldBeReturned(int numberOfItems, bool shouldBeEmpty)
    {
        // Arrange
        var sources = GenerateSources(numberOfItems);
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.FrameSource, sources),
            new(CspConstants.Directives.ScriptSource, sources),
            new(CspConstants.Directives.StyleSource, sources),
            new(CspConstants.Directives.ConnectSource, sources),
            new(CspConstants.Directives.FontSource, sources),
            new(CspConstants.Directives.ImageSource, sources),
            new(CspConstants.Directives.ManifestSource, sources),
            new(CspConstants.Directives.MediaSource, sources),
            new(CspConstants.Directives.ObjectSource, sources),
            new(CspConstants.Directives.ReportTo, "report-url-header")
        };
        
        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        
        // Assert
        Assert.That(result.Count == 0, Is.EqualTo(shouldBeEmpty));
    }

    [Test]
    [TestCase(31, 1)]
    [TestCase(32, 5)]
    public void WhenContentWouldExceedHeaderThreshold_ThenNoContentShouldBeSplit(int numberOfItems, int expectedCount)
    {
        // Arrange
        var sources = GenerateSources(numberOfItems);
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.FrameSource, sources),
            new(CspConstants.Directives.ScriptSource, sources),
            new(CspConstants.Directives.ScriptSourceElement, sources),
            new(CspConstants.Directives.StyleSource, sources),
            new(CspConstants.Directives.StyleSourceElement, sources),
            new(CspConstants.Directives.ConnectSource, sources),
            new(CspConstants.Directives.FontSource, sources),
            new(CspConstants.Directives.ImageSource, sources),
            new(CspConstants.Directives.ManifestSource, sources),
            new(CspConstants.Directives.MediaSource, sources),
            new(CspConstants.Directives.ObjectSource, sources),
            new(CspConstants.Directives.ReportTo, "report-url-header")
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);

        // Assert
        Assert.That(result, Has.Count.EqualTo(expectedCount));
    }

    [Test]
    [TestCase(39, 4)]
    [TestCase(40, 2)]
    public void WhenContentWouldExceedSimplificationThreshold_ThenContentShouldBeSimplified(int numberOfItems, int expectedCount)
    {
        // Arrange
        var sources = GenerateSources(numberOfItems);
        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.FrameSource, sources),
            new(CspConstants.Directives.ScriptSource, sources),
            new(CspConstants.Directives.ScriptSourceElement, sources),
            new(CspConstants.Directives.ScriptSourceAttribute, sources),
            new(CspConstants.Directives.StyleSource, sources),
            new(CspConstants.Directives.StyleSourceElement, sources),
            new(CspConstants.Directives.StyleSourceAttribute, sources),
            new(CspConstants.Directives.ConnectSource, sources),
            new(CspConstants.Directives.FontSource, sources),
            new(CspConstants.Directives.ImageSource, sources),
            new(CspConstants.Directives.ManifestSource, sources),
            new(CspConstants.Directives.MediaSource, sources),
            new(CspConstants.Directives.ObjectSource, sources),
            new(CspConstants.Directives.ReportTo, "report-url-header")
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);
        var styleGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.StyleSource)));
        var scriptGroup = result.FirstOrDefault(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.ScriptSource)));

        // Assert
        Assert.That(scriptGroup, Has.Count.EqualTo(expectedCount));
        Assert.That(styleGroup, Has.Count.EqualTo(expectedCount));
    }

    [Test]
    public void GivenVeryLongDomainNames_ThenOptimizationHandlesCorrectly()
    {
        // Arrange - Create domains with realistic long names (60+ characters)
        // This tests size calculation with non-uniform domain lengths
        var longDomains = new List<string>
        {
            "https://very-long-subdomain-name.extremely-long-domain-provider-name.co.uk",
            "https://another-exceptionally-lengthy-analytics-provider.example.com",
            "https://content-delivery-network-with-geographic-identifier.cloudfront.net",
            "https://third-party-marketing-automation-platform.marketing-tools.io",
            "https://enterprise-resource-planning-system.corporate-domain.com"
        };

        // Repeat to create substantial size
        var sources = new List<string>();
        for (int i = 0; i < 50; i++)
        {
            sources.AddRange(longDomains);
        }

        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.ScriptSource, sources),
            new(CspConstants.Directives.StyleSource, sources)
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);

        // Assert - Should handle long domains by splitting appropriately
        Assert.That(result, Is.Not.Empty, "Should produce at least one group");
        Assert.That(result.Any(g => g.Any(d => d.Directive == CspConstants.Directives.ScriptSource)), Is.True);
        Assert.That(result.Any(g => g.Any(d => d.Directive == CspConstants.Directives.StyleSource)), Is.True);
    }

    [Test]
    public void GivenMixedDomainLengths_ThenOptimizationCalculatesSizeCorrectly()
    {
        // Arrange - Mix of very short and very long domains (realistic scenario)
        // Tests that size prediction works correctly with varied domain lengths
        var mixedSources = new List<string>
        {
            "https://cdn.com",  // Very short (15 chars)
            "https://example.com",  // Short (20 chars)
            "https://very-long-subdomain-name.extremely-long-domain-provider-name.co.uk",  // Very long (77 chars)
            "https://api.example.com",  // Medium (23 chars)
            "https://another-exceptionally-lengthy-analytics-provider.tracking-domain.com"  // Very long (79 chars)
        };

        // Create enough mixed sources to approach threshold
        var sources = new List<string>();
        for (int i = 0; i < 100; i++)
        {
            sources.Add(mixedSources[i % mixedSources.Count]);
        }

        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.ScriptSource, sources),
            new(CspConstants.Directives.ScriptSourceElement, sources),
            new(CspConstants.Directives.StyleSource, sources)
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);

        // Assert - Should handle mixed lengths correctly
        Assert.That(result, Is.Not.Empty);
        Assert.That(result.Count(x => x.Any(y => y.Directive.StartsWith(CspConstants.Directives.ScriptSource))), Is.EqualTo(1));
    }

    [Test]
    public void GivenOnlySpecialKeywords_ThenHandlesCorrectly()
    {
        // Arrange - Only special CSP keywords (no domains)
        // Tests edge case where CSP uses only keywords for tight security
        var specialSources = new List<string>
        {
            CspConstants.Sources.Self,
            CspConstants.Sources.UnsafeInline,
            CspConstants.Sources.UnsafeEval,
            CspConstants.Sources.StrictDynamic,
            CspConstants.Sources.UnsafeHashes,
            CspConstants.Sources.Nonce
        };

        var directives = new List<CspDirectiveDto>
        {
            new(CspConstants.Directives.ScriptSource, specialSources),
            new(CspConstants.Directives.StyleSource, specialSources)
        };

        // Act
        var result = CspOptimizer.GroupDirectives(_mockSettings.Object, directives);

        // Assert - Should handle keyword-only sources (very small, no splitting needed)
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Has.Count.LessThanOrEqualTo(5), "Keywords are small, should not require splitting");
    }

    private static List<string> GenerateSources(int amount, bool useNonce = false, bool useStrictDynamic = false)
    {
        var items = Enumerable.Range(0, amount).Select(i => $"https://{i}.example.com").ToList();

        if (useNonce)
        {
            items.Add(CspConstants.Sources.Nonce);
        }

        if (useStrictDynamic)
        {
            items.Add(CspConstants.Sources.StrictDynamic);
        }

        return items;
    }
}