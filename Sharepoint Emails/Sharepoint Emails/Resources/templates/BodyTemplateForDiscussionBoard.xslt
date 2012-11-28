<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" 
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" 
                exclude-result-prefixes="msxsl" 
                xmlns:d="urn:sharepointemail-context">
  <xsl:output method="html" indent="yes"/>
  <xsl:include href="EmailHeader.xslt"/>
  <xsl:template match="@* | node()">
    <Html>
      <head>
        <base href="{SSite.Url}"/>
      </head>
      <Body>
        <xsl:call-template  name="emailheader"/>
        <div>
          <xsl:variable name="Discussion" select="descendant::d:Discussion[1]"/>
          <xsl:variable name="DiscussionAdded" select="$Discussion/@Current"/>
          <xsl:call-template name="Greeting"/>
          <p>
            <xsl:call-template name="normalTextStyle"/>
            <xsl:call-template name="SUserDisplayName"/> added new 
            <xsl:choose>
              <xsl:when test="$DiscussionAdded">discussion</xsl:when>
              <xsl:otherwise>message</xsl:otherwise>
            </xsl:choose>
          </p>
          <div>
            <xsl:call-template name="postHeader">
              <xsl:with-param name="node" select="$Discussion"/>
            </xsl:call-template>
            <div>
              <xsl:call-template name="postBodyStyle"/>
              <div>
                <xsl:call-template name="postTitleStyle"/>
                <xsl:value-of select="$Discussion/d:Subject/d:ClearValue"/>
              </div>
              <div>
                <xsl:value-of select="$Discussion/d:Body/d:ClearValue"/>
              </div>
            </div>
          </div>
          <xsl:apply-templates select="$Discussion/d:Message">
              <xsl:with-param select="15" name="otstup"/>
              <xsl:with-param select="15" name="step"/>
          </xsl:apply-templates>
        </div>
        <xsl:call-template  name="emailfooter"/>
      </Body>
    </Html>
  </xsl:template>

  <xsl:template name="postHeader">
    <xsl:param name="node"/>
    <div>
      <xsl:call-template name="postHeaderStyle"/>
      <xsl:if test="$node/@Current">
       <xsl:call-template name="newLabel"/>
      </xsl:if>
      <xsl:choose>
        <xsl:when test="local-name($node)='Discussion'">Discussion started: </xsl:when>
        <xsl:otherwise>Message posted: </xsl:otherwise>
      </xsl:choose>
      <xsl:value-of select="$node/@DateTimeAsString"/> by                  
      <xsl:call-template name="getUser">
        <xsl:with-param name="node" select="$node"/>
      </xsl:call-template> 
    </div>
  </xsl:template>
    
  <xsl:template match="d:Message">
    <xsl:param name="otstup"/>
    <xsl:param name="step"/>
    <div>
      <xsl:attribute name="style">
        <xsl:value-of select="concat('margin-left:',$otstup,'px')"/>
      </xsl:attribute>
      <xsl:call-template name="postHeader">
        <xsl:with-param name="node" select="."/>
      </xsl:call-template>
      <div>
        <xsl:call-template name="postBodyStyle"/>
        <xsl:value-of select ="d:Body/d:ClearValue"/>
      </div>
    </div>
    <xsl:apply-templates select="./d:Message">
      <xsl:with-param name="otstup" select="$otstup+$step"/>
    </xsl:apply-templates>
  </xsl:template>  

<xsl:template name="newLabel">
  <span style="color:green; font-weight:bold;">
    **New**
  </span>
</xsl:template>
 </xsl:stylesheet>


