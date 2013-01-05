<xsl:stylesheet xmlns:xsl='http://www.w3.org/1999/XSL/Transform' version='1.0' xmlns:d="urn:sharepointemail-context">
  <xsl:template match='d:Data'>
    <subject>
      Subject for "{SList.Title}"
    </subject>
  </xsl:template>
</xsl:stylesheet>