using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace TeamC.SKS.BusinessLogic.Entities.Validation
{
    public class WarehouseValidation : AbstractValidator<Warehouse>
    {
        public WarehouseValidation()
        {
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}
