using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AutoHotkeyRemaster.Models.MacroFeatures
{
    public enum FileNameChangeTrigger
    {
        Contains,
        ExactlyMatch,
    }

    public enum FileChangeActionType
    {
        Delete,
        ChangeName,
    }

    public class FileChangeAction : MacroAction
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string TargetDir { get; set; } = Directory.GetCurrentDirectory();
        public FileNameChangeTrigger Trigger { get; set; } = FileNameChangeTrigger.Contains;
        public FileChangeActionType FileChangeActionType { get; set; } = FileChangeActionType.ChangeName;
        public string Condition { get; set; } = "";
        public string ChangeTo { get; set; }

        public FileChangeAction()
        {
            Name = nameof(FileChangeAction);
        }

        public override void Run()
        {
            var files = (new DirectoryInfo(TargetDir)).GetFiles();
            IEnumerable<FileInfo> targetFiles = new List<FileInfo>();
            switch (Trigger)
            {
                case FileNameChangeTrigger.Contains:
                    targetFiles = files
                        .Where((f) => f.Name.Contains(Condition))
                        .ToList();
                    break;

                case FileNameChangeTrigger.ExactlyMatch:
                    targetFiles = files
                        .Where((f) => f.Name == Condition || f.Name == (Condition + f.Extension))
                        .ToList();

                    break;

                default:
                    break;
            }

            if (FileChangeActionType == FileChangeActionType.ChangeName)
            {
                foreach (var targetfile in targetFiles)
                {
                    string newPath = Path.Combine(targetfile.DirectoryName, targetfile.Name.Replace(Condition, ChangeTo));

                    targetfile.MoveTo(newPath);
                }
            }
            else
            {
                foreach (var targetfile in targetFiles)
                {
                    targetfile.Delete();
                }
            }
        }
    }
}
