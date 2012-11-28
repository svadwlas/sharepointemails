<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"  xmlns:d="urn:sharepointemail-context">
    <xsl:output method="xml" indent="yes"/>
    <xsl:include href="EmailHeader.xslt"/>
    <xsl:template match="@* | node()">
      <Html>
        <xsl:variable name ="eventData" select="./d:EventData[1]"/>
        <xsl:variable name ="fields" select="$eventData/d:Field"/>
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
                item
              </a>
            </p>
            <h3>The following fields of <xsl:call-template name="itemType"/> was modified</h3>
            <table width="400" border="1">
              <tr>
                <th>FieldName</th>
                <th>Previous Value</th>
                <th>Next Value</th>
              </tr>
              <xsl:for-each select="$fields[(@Hidden='false') and (@Changed = 'true') and (string-length(./@DisplayName) &gt; 0)]">
                <xsl:variable name="new" select="@New"/>
                <xsl:variable name="old" select="@Old"/>
                    <tr>
                      <td>
                        <xsl:value-of select="@DisplayName"/>
                      </td>
                      <td>
                        <xsl:choose>
                          <xsl:when test="$old!=''">
                            <xsl:value-of select="$old"/>
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:attribute name="style">
                              text-align:center;
                            </xsl:attribute>
                            <xsl:text>-</xsl:text>
                          </xsl:otherwise>
                        </xsl:choose>
                      </td>
                      <td>
                        <xsl:choose>
                          <xsl:when test="$new!=''">
                            <xsl:value-of select="$new"/>
                          </xsl:when>
                          <xsl:otherwise>
                            <xsl:attribute name="style">
                              text-align:center;
                            </xsl:attribute>
                            <xsl:text>-</xsl:text>
                          </xsl:otherwise>
                        </xsl:choose>
                      </td>
                    </tr>
              </xsl:for-each>
            </table>

            <h3>Other fields of <xsl:call-template name="itemType"/>: </h3>
            <table width="400" border="1">
              <tr>
                <th>FieldName</th>
                <th>Previous Value</th>
              </tr>
              <xsl:for-each select="$fields[(@Changed = 'false') and (string-length(./@DisplayName) &gt; 0)]">
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
            <p>
              You have the permissions to approve or reject these chanegs 
              <a href="/_layouts/approve.aspx?List={SList.ID}&amp;ID={SItem.ID}" class="approveActionLink">
                <input type="button" value="Approve Page"/>
              </a>
            </p>
          </div>
          <xsl:call-template  name="emailfooter"/>
        </Body>
      </Html>
    </xsl:template> 
</xsl:stylesheet>
