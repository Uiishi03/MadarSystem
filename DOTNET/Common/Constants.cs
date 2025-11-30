namespace Madar.Common
{
    public static class Constants
    {
        // Auth / Roles
        public const string ROLE_USER = "user";
        public const string ROLE_ADMIN = "admin";
        public const string ROLE_MANAGEMENT = "Management";
        public const string ROLE_AUDITOR = "Auditor";
        public const string ROLE_AREA_OWNER = "AreaOwner";
        public const string ROLE_RESPONSIBLE_PERSON = "ResponsiblePerson";
        public static readonly string[] ROLES = { ROLE_USER, ROLE_ADMIN };
        public static readonly string[] USER_TYPES = { ROLE_MANAGEMENT, ROLE_AUDITOR, ROLE_AREA_OWNER, ROLE_RESPONSIBLE_PERSON };

        // Genders
        public const string GENDER_MALE = "male";
        public const string GENDER_FEMALE = "female";
        public const string GENDER_OTHER = "other";
        public static readonly string[] GENDERS = { GENDER_MALE, GENDER_FEMALE, GENDER_OTHER };

        // User Status
        public const string USER_STATUS_ACTIVE = "active";
        public const string USER_STATUS_INACTIVE = "inactive";
        public static readonly string[] USER_STATUSES = { USER_STATUS_ACTIVE, USER_STATUS_INACTIVE };

        // Plant Status
        public const string PLANT_STATUS_ACTIVE = "Active";
        public const string PLANT_STATUS_INACTIVE = "Inactive";
        public const string PLANT_STATUS_UNDER_MAINTENANCE = "Under_Maintenance";
        public const string PLANT_STATUS_DECOMMISSIONED = "Decommissioned";
        public static readonly string[] PLANT_STATUSES = {
            PLANT_STATUS_ACTIVE,
            PLANT_STATUS_INACTIVE,
            PLANT_STATUS_UNDER_MAINTENANCE,
            PLANT_STATUS_DECOMMISSIONED
        };

        // Equipment Status
        public const string EQUIPMENT_STATUS_OPERATIONAL = "Operational";
        public const string EQUIPMENT_STATUS_UNDER_MAINTENANCE = "Under_Maintenance";
        public const string EQUIPMENT_STATUS_OUT_OF_SERVICE = "Out_Of_Service";
        public const string EQUIPMENT_STATUS_DECOMMISSIONED = "Decommissioned";
        public static readonly string[] EQUIPMENT_STATUSES = {
            EQUIPMENT_STATUS_OPERATIONAL,
            EQUIPMENT_STATUS_UNDER_MAINTENANCE,
            EQUIPMENT_STATUS_OUT_OF_SERVICE,
            EQUIPMENT_STATUS_DECOMMISSIONED
        };

        // Audit Schedule Status
        public const string AUDIT_SCHEDULE_SCHEDULED = "Scheduled";
        public const string AUDIT_SCHEDULE_POSTPONED = "Postponed";
        public const string AUDIT_SCHEDULE_COMPLETED = "Completed";
        public const string AUDIT_SCHEDULE_CANCELLED = "Cancelled";
        public static readonly string[] AUDIT_SCHEDULE_STATUSES = {
            AUDIT_SCHEDULE_SCHEDULED,
            AUDIT_SCHEDULE_POSTPONED,
            AUDIT_SCHEDULE_COMPLETED,
            AUDIT_SCHEDULE_CANCELLED
        };

        // Audit Status
        public const string AUDIT_STATUS_DRAFT = "Draft";
        public const string AUDIT_STATUS_IN_PROGRESS = "In_Progress";
        public const string AUDIT_STATUS_UNDER_REVIEW = "Under_Review";
        public const string AUDIT_STATUS_CLOSED = "Closed";
        public const string AUDIT_STATUS_CANCELLED = "Cancelled";
        public const string AUDIT_STATUS_COMPLETED = "Closed"; // Alias for Closed
        public static readonly string[] AUDIT_STATUSES = {
            AUDIT_STATUS_DRAFT,
            AUDIT_STATUS_IN_PROGRESS,
            AUDIT_STATUS_UNDER_REVIEW,
            AUDIT_STATUS_CLOSED,
            AUDIT_STATUS_CANCELLED
        };

        // Corrective Action Status
        public const string ACTION_STATUS_PENDING = "Pending";
        public const string ACTION_STATUS_IN_PROGRESS = "InProgress";
        public const string ACTION_STATUS_COMPLETED = "Completed";
        public const string ACTION_STATUS_OVERDUE = "Overdue";
        public const string ACTION_STATUS_EXTENDED = "Extended";
        public const string ACTION_STATUS_REJECTED = "Rejected";
        public const string ACTION_STATUS_ESCALATED = "Escalated";
        public const string ACTION_STATUS_CANCELLED = "Cancelled";
        public static readonly string[] CORRECTIVE_ACTION_STATUSES = {
            ACTION_STATUS_PENDING,
            ACTION_STATUS_IN_PROGRESS,
            ACTION_STATUS_COMPLETED,
            ACTION_STATUS_OVERDUE,
            ACTION_STATUS_EXTENDED,
            ACTION_STATUS_REJECTED,
            ACTION_STATUS_ESCALATED,
            ACTION_STATUS_CANCELLED
        };

        // Audit Attendance Status
        public const string ATTENDANCE_STATUS_PRESENT = "Present";
        public const string ATTENDANCE_STATUS_ABSENT = "Absent";
        public const string ATTENDANCE_STATUS_LATE = "Late";
        public const string ATTENDANCE_STATUS_EXCUSED = "Excused";
        public static readonly string[] ATTENDANCE_STATUSES = {
            ATTENDANCE_STATUS_PRESENT,
            ATTENDANCE_STATUS_ABSENT,
            ATTENDANCE_STATUS_LATE,
            ATTENDANCE_STATUS_EXCUSED
        };

        // Audit History Status
        public const string AUDIT_HISTORY_STATUS_PENDING = "Pending";
        public const string AUDIT_HISTORY_STATUS_APPROVED = "Approved";
        public const string AUDIT_HISTORY_STATUS_REJECTED = "Rejected";
        public const string AUDIT_HISTORY_STATUS_UNDER_REVIEW = "Under_Review";
        public static readonly string[] AUDIT_HISTORY_STATUSES = {
            AUDIT_HISTORY_STATUS_PENDING,
            AUDIT_HISTORY_STATUS_APPROVED,
            AUDIT_HISTORY_STATUS_REJECTED,
            AUDIT_HISTORY_STATUS_UNDER_REVIEW
        };

        // Escalation Levels
        public const string ESCALATION_LEVEL_LOW = "Low";
        public const string ESCALATION_LEVEL_MEDIUM = "Medium";
        public const string ESCALATION_LEVEL_HIGH = "High";
        public const string ESCALATION_LEVEL_CRITICAL = "Critical";
        public static readonly string[] ESCALATION_LEVELS = {
            ESCALATION_LEVEL_LOW,
            ESCALATION_LEVEL_MEDIUM,
            ESCALATION_LEVEL_HIGH,
            ESCALATION_LEVEL_CRITICAL
        };

        // Auditor Role Types
        public const string AUDITOR_ROLE_LEAD = "Lead_Auditor";
        public const string AUDITOR_ROLE_AUDITOR = "Auditor";
        public const string AUDITOR_ROLE_OBSERVER = "Observer";
        public static readonly string[] AUDITOR_ROLE_TYPES = {
            AUDITOR_ROLE_LEAD,
            AUDITOR_ROLE_AUDITOR,
            AUDITOR_ROLE_OBSERVER
        };

        // Report Periods
        public const string PERIOD_MONTHLY = "monthly";
        public const string PERIOD_QUARTERLY = "quarterly";
        public const string PERIOD_YEARLY = "yearly";
        public static readonly string[] REPORT_PERIODS = {
            PERIOD_MONTHLY,
            PERIOD_QUARTERLY,
            PERIOD_YEARLY
        };

        // Report Formats
        public const string REPORT_FORMAT_PDF = "PDF";
        public const string REPORT_FORMAT_EXCEL = "Excel";
        public const string REPORT_FORMAT_CSV = "CSV";
        public static readonly string[] REPORT_FORMATS = {
            REPORT_FORMAT_PDF,
            REPORT_FORMAT_EXCEL,
            REPORT_FORMAT_CSV
        };

        // Priority Levels
        public const string PRIORITY_LOW = "Low";
        public const string PRIORITY_MEDIUM = "Medium";
        public const string PRIORITY_HIGH = "High";
        public const string PRIORITY_CRITICAL = "Critical";
        public static readonly string[] PRIORITY_LEVELS = {
            PRIORITY_LOW,
            PRIORITY_MEDIUM,
            PRIORITY_HIGH,
            PRIORITY_CRITICAL
        };

        // Validation Constants
        public const int MAX_FILE_SIZE_MB = 10;
        public const int MAX_FILE_SIZE_BYTES = MAX_FILE_SIZE_MB * 1024 * 1024;
        public static readonly string[] ALLOWED_IMAGE_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        public static readonly string[] ALLOWED_DOCUMENT_EXTENSIONS = { ".pdf", ".doc", ".docx", ".xls", ".xlsx" };
        public static readonly string[] ALLOWED_FILE_EXTENSIONS = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".pdf", ".doc", ".docx", ".xls", ".xlsx" };

        // Pagination
        public const int DEFAULT_PAGE_SIZE = 10;
        public const int MAX_PAGE_SIZE = 100;

        // Session Keys
        public const string SESSION_USER_ID = "UserId";
        public const string SESSION_USER_TYPE = "UserType";
        public const string SESSION_USER_EMAIL = "UserEmail";
        public const string SESSION_USER_NAME = "UserName";

        // Score Ranges
        public const decimal MIN_AUDIT_SCORE = 0;
        public const decimal MAX_AUDIT_SCORE = 100;
        public const decimal PASSING_AUDIT_SCORE = 60;
        public const decimal EXCELLENT_AUDIT_SCORE = 90;

        // Date Ranges
        public const int MAX_AUDIT_DURATION_HOURS = 480; // 20 days
        public const int MIN_AUDIT_DURATION_HOURS = 1;
        public const int DEFAULT_AUDIT_DURATION_HOURS = 8;

        // Email Templates
        public const string EMAIL_AUDIT_SCHEDULED = "AuditScheduled";
        public const string EMAIL_AUDIT_COMPLETED = "AuditCompleted";
        public const string EMAIL_ACTION_ASSIGNED = "ActionAssigned";
        public const string EMAIL_ACTION_OVERDUE = "ActionOverdue";
        public const string EMAIL_ACTION_COMPLETED = "ActionCompleted";

        // Helper Methods
        public static bool IsValidPlantStatus(string status)
        {
            return PLANT_STATUSES.Contains(status);
        }

        public static bool IsValidAuditScheduleStatus(string status)
        {
            return AUDIT_SCHEDULE_STATUSES.Contains(status);
        }

        public static bool IsValidCorrectiveActionStatus(string status)
        {
            return CORRECTIVE_ACTION_STATUSES.Contains(status);
        }

        public static bool IsValidUserType(string userType)
        {
            return USER_TYPES.Contains(userType);
        }

        public static string GetStatusColor(string status)
        {
            return status switch
            {
                var s when s == AUDIT_STATUS_COMPLETED || s == AUDIT_SCHEDULE_COMPLETED || s == ACTION_STATUS_COMPLETED => "success",
                var s when s == AUDIT_STATUS_IN_PROGRESS || s == ACTION_STATUS_IN_PROGRESS => "primary",
                var s when s == AUDIT_STATUS_DRAFT || s == ACTION_STATUS_PENDING => "secondary",
                var s when s == AUDIT_STATUS_UNDER_REVIEW => "info",
                var s when s == ACTION_STATUS_OVERDUE || s == AUDIT_STATUS_CANCELLED => "danger",
                var s when s == ACTION_STATUS_EXTENDED => "warning",
                _ => "secondary"
            };
        }

        public static string GetPriorityColor(string priority)
        {
            return priority switch
            {
                PRIORITY_CRITICAL => "danger",
                PRIORITY_HIGH => "warning",
                PRIORITY_MEDIUM => "info",
                PRIORITY_LOW => "secondary",
                _ => "secondary"
            };
        }
    }
}