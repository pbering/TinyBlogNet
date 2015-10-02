---
title: Sitecore CD nodes not picking up events after replication reinitialization or reconfiguration
summary: Test Summary
date: 2015-09-14
tags: Sitecore, SQL
---
###Scenario###

You are using SQL Server replication on one or more Sitecore databases, typically the web database to keep databases in sync. Things runs fine but then you need to make changes to replication where you need to remove subscribers and reinitialize them again. Could be to physically move databases to another cluster or during recovery. 

In my case I had the web database on-premise as the publisher and two subscribers, one located in a datacenter in europe and one in the US. During a runtine task the DBA had to remove the replication and reinitialize subscribers from a snapshot. After that our editors started to report that they could not see their changes after publish. While investigating logs on the CD nodes I found that no remote events was beeing fired at all, not even publish:end:remote where HtmlCacheClearer normally would write an entry when it does. But still we could see that data *was* beeing replicated and all events in the EventQueue tables was up to date.

###So what happed?###

On a Sitecore CD node the EventQueue is beeing checked for new events every 2 second by default (configured in "processingInterval"), in this check Sitecore uses a value from the Properties table named:

	EQStamp_<instance name>
	
It has the value of the latest raised event from the [Stamp] column (the type is [timestamp]. So with each check Sitecore asks only for events newer that the last raised event.

The problem with this is the use of [timestamp], the value of [timestamp] on insert and deleted is a binary counter that is relative in the database, from [MSDN](https://msdn.microsoft.com/en-us/library/ms182776.aspx):

>Each database has a counter that is incremented for each insert or update operation that is performed on a table that contains a rowversion column within the database. This counter is the database rowversion. **This tracks a relative time within a database, not an actual time that can be associated with a clock**

By the way, the **timestamp** data type has been deprecated since SQL Server 2008 and should not be used, check [MSDN](https://msdn.microsoft.com/en-us/library/ms182776.aspx):

>The timestamp syntax is deprecated. This feature will be removed in a future version of Microsoft SQL Server. Avoid using this feature in new development work, and plan to modify applications that currently use this feature.

###Summary###
Always clear the Properties table after fiddling with SQL Server replication...