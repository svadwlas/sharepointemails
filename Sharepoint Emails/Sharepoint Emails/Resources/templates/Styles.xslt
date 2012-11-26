<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" >
    <xsl:output method="xml" indent="yes"/>

    <xsl:template name="menuTable">
      <xsl:attribute name="border">1</xsl:attribute>
    </xsl:template>
  <xsl:template name="headerTable-menutd">
    <xsl:attribute name="style">vertical-align:top</xsl:attribute>
  </xsl:template>
    <xsl:template name="customlinks">
      <xsl:attribute name="style">
        <xsl:text>
            color: #555;
            text-decoration: none;
          </xsl:text>
      </xsl:attribute>
    </xsl:template>
</xsl:stylesheet>
