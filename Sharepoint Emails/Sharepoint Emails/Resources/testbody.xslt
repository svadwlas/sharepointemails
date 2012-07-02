<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='1.0'>
  <xsl:template match='Data'>
    <HTML>
      <BODY>
        <TABLE BORDER='2'>
          <TR>
            <TD>Field Name</TD>
            <TD>Old Value</TD>
            <TD>New Value</TD>
          </TR>
          <xsl:apply-templates select='EventData'/>
        </TABLE>
      </BODY>
    </HTML>
  </xsl:template>
  <xsl:template match='EventData'>
    <xsl:apply-templates select='Field'/>
  </xsl:template>
  <xsl:template match='Field'>
    <tr>
      <td>
        <xsl:choose>
          <xsl:when test="@DisplayName =''">
            <xsl:value-of select='@Name'/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select='@DisplayName'/>
          </xsl:otherwise>
        </xsl:choose>
      </td>
      <td>
        <xsl:choose>
          <xsl:when test="@Old =''">
            no value
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select='@Old'/>
          </xsl:otherwise>
        </xsl:choose>
			</td>
      <td>
        <xsl:choose>
          <xsl:when test="@New =''">
            no value
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select='@New'/>
          </xsl:otherwise>
        </xsl:choose>
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
