<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:user="urn:my-scripts">
    <xsl:output method="xml" indent="yes"/>
    <xsl:include href="Utils.xslt"/>
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
                    <image src="http://dev/_layouts/images/SharePointEmails/logo.jpg" alt="Logo Inage" width="50" height="50"/>
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
            <p>Announcmenting you about events on the server</p>
            <p>
              User <a href="/_layouts/userdisp.aspx?ID={SUser.ID}">{SUser.LoginName}</a> <xsl:call-template name="eventType"/> 
              <a href="viewItem">
                <xsl:attribute name="href">
                  <xsl:call-template name="itemViewUrl"/>
                </xsl:attribute>
                item
              </a>
            </p>
            <p>The following fields of <xsl:call-template name="itemType"/> was modified</p>
            <table class="table">
              <tr>
                <th>FieldName</th>
                <th>Previous Value</th>
                <th>Next Value</th>
              </tr>
              <xsl:for-each select="./EventData[1]/Field[(@Changed = 'true') and (string-length(./@DisplayName) &gt; 0)]">
                    <tr>
                      <th><xsl:value-of select="@DisplayName"/></th>
                      <th><xsl:value-of select="@Old"/></th>
                      <th><xsl:value-of select="@New"/></th>
                    </tr>
              </xsl:for-each>
            </table>

            <p>Other fields of <xsl:call-template name="itemType"/>: </p>
            <table class="table">
              <tr>
                <th>FieldName</th>
                <th>Previous Value</th>
              </tr>
              <xsl:for-each select="./EventData[1]/Field[(@Changed = 'false') and (string-length(./@DisplayName) &gt; 0)]">
                    <tr>
                      <th>
                        <xsl:value-of select="@DisplayName"/>
                      </th>
                      <th>
                        <xsl:value-of select="@Value"/>
                      </th>
                    </tr>
              </xsl:for-each>
            </table>
            <p>
              You have the prermissions to approve or reject these chanegs 
              <a href="/_layouts/approve.aspx?List={SList.ID}&amp;ID={SItem.ID}" class="approveActionLink">
                <input type="button" value="Approve"/>
              </a>
            </p>
            <!--<table>
              <tr>
                <td>Approve these changes</td>
                <td>Reject these changes</td>
              </tr>
              <tr>
                <td>
                  <a href="/_layouts/SharePointEmails/ApprovePage.aspx?action=approve;comment=no" class="approveActionLink">
                    <input type="button" value="Approve"/>
                  </a>
                </td>
                <td>
                  <a href="/_layouts/SharePointEmails/ApprovePage.aspx?action=approve;comment=no" class="approveActionLink">
                    <input type="button" value="Reject"/>
                  </a>
                </td>
              </tr>
              <tr>
                <td>
                  <a href="/_layouts/SharePointEmails/ApprovePage.aspx?action=approve$amp;comment=yes" class="approveActionLink">
                    <input type="button" value="Approve with comments"/>
                  </a>
                </td>
                <td>
                  <a href="/_layouts/SharePointEmails/ApprovePage.aspx?action=approve&amp;comment=yes" class="approveActionLink">
                    <input type="button" value="Reject with comments"/>
                  </a>
                </td>
              </tr>
            </table>-->
          </div>
          <div class="footer">
            <span>
              generated by 
              <a href="https://sharepointemails.codeplex.com/">SharePointEmails</a>,  <xsl:value-of select="user:GetDate('dddd, dd MMMM yyyy')" />
            </span>
          </div>
        </Body>
      </Html>
    </xsl:template>  
</xsl:stylesheet>
