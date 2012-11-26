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
        
        <div class="main">
          <xsl:variable name="DiscussionAdded" select="descendant::d:Discussion[1]/@Current"/>
          <p>Hello {DUser.LoginName}</p>
          <p>
            {SUser.LoginName} added new 
            <xsl:choose>
              <xsl:when test="$DiscussionAdded = 'true'">
                discussion
              </xsl:when>
              <xsl:otherwise>
                message
              </xsl:otherwise>
            </xsl:choose>
          </p>
          <div >
            <div style="height:10px;border-width:1px;border-color:lightblue;border-style:solid">
              Started:
            </div>
            <div style="border:2px inset orange">
              <p>
                Discussion Subject : <xsl:value-of select="descendant::d:Discussion[1]/d:Subject/d:ClearValue"/>
              </p>
              <p>
                Discussion Text : <xsl:value-of select="descendant::d:Discussion[1]/d:Body/d:ClearValue"/>
              </p>
            </div>
          </div>
          <xsl:apply-templates select="descendant::d:Discussion[1]/d:Message">
              <xsl:with-param select="5" name="otstup"/>
              <xsl:with-param select="5" name="step"/>
          </xsl:apply-templates>
        </div>

        <xsl:call-template  name="emailfooter"/>
      </Body>
    </Html>
  </xsl:template>

  <xsl:template match="d:Message">
    <xsl:param name="otstup"/>
    <xsl:param name="step"/>
    <div>
      <xsl:attribute name="style">
        <xsl:value-of select="concat('margin-left:',$otstup,'px')"/>
      </xsl:attribute>
      <div style="border:2px inset orange">
        <p>User : <xsl:value-of select="@User"/></p>
        <p>Message Text : <xsl:value-of select ="d:Body/d:ClearValue"/></p>
      </div>
      <xsl:apply-templates select="./d:Message">
        <xsl:with-param name="otstup" select="$otstup+$step"/>
      </xsl:apply-templates>
    </div>
  </xsl:template>  
 </xsl:stylesheet>

