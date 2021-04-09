/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 9, 2021</date>
/////////////////////////////////////////////////////////

using JeremySnyder.Shared.Data.Base.Models;

namespace JeremySnyder.Shared.Data.Base.DTO
{
    public abstract class BaseDTOWithID : BaseDTO
    {
        public long ID { get; set; }

        protected BaseDTOWithID()
        {
        }

        protected BaseDTOWithID(BaseModelWithID baseModel)
        {
            ID = baseModel.ID;
        }
    }
}
