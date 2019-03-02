# Sitecore Hackathon 2019

## LasVegans Team - Poland

 - Tomasz Juranek
 - Robert Debowski
 - Rafal Dolzynski

## Category: Best enhancement to the Sitecore Admin (XP) UI for Content Editors & Marketers


# Customizable Content Tagging

LasVegans team would like to present to you Cusomizable Content Tagging module.

[Youtube Presentation Movie](https://www.youtube.com/todo)

----------

# Basic Usage

TODO

# Multisite Usage

TODO

# Installation

To use the module you will need to:
- Install [Sitecore 9.1 (Initial Release)](https://dev.sitecore.net/Downloads/Sitecore_Experience_Platform/91/Sitecore_Experience_Platform_91_Initial_Release.aspx)
- Configure content tagging provider. For example for [Open Calais](http://www.opencalais.com/) create new config file in `{sitecore website root}\App_Config\Environment\Sitecore.ContentTagging.OpenCalais.config` with following content:
                                         
		   <?xml version="1.0" encoding="utf-8" ?>
           <configuration xmlns:patch="http://www.sitecore.net/xmlconfig/" xmlns:role="http://www.sitecore.net/xmlconfig/role/">
               <sitecore role:require="Standalone or ContentManagement">
                   <settings>
                       <setting name="Sitecore.ContentTagging.OpenCalais.CalaisAccessToken" value="{your-token-value}" />
                   </settings>
               </sitecore>
           </configuration>

- Install [LV.CustomizableTagger.zip](sc.package/LV.CustomizableTagger.zip) package using Sitecore Installation Wizard.

## Manual Installation/Install from Source

* Clone repository.
* Update `publishUrl` to your Sitecore instance URL in `publishsettings.targets` file.
* Update path in `sourceFolderCustomTagger` variable to your local repository folder in `zz.LV.Foundation.Serialization.Settings.config` file.
* Publish `LV.Feature.AI.CustomCortexTagger` and `LV.Foundation.AI.CustomCortexTagger.Settings` projects from Visual Studio.
* Publish `LV.Foundation.Serialization` project. This project contains Unicorn assemblies and configuration. If you already have Unicorn in your project you can deploy only `zz.LV.Foundation.Serialization.Settings.config` file.  
* Go to http://your-sitecore-instance/unicorn.aspx and sync `Foundation.CustomTaggerSettings` project.

### Test Website Deployment

Source code contains sample website, which can be used to test functionality of the module. To install it:
* Follow the steps for manual installation of the module
* Publish `LV.Project.SamplePageTags` project form Visual Studio.
* Go to http://your-sitecore-instance/unicorn.aspx and sync `Project.SamplePageTags` project.
* Test pages are installed under `/sitecore/content/SampleTagsHome` and `/sitecore/content/SampleTagsData` items.