Composite.Integration.Nuget
===========================

This is a Nuget Integration for Composite C1. Its is an early release for testing.

Reference-style: 
![One look at how it looked in 1.1.2][preview]



##Current Features:

###FragmentInstaller

It is possible to, when creating normal C1 Packages to specify dependencies from Nuget.

```
    <mi:Add installerType="Composite.Integration.Nuget.NugetPackageFragmentInstaller,Composite.Integration.Nuget"
            uninstallerType="Composite.Integration.Nuget.NugetPackageFragmentUninstaller,Composite.Integration.Nuget" >
      <nuget id="Composite.Integration.Nuget" version="1.1.1" pre="false"></nuget>
    </mi:Add>
```

This package also uses this and loads the dlls in the installer for installing itself from nuget. 
Why? Because then its possible to update the package when updates are pushed to Nuget.



###Console Integration
You can update and install directly from the console.


In the coming weeks i will update this infomation with more examples and screenshoots.

[preview]: https://1onobg.dm1.livefilestore.com/y2piubkSBOXJWF6DLH69BCLa010m7mtaAYwLt6r-YW7Rc3nSbyG1xiZqQpTxuIeA4LUlcGlamjfk1pyU0ktY8RowqQb_UtZdReqrzahXKk7zv0/self_updateable.png?psid=1 "Preview"
