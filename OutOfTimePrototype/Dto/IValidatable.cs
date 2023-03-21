using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OutOfTimePrototype.Dto
{
    public interface IValidatable
    {
        public ModelStateDictionary Validate();
    }
}
