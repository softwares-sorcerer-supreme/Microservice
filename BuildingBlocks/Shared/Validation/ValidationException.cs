using System.Runtime.Serialization;

namespace Shared.Validation;

[Serializable]
public class ValidationException : ExceptionError
{
    public ValidationResultModel ValidationResultModel { get; }

    public ValidationException(ValidationResultModel validationResultModel) : base(validationResultModel.ToString())
    {
        this.ValidationResultModel = validationResultModel;
    }

    protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}