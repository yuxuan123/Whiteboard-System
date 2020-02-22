using System;
using System.Collections.Generic;
using System.Linq;
using WhiteboardAPI.Entities;
using WhiteboardAPI.Model;

namespace WhiteboardAPI.Helpers
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
    }

    public class PropertyMappingService : IPropertyMappingService
    {
        public PropertyMappingService()
        {
            propertyMappings.Add(new PropertyMapping<UserDto, UserDE>(_userPropertyMapping));
            //propertyMappings.Add(new PropertyMapping<FeedbackDto, Feedback>(_feedbackPropertyMapping));
            //propertyMappings.Add(new PropertyMapping<CarparkDto, Carpark>(_carparkPropertyMapping));
        }

        private Dictionary<string, PropertyMappingValue> _userPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "UserName", new PropertyMappingValue(new List<string>() { "UserName" } ) },
                { "Email", new PropertyMappingValue(new List<string>() { "Email"} ) },
                { "PhoneNumber", new PropertyMappingValue(new List<string>() { "PhoneNumber" } ) },
                { "Role", new PropertyMappingValue(new List<string>() { "Role" }) },
            };

        public IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping
            <TSource, TDestination>()
        {
            var matchingMapping = propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance for <{typeof(TSource)}>");
        }
    }
}
