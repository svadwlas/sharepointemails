<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
                 xmlns:user="urn:my-scripts"
                xmlns:d="urn:sharepointemail-context"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <msxsl:script language="C#" implements-prefix="user">
    <msxsl:assembly name="System" />
    <msxsl:using namespace="System" /><![CDATA[public string GetDate(string DateFormat){return DateTime.Now.ToString(DateFormat);}]]>
  </msxsl:script>
  <xsl:template name="eventType">
    <xsl:choose>
      <xsl:when test="./d:EventData[1]/@EventType = 1">added</xsl:when>
      <xsl:when test="./d:EventData[1]/@EventType = 2">modified</xsl:when>
      <xsl:when test="./d:EventData[1]/@EventType = 4">deleted</xsl:when>
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

  <xsl:template name="getUser">
    <xsl:param name="node" />
    <xsl:choose>
      <xsl:when test="$node/@UserName!=''">
        <xsl:value-of select="$node/@UserName"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$node/@User"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="DUserDisplayName">
    {DUser.LoginName}
  </xsl:template>
  <xsl:template name="SUserDisplayName">
    <xsl:variable name="SUserName" select="d:EventData[1]/@SUserName"/>
    <xsl:choose>
      <xsl:when test="'{SUser.ID}'!=''">
        <a href="/_layouts/userdisp.aspx?ID={SUser.ID}">{SUser.LoginName}</a>
      </xsl:when>
      <xsl:when test="$SUserName !=''">
        <xsl:value-of select="$SUserName"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:text>Someone</xsl:text>
    </xsl:otherwise>
    </xsl:choose>
    
  </xsl:template>

  <xsl:template name="Greeting">
    <p>
      <xsl:call-template name="titleTextStyle"/>
      Hello <xsl:call-template name="DUserDisplayName"/>
    </p>
  </xsl:template>
  <xsl:template name="Approvement">
    <xsl:variable name="appNode"  select="./d:EventData[1]/d:Approve"/>
    <xsl:variable name="status"  select="./d:EventData[1]/d:Approve[1]/@Status"/>
    <xsl:variable name="canApprove"  select="./d:EventData[1]/d:Approve[1]/@CanApprove='true'"/>
    <xsl:if test="count($appNode)!=0">
      <xsl:if test="$appNode[1]/@Enabled='true'">
          <p>
            Current approvement status is <xsl:value-of select="$status"/>
            <xsl:if test="$canApprove">
                You have the permissions to approve or reject these chanegs
                <a href="/_layouts/approve.aspx?List={SList.ID}&amp;ID={SItem.ID}" class="approveActionLink">
                  <input type="button" value="Approve Page"/>
                </a>
            </xsl:if>
          </p>
        </xsl:if>
    </xsl:if>
  </xsl:template>

  <xsl:template name ="displayValue">
    <xsl:param name="value"/>
    <xsl:param name="noValue"/>
    <xsl:choose>
      <xsl:when test="$value!=''">
        <xsl:value-of select="$value"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$noValue"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="dispayFieldValue">
    <xsl:call-template name="displayValue">
      <xsl:with-param name="value" select="@Value"/>
      <xsl:with-param name="noValue" select="'-'"/>
    </xsl:call-template>
  </xsl:template>

  <xsl:template name="dispayNewFieldValue">
    <xsl:call-template name="displayValue">
      <xsl:with-param name="value" select="@New"/>
      <xsl:with-param name="noValue" select="'-'"/>
    </xsl:call-template>
  </xsl:template>

  <xsl:template name="dispayOldFieldValue">
    <xsl:call-template name="displayValue">
      <xsl:with-param name="value" select="@Old"/>
      <xsl:with-param name="noValue" select="'-'"/>
    </xsl:call-template>
  </xsl:template>
  
</xsl:stylesheet>
