using Ingeniux.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Documents;
using Ingeniux.Search.Configuration;
using NLog;

namespace SearchSourceWithTriggerObject
{
	public class DummySearchSource : ContentSearchDocumentSource
	{
		public ManualTrigger IndexUpdateTrigger;

		public override IEnumerable<string> PubliclySearchableFields
		{
			get
			{
				return new[] { "Title", "Copy" };
			}
		}

		public override IEnumerable<string> PubliclySearchableTypes
		{
			get
			{
				return Enumerable.Empty<string>();
			}
		}

		public DummySearchSource(IndexingSourceEntryConfig entryConfig, SiteSearch siteSearch, Logger logger)
			: base(entryConfig, siteSearch, logger)
		{
		}

		public override bool IncludeInIndex(SearchItem item)
		{
			return true;
		}

		public override void InitializeActual(bool indexExists)
		{
			Logger.Info("Dummy Search Source initialized");
		}

		public override bool IsViewable(Document item)
		{
			return true;
		}

		protected override IEnumerable<IndexUpdateTrigger> _InitializeUpdateTriggers()
		{
			IndexUpdateTrigger = ManualTrigger.Get();

			return base._InitializeUpdateTriggers()
				.Concat(new[] {
					new IndexUpdateTrigger(IndexUpdateTrigger.GetType().GetEvent("Triggered"),
					IndexUpdateTrigger,
					(sender, e) =>
					{
						var triggerState = e.ToString();
						Logger.Info("Dummy Search Indexing triggered on state '{0}', performing full reindex", triggerState);

						_index();
					})
				});
		}

		private void _index()
		{
			//sample here just add one item to index
			SearchItem item = new SearchItem(
				this, //this search document source
				"DummyItem", //item type name
				Guid.NewGuid().ToString("N"), //unique id
				null //original object, optional
				);

			item["Title"] = new SearchField("Some Title Text",
				2, //field boost value, no more than 4				
				Field.Index.ANALYZED);
			item["Copy"] = new SearchField("Some Copy Text", 1, Field.Index.ANALYZED);
			item["Comments"] = new SearchField("Some text that will be stored, but not searchabled",
				1, Field.Index.NOT_ANALYZED_NO_NORMS);


			OnIndexUpdateNeeded(new SearchIndexUpdateEventArgs(this)
			{
				ReindexAll = true,
				ItemsType = this.GetType().Name,
				ItemsToAddCallback = () => new [] {item}
			});
		}
	}
}
