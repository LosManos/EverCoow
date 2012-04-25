puts "***"

require 'cgi'
require 'rexml/document'
include REXML

Element.class_eval do
	def createChildElement( elementName )
		ret = self.add_element elementName
		return ret
	end
	def setText( newText )
		self.text = newText
		return self
	end
	def addAttribute( name, value )
		self.add_attribute( name, value )
		return self
	end
	def appendText( text )
		self.add_text( text )
		return self
	end
end

def createNoteElement( elemNote, rootOut )
		noteElem = rootOut.createChildElement "Note"

		title = elemNote.elements[ "title" ]
		titleElem = noteElem.createChildElement( "Title" ).
			setText( title.text )

		contentText = elemNote.elements[ "content" ].text
		# contentText = wash( contentText )
		contentElem = noteElem.createChildElement( "Content" ).
			setText( contentText )
end

def createNoteElements( inputDoc, rootOut, chapterName )
	# http://emdin-here.ru/r/xpath_checker/

	rootOut.addAttribute( "Title", chapterName )
	XPath.each( inputDoc, "//note[tag[contains(.,'mbb')]]" ){
		|elemNote|

		createNoteElement( elemNote, rootOut )
	}
end

def convertEnexFile( filename, cccommunicateElement, chapterName )
	xmlInputFile = File.new( filename + ".enex" )
	xmlInputDoc = Document.new( xmlInputFile )

	createNoteElements( 
		xmlInputDoc, 
		cccommunicateElement.add_element( "Notebook" ), 
		chapterName )

	#xmlOutputDoc.write( $stdout, 4 )

end

def convertEnexFiles
	xmlOutputDoc = Document.new
	docRoot = xmlOutputDoc.add_element( "Cccommunicate" )

	puts( "Converting mb1." )
	convertEnexFile( "mb-1-CodeAndDevelopment", docRoot, "Code and development" )
	puts( "Converting mb2." )
	convertEnexFile( "mb-2-ProjectsAndLeadership", docRoot, "Projects and leadership" )
	puts( "Converting mb3." )
	convertEnexFile( "mb-3-ProductsAndReleases", docRoot, "Products and releases" )
	puts( "Converting mb4." )
	convertEnexFile( "mb-4-PrivacySecurityAndRights", docRoot, "Security, privacy and rights" )
	puts( "Converting mb5." )
	convertEnexFile( "mb-5-Miscellaneous", docRoot, "Miscellaneous" )

	puts( "Writing xml file." )
	File.open( "CCC.xml", "w" ){
		|file|
		xmlOutputDoc.write( file, 4 )
	}
end

def convertCccFile
	# I wanted to use XSL but didn't get
	# require 'libxslt' to work

	xmlInputFile = File.new( "CCC.xml" )
	xmlInputDoc = Document.new( xmlInputFile )
	xmlOutputDoc = Document.new
	htmlRoot = xmlOutputDoc.add_element( "html" )
	htmlRoot.add_element( "head" )
	bodyRoot = htmlRoot.add_element( "body" )

	XPath.each( xmlInputDoc, "//Cccommunicate/Notebook" ){
		|notebookElement|
		h1 = bodyRoot.add_element( "h1" )
		h1.setText( notebookElement.attributes["Title"] )
		XPath.each( notebookElement, "Note" ){
			|noteElement|
			h2 = bodyRoot.add_element( "h2" )
			h2.setText( noteElement.elements["Title"].text )
			bodyRoot.appendText( noteElement.elements["Content"].text )
			bodyRoot.add_element("br")
		}
	}

	bodyRoot.add_element( "p" ).add_element( "b").add_text( "*EOF*")
	
	File.open( "ToGoogleDocs.xml", "w" ){
		|file|
		xmlOutputDoc.write( file, 4 )
	}
end

def convertHtmlFile
	File.open( 'ToGoogleDocs.xml', 'r' ) do
		|inFile|

		filename = 'CCC' + Time.now.strftime( "%Y%m%d") + ' -'
		
		File.open( filename + '.html', 'w' ) do
			|outFile|
			
			while inLine = inFile.gets
				outFile.puts wash( inLine )
			end
		end
	end
end

def wash( text )
#	text.gsub!( '<?xml version="1.0" encoding="UTF-8" standalone="no"?>', '' )
#	text.gsub!( '<!DOCTYPE en-note SYSTEM "http://xml.evernote.com/pub/enml2.dtd">', '' )
	
#	text.gsub!( '<en-note style="word-wrap: break-word; -webkit-nbsp-mode: space; -webkit-line-break: after-white-space;">', '' )
#	text.gsub!( '</en-note>', '' )
	return CGI.unescapeHTML( text )
end

convertEnexFiles()
convertCccFile()
convertHtmlFile()
puts("***")

=begin
<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template match="/">
  <html>
  <body>
  <xsl:for-each select="//Notebook">
    <h1><xsl:value-of select="@Title" /></h1>
      <xsl:for-each select="Note">
        <h2><xsl:value-of select="Title" /></h2>
        <xsl:value-of select="Content" />
      </xsl:for-each>
  </xsl:for-each>
  </body>
  </html>
</xsl:template>

</xsl:stylesheet>
=end