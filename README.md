# Stott.Security.Optimizely

[![Platform](https://img.shields.io/badge/Platform-.NET%206-blue.svg?style=flat)](https://docs.microsoft.com/en-us/dotnet/)
[![Platform](https://img.shields.io/badge/Optimizely-%2012-blue.svg?style=flat)](http://world.episerver.com/cms/)
[![GitHub](https://img.shields.io/github/license/GeekInTheNorth/Stott.Security.Optimizely)](https://github.com/GeekInTheNorth/Stott.Security.Optimizely/blob/main/LICENSE.txt)
![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/GeekInTheNorth/Stott.Security.Optimizely/dotnet.yml?branch=develop)
![Nuget](https://img.shields.io/nuget/v/Stott.Security.Optimizely)

Stott.Security.Optimizely is a security header editor for Optimizely CMS 12 that provides the user with the ability to define the Content Security Policy and other security headers.  What makes this module unique in terms of Content Security Policy management is that users are presented with the ability to define a source and to select the permissions for that source. e.g. can https://www.example.com be used a script source, can it contain the current site in an iFrame, etc.

**Please note that this is currently under active development and is already live on a small selection of production sites.  Please reach out if you would like to use this module in it's current beta state.**

## Configuration

After pulling in a reference to the Stott.Security.Optimizely project, you only need to ensure the following lines are added to the startup class of your solution:

```C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddRazorPages();
    services.AddCspManager();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseCspManager();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapContent();
        endpoints.MapRazorPages();
    });
}
```

The call to ```services.AddRazorPages()``` is a standard .NET 6.0 call to ensure razor pages are included in your solution.

The call to ```services.AddCspManager()``` in the ```ConfigureServices(IServiceCollection services)``` sets up the dependency injection requirements for the CSP solution and is required to ensure the solution works as intended.  This works by following the Services Extensions pattern defined by microsoft.

The call to ```app.UseCspManager()``` in the ```Configure(IApplicationBuilder app, IWebHostEnvironment env)``` method sets up the CSP middleware.  This should be declared immediately before the ```app.UseEndpoints(...)``` method to ensure that the headers are added to content pages.

This solution also includes an implementation of ```IMenuProvider``` which ensures that the CSP administration pages are included in the CMS Admin menu under the title of "CSP".  You do not have to do anything to make this work as Optimizely CMS will scan and action all implementations of ```IMenuProvider```.

## Additional Configuration Customisation

The configuration of the module has some scope for modification by providing configuration in the service extension methods.  Both the provision of ```cspSetupOptions``` and ```authorizationOptions``` are optional in the following example.

Example:
```C#
services.AddCspManager(cspSetupOptions =>
{
    cspSetupOptions.ConnectionStringName = "EPiServerDB";
},
authorizationOptions => 
{
    authorizationOptions.AddPolicy(CspConstants.AuthorizationPolicy, policy =>
    {
        policy.RequireRole("WebAdmins");
    });
});
```

### Default Configuration Options

| Configuration | Default Values | Notes |
|---------------|----------------|-------|
| Allowed Roles | **WebAdmins** or **CmsAdmins** or **Administrator** | Defines the roles required in order to access the Admin interface. |
| Connection String Name | **EPiServerDB** | Defines which connection string to use for modules data storage.  Must be a SQL Server connection string. |

## CSP Reporting

A Content Security Policy can be set to report violations to a given end point. An API endpoint has been added to the solution which allows for CSP reports to be sent to the CMS. Browsers can batch up these reports and send them at a later point in time. This can lead to monitoring violations to be non-responsive. By adding the following ViewComponent to your layout files, violations will be posted to the CMS as they occur.

```C#
@await Component.InvokeAsync("CspReporting")
```

This works by adding an event listener for the security violation and are raised by the browser by adding a listener to the security policy violation event.

## Agency Whitelisting

SEO and Data teams within Digital Agencies, may have many sites which they have to maintain collectively as a team.  Approving a new tool to be injected via GTM may be made once, but may need applying to dozens of websites, each of which may have it's own CSP whitelist.

If you have applied the CSP Reporting component (see above), then this plugin can automatically extend the whitelist for the site based on centralized approved list.

### Central Whitelist Structure

The structure of the central whitelist must exist as a JSON object reachable by a GET method for the specified Whitelist Url.  The JSON returned should be an array with each entry having a ```sourceUrl``` and an array of ```directives```. All of these should be valid strings.

```JSON
[
	{
		"sourceUrl": "https://*.google.com",
		"directives": [ "default-src" ]
	},
	{
		"sourceUrl": "https://*.twitter.com",
		"directives": [ "script-src", "style-src" ]
	},
	{
		"sourceUrl": "https://pbs.twimg.com",
		"directives": [ "img-src" ]
	}
]
```

## Default CSP Settings

In order to prevent a CSP from preventing Optimizely CMS from functioning optimally, the following sources and directives are automatically generated on application start provided that no CSP Sources currently exist:

| Source | Default Directives |
|--------|--------------------|
| 'self' | child-src, connect-src, default-src, font-src, frame-src, img-src, script-src, style-src |
| 'unsafe-inline' | script-src, style-src |
| 'unsafe-eval' | script-src |
| ```https://dc.services.visualstudio.com``` | connect-src, script-src |
| ```https://*.msecnd.net``` | script-src |

## Extending the CSP for a single content page

If you have the need to extend the Content Security Policy for individual pages, then you can decorate the page content type with the `IContentSecurityPolicyPage` interface and implement the `ContentSecurityPolicySources` as per the following example:

```
public class MyPage : PageData, IContentSecurityPolicyPage
{
    [Display(
        Name = "Content Security Policy Sources",
        Description = "The following Content Security Policy Sources will be merged into the global Content Security Policy when visiting this page",
        GroupName = "Security",
        Order = 10)]
    [EditorDescriptor(EditorDescriptorType = typeof(CspSourceMappingEditorDescriptor))]
    public virtual IList<PageCspSourceMapping> ContentSecurityPolicySources { get; set; }
}
```

When a user visits this page, the sources added to this control will be merged into the main content security policy. As caching is used to improve the performance of the security header resolution, if a page implements `IContentSecurityPolicyPage` then the cache key used will include both the Content Id and ticks from the modified date of the page.  If the page being visited does not implement this interface, then the cache key used will be the globally unique value.

This module hooks into the Optimizely PublishingContent events as exposed by `IContentEvents`.  When a publish event is raised for a page that inherits `IContentSecurityPolicyPage`, then ALL CSP related cache is removed based on a master key.  If for some reason, the publishing events are not clearing the cache for any given page, then forcing an update of the Modified Date for the page will result in a new cache key being required for that page.

## FAQ

### My static files like server-error.html do not have the CSP applied

Make sure that the call to `app.UseStaticFiles()` is made after the call to `app.UseCspManager()` to ensure that the CSP middleware is applied to the static file request.

### My Page which implements `IContentSecurityPolicyPage` is not updating with the global content security policy changes.

Pages that use `IContentSecurityPolicyPage` use a separate CSP cache entry to the global CSP cache.  The cache will expire after 1 hour, or you can force a cache clearance for that page by updating the modified date of the page.

### What mode is the best mode to test my CSP with?

It is highly recommended that you put your global CSP into Report Only mode while you test changes to the Content Security Policy.  As this is applied globally (including to the CMS back end) there is a potential for you to damage your CMS editor experience if your Content Security Policy disallows essential CMS functions.

## Contributing

I am open to contributions to the code base.  The following rules should be followed:

1. Contributions should be made by Pull Requests.
2. All commits should have a meaningful messages.
3. All commits should have a reference to your GitHub user.
4. Ideally all new changes should include appropriate unit test coverage.

### Technologies Used

- .NET 6.0
- Optimizely CMS (EPiServer.CMS.UI.Core 12.9.0)
- MVC
- Razor Class Libraries
- React
- Bootstrap for React
- NUnit & Moq
