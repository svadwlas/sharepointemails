<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:d="urn:sharepointemail-context" >
    <xsl:output method="xml" indent="yes"/>
    <xsl:include href="EmailHeader.xslt"/>
    <xsl:template match="@* | node()">
      <Html>
        <xsl:variable name ="eventData" select="./d:EventData[1]"/>
        <xsl:variable name ="fields" select="$eventData/d:Field"/>
        <xsl:variable name="changedFields" select="$fields[(@Hidden='false') and (@Changed = 'true') and (string-length(./@DisplayName) &gt; 0)]"/>
        <xsl:variable name="notChangedFields" select="$fields[(@Hidden='false') and (@Changed = 'false') and (string-length(./@DisplayName) &gt; 0)]"/>
        <xsl:variable name="visibleFields" select="$fields[(@Hidden='false') and (string-length(./@DisplayName) &gt; 0)]"/>
        <xsl:variable name="eventType" select="./d:EventData[1]/@EventType"/>
        <head>
          <base href="{SSite.Url}"/>
        </head>
        <Body>
          <xsl:call-template  name="emailheader"/>
          <div class="main">
            <xsl:call-template name="Greeting"/>
            <p>
              <xsl:call-template name="normalTextStyle"/>
              Announcmenting you about events on the server
            </p>
            <p>
              <xsl:call-template name="normalTextStyle"/>
              User <xsl:call-template name="SUserDisplayName"/> <xsl:call-template name="eventType"/>
              <a href="viewItem">
                <xsl:attribute name="href">
                  <xsl:call-template name="itemViewUrl"/>
                </xsl:attribute>
                task
              </a>
            </p>
            <xsl:call-template name="changeTable">
              <xsl:with-param name="changedFields" select="$changedFields"/>
              <xsl:with-param name="eventType" select="$eventType"/>
              <xsl:with-param name="notChangedFields" select="$notChangedFields"/>
              <xsl:with-param name="visibleFields" select="$visibleFields"/>
            </xsl:call-template>
            <xsl:call-template name="Approvement"/>
          </div>
          <xsl:call-template  name="emailfooter"/>
        </Body>
      </Html>
    </xsl:template>  
</xsl:stylesheet>
