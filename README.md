EverCoow
========

A customised Evernote converter to Google docs and Mail chimp.

Ruby version has come further but I noticed Rexml manipulates the text I put into Xml (adds and removes line breaks) and I got tired of no compiler which forces all but syntax bugs to runtime and wasting time on debugging instead of progressing the project.  Then add a clunky debugger to this.

So I am rewriting it to dotnet.


Here is a short explanation of what it is.

I write a news letter with uneven intervals.  I use Evernote to gather stuff.  I use Googledocs/Gmail to send the news letter.

The work progess is to gather data in Evernote.
When I find myself satisfied I use Applescript to extract xml files from Evernote.  
Then I run a Ruby script to create an XML file and then an HTML file.  
This HTML file I import into Google docs.  The result is for instance [here](https://docs.google.com/document/pub?id=1oa8JG2Eut3vF7WYv5hYf2J3td-Fd4bAHacbENRIzVu8).  
(Linked from [here](http://www.selfelected.com/category/cccommunicate/) if you like it.)

Maybe peut-être kanske möjligen someone else has a need for this code.
