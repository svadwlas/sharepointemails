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
  <xsl:template name="postHeaderStyle">
    <xsl:attribute name="style">
      <xsl:text>
            border-width:1px;
            border-color:#79A7E3;
            border-style:solid;
            background-color:rgb(215,232,255);
            padding-left:5px;
          </xsl:text>
    </xsl:attribute>
  </xsl:template>
  <xsl:template name="postTitleStyle">
    <xsl:attribute name="style">
      <xsl:text>
            border-bottom-width:1px;
            border-bottom-color:#79A7E3;
            border-bottom-style:solid;
            padding:8px;
            padding-bottom:0px;
            margin-bottom:10px;
            font-weight:bold;
          </xsl:text>
    </xsl:attribute>
  </xsl:template>
  <xsl:template name="postBodyStyle">
    <xsl:attribute name="style">
      <xsl:text>
            border-left-width:1px;
            border-left-color:#79A7E3;
            border-left-style:solid;
            padding:8px;
          </xsl:text>
    </xsl:attribute>
  </xsl:template>

  <xsl:template name="footerStyle">
    <xsl:attribute name="style">
      <xsl:text>
            background-color:rgb(215,232,255);
            border-color:#79A7E3;
            border-style:solid;
            border-width:1px;
            padding:8px;
          </xsl:text>
    </xsl:attribute>
  </xsl:template>
  
</xsl:stylesheet>
