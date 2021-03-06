using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities;
using Signum.Entities.Basics;
using Signum.Entities.Mailing;
using Signum.Entities.SMS;
using Signum.Utilities;
using System.Linq.Expressions;
using Signum.Entities.Authorization;
using Signum.Entities.Workflow;
using Signum.Entities.Files;

namespace Southwind.Entities
{
    [Serializable, EntityKind(EntityKind.Main, EntityData.Master)]
    public class ApplicationConfigurationEntity : Entity
    {
        [StringLengthValidator(Min = 3, Max = 100)]
        public string Environment { get; set; }

        [StringLengthValidator(Min = 3, Max = 100)]
        public string DatabaseName { get; set; }

        /*Email*/
        public EmailConfigurationEmbedded Email { get; set; }

        public EmailSenderConfigurationEntity EmailSender { get; set; }

        /*Sms*/
        public SMSConfigurationEmbedded Sms { get; set; }

        /*AuthTokens*/
        public AuthTokenConfigurationEmbedded AuthTokens { get; set; }

        /*Workflow*/
        public WorkflowConfigurationEmbedded Workflow { get; set; }

        public FoldersConfigurationEmbedded Folders { get; set; }
    }

    [AutoInit]
    public static class ApplicationConfigurationOperation
    {
        public static ExecuteSymbol<ApplicationConfigurationEntity> Save;
    }

    [AutoInit]
    public static class SouthwindGroup
    {
        public static TypeConditionSymbol UserEntities;
        public static TypeConditionSymbol RoleEntities;
        public static TypeConditionSymbol CurrentCustomer;
    }

    [Serializable]
    public class FoldersConfigurationEmbedded : EmbeddedEntity
    {
        /*Predictor*/
        [StringLengthValidator(Max = 300), FileNameValidator]
        public string PredictorModelFolder { get; set; }

        /*Exceptions*/
        [StringLengthValidator(Max = 300), FileNameValidator]
        public string ExceptionsFolder { get; set; }

        /*OperationLog*/
        [StringLengthValidator(Max = 300), FileNameValidator]
        public string OperationLogFolder { get; set; }

        /*ViewLog*/
        [StringLengthValidator(Max = 300), FileNameValidator]
        public string ViewLogFolder { get; set; }

        /*EmailMessage*/
        [StringLengthValidator(Max = 300), FileNameValidator]
        public string EmailMessageFolder { get; set; }
    }

    [AutoInit]
    public static class BigStringFileType
    {
        public static FileTypeSymbol Exceptions;
        public static FileTypeSymbol OperationLog;
        public static FileTypeSymbol ViewLog;
        public static FileTypeSymbol EmailMessage;
    }
}
