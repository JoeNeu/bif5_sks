using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace TeamC.SKS.BusinessLogic.Entities.Validation
{
    public class HopArrivalValidation : AbstractValidator<HopArrival>
    {
        public HopArrivalValidation()
        {
            RuleFor(x => x.Code).Matches(@"^[A-Z]{4}[0-9]{1,4}$");
        }
    }
}
