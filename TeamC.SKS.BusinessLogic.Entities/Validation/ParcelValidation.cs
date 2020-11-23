using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace TeamC.SKS.BusinessLogic.Entities.Validation
{
    public class ParcelValidation :AbstractValidator<Parcel>
    {
        public ParcelValidation()
        {
            RuleFor(x => x.Receipient.PostalCode).Matches(@"^[A]\-[0-9]{4}$");
            RuleFor(x => x.Receipient.Street).NotEmpty();
            RuleFor(x => x.Receipient.City).Matches(@"^([A-ZÖÄÜ]{1}[a-zßöäü]+)+(\s{1}[0-9A-zßäöü/]+)*$");
            RuleFor(x => x.Receipient.Name).Matches(@"^([A-ZÖÄÜ]{1}[a-zßöäü]+)+(\s{1}[0-9A-zßäöü/]+)*$");
            RuleFor(x => x.Sender.PostalCode).Matches(@"^[A]\-[0-9]{4}$");
            RuleFor(x => x.Sender.Street).NotEmpty();
            RuleFor(x => x.Sender.City).Matches(@"^([A-ZÖÄÜ]{1}[a-zßöäü]+)+(\s{1}[0-9A-zßäöü/]+)*$");
            RuleFor(x => x.Sender.Name).Matches(@"^([A-ZÖÄÜ]{1}[a-zßöäü]+)+(\s{1}[0-9A-zßäöü/]+)*$");
            RuleFor(x => x.Weight).GreaterThan(0.0f);
            RuleFor(x => x.TrackingId).Matches(@"^[A-Z0-9]{9}$");
        }
    }
}
