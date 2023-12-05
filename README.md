# Stott Security

[![Platform](https://img.shields.io/badge/Platform-.NET%206-blue.svg?style=flat)](https://docs.microsoft.com/en-us/dotnet/)
[![Platform](https://img.shields.io/badge/Optimizely-%2012-blue.svg?style=flat)](http://world.episerver.com/cms/)
[![GitHub](https://img.shields.io/github/license/GeekInTheNorth/Stott.Security.Optimizely)](https://github.com/GeekInTheNorth/Stott.Security.Optimizely/blob/main/LICENSE.txt)
![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/GeekInTheNorth/Stott.Security.Optimizely/dotnet.yml?branch=develop)
![Nuget](https://img.shields.io/nuget/v/Stott.Security.Optimizely)

Stott.Security.Optimizely is a security header editor for Optimizely CMS 12 that provides the user with the ability to define the Content Security Policy (CSP), Cross-origin Resource Sharing (CORS) and other security headers.  What makes this module unique in terms of Content Security Policy management is that users are presented with the ability to define a source and to select the permissions for that source. e.g. can https://www.example.com be used a script source, can it contain the current site in an iFrame, etc.

## Interface

### Content Security Policy Settings

The CSP Settings tab is the first of four tabs dedicated to managing your Content Security Policy.  This tab allows you to enable or disable your content security policy as well as to put it into a reporting only mode.  If Use Report Only Mode is enabled, then any third party source that is not included in your list of CSP Sources will not be blocked, but will show up in your browser console as an error while still executing.  It is recommended that you enable the Report Only mode when you are first configuring and testing your Content Security Policy.

Some digital agencies will be responsible for multiple websites and will have a common set of tools that they use for tracking user interactions.  The Remote Allow List properties allow you to configure a central allow list of sources and directives.  When a violation is detected, this module can check this allow list and add the extra dependencies into the CSP Sources.  You can read more about this further on in this documentation.

![CSP Settings Tab](/Images/CspSettingsTab.png)

| Setting | Default | Recommended |
|---------|---------|-------------|
| Enable Content Security Policy (CSP) | false | true |
| Use Report Only Mode | false | false (true during initial configuration) |
| Use Remote CSP Allow List | false | |
| Remote CSP Allow List Address | *empty* | |
| Upgrade Insecure Requests | false | false |

### Content Security Policy Sources

The CSP Sources tab is the second of four tabs dedicated to managing your Content Security Policy.  This tab has been designed with the premise of understanding what a third party can do and to allow you to grant a third party access to multiple directives all at once and so that you can remove the same third party source just as easily.  Each directive is given a user friendly description to allow less technical people to understand what a third party can do.

**Updated in version 2.0.0.0 to include source and directive filtering.**

![CSP Sources Tab](/Images/CspSourcesTab.png)

Recommendations:

- Only grant **default-src** to either the **'self'** or **'none'** directive.
  - Granting **'self'** the **default-src** directive will say that the current site can perform actions on itself by default.
  - Granting **'none'** the **default-src** directive will say that neither the current site or any third party can perform any action by default.  This will require you to grant specific directives to **'self'**
- Make sure that you turn on Report Only mode when altering and testing your Content Security Policy.
- Make sure that you turn off Report Only mode when you are confident the right sources have the right directives.
- Make sure that you test all of the following to make sure they do not report errors before turning off report only mode.
  - CMS Editor Interface
  - CMS Admin Interface
  - Third Party Plugin Interface
  - Login / Logout functionality

### Content Security Policy Sandbox

The CSP Sandbox tab is the third of four tabs dedicated to managing your Content Security Policy.  This tab is dedicated to the **sandbox** directive.  Unlike other directives such as **script-src**, the **sandbox** directive does not operate grant permissions to sources, but instead instruct the browser on what APIs and browser functionality the website can access.

![CSP Sandbox Tab](/Images/CspSandboxTab.png)

### Content Security Policy Violations

The CSP Violations tab is the forth tab dedicated to managing your Content Security Policy.  This tab requires a developer to add the reporting view component to the website (read more below under CSP Reporting).  When the plugin receives a report of a violation of the Content Security Policy, it will make a record of the third party source and what directive was violated. This is then presented to the user so that that can see how often a violation is happening and when it last happened.  A handy **Create CSP Entry** button allows the user to quickly merge the violated source and directive into the Content Security Policy.

**Updated in version 2.0.0.0 to include source and directive filtering.**

![CSP Violations Tab](/Images/CspViolationTab.png)

### Cross Origin Resource Sharing

**New in version 2.0.0.0**

The CORS tab is new in version 2.0.0.0 and allows the user to configure the Cross-Origin Resource Sharing headers for the website.  This is used to grant permissions to third party websites to consume APIs and content from your website.  As trends have moved towards headless and hybrid solutions, controlling your CORS headers can be essential to allowing hybrid solutions to work.

![CORS Tab](/Images/CorsTab.png)

| Setting | Default | Recommended |
|---------|---------|-------------|
| Enable Cross-Origin Resource Sharing (CORS) | false | false |
| Allowed Origins | *empty* | *populated when enabling CORS* |
| Allowed HTTP Methods | *empty* | *populated when enabling CORS* |
| Allowed Headers | *empty* | *populated when enabling CORS* |
| Expose Headers | *empty* | *populated when enabling CORS* |
| Allow Credentials | false |  |
| Maximum Age | 1 second | 2 hours (1 second when testing third party access) |

### Miscellaneous Headers

The Security Headers tab is a catch all for many simple security headers.  Some of these are deprecated by the existance of a Content Security Policy, but may still be required for older browsers which do not support a Content Security Policy.

![CORS Tab](/Images/SecurityHeadersTab1.png)

| Setting | Default | Recommended |
|---------|---------|-------------|
| Include Anti-Sniff Header (X-Content-Type-Options) | disabled | No Sniff (nosniff) |
| Include XSS Protection Header (X-XSS-Protection) | disabled | disabled |
| Include Frame Security Header (X-Frame-Options) | disabled | Allow Framing only by this site (SAMEORIGIN) |
| Include Referrer Policy (Referrer-Policy) | disabled | Strict Origin When Cross Origin |

Please note that the X-XSS-Protection header is classed as non-standard and deprecated by the Content Security Policy and in some implementations can introduce vulnerabilities.  This option may be removed in future. You can read more here: [X-XSS-Protection](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-XSS-Protection)

![CORS Tab](/Images/SecurityHeadersTab2.png)

| Setting | Default | Recommended |
|---------|---------|-------------|
| Include Cross Origin Embedder Policy (Cross-Origin-Embedder-Policy) | disabled | Requires CORP |
| Include Cross Origin Opener Policy (Cross-Origin-Opener-Policy) | disabled | Same Origin |
| Include Cross Origin Resource Policy (Cross-Origin-Resource-Policy) | disabled | Same Origin |

![CORS Tab](/Images/SecurityHeadersTab3.png)

| Setting | Default | Recommended |
|---------|---------|-------------|
| Enable Strict Transport Security Header | false | true |
| Include Subdomains | false | |
| Maximum Age | 0 Days | 2 Years |

### Audit

Any change to any of the security headers requires an Authorised user. Every API that writes data for this module will reject any change that does not contain an authorised user.  This is true even if a developer was to grant the *Everyone* role access to the security module in the website startup code (don't do this!).  Every change that is made is attributed to that user along with a detailed breakdown of every single property changed.

Please note that this module does not contain any code that clears down the audit table.

![CORS Tab](/Images/AuditTab.png)

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

## Agency Allow Listing

SEO and Data teams within Digital Agencies, may have many sites which they have to maintain collectively as a team.  Approving a new tool to be injected via GTM may be made once, but may need applying to dozens of websites, each of which may have it's own CSP allow list.

If you have applied the CSP Reporting component (see above), then this plugin can automatically extend the allow list for the site based on centralized approved list.

### Central Allow List Structure

The structure of the central allow list must exist as a JSON object reachable by a GET method for the specified Allow List Url.  The JSON returned should be an array with each entry having a ```sourceUrl``` and an array of ```directives```. All of these should be valid strings.

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
| 'none' | default-src |
| 'self' | child-src, connect-src, font-src, frame-src, img-src, script-src, script-src-elem, style-src, style-src-elem |
| 'unsafe-inline' | script-src, script-src-elem, style-src, style-src-elem |
| 'unsafe-eval' | script-src |
| data: | img-src |
| https://*.cloudfront.net/graphik/ | font-src |
| https://*.cloudfront.net/lato/ | font-src |

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

## Cross-Origin Resource Sharing

Support for managing the CORS headers has been introduced within version 2.0.0.0 and is currently in BETA.

### Configuration

The Service Extensions for setting up the CORS functionality now call the default microsoft service extensions for setting up CORS.  If your solution is already configured to use CORS then remove the following from the startup.cs:

```
// REMOVE THIS
services.AddCors();

// REMOVE THIS
builder.UseCors(...);
```

In beta, the standard configuration will set up a CORS Policy of `Stott:SecurityOptimizely:CORS` which is defined as a static variable as `CspConstants.CorsPolicy` that will be used for the entire website.  Microsoft's default implementation of `ICorsPolicyProvider` is replaced with a custom implementation within this package called `CustomCorsPolicyProvider` that will always load the policy as defined in the administration interface.

Intent exists to update this Custom CORS Policy Provider so that it will load the policy defined within the interface based on specified policy of `Stott:SecurityOptimizely:CORS` and to allow additional hard coded policies to be provided for use within CORS controller attributes.

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
- Optimizely CMS (EPiServer.CMS.UI.Core 12.23.0)
- MVC
- Razor Class Libraries
- React
- Bootstrap for React
- NUnit & Moq
- Entity Framework (Microsoft.EntityFrameworkCore.SqlServer 6.0.6)
