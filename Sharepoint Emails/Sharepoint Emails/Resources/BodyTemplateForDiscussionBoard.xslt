<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:user="urn:my-scripts">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="@* | node()">
    <Html>
      <head>
        <base href="{SSite.Url}"/>
        <style type="text/css">
          .main table{
          border: 1px solid black;
          }
          .main th{
          border: 1px solid black;
          }
          .main td{
          border: 1px solid black;
          text-align: center;
          }
        </style>
      </head>
      <Body>
        <div class="header">
          <table>
            <tr>
              <td>
                <div>
                  <image src="http://dev/_layouts/images/SharepointEmails/logo.jpg" alt="Logo Inage" width="50" height="50"/>
                </div>
              </td>
              <td>
                <div class="menu">
                  <ul>
                    <li>
                      <a href="{SList.DefaultViewUrl}">View list</a>
                    </li>
                    <xsl:if test="'{SItem}' != ''">
                      <li>
                        <a>
                          <xsl:attribute name="href">
                            <xsl:call-template name="itemViewUrl"/>
                          </xsl:attribute>
                          View <xsl:call-template name="itemType"/>
                        </a>
                      </li>
                    </xsl:if>
                    <li>
                      <a href="/_layouts/MySubs.aspx">Manage My allerts</a>
                    </li>
                    <li>
                      <a href="mailto:melnikvitaly@gmail.com?subject=Feedback about SharePoinr Emails&amp;body=Write your feedback">Feedback</a>
                    </li>
                  </ul>
                </div>
              </td>
            </tr>
          </table>
        </div>
        <div class="main">
          <p>Hello {DUser.LoginName}</p>
          <p>{SUser.LoginName} added new message</p>
          <p>
            <xsl:value-of select="descendant::Message[0]/Body[@Current='true']/Value"/>
          </p>
        </div>
      </Body>
    </Html>
  </xsl:template>
  <msxsl:script language="C#" implements-prefix="user">
    <msxsl:assembly name="System" />
    <msxsl:using namespace="System" /><![CDATA[public string GetDate(string DateFormat){return DateTime.Now.ToString(DateFormat);}]]>
  </msxsl:script>
  <xsl:template name="eventType">
    <xsl:choose>
      <xsl:when test="./EventData[1]/@EventType = 1">added</xsl:when>
      <xsl:when test="./EventData[1]/@EventType = 2">modified</xsl:when>
      <xsl:when test="./EventData[1]/@EventType = 4">deleted</xsl:when>
      <xsl:otherwise>changed</xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="itemType">
    <xsl:choose>
      <xsl:when test="a = b">item</xsl:when>
      <xsl:when test="a = b">file</xsl:when>
      <xsl:otherwise>item(file)</xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="itemViewUrl">
    <xsl:value-of select="concat('{SList.DefaultDisplayFormUrl}','?ID=','{SItem.ID}')"/>
  </xsl:template>

</xsl:stylesheet>

