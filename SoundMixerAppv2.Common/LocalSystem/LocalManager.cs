using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;
using SoundMixerAppv2.Common.Extension;
using SoundMixerAppv2.Common.Property;

namespace SoundMixerAppv2.Common.LocalSystem
{
    /// <summary>
    /// Contains registry and allow to pre-create files.
    /// </summary>
    public class LocalManager : IProperties<object, object>
    {
        #region Logger
        /// <summary>
        /// Use for logging in current class.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        #endregion
        
        #region Private Fields
        
        private readonly Hashtable _registry = new Hashtable();
        
        #endregion
        
        #region Public Properties

        /// <summary>
        /// Gets initialized type.
        /// </summary>
        public Type Type { get; }

        #endregion
        
        #region Constructor

        /// <summary>
        /// Create <see cref="LocalManager"/> instance.
        /// </summary>
        /// <param name="type">Current operating type.</param>
        /// <exception cref="ArgumentException">Throws when does not meet requirements.</exception>
        public LocalManager(Type type)
        {
            if (type.IsSealed && type.IsAbstract && Attribute.IsDefined(type, typeof(LocalContainerAttribute))) 
                this.Type = type;
            else
                throw new ArgumentException("Local Container does not meet the requirements.");
        }
        
        #endregion
        
        #region Public Methods

        /// <summary>
        /// Create file with specified attributes.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="attributes"></param>
        public void CreateFile(string path, FileAttributes attributes)
        {
            if (IsFile(attributes))
            {
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                    Logger.Info($"Created file: {path}");
                }
            }
            else
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    Logger.Info($"Created directory: {path}");
                }
            }

        }

        /// <summary>
        /// Create files and add values to registry.
        /// </summary>
        public void ResolveLocal()
        {
            foreach (var field in GetValidFields())
            {
                if (Attribute.IsDefined(field, typeof(PathAttribute)))
                {
                    var pathAttribute = (PathAttribute)Attribute.GetCustomAttribute(field, typeof(PathAttribute));
                    var fieldValue = field.GetValue(null);
                    _registry.Add(pathAttribute.PropertyName, fieldValue);
                    if (field.FieldType == typeof(string))
                    {
                        if (pathAttribute.Create)
                            CreateFile((string) fieldValue, pathAttribute.FileAttributes);
                    }
                    else 
                        Logger.Warn($"Field type is not valid: {field.FieldType} required: {typeof(string)}");
                }
                else if (Attribute.IsDefined(field, typeof(PropertyAttribute)))
                {
                    var propertyAttribute = (PropertyAttribute) Attribute.GetCustomAttribute(field, typeof(PropertyAttribute));
                    _registry.Add(propertyAttribute.PropertyName, field.GetValue(null));
                }
            }
        }
        
        public object GetValue(object key)
        {
            return _registry[key];
        }

        public bool RemoveValue(object key)
        {
            if (!_registry.Contains(key))
                return false;
            _registry.Remove(key);
            return true;
        }

        public object[] GetValues()
        {
            return _registry.Values.ToArray();
        }

        public object[] GetKeys()
        {
            return _registry.Keys.ToArray();
        }
        
        #endregion

        #region Private Methods

        /// <summary>
        /// Get fields valid for local processing.
        /// </summary>
        /// <returns>Valid fields.</returns>
        private IEnumerable<FieldInfo> GetValidFields()
        {
            return Type.GetFields(BindingFlags.Static | BindingFlags.Public).Where(field => Attribute.IsDefined(field, typeof(PathAttribute)) || Attribute.IsDefined(field, typeof(PropertyAttribute)));
        }
        
        private static bool IsFile(FileAttributes attributes)
        {
            return (attributes & FileAttributes.Directory) != FileAttributes.Directory;
        }
        
        #endregion
    }
}