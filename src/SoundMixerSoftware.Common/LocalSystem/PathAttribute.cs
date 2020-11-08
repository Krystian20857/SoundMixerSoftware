using System;
using System.IO;

namespace SoundMixerSoftware.Common.LocalSystem
{
    /// <summary>
    /// <see cref="PathAttribute"/> can be use to creating files and storing paths.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PathAttribute : PropertyAttribute
    {

        /// <summary>
        /// If sets to true file will be created.
        /// </summary>
        public bool Create { get; set; }
        
        /// <summary>
        /// Gets and Sets <see cref="FileAttributes"/> use while creating file.
        /// </summary>
        public FileAttributes FileAttributes { get; set; }

        /// <summary>
        /// Create attribute object use to creating files and storing paths.
        /// </summary>
        /// <param name="propertyName">Set key in registry.</param>
        /// <param name="create">If sets to true file will be created.</param>
        /// <param name="fileAttributes">File will be created with these attributes.</param>
        public PathAttribute(string propertyName, bool create, FileAttributes fileAttributes) : base(propertyName)
        {
            Create = create;
            FileAttributes = fileAttributes;
        }
    }
}