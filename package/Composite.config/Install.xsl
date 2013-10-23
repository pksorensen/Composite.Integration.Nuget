<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.C1Console.Elements.Plugins.ElementActionProviderConfiguration/ElementActionProviderPlugins">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='NugetElementActionProvider'])=0">
				<add name="NugetElementActionProvider" type="Composite.Integration.Nuget.C1Console.NugetElementActionProvider, Composite.Integration.Nuget" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
	<xsl:template match="/configuration/Composite.C1Console.Elements.Plugins.ElementAttachingProviderConfiguration/ElementAttachingProviderPlugins">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()" />
			<xsl:if test="count(add[@name='NugetElementAttachingProvider'])=0">
				<add name="NugetElementAttachingProvider" type="Composite.Integration.Nuget.C1Console.NugetElementAttachingProvider, Composite.Integration.Nuget" />
			</xsl:if>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>