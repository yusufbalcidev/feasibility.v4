using feasibility.Entity.Dtos.Study;
using FluentValidation;

namespace feasibility.Business.Validators;

public class StudyCreateDtoValidator : AbstractValidator<StudyCreateDto>
{
    public StudyCreateDtoValidator()
    {
        RuleFor(x => x.LocationId).NotEmpty().WithMessage("Lokasyon seçimi zorunludur.");

        RuleFor(x => x.FeasibilityName)
            .NotEmpty().WithMessage("Fizibilite adı zorunludur.")
            .MaximumLength(200);

        RuleFor(x => x.UsdRate)
            .GreaterThan(0).WithMessage("USD kuru 0'dan büyük olmalıdır.");

        RuleFor(x => x.EurRate)
            .GreaterThan(0).WithMessage("EUR kuru 0'dan büyük olmalıdır.");

        RuleFor(x => x.InflationTl).GreaterThanOrEqualTo(0);
        RuleFor(x => x.InflationUsd).GreaterThanOrEqualTo(0);
        RuleFor(x => x.InflationEur).GreaterThanOrEqualTo(0);

        RuleFor(x => x.MonthlyRent)
            .GreaterThanOrEqualTo(0)
            .When(x => x.HasRent);

        RuleFor(x => x.LoanAmount)
            .GreaterThan(0).WithMessage("Kredi tutarı 0'dan büyük olmalıdır.")
            .When(x => x.HasLoan);

        RuleFor(x => x.LoanAnnualInterestRate)
            .GreaterThan(0).WithMessage("Faiz oranı 0'dan büyük olmalıdır.")
            .When(x => x.HasLoan);

        RuleFor(x => x.LoanTermMonths)
            .GreaterThan(0).WithMessage("Kredi vadesi 0'dan büyük olmalıdır.")
            .When(x => x.HasLoan);
    }
}

public class StudyUpdateDtoValidator : AbstractValidator<StudyUpdateDto>
{
    public StudyUpdateDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.LocationId).NotEmpty().WithMessage("Lokasyon seçimi zorunludur.");

        RuleFor(x => x.FeasibilityName)
            .NotEmpty().WithMessage("Fizibilite adı zorunludur.")
            .MaximumLength(200);

        RuleFor(x => x.UsdRate).GreaterThan(0).WithMessage("USD kuru 0'dan büyük olmalıdır.");
        RuleFor(x => x.EurRate).GreaterThan(0).WithMessage("EUR kuru 0'dan büyük olmalıdır.");

        RuleFor(x => x.InflationTl).GreaterThanOrEqualTo(0);
        RuleFor(x => x.InflationUsd).GreaterThanOrEqualTo(0);
        RuleFor(x => x.InflationEur).GreaterThanOrEqualTo(0);

        RuleFor(x => x.MonthlyRent).GreaterThanOrEqualTo(0).When(x => x.HasRent);

        RuleFor(x => x.LoanAmount).GreaterThan(0).When(x => x.HasLoan);
        RuleFor(x => x.LoanAnnualInterestRate).GreaterThan(0).When(x => x.HasLoan);
        RuleFor(x => x.LoanTermMonths).GreaterThan(0).When(x => x.HasLoan);
    }
}
