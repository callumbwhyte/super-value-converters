using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models.Blocks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Our.Umbraco.SuperValueConverters.Models
{
    // Extends BlockListModel so Umbraco's rendering helpers still accept it.
    public class BlockListModel<TContent> : BlockListModel
        where TContent : IPublishedElement
    {
        public BlockListModel(IList<BlockListItem<TContent>> items)
            : base(items.Cast<BlockListItem>().ToList())
        {

        }

        public new IEnumerable<BlockListItem<TContent>> Items => ((IEnumerable<BlockListItem>)this).Cast<BlockListItem<TContent>>();

        public new BlockListItem<TContent> this[int index] => (BlockListItem<TContent>)base[index];

        public new BlockListItem<TContent> this[Guid contentKey] => (BlockListItem<TContent>)base[contentKey];

        public new IEnumerator<BlockListItem<TContent>> GetEnumerator() => Items.GetEnumerator();
    }

    public class BlockListModel<TContent, TSettings> : BlockListModel<TContent>
        where TContent : IPublishedElement
        where TSettings : IPublishedElement
    {
        public BlockListModel(IList<BlockListItem<TContent, TSettings>> items)
            : base(items.Cast<BlockListItem<TContent>>().ToList())
        {

        }

        public new IEnumerable<BlockListItem<TContent, TSettings>> Items => ((IEnumerable<BlockListItem>)this).Cast<BlockListItem<TContent, TSettings>>();

        public new BlockListItem<TContent, TSettings> this[int index] => (BlockListItem<TContent, TSettings>)base[index];

        public new BlockListItem<TContent, TSettings> this[Guid contentKey] => (BlockListItem<TContent, TSettings>)base[contentKey];

        public new IEnumerator<BlockListItem<TContent, TSettings>> GetEnumerator() => Items.GetEnumerator();
    }
}
