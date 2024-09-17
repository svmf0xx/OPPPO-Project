using System;
using System.ComponentModel.DataAnnotations;
using Cronos;
namespace Shcheduler.Core.Validations
{
    public class CronExpressionAttribute : ValidationAttribute
    {
        public CronExpressionAttribute()
        {
            ErrorMessage = "Invalid cron expression.";
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            var cronExpression = value.ToString();
            try
            {
                CronExpression.Parse(cronExpression);
                return true;
            }
            catch (CronFormatException)
            {
                return false;
            }
        }
    }
}