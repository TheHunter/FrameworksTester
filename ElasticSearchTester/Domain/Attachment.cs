using Dynamic.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ElasticSearchTester.Extensions;

namespace ElasticSearchTester.Domain
{
    public class Attachment
        : DynamicBinder
    {
        /// <summary>
        /// 
        /// </summary>
        protected Attachment() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="owner"></param>
        public Attachment(FileInfo source, string owner)
        {
            if (source == null)
                throw new ArgumentNullException("source", "The attachment instance must have a not null file info instance on ctor.");

            if (!source.Exists)
                throw new FileNotFoundException("The file in input doesn't exists", source.FullName);

            if (string.IsNullOrWhiteSpace(owner))
                throw new ArgumentException("The documents owner cannot be null or empty.", "owner");

            this.Name = System.IO.Path.GetFileNameWithoutExtension(source.Name);
            this.Path = System.IO.Path.GetDirectoryName(source.FullName);
            this.Owner = owner.Trim();
            this.Extension = System.IO.Path.GetExtension(source.Name);
            this.CreationDate = source.CreationTime;
            this.Size = source.Length;

            try
            {
                using (FileStream stream = source.OpenRead())
                {
                    byte[] buffer = new byte[this.Size];
                    stream.Read(buffer, 0, Convert.ToInt32(this.Size));
                    this.DataEncoded = buffer.EncodeTo64();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error when file info is read, see inner exception for details.", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameFile"></param>
        /// <param name="owner"></param>
        public Attachment(string nameFile, string owner)
            : this(new FileInfo(string.IsNullOrWhiteSpace(nameFile) ? "[NULL OR EMPTY PATH]" : nameFile), owner)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameFile"></param>
        /// <param name="owner"></param>
        /// <param name="path"></param>
        /// <param name="creationDate"></param>
        /// <param name="data"></param>
        public Attachment(string nameFile, string owner, string path, DateTime creationDate, byte[] data)
        {
            if (string.IsNullOrWhiteSpace(nameFile))
                throw new ArgumentException("The file name cannot be empty or null.", "nameFile");

            if (string.IsNullOrWhiteSpace(owner))
                throw new ArgumentException("The documents owner cannot be null or empty.", "owner");

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("The file path cannot be empty or null.", "path");

            if (creationDate == null)
                throw new ArgumentNullException("creationDate", "The creation date cannot be null.");

            if (data == null)
                throw new ArgumentNullException("data", "The data which rappresent document cannot be null.");

            this.Name = System.IO.Path.GetFileNameWithoutExtension(nameFile);
            this.Size = data.Length;
            this.Path = path;
            this.Owner = owner;
            this.Extension = System.IO.Path.GetExtension(nameFile);
            this.DataEncoded = data.EncodeTo64();
            this.CreationDate = creationDate;
        }


        public string Owner { get; private set; }


        public string Name { get; private set; }


        public string Extension { get; private set; }


        public string Path { get; private set; }


        public long Size { get; private set; }


        public string DataEncoded { get; private set; }


        public long SrcVersion { get; private set; }


        public DateTime? CreationDate { get; private set; }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is Attachment)
                return this.GetHashCode() == obj.GetHashCode();

            return false;
        }


        public override int GetHashCode()
        {
            return string.Format(@"{0}\{1}.{2}", this.Path, this.Name, this.Extension).GetHashCode() - this.DataEncoded.GetHashCode();
        }


        public override string ToString()
        {
            return string.Format("Name: {0}, extension: {1}", this.Name, this.Extension);
        }
    }
}
