# Stott.Optimizely.Csp

## Overview

Stott.Optimizely.Csp is a CSP editor for Optimizely CMS 12 that provides the user with the ability to define the CSP.  Users are presented with the ability to define a source and to select what CSP directives that can be used with that source.

**Please note that this is currently under active development and is not yet ready for a version 1.0 release.**

## Configuration

After pulling in a reference to the Stott.Optimizely.Csp project, you only need to ensure the following lines are added to the startup class of your solution:

```
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

The call to ```services.AddRazorPages()``` is a standard .NET 5.0 call to ensure razor pages are included in your solution.

The call to ```services.AddCspManager()``` in the ```ConfigureServices(IServiceCollection services)``` sets up the dependency injection requirements for the CSP solution and is required to ensure the solution works as intended.  This works by following the Services Extensions pattern defined by microsoft.

The call to ```app.UseCspManager()``` in the ```Configure(IApplicationBuilder app, IWebHostEnvironment env)``` method sets up the CSP middleware.  This should be declared immediately before the ```app.UseEndpoints(...)``` method to ensure that the headers are added to content pages.

This solution also includes an implementation of ```IMenuProvider``` which ensures that the CSP administration pages are included in the CMS Admin menu under the title of "CSP".  You do not have to do anything to make this work as Optimizely CMS will scan and action all implementations of ```IMenuProvider```.

## Alternate DB Storage

By default, the data for the CSP Manager will use the connection string with the name of "EPiServerDB" when setting up and consuming tables with entity framework.  It is possible to override the connection string used by setting the connection string name in appsettings.config:

```
{
    "Csp": {
        "ConnectionStringName": "EPiServerDB"
    }
}
```

## CSP Reporting

A Content Security Policy can be set to report violations to a given end point.  An API endpoint has been added to the solution which allows for CSP reports to be sent to the CMS.  Browsers can batch up these reports and send them at a later point in time.  This can lead to monitoring violations to be non-responsive.  By adding the following ViewComponent to your layout files, violations will be posted to the CMS as they occur.

```
@await Component.InvokeAsync("CspReporting")
```

This works by adding an event listener for the security violation and are raised by the browser by adding a listener to the security policy violation event.

## Agency Whitelisting

SEO and Data teams within Digital Agencies, may have many sites which they have to maintain collectively as a team.  Approving a new tool to be injected via GTM may be made once, but may need applying to dozens of websites, each of which may have it's own CSP whitelist.

If you have applied the CSP Reporting component (see above), then this plugin can automatically extend the whitelist for the site based on centralized approved list.

### Configuring the Cental Whitelist

The whitelist functionality just requires two additional parameters to be configured within the appsettings.config.

n.b. If you are also overriding the database connection string, then you will need to combine both parameters into a single "Csp" section.

Example:
```
{
    "Csp": {
        "UseWhitelist": true,
        "WhitelistUrl": "https://raw.githubusercontent.com/GeekInTheNorth/Stott.Optimizely.Csp/main/Example%20Documents/whitelistentries.json"
    }
}
```

### Central Whitelist Structure

The structure of the central whitelist must exist as a JSON object reachable by a GET method for the specified 'WhitelistUrl'.  The JSON returned should be an array with each entry having a 'sourceUrl' and an array of 'directives'. All of these should be valid strings.

Example:
```
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

## Contributing

I am open to contributions to the code base.  The following rules should be followed:

1. Contributions should be made by Pull Requests.
2. All commits should have a meaningful messages.
3. All commits should have a reference to your GitHub user.
4. Ideally all new changes should include appropriate unit test coverage.

### Technologies Used

- .NET 5.0
- Optimizely CMS 12.0
- MVC
- Razor Class Libraries
- React
- Bootstrap for React
- NUnit & Moq

## Roadmap

The following changes are planned for future versions:
- Source Management UI to be updated to provide more intuitive data entry for sources.
