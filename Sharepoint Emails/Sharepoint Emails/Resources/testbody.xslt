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
          <xsl:apply-templates select='EventData/Field'/>
        </TABLE>
          <xsl:choose>
            <xsl:when test="EventData/Approve[1]/@Status ='Pending'">
              <xsl:choose>
                <xsl:when test="EventData/Approve[1]/@CanApprove ='true'">
                  <span>You can approve this item using <a href="">Approve</a></span>
                </xsl:when>
                <xsl:otherwise>
                  <span>You cannot approve this item</span>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:when>
            <xsl:otherwise>
              <span>This item does not need approvement</span>
            </xsl:otherwise>
          </xsl:choose>
        
      </BODY>
    </HTML>
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
