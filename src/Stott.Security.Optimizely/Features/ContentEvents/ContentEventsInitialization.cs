namespace Stott.Security.Optimizely.Features.ContentEvents;

using System;

using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

using Microsoft.Extensions.Logging;

using Stott.Security.Optimizely.Common;
using Stott.Security.Optimizely.Features.Caching;
using Stott.Security.Optimizely.Features.Pages;

[ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
public sealed class ContentSecurityPolicyPageEventsInitialization : IInitializableModule
{
    public void Initialize(InitializationEngine context)
    {
        try
        {
            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.PublishingContent += ContentEvents_PublishingContent;
        }
        catch(Exception exception)
        {
            var logger = ServiceLocator.Current.GetInstance<ILogger<ContentSecurityPolicyPageEventsInitialization>>();
            logger.LogError(exception, "{LogPrefix} Failed to attach to content publishing event.", CspConstants.LogPrefix);
        }
    }

    public void Uninitialize(InitializationEngine context)
    {
        try
        {
            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.PublishingContent -= ContentEvents_PublishingContent;
        }
        catch (Exception exception)
        {
            var logger = ServiceLocator.Current.GetInstance<ILogger<ContentSecurityPolicyPageEventsInitialization>>();
            logger.LogError(exception, "{LogPrefix} Failed to deattach to content publishing event.", CspConstants.LogPrefix);
        }
    }

    private void ContentEvents_PublishingContent(object? sender, ContentEventArgs eventArgs)
    {
        try
        {
            if (eventArgs.Content is IContentSecurityPolicyPage)
            {
                var cacheWrapper = ServiceLocator.Current.GetInstance<ICacheWrapper>();
                cacheWrapper.RemoveAll();
            }
        }
        catch (Exception exception)
        {
            var logger = ServiceLocator.Current.GetInstance<ILogger<ContentSecurityPolicyPageEventsInitialization>>();
            logger.LogError(exception, "{LogPrefix} Failed to deattach to content publishing event.", CspConstants.LogPrefix);
        }
    }
}