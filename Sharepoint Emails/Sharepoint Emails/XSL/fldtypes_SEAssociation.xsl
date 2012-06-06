<xsl:stylesheet xmlns:x="http://www.w3.org/2001/XMLSchema" xmlns:d="http://schemas.microsoft.com/sharepoint/dsp" version="1.0"
    exclude-result-prefixes="xsl msxsl ddwrt" xmlns:ddwrt="http://schemas.microsoft.com/WebParts/v2/DataView/runtime"
    xmlns:asp="http://schemas.microsoft.com/ASPNET/20" xmlns:__designer="http://schemas.microsoft.com/WebParts/v2/DataView/designer"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:SharePoint="Microsoft.SharePoint.WebControls" xmlns:ddwrt2="urn:frontpage:internal" ddwrt:oob="true">
     <xsl:output method="html" indent="no"/>
    <xsl:template match="FieldRef[@FieldType='SPAssociation']" mode="Note_body">
      <xsl:param name="thisNode" select="."/>
      <xsl:variable name="AssCount"
                    select="$thisNode/@*[name()=current()/@Name]" />
        <!--<xsl:value-of select="$thisNode/@*[name()=current()/@Name]"
                    disable-output-escaping ="yes"/>-->
      <xsl:value-of select="$AssCount" disable-output-escaping="yes" />
    </xsl:template>
</xsl:stylesheet>