using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniverseStaffingApp.Data
{
    class AccessPolicyManager
    {
        public static BlobContainerPermissions GetStandardUserBlobContainerPolicy()
        {
            SharedAccessBlobPolicy sharedPolicy = new SharedAccessBlobPolicy()
            {
                // When the start time for the SAS is omitted, the start time is assumed to be the time when the storage service receives the request.
                // Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List |
            SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create | SharedAccessBlobPermissions.Delete
            };

            BlobContainerPermissions permissions = new BlobContainerPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Off;
            permissions.SharedAccessPolicies.Add("StandardUserPolicy", sharedPolicy);

            return permissions;
        }

        public static SharedAccessBlobPolicy GetStandardUserBlobPolicy()
        {
            SharedAccessBlobPolicy sharedPolicy = new SharedAccessBlobPolicy()
            {
                // When the start time for the SAS is omitted, the start time is assumed to be the time when the storage service receives the request.
                // Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List |
            SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create | SharedAccessBlobPermissions.Delete
            };
            
            return sharedPolicy;
        }

    }
}
