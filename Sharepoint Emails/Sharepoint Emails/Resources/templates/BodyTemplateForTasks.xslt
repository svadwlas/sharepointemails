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
            <xsl:choose>
              <xsl:when test="$eventType=1">
                <h3>
                  Fields values:
                </h3>
                <table width="400" border="1">
                  <tr>
                    <th>FieldName</th>
                    <th>Value</th>
                  </tr>
                  <xsl:for-each select="$visibleFields">
                    <tr>
                      <td>
                        <xsl:value-of select="@DisplayName"/>
                      </td>
                      <td>
                        <xsl:call-template name="dispayFieldValue"/>
                      </td>
                    </tr>
                  </xsl:for-each>
                </table>
              </xsl:when>
              <xsl:when test="$eventType=2">
                <h3>
                  The following fields of <xsl:call-template name="itemType"/> was modified
                </h3>
                <table width="400" border="1">
                  <tr>
                    <th>FieldName</th>
                    <th>Previous Value</th>
                    <th>Next Value</th>
                  </tr>
                  <xsl:for-each select="$changedFields">
                    <xsl:variable name="new" select="@New"/>
                    <xsl:variable name="old" select="@Old"/>
                    <tr>
                      <td>
                        <xsl:value-of select="@DisplayName"/>
                      </td>
                      <td>
                        <xsl:if test="$old=''">
                            <xsl:attribute name="style">text-align:center;</xsl:attribute>
                        </xsl:if>
                        <xsl:call-template name="dispayOldFieldValue"/>
                      </td>
                      <td>
                         <xsl:if test="$new=''">
                            <xsl:attribute name="style">text-align:center;</xsl:attribute>
                        </xsl:if>
                        <xsl:call-template name="dispayNewFieldValue"/>
                      </td>
                    </tr>
                  </xsl:for-each>
                </table>

                <xsl:if test="count($notChangedFields)!=0">
                  <h3>
                    Other fields of <xsl:call-template name="itemType"/>:
                  </h3>
                  <table width="400" border="1">
                    <tr>
                      <th>FieldName</th>
                      <th>Previous Value</th>
                    </tr>
                    <xsl:for-each select="$notChangedFields">
                      <tr>
                        <td>
                          <xsl:value-of select="@DisplayName"/>
                        </td>
                        <td>
                          <xsl:value-of select="@Value"/>
                        </td>
                      </tr>
                    </xsl:for-each>
                  </table>
                </xsl:if>
              </xsl:when>
              <xsl:when test="$eventType=4"></xsl:when>
            </xsl:choose>
            <xsl:call-template name="Approvement"/>
          </div>
          <xsl:call-template  name="emailfooter"/>
        </Body>
      </Html>
    </xsl:template>  
</xsl:stylesheet>
