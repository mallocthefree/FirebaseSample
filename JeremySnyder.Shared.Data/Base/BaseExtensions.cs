/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 9, 2021</date>
/////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Reflection;
using JeremySnyder.Shared.Data.Base.DTO;
using JeremySnyder.Shared.Data.Base.Models;

namespace JeremySnyder.Shared.Data.Base
{
    public static class BaseExtensions
    {
        /// <summary>
        /// Convert a DTO type inherited from <seealso cref="BaseDTO"/>
        /// to a model inherited from <seealso cref="BaseModel"/>
        ///
        /// The model must be instantiated first and passed in by reference
        /// because static extensions cannot instantiate templates
        /// </summary>
        /// <param name="thisDTO">Connected instantiated <see cref="BaseDTO"/> object</param>
        /// <param name="model"><see cref="BaseModel"/> object to copy into</param>
        /// <typeparam name="TD">The inherited <see cref="BaseDTO"/> class</typeparam>
        /// <typeparam name="TM">The inherited <see cref="BaseModel"/> class</typeparam>
        /// <returns>The <see cref="BaseModel"/> passed in by reference</returns>
        public static TM ToModel<TD, TM>(this TD thisDTO, ref TM model)
            where TD : BaseDTO
            where TM : BaseModel
        {
            model = (TM)thisDTO.CopyProperties(model);
            return model;
        }

        /// <summary>
        /// Convert a model type inherited from <seealso cref="BaseModel"/>
        /// to a DTO inherited from <seealso cref="BaseDTO"/>
        ///
        /// The DTO must be instantiated first and passed in by reference
        /// because static extensions cannot instantiate templates
        /// </summary>
        /// <param name="thisModel">Connected instantiated <see cref="BaseModel"/> object</param>
        /// <param name="dto"><see cref="BaseDTO"/> object to copy into</param>
        /// <typeparam name="TM">The inherited <see cref="BaseModel"/> class</typeparam>
        /// <typeparam name="TD">The inherited <see cref="BaseDTO"/> class</typeparam>
        /// <returns>The <see cref="BaseDTO"/> passed in by reference</returns>
        public static TD ToDTO<TM, TD>(this TM thisModel, ref TD dto)
            where TM : BaseModel
            where TD : BaseDTO
        {
            dto = (TD)thisModel.CopyProperties(dto);
            return dto;
        }
        
        /// <summary>
        /// Copy property values from one object to another.
        ///
        /// Note that this is not yet crafted safely enough to use more broadly
        /// which is why it is restricted to models and DTOs for now
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <exception cref="Exception"></exception>
        private static object CopyProperties(this object source, object destination)
        {
            if (source == null || destination == null)
            {
                throw new Exception("BaseExtensions: Source or/and destination objects are null");
            }
            
            // Getting the Types of the objects
            Type destinationType = destination.GetType();
            Type sourceType = source.GetType();

            // Iterate the Properties of the source instance and  
            // populate them from their destination counterparts  
            PropertyInfo[] properties = sourceType.GetProperties();
            foreach (var propertyInfo in properties)
            {
                if (!propertyInfo.CanRead)
                {
                    continue;
                }

                PropertyInfo targetProperty = destinationType.GetProperty(propertyInfo.Name);
                if (targetProperty == null || !targetProperty.CanWrite)
                {
                    continue;
                }

                var getSetMethod = targetProperty.GetSetMethod();
                if (getSetMethod == null)
                {
                    continue;
                }

                if ((getSetMethod.Attributes & MethodAttributes.Static) != 0)
                {
                    continue;
                }

                if (!targetProperty.PropertyType.IsAssignableFrom(propertyInfo.PropertyType))
                {
                    continue;
                }

                targetProperty.SetValue(destination, propertyInfo.GetValue(source, null), null);
            }

            return destination;
        }
    }
}
