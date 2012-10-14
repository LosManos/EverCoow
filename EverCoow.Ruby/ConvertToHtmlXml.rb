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


# This method appends the string to the file
def appendToFile( fileName, theString )
  File.open( fileName, 'a')do
    |outFile|
    outFile.puts(theString)
  end
end


# This method creates an XML element for the article
# and inserts it into the XML document sent as parameter.
def createNoteElement( elemNote, noteRootOut, markdownStringOut )

    # Create a new Note child to later return as the base
    # for the Xml this method creates.
		noteElem = noteRootOut.createChildElement 'Note'

    # Get the title of the EN note and create a new XMl element of it.
		title = elemNote.elements[ "title" ]
		titleElem = noteElem.createChildElement( "Title" ).
			setText( title.text )

    # Get the text of the EN note and create a new Xml element of it.
		contentText = elemNote.elements[ "content" ].text
		# contentText = washOld( contentText )
    contentElem = noteElem.createChildElement( 'Content' ).
      setText( contentText )

    # Create the markdown text and insert it into the xmldocument.
    contentAsXml = REXML::Document.new( contentText )
    enNoteElement = contentAsXml.elements["en-note"]
    md = enNoteElementToMarkdown( enNoteElement )
    mlElem = noteElem.createChildElement( 'MarkdownContent').
        setText( md )

    # Create a new Xml document of the EN note's content
    # and get the text out of it.  Lastly wash it from unwanted
    # html tags.
		contentAsXml = REXML::Document.new( contentText )
    contentAsXml.
		enNoteElement = contentAsXml.elements["en-note"]
    pureText = '## ' + title.text + "\n"
    pureText = pureText + enNoteElementToMarkdown( enNoteElement )

    # Send the string back through the parameter as reference.  We cannot use =
    # since it will replace the string and not update it.
    markdownStringOut.replace( pureText )
end


# This method takes an EN Notebook
# and for each Note with an 'mbb' tag creates a custom Xml Note we can use.
# it also creates a markdown string of all Notes.
def createNoteElements( inputDoc, chapterName, rootOut, markdownStringOut )
	# http://emdin-here.ru/r/xpath_checker/

  # Commence with adding the title as an attribute.
  # Typically "Code and development" or "Projects and leadership".
	rootOut.addAttribute( "Title", chapterName )
  markdownString = '# ' + chapterName + "\n"

  # Then loop through all Notes and create a child item for every such.
  noteMarkdownArray = Array.new()
  XPath.each( inputDoc, "//note[tag[contains(.,'mbb')]]" ){
		|elemNote|

    noteMarkdownStringOut = ''
		createNoteElement( elemNote, rootOut, noteMarkdownStringOut )
    noteMarkdownArray.push( noteMarkdownStringOut)
	}

  # Return an array with the articles.  One item per array.
  mdArticleArrayOut = noteMarkdownArray

  # Return a string with the chapter's header and the articles, formatted as markdown.
  markdownString = markdownString + noteMarkdownArray.join("\n")
  markdownStringOut << markdownString
end


# This method opens an XML file
# and washes the lines some what
# and outputs as an Html file.
# The input file name is hard coded and is output from this code.
# The output file is an Html file that can be imported into Google docs.
# The output fils is also named with today's date.
def convertAndWriteToGoogleDocCompatibleHtmlFile
	File.open( 'ToGoogleDocs.xml', 'r' ) do
		|inFile|

		filename = 'CCC' + Time.now.strftime( "%Y%m%d") + ' -'

    puts 'Writing file ' + filename + '.html'
		File.open( filename + '.html', 'w' ) do
			|outFile|

			while inLine = inFile.gets
				outFile.puts washOld( inLine )
			end
		end
	end
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


# => This method adds an Enex file (as xml) to the cccommunicateElement parameter
# => as a <Notebook> tag.
def convertEnexFile( filename, chapterName, cccommunicateElement, markdownStringOut )
	xmlInputFile = File.new( filename + ".enex" )
	xmlInputDoc = Document.new( xmlInputFile )
  markdownStringOut = ''

	createNoteElements(
		xmlInputDoc,
		chapterName ,
    cccommunicateElement.add_element( "Notebook" ),
    markdownStringOut
  )
end


# This method takes all files exported from EN and converts to one big Xml file.
def convertEnexFiles
	xmlOutputDoc = Document.new
	docRoot = xmlOutputDoc.add_element( "Cccommunicate" )
  mdString2PALOut = ''
  mdString4PSAROut = ''

#	puts( 'Converting mb1.' )
#	convertEnexFile( "mb-1-CodeAndDevelopment", "Code and development", docRoot, markdownStringOut )
  puts( 'Converting mb2.' )
  convertEnexFile( "mb-2-ProjectsAndLeadership", "Projects and leadership", docRoot, mdString2PALOut )
# puts( 'Converting mb3.' )
# convertEnexFile( "mb-3-ProductsAndReleases", "Products and releases", docRoot, markdownStringOut )
  puts( 'Converting mb4.' )
  convertEnexFile( "mb-4-PrivacySecurityAndRights", "Security, privacy and rights", docRoot, mdString4PSAROut )
#	puts( 'Converting mb5.' )
#	convertEnexFile( "mb-5-Miscellaneous", "Miscellaneous", docRoot )

	puts( 'Writing xml file CCC.xml.  This file contains the head lines and contents in XML format.')
	File.open( "CCC.xml", "w" ){
		|file|
		xmlOutputDoc.write( file, 4 )
	}

  puts( 'Writing markdown file CCC.md.  This file contains the articles of the letter as markdown.' )
  mdString = mdString2PALOut << "\n" << mdString4PSAROut
  outFile = File.open( 'CCC.md', 'w')
  outFile.write( mdString )
  outFile.close()
end


# This method converts a Markdown string to a Html suitable for Email.
def convertMarkdownToMailHtml( mdString )
  return '###asdf' + mdString
end



# This method takes the Mail template, updates it's contents and outputs a new file.
# The new file's filename is the parameter.
def createMailFile( resultFileName )

  print 'Writing file ' + resultFileName + '...'
  File.open( resultFileName, 'w')do
    |outFile|
    outFile.puts( textInFile( 'First.part.html'))
  end

  # Open the XML document we use.
  xmlInputDoc = Document.new( File.new( "CCC.xml" ) )

  print 'leader...'
  leaderPart = textInFile( 'leader.part.html' )
  leaderPart.sub!('{{ledare}}', '*** this is my leader ***')
  appendToFile( resultFileName, leaderPart )

  # Loop the chapters.
  XPath.each( xmlInputDoc, "//Cccommunicate/Notebook" ){
      |notebookElement|
      chapterTitle = notebookElement.attributes["Title"]

      print 'header...'
      headerPart = textInFile( 'header.part.html' )
      headerPart.sub!( '{{header}}', chapterTitle )
      appendToFile( resultFileName, headerPart )

      # Loop the articles.
      XPath.each( notebookElement, "Note" ){
        |noteElement|

        articleTitle = noteElement.elements["Title"].text
        articleMdContent = noteElement.elements["MarkdownContent"].text
        articleMailHtmlContent = convertMarkdownToMailHtml( articleMdContent )
        print 'article...'
        articlePart = textInFile( 'article.part.html')
        articlePart.sub!('{{BodyHeader}}', articleTitle )
        articlePart.sub!( /\{\{Body.*\}\}/, articleMailHtmlContent )
        appendToFile( resultFileName, articlePart )
      }
    }

  print 'last...'
  appendToFile( resultFileName, textInFile( 'Last.part.html'))

  puts 'finished.'
end


# This method takes an EN note
# which typically is something like
# <...>
#   <div>text</div>
#   <div>more text</div>
#   ...
# </...>
# and removes all divs.
# and returns it as a long CR separated string, markdown formatted as good as I manage/bother.
# Regex samples: http://warpedvisions.org/projects/markdown-cheat-sheet/
def enNoteElementToMarkdown( enNoteElement )

  # Start with getting all non-empty elements to an array.
  rowArray = Array.new()
  for child in enNoteElement.children
    if child.to_s().strip != ''
      rowArray.push( child.to_s())
    end
  end

  # For each row: wash and then do further conversion.
  rowArray.each_index{|index|
    rowArray[index] = wash( rowArray[index])

    # if begins with '- ' then we know it is a link.
    if isUrlLine( rowArray[index])

      linkOut = ''
      linkCommentOut = ''

      washUrlLine( rowArray[index], linkOut, linkCommentOut )

      rowArray[index] = '- [' + linkOut + '](' + linkOut + ')' + linkCommentOut
    end
  }
  ret = rowArray.join( "\r\n")
  return ret
end


# This method returns true if the line starts with a dash to symbolise a url.
# Like so:
# -
def isUrlLine( line )
  return line.match(/^-\S/)
end


# This method returns the context of a text file as a string.
# There is most certainly a better way to do this than iterating
# the lines as is done right now.
def textInFile( inputFileName )
  templateText = ''
  File.open( inputFileName, 'r') do
  |inFile|

    while inLine = inFile.gets
      templateText = templateText + inLine + "\n"
    end
  end
  return templateText
end


# This method takes a line on the format
# - <a href="asdf">asdf</a> <- a comment
# and returns the link: "asdf" and the comment: "a comment".
def washUrlLine( line, linkOut, linkCommentOut )
  # Get the link.  It's alink.  Remove the suffixing </a> and prefixing > to clean.
  link = line.match(/>http:.*<\/a>/).to_s().gsub!('</a>','').gsub!('>','')

  # Get trailing comment, its behind '<-'
  # like '- mylink  <- mycomment'
  linkComment = ''
  possibleLinkCommentMatch = line.match(/<\/a>\S*&lt.-.*$/)
  if possibleLinkCommentMatch
    linkComment = possibleLinkCommentMatch.to_s()
    # Clean for the </a> we searched for.
    linkComment.gsub!( '</a>', '')
    # Remove other Html tags.
    linkComment.gsub!(%r{</?[^>]+?>}, '')
    # Replace so we have a leading <- again.
    linkComment = CGI.unescapeHTML( linkComment )
  end

  linkOut.replace( link )
  linkCommentOut.replace( linkComment )

end


# This method takes a string and removes the leading and trailing <div> tags.
# It also converts all <br/> to newline characters.  Hmm.  Right now it just strips them
# since we are joining with \n later on anyway.
# <i>xyz</i> becomes _xyz_
# <b>xyz</b> becomes **xyz**
def wash( text )
  text.gsub!( '<div>', '')
  text.gsub!('</div>', '')
  text.gsub!('<br/>', '')
  text.gsub!('&quot;', '"')
  text.gsub!('<i>','_')
  text.gsub!('</i>','_')
  text.gsub!('<b>', '**')
  text.gsub!('</b>', '**')
  return text
end


def washOld( text )
#	text.gsub!( '<?xml version="1.0" encoding="UTF-8" standalone="no"?>', '' )
#	text.gsub!( '<!DOCTYPE en-note SYSTEM "http://xml.evernote.com/pub/enml2.dtd">', '' )

#	text.gsub!( '<en-note style="word-wrap: break-word; -webkit-nbsp-mode: space; -webkit-line-break: after-white-space;">', '' )
#	text.gsub!( '</en-note>', '' )
	return CGI.unescapeHTML( text )
end



convertEnexFiles()
convertCccFile()
createMailFile( 'Mail.' + Time.now.strftime( "%Y%m%d") + '.html' )
convertAndWriteToGoogleDocCompatibleHtmlFile()
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