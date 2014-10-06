<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="/">
      <userErrorConfiguration>
        <errors>
          <xsl:for-each select="Root/data">
            <xsl:element name="add">
              <xsl:attribute name="errorCode">
                <xsl:value-of select="format-number(10000 + position(), '#')"/>
              </xsl:attribute>
              <xsl:attribute name="errorMessage">
                <xsl:value-of select="value" />
              </xsl:attribute>
            </xsl:element>
          </xsl:for-each>
        </errors>
        </userErrorConfiguration>
    </xsl:template>
</xsl:stylesheet>
