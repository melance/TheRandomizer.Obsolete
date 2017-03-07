<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" version="1.0">
  <xsl:template match="Grammar">
    <generator><xsl:apply-templates select="@*|node()"/></generator>
  </xsl:template>
  <xsl:template match="Grammar/@version">
    <xsl:attribute name="version">
      <xsl:value-of select="2.0" />
    </xsl:attribute>
  </xsl:template>
  <xsl:template match="Grammar/@type">
    <xsl:attribute name="xsi:type" >
      <xsl:if test=". = 'DiceRoll'">
      	<xsl:value-of select="'Dice'" />
      </xsl:if>
      <xsl:if test=". = 'AssignmentGrammar'">
        <xsl:value-of select="'Assignment'" />
      </xsl:if>
      <xsl:if test=". = 'LuaGrammar'">
        <xsl:value-of select="'Lua'" />
      </xsl:if>
      <xsl:if test=". = 'PhonotacticsGrammar'">
        <xsl:value-of select="'Phonotactics'" />
      </xsl:if>
      <xsl:if test=". = 'TableGrammar'">
        <xsl:value-of select="'Table'" />
      </xsl:if>
    </xsl:attribute>
  </xsl:template>
  <xsl:template match="@*|node()">
      <xsl:copy>
        <xsl:apply-templates select="@*|node()"/>
      </xsl:copy>
    </xsl:template>
</xsl:stylesheet>