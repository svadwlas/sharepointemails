<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" >
  <xsl:template name="menuTableStyle">
    <xsl:attribute name="border">1</xsl:attribute>
    <xsl:attribute name="style">
      <xsl:call-template name="replaceChars">
        <xsl:with-param name="str">
          background-color:rgb(215,232,255);
        </xsl:with-param>
    </xsl:call-template>
    </xsl:attribute>
  </xsl:template>
  <xsl:template name="headerTable-menutd">
    <xsl:attribute name="style">vertical-align:top</xsl:attribute>
  </xsl:template>
  <xsl:template name="menuLinksStyle">
    <xsl:attribute name="style">
      <xsl:call-template name="replaceChars">
        <xsl:with-param name="str">
          color: #555;
          text-decoration: none;
        </xsl:with-param>
      </xsl:call-template>
    </xsl:attribute>
  </xsl:template>
  <xsl:template name="postHeaderStyle">
    <xsl:attribute name="style">
      <xsl:call-template name="replaceChars">
        <xsl:with-param name="str">
            border-width:1px;
            border-color:#79A7E3;
            border-style:solid;
            background-color:rgb(215,232,255);
            padding-left:5px;
        </xsl:with-param>
      </xsl:call-template>
    </xsl:attribute>
  </xsl:template>

  <xsl:template name="titleTextStyle">
    <xsl:attribute name="style">
      <xsl:call-template name="replaceChars">
        <xsl:with-param name="str">
          font-size:x-large;
        </xsl:with-param>
      </xsl:call-template>
    </xsl:attribute>
  </xsl:template>

  <xsl:template name="normalTextStyle">
    <xsl:attribute name="style">
      <xsl:call-template name="replaceChars">
        <xsl:with-param name="str">
          font-size:medium;
        </xsl:with-param>
      </xsl:call-template>
    </xsl:attribute>
  </xsl:template>
  
  <xsl:template name="postTitleStyle">
    <xsl:attribute name="style">
      <xsl:call-template name="replaceChars">
        <xsl:with-param name="str">
          border-bottom-width:1px;
          border-bottom-color:#79A7E3;
          border-bottom-style:solid;
          padding:8px;
          padding-bottom:0px;
          margin-bottom:10px;
          font-weight:bold;
        </xsl:with-param>
      </xsl:call-template>
    </xsl:attribute>
  </xsl:template>
  <xsl:template name="postBodyStyle">
    <xsl:attribute name="style">
      <xsl:call-template name="replaceChars">
        <xsl:with-param name="str">
            border-left-width:1px;
            border-left-color:#79A7E3;
            border-left-style:solid;
            padding:8px;
        </xsl:with-param>
      </xsl:call-template>
    </xsl:attribute>
  </xsl:template>

  <xsl:template name="footerStyle">
    <xsl:attribute name="style">
      <xsl:call-template name="replaceChars">
        <xsl:with-param name="str">
            background-color:rgb(215,232,255);
            border-color:#79A7E3;
            border-style:solid;
            border-width:1px;
            padding:8px;
       </xsl:with-param>
      </xsl:call-template>
    </xsl:attribute>
  </xsl:template>

  <xsl:template name="replaceChars">
    <xsl:param name="str"/>
    <xsl:variable name="toReplace" select="' &#xA;'"/>
    <xsl:value-of select="translate($str,$toReplace,'')"/>
  </xsl:template>
</xsl:stylesheet>
