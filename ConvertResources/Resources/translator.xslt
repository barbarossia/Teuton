<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>

    <xsl:template match="/">
      <html>
        <head>
          <title>Resource Strings Table</title>
        </head>
        <body>
          <h1>CWF Foundry Resource String of All Messages</h1>
          <table border="1">
            <tr bgcolor="#9acd32">
              <th style="text-align:left">Key</th>
              <th style="text-align:left">Message</th>
            </tr>
            <xsl:for-each select="Root/data">
              <tr>
                <td>
                  <xsl:value-of select="@name"/>
                </td>
                <td>
                  <xsl:value-of select="value"/>
                </td>
              </tr>
            </xsl:for-each>
          </table>
        </body>
      </html>
    </xsl:template>
</xsl:stylesheet>
