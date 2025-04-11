# Stott Security

**Please ensure that you are using version 2.8.2 or later of Stott Security.  See [Stott Security 2.8](https://github.com/GeekInTheNorth/Stott.Security.Optimizely/discussions/244)

[![Platform](https://img.shields.io/badge/Platform-.NET%206%20%2F%20.NET%208-blue.svg?style=flat)](https://docs.microsoft.com/en-us/dotnet/)
[![Platform](https://img.shields.io/badge/Optimizely-%2012-blue.svg?style=flat)](http://world.episerver.com/cms/)
[![GitHub](https://img.shields.io/github/license/GeekInTheNorth/Stott.Security.Optimizely)](https://github.com/GeekInTheNorth/Stott.Security.Optimizely/blob/main/LICENSE.txt)
![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/GeekInTheNorth/Stott.Security.Optimizely/dotnet.yml?branch=main)
![Nuget](https://img.shields.io/nuget/v/Stott.Security.Optimizely)

Stott.Security.Optimizely is a security header editor for Optimizely CMS 12 that provides the user with the ability to define the Content Security Policy (CSP), Cross-origin Resource Sharing (CORS) and other security headers.  What makes this module unique in terms of Content Security Policy management is that users are presented with the ability to define a source and to select the permissions for that source. e.g. can https://www.example.com be used a script source, can it contain the current site in an iFrame, etc.

If you have any questions, please feel free to start up a new discussion over on the [Discussions](https://github.com/GeekInTheNorth/Stott.Security.Optimizely/discussions) section for this repo.

Stott Security is a completely free module, proudly offered under the [MIT License](./LICENSE.txt). If you enjoy using it and want to show your support, consider buying me a coffee on Ko-fi!! ☕️

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/V7V0RX2BQ)

## Interface

The user interface is split into 8 tabs:

- Sections One to Four focus on the Content Security Policy.
- Section Five focuses on the Cross Origin Resource Sharing functionality.
- Section Six focuses on the Permissions Policy (**Introduced in v3.0.0**)
- Section Seven focuses on miscellaneous response headers.
- Section Eight provides you with a preview of the headers the module will generate.
- Section Nine provides you with the audit history for all changes made within the module.
- Section Ten provides you with additional tools to import and export settings.

![Stott Security Menu](/Images/TabList.png)

### Content Security Policy - General Settings

This section allows you to enable or disable your content security policy as well as to put it into a reporting only mode.  If Use Report Only Mode is enabled, then any third party source that is not included in your list of CSP Sources will not be blocked, but will show up in your browser console as an error while still executing.  It is recommended that you enable the Report Only mode when you are first configuring and testing your Content Security Policy.

This AddOn has the ability to add both internal and external Content Security Policy reporting endpoints to the CSP definition. In order for functionality like the Violation tab and External Allowlist to function, the "Use Internal Reporting Endpoints" option needs to be turned on. If you want to send your CSP reports to an external provider, then you will need to tick "Use External Reporting Endpoints" and provide values for both "External Report-To Endpoint" and "External Report-Uri Endpoint"

Some digital agencies will be responsible for multiple websites and will have a common set of tools that they use for tracking user interactions.  The Remote Allow List properties allow you to configure a central allow list of sources and directives.  When a violation is detected, this module can check this allow list and add the extra dependencies into the CSP Sources.  You can read more about this further on in this documentation.

![CSP Settings Tab - General Settings](/Images/CspSettingsTab-2A.png)

| Setting | Default | Recommended |
|---------|---------|-------------|
| Enable Content Security Policy (CSP) | false | true |
| Use Report Only Mode | false | false (true during initial configuration) |
| Use Internal Reporting Endpoints | false | true |
| Use External Reporting Endpoints | false | false |
| External Report-To Endpoint | *empty* | *empty* |
| External Report-Uri Endpoint | *empty* | *empty* |
| Use Remote CSP Allow List | false | |
| Remote CSP Allow List Address | *empty* | |
| Upgrade Insecure Requests | false | false |
| Generate Nonce | false | true |
| Use Strict Dynamic | false | true |

### Content Security Policy - Sandbox Settings

The CSP Sandbox section is dedicated to the **sandbox** directive.  Unlike other directives such as **script-src**, the **sandbox** directive does not operate grant permissions to sources, but instead instruct the browser on what APIs and browser functionality the website can access.

![CSP Settings Tab - Sandbox Settings Section](/Images/CspSettingsTab-2B.png)

### Content Security Policy Sources

The CSP Sources tab is the second of four tabs dedicated to managing your Content Security Policy.  This tab has been designed with the premise of understanding what a third party can do and to allow you to grant a third party access to multiple directives all at once and so that you can remove the same third party source just as easily.  Each directive is given a user friendly description to allow less technical people to understand what a third party can do.

**Updated in version 2.0.0 to include source and directive filtering.**

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

### Content Security Policy Violations

The CSP Violations tab is the forth tab dedicated to managing your Content Security Policy.  This tab requires a developer to add the reporting view component to the website (read more below under CSP Reporting).  When the plugin receives a report of a violation of the Content Security Policy, it will make a record of the third party source and what directive was violated. This is then presented to the user so that that can see how often a violation is happening and when it last happened.  A handy **Create CSP Entry** button allows the user to quickly merge the violated source and directive into the Content Security Policy.

**Updated in version 2.0.0 to include source and directive filtering.**

![CSP Violations Tab](/Images/CspViolationTab.png)

### Cross Origin Resource Sharing

**New in version 2.0.0**

The CORS section is new in version 2.0.0 and allows the user to configure the Cross-Origin Resource Sharing headers for the website.  This is used to grant permissions to third party websites to consume APIs and content from your website.  As trends have moved towards headless and hybrid solutions, controlling your CORS headers can be essential to allowing hybrid solutions to work.

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

**New in version 2.7.0**

A new button has been added called "Add Content Delivery API Headers".  When clicked, the following headers will be added to the list of Expose Headers:

- x-epi-contentguid
- x-epi-branch
- x-epi-siteid
- x-epi-startpageguid
- x-epi-remainingroute
- x-epi-contextmode

### Permission Policy

**New in version 3.0.0**

The Permission Policy section is new as of version 3.0.0 and introduces support for the `Permission-Policy` header.  The header can be activated or deactivated as a whole and each directive can can be configured individually to:

- Disabled: Omitted from the Permission Policy
- Allow None: Outputs as `directive-name:()`
- Allow All Websites: Outputs as `directive-name:*`
- Allow Just This Website: Outputs as `directive-name:(self)`
- Allow this website and specific third party websites: Outputs like `directive-name:(self "https://www.example.com)`
- Allow specific third party websites: Outputs as `directive-name:("https://www.example.com)`

![Security Headers Tab](/Images/PermissionPolicy.png)

### Response Headers

The Security Headers tab is a catch all for many simple security headers.  Some of these are deprecated by the existance of a Content Security Policy, but may still be required for older browsers which do not support a Content Security Policy.

![Security Headers Tab](/Images/SecurityHeadersTab1.png)

| Setting | Default | Recommended |
|---------|---------|-------------|
| Include Anti-Sniff Header (X-Content-Type-Options) | disabled | No Sniff (nosniff) |
| Include XSS Protection Header (X-XSS-Protection) | disabled | disabled |
| Include Frame Security Header (X-Frame-Options) | disabled | Allow Framing only by this site (SAMEORIGIN) |
| Include Referrer Policy (Referrer-Policy) | disabled | Strict Origin When Cross Origin |

Please note that the X-XSS-Protection header is classed as non-standard and deprecated by the Content Security Policy and in some implementations can introduce vulnerabilities.  This option may be removed in future. You can read more here: [X-XSS-Protection](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-XSS-Protection)

![Security Headers Tab](/Images/SecurityHeadersTab2.png)

| Setting | Default | Recommended |
|---------|---------|-------------|
| Include Cross Origin Embedder Policy (Cross-Origin-Embedder-Policy) | disabled | Requires CORP |
| Include Cross Origin Opener Policy (Cross-Origin-Opener-Policy) | disabled | Same Origin |
| Include Cross Origin Resource Policy (Cross-Origin-Resource-Policy) | disabled | Same Origin |

![Security Headers Tab](/Images/SecurityHeadersTab3.png)

| Setting | Default | Recommended |
|---------|---------|-------------|
| Enable Strict Transport Security Header | false | true |
| Include Subdomains | false | |
| Maximum Age | 0 Days | 2 Years |

### Preview

The preview screen will show you the compiled headers that will be returned as part of any GET request.  This does not include CORS headers as these vary based on request or may only be exposed as part of a pre-flight request by the browser.

**New in version 2.2.0**

![CORS Tab](/Images/PreviewTab.png)

### Audit

Any change to any of the security headers requires an Authorised user. Every API that writes data for this module will reject any change that does not contain an authorised user.  This is true even if a developer was to grant the *Everyone* role access to the security module in the website startup code (don't do this!).  Every change that is made is attributed to that user along with a detailed breakdown of every single property changed.

Please note that this module does not contain any code that clears down the audit table.

![CORS Tab](/Images/AuditTab.png)

### Tools

The tools tab introduces the ability to import and export your entire configuration.  The Export function will provide you with a JSON file of all of your configuration settings.  The Import function will require the same JSON file structure and will validate the content of the configuration before applying it.

**New in version 2.6.0**

![Tools Tab](/Images/ToolsTab.png)

## Configuration

After pulling in a reference to the Stott.Security.Optimizely project, you only need to ensure the following lines are added to the startup class of your solution:

```C#
public void ConfigureServices(IServiceCollection services)
{
    services.AddStottSecurity();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseStottSecurity();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapContent();
        endpoints.MapControllers();
    });
}
```

The call to ```services.AddStottSecurity()``` in the ```ConfigureServices(IServiceCollection services)``` sets up the dependency injection requirements for the security module and is required to ensure the solution works as intended.  This works by following the Services Extensions pattern defined by microsoft.

The call to ```app.UseStottSecurity()``` in the ```Configure(IApplicationBuilder app, IWebHostEnvironment env)``` method sets up the CSP middleware.  This should be declared immediately before the ```app.UseEndpoints(...)``` method to ensure that the headers are added to content pages.

This solution also includes an implementation of ```IMenuProvider``` which ensures that the CSP administration pages are included in the CMS Admin menu under the title of "CSP".  You do not have to do anything to make this work as Optimizely CMS will scan and action all implementations of ```IMenuProvider```.

### NONCE Specific Support

Optimizely CMS supports `nonce` for rendered content pages, but unfortunately does not support it for the CMS editor or admin interfaces.  In order to maintain compatibility, `nonce` and `strict-dynamic` will only be added to the `Content-Security-Policy` for content page requests and the header list api.  As there is no browser specification on how to apply NONCE to `script-src-attr` and `style-src-attr`, when `nonce` is enabled, it will only be applied to `script-src`, `script-src-elem`, `style-src` and `style-src-elem`.

A tag helper has been created which targets `<script>` and `<style>` tags which have a `nonce` attribute.  This will ensure that the generated nonce for the current request will be updated to have the correct `nonce` value that matches the `content-security-policy` header.  In order for this to work, you will need to do the following:

Add the following line to `_ViewImports.cshtml`:
```
@addTagHelper *, Stott.Security.Optimizely
```

Decorate your `<script>` tags with either an empty or unassigned `nonce` attribute:
```
<script nonce src="https://www.example.com/script-one.min.js"></script>
<script nonce="" src="https://www.example.com/script-two.min.js"></script>
```

Decorate your `<style>` tags with either an empty or unassigned `nonce` attribute:
```
<style nonce>...</style>
<style nonce="">...</style>
```

The `services.AddStottSecurity()` method in your `startup.cs` will automatically instruct Optimizely CMS to generate `nonce` attributes on all script tags generated by the CMS.

## Additional Configuration Customisation

The configuration of the module has some scope for modification by providing configuration in the service extension methods.  Both the provision of ```cspSetupOptions``` and ```authorizationOptions``` are optional in the following example.

Example:
```C#
services.AddStottSecurity(cspSetupOptions =>
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

### Authentication With Optimizely Opti ID

If you are using the new Optimizely Opti ID package for authentication into Optimizely CMS and the rest of the Optimizely One suite, then you will need to define the `authorizationOptions` for this module as part of your application start up.  This should be a simple case of adding `policy.AddAuthenticationSchemes(OptimizelyIdentityDefaults.SchemeName);` to the `authorizationOptions` as per the example below.

```C#
serviceCollection.AddStottSecurity(cspSetupOptions =>
{
    cspSetupOptions.ConnectionStringName = "EPiServerDB";
},
authorizationOptions =>
{
    authorizationOptions.AddPolicy(CspConstants.AuthorizationPolicy, policy =>
    {
        policy.AddAuthenticationSchemes(OptimizelyIdentityDefaults.SchemeName);
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

**Updated in 2.2.0**

The CSP will always be generated with both the `report-to` and `report-uri` directives.  This is because browser support for `report-to` is still growing while support for `report-uri` is wide spread.  Browsers which support `report-to` will also ignore `report-uri`.

Please note that reports sent to the `report-uri` are sent on an individual error basis.  With the introduction of `report-to`, browsers are meant to send errors in batches in a report.  However it is noted that browsers such as MacOs Safari are sending reports in the style of `report-uri` to the `report-to` endpoints.

It is recommended that you only allow Internal Reporting to be turned on while you are actively monitoring the website for errors as the reports increase traffic to the webserver.

## Agency Allow Listing

SEO and Data teams within Digital Agencies, may have many sites which they have to maintain collectively as a team.  Approving a new tool to be injected via GTM may be made once, but may need applying to dozens of websites, each of which may have it's own CSP allow list.

When the plugin receives a report of a CSP violation, then this plugin can automatically extend the allow list for the site based on centralized approved list.

Please note a consultation is in progress which affects long term support for this feature.  Share your voice here: [Consultation : Do you use the Remote CSP Allow List?](https://github.com/GeekInTheNorth/Stott.Security.Optimizely/discussions/258)

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
| 'self' | default-src, child-src, connect-src, font-src, frame-src, img-src, script-src, script-src-elem, style-src, style-src-elem |
| 'unsafe-inline' | script-src, script-src-elem, style-src, style-src-elem |
| 'unsafe-eval' | script-src, script-src-elem |
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

Support for managing the CORS headers has been introduced within version 2.0.0.0.

### Configuration

The Service Extensions for setting up the CORS functionality now call the default microsoft service extensions for setting up CORS.  If your solution is already configured to use CORS then remove the following from the startup.cs:

```
// REMOVE THIS
services.AddCors();

// REMOVE THIS
builder.UseCors(...);
```

The standard configuration will set up a CORS Policy of `Stott:SecurityOptimizely:CORS` which is defined as a static variable as `CspConstants.CorsPolicy` that will be used for the entire website.  Microsoft's default implementation of `ICorsPolicyProvider` is replaced with a custom implementation within this package called `CustomCorsPolicyProvider` that will always load the policy as defined in the administration interface.

### Support For Additional CORS Policies

*Introduced in 2.2.0*

If you want to make an exception to the CORS Policy of `Stott:SecurityOptimizely:CORS` for a specific route.  Then you can define an additional hard coded CORS Policy using the `services.AddCors(...)` method as follows:

```C#
services.AddCors(x =>
{
    x.AddPolicy("TEST-POLICY", x =>
    {
        x.AllowAnyMethod();
        x.AllowAnyOrigin();
        x.AllowAnyHeader();
    });
});
```

On a Controller Action that then uses the `[EnableCors("TEST-POLICY")]`, if the policy has been defined in code, then it will be used.  In all other cases the CORS policy defined by this module will be used instead.  The priority of which policy is used is in the following order:

- If the provided policy name is null or empty or whitespace, then the module policy will be used.
- If the provided policy name matches the module policy name, then the module policy will be used.
- If a code based policy is found that matches the provided policy name, then the code based policy will be used.
- If a code based policy cannot be found that matches the requested policy name, then the module policy will be used.

## Headless Support

This module was originally built to support a traditional headed CMS solution.  In order to support hybrid and headless solutions, the header configuration can be retrieved from the CMS using an API request.  The following end points do not require authorisation by design and include absolute urls for reporting violations.

Both of the following APIs accept an optional query string of `pageId` which can be used to render the headers in the context of a specific content page.  This allows the headless solution to support the extension of CSP Sources for pages implementing `IContentSecurityPolicyPage`.

### Header Listing API:

Url Examples:
- /stott.security.optimizely/api/compiled-headers/list/
- /stott.security.optimizely/api/compiled-headers/list/?pageId=123

Example Response:
```
[
    {
        "key": "Content-Security-Policy",
        "value": "default-src \u0027none\u0027; ..." // Full CSP will be returned
    },
    {
        "key": "Cross-Origin-Embedder-Policy",
        "value": "unsafe-none"
    },
    {
        "key": "Referrer-Policy",
        "value": "strict-origin-when-cross-origin"
    },
    {
        "key": "Reporting-Endpoints",
        "value": "stott-security-endpoint=\u0022https://www.example.com/stott.security.optimizely/api/cspreporting/reporttoviolation/\u0022"
    },
    {
        "key": "X-Content-Type-Options",
        "value": "nosniff"
    },
    {
        "key": "X-Frame-Options",
        "value": "SAMEORIGIN"
    }
]
```

### Header Content API

Url Examples:
- /stott.security.optimizely/api/compiled-headers/{headerName}
- /stott.security.optimizely/api/compiled-headers/X-Frame-Options

Example Response:
```
SAMEORIGIN
```

## FAQ

### My static files like server-error.html do not have the CSP applied

Make sure that the call to `app.UseStaticFiles()` is made after the call to `app.UseStottSecurity()` to ensure that the CSP middleware is applied to the static file request.

### My Page which implements `IContentSecurityPolicyPage` is not updating with the global content security policy changes.

Pages that use `IContentSecurityPolicyPage` use a separate CSP cache entry to the global CSP cache.  The cache will expire after 1 hour, or you can force a cache clearance for that page by updating the modified date of the page.

### What mode is the best mode to test my CSP with?

It is highly recommended that you put your global CSP into Report Only mode while you test changes to the Content Security Policy.  As this is applied globally (including to the CMS back end) there is a potential for you to damage your CMS editor experience if your Content Security Policy disallows essential CMS functions.

### What if I lock myself out of the website with a bad CSP?

Browser extensions exist which allow a browser to ignore the Content Security Policy.  Edge for example has an extension called [Disable Content-Security-Policy](https://microsoftedge.microsoft.com/addons/detail/disable-contentsecurity/ecmfamimnofkleckfamjbphegacljmbp). Enabling this extension will allow you to get back into your website and correct the content security policy.

### How Can I Tell What Version I Have Installed

The version number now appears as part of the browser tab title when the user is viewing the Stott Security interface, e.g. "Stott Security | 3.0.0.0".  Alternaltively navigate to the Plugin Manager within the CMS Admin interface.

## Contributing

I am open to contributions to the code base.  The following rules should be followed:

1. Contributions should be made by Pull Requests.
2. All commits should have a meaningful messages.
3. All commits should have a reference to your GitHub user.
4. Ideally all new changes should include appropriate unit test coverage.

### Technologies Used

- .NET 6.0 / .NET 8.0 / .NET 9.0
- Optimizely CMS (EPiServer.CMS.UI.Core 12.23.0)
- MVC
- Razor Class Libraries
- React
- Bootstrap for React
- NUnit & Moq
- Entity Framework (Microsoft.EntityFrameworkCore.SqlServer 6.0.6 / 8.0.1 / 9.0.0)
