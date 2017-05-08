using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace DirectoryScanner.Models
{
    internal class NodeInfo
    {
        public NodeInfo(string path)
        {
            DirectoryInfo fileInfo = new DirectoryInfo(path);
            CreationTime = fileInfo.CreationTime;
            LastWriteTime = fileInfo.LastWriteTime;
            LastAccessTime = fileInfo.LastAccessTime;
            FileAttributes fileAttributes = fileInfo.Attributes;
            Attributes = fileAttributes.ToString(); 
            if (!fileAttributes.HasFlag(FileAttributes.Directory))
            {
                Length = new FileInfo(path).Length;
            }
            NTAccount identity = fileInfo.GetAccessControl().GetOwner(typeof(SecurityIdentifier)).Translate(typeof(NTAccount)) as NTAccount;
            Owner = identity.Value;
            FileSystemRights fileSystemRights = 0; 
            WindowsPrincipal currentuser = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            foreach (FileSystemAccessRule rule in fileInfo.GetAccessControl().GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier)))
            {
                if (currentuser.IsInRole(new SecurityIdentifier(rule.IdentityReference.Value)))
                {
                    fileSystemRights = fileSystemRights | rule.FileSystemRights;
                }
            }
            AccessRules = fileSystemRights.ToString(); 
        }
        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public DateTime LastAccessTime { get; set; }
        public string Attributes { get; set; }
        public Int64 Length { get; set; }
        public string Owner { get; set; }
        public string AccessRules { get; set; }

    }
}
