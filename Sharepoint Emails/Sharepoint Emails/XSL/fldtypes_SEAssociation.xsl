<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet xmlns:x="http://www.w3.org/2001/XMLSchema" xmlns:d="http://schemas.microsoft.com/sharepoint/dsp" version="1.0"
    exclude-result-prefixes="xsl msxsl ddwrt" xmlns:ddwrt="http://schemas.microsoft.com/WebParts/v2/DataView/runtime"
    xmlns:asp="http://schemas.microsoft.com/ASPNET/20" xmlns:__designer="http://schemas.microsoft.com/WebParts/v2/DataView/designer"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:SharePoint="Microsoft.SharePoint.WebControls" xmlns:ddwrt2="urn:frontpage:internal" ddwrt:oob="true">
     <xsl:output method="html" indent="no"/>
      <xsl:template match="FieldRef[@FieldType='SPAssociationFieldType']" mode="Note_body">
         <!--Set the thisNode parameter to the current node-->
         <xsl:param name="thisNode" select="." />
          <!--Store the current element-->
          <xsl:variable name="curElement" select="current()" />
    
          <!--Store the field value we are rendering in a variable for easier access-->
          <xsl:variable name="fldVal">
            <!--We want to return the value of the field that has the same name as the current element being processed-->
            <xsl:value-of select="$thisNode/@*[name()=curElement/@Name]"/>
           </xsl:variable>
      <!--Render our result field with the home score - away score construct-->
      <xsl:value-of select="substring-before($fldVal, ', ')"/> - <xsl:value-of select="substring-after($fldVal, ', ')"/>
  </xsl:template>
</xsl:stylesheet>