using Raven.Abstractions;
using Raven.Database.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System;
using Raven.Database.Linq.PrivateExtensions;
using Lucene.Net.Documents;
using System.Globalization;
using System.Text.RegularExpressions;
using Raven.Database.Indexing;

public class Index_Auto_RSVPs_ByTag : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_Auto_RSVPs_ByTag()
	{
		this.ViewText = @"from doc in docs.RSVPs
select new {
	Tag = doc.Tag
}";
		this.ForEntityNames.Add("RSVPs");
		this.AddMapDefinition(docs => 
			from doc in ((IEnumerable<dynamic>)docs)
			where string.Equals(doc["@metadata"]["Raven-Entity-Name"], "RSVPs", System.StringComparison.InvariantCultureIgnoreCase)
			select new {
				Tag = doc.Tag,
				__document_id = doc.__document_id
			});
		this.AddField("Tag");
		this.AddField("__document_id");
		this.AddQueryParameterForMap("Tag");
		this.AddQueryParameterForMap("__document_id");
		this.AddQueryParameterForReduce("Tag");
		this.AddQueryParameterForReduce("__document_id");
	}
}
