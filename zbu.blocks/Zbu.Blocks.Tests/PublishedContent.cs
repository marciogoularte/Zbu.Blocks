using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core.Models;

namespace Zbu.Blocks.Tests
{
    class PublishedContent : IPublishedContent
    {
        public StructureDataValue[] Structures { get; set; }


        public IEnumerable<IPublishedContent> Children
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IPublishedContent> ContentSet
        {
            get { throw new NotImplementedException(); }
        }

        public Umbraco.Core.Models.PublishedContent.PublishedContentType ContentType
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime CreateDate
        {
            get { throw new NotImplementedException(); }
        }

        public int CreatorId
        {
            get { throw new NotImplementedException(); }
        }

        public string CreatorName
        {
            get { throw new NotImplementedException(); }
        }

        public string DocumentTypeAlias
        {
            get { throw new NotImplementedException(); }
        }

        public int DocumentTypeId
        {
            get { throw new NotImplementedException(); }
        }

        public int GetIndex()
        {
            throw new NotImplementedException();
        }

        public IPublishedProperty GetProperty(string alias, bool recurse)
        {
            throw new NotImplementedException();
        }

        public IPublishedProperty GetProperty(string alias)
        {
            throw new NotImplementedException();
        }

        public int Id
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsDraft
        {
            get { throw new NotImplementedException(); }
        }

        public PublishedItemType ItemType
        {
            get { throw new NotImplementedException(); }
        }

        public int Level
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public IPublishedContent Parent
        {
            //get { throw new NotImplementedException(); }
            get; set;
        }

        public string Path
        {
            get { throw new NotImplementedException(); }
        }

        public ICollection<IPublishedProperty> Properties
        {
            get { throw new NotImplementedException(); }
        }

        public int SortOrder
        {
            get { throw new NotImplementedException(); }
        }

        public int TemplateId
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime UpdateDate
        {
            get { throw new NotImplementedException(); }
        }

        public string Url
        {
            get { throw new NotImplementedException(); }
        }

        public string UrlName
        {
            get { throw new NotImplementedException(); }
        }

        public Guid Version
        {
            get { throw new NotImplementedException(); }
        }

        public int WriterId
        {
            get { throw new NotImplementedException(); }
        }

        public string WriterName
        {
            get { throw new NotImplementedException(); }
        }

        public object this[string alias]
        {
            get { throw new NotImplementedException(); }
        }
    }
}
