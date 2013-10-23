Composite.Integration.Nuget
===========================

This is a Nuget Integration for Composite C1. Its is an early release for testing.


##Current Features:

###FragmentInstaller

It is possible to, when creating normal C1 Packages to specify dependencies from Nuget.

'''
    <mi:Add installerType="Composite.Integration.Nuget.NugetPackageFragmentInstaller,Composite.Integration.Nuget"
            uninstallerType="Composite.Integration.Nuget.NugetPackageFragmentUninstaller,Composite.Integration.Nuget" >
      <nuget id="Composite.Integration.Nuget" version="1.1.1" pre="false"></nuget>
    </mi:Add>
'''

This package also uses this and loads the dlls in the installer for installing itself from nuget. 
Why? Because then its possible to update the package when updates are pushed to Nuget.



###Console Integration
You can update and install directly from the console.


In the coming weeks i will update this infomation with more examples and screenshoots.

